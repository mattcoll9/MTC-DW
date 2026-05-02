# Test-RevSportLogin.ps1
# Reads RevSport credentials from SQL Server and steps through the login flow.
# Dumps HTML at each stage to $OutDir for inspection.

param(
    [string]$ConnStr = "Server=localhost;Database=MTC_DataWarehouse;Integrated Security=True;TrustServerCertificate=True;",
    [string]$Email    = "",
    [string]$Password = "",
    [string]$TotpSeed = ""
)

$BASE_URL  = "https://portal.revolutionise.com.au"
$LOGIN_PATH = "/bsyc/login"
$TFA_PATH   = "/bsyc/tfa"
$OutDir    = "$env:TEMP\revsport-debug"

if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Path $OutDir | Out-Null }
Write-Host "=== RevSport Login Diagnostic ===" -ForegroundColor Cyan
Write-Host "Dumps: $OutDir"

# ── 1. Read credentials ───────────────────────────────────────────────────────
Write-Host "`n[1] Reading credentials..."
if ($Email -and $Password) {
    $rsEmail = $Email; $rsPassword = $Password; $rsTotpSeed = $TotpSeed
    Write-Host "  Source: parameters"
} else {
    Write-Host "  Source: database"
    Add-Type -AssemblyName System.Data
    $conn = New-Object System.Data.SqlClient.SqlConnection($ConnStr)
    try { $conn.Open() } catch { Write-Host "  DB CONNECT FAILED: $_" -ForegroundColor Red; exit 1 }
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT [Key], Value FROM dbo.Config WHERE [Key] LIKE 'RevSport.%'"
    $rdr = $cmd.ExecuteReader()
    $cfg = @{}
    while ($rdr.Read()) { $cfg[$rdr["Key"]] = $rdr["Value"] }
    $rdr.Close(); $conn.Close()
    $rsEmail = $cfg["RevSport.Email"]; $rsPassword = $cfg["RevSport.Password"]; $rsTotpSeed = $cfg["RevSport.TotpSeed"]
}
Write-Host "  Email:     $rsEmail"
Write-Host "  Password:  $('*' * [Math]::Min($rsPassword.Length, 12))"
Write-Host "  TotpSeed:  $('*' * [Math]::Min($rsTotpSeed.Length, 8))... (len=$($rsTotpSeed.Length))"

# ── TOTP helper ───────────────────────────────────────────────────────────────
function Get-Totp($base32) {
    $alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"
    $s = $base32.ToUpper().Replace(" ","").TrimEnd("=")
    $bits = [long]0; $bc = 0
    $bytes = New-Object System.Collections.Generic.List[byte]
    foreach ($c in $s.ToCharArray()) {
        $i = $alpha.IndexOf($c)
        if ($i -lt 0) { continue }
        $bits = ($bits -shl 5) -bor $i
        $bc += 5
        if ($bc -ge 8) { $bc -= 8; $bytes.Add([byte](($bits -shr $bc) -band 0xFF)) }
    }
    $counter = [long]([DateTimeOffset]::UtcNow.ToUnixTimeSeconds() / 30)
    $cb = [BitConverter]::GetBytes($counter)
    if ([BitConverter]::IsLittleEndian) { [Array]::Reverse($cb) }
    $hmac = New-Object System.Security.Cryptography.HMACSHA1(,$bytes.ToArray())
    $hash = $hmac.ComputeHash($cb)
    $off  = $hash[$hash.Length - 1] -band 0xF
    $code = (([int]$hash[$off]   -band 0x7F) -shl 24) -bor `
            (([int]$hash[$off+1] -band 0xFF) -shl 16) -bor `
            (([int]$hash[$off+2] -band 0xFF) -shl  8) -bor `
             ([int]$hash[$off+3] -band 0xFF)
    return ($code % 1000000).ToString("D6")
}

# ── HTTP client helpers ───────────────────────────────────────────────────────
Add-Type -AssemblyName System.Net.Http
$jar = New-Object System.Net.CookieContainer
$handler = New-Object System.Net.Http.HttpClientHandler
$handler.CookieContainer = $jar
$handler.AllowAutoRedirect = $true
$http = New-Object System.Net.Http.HttpClient($handler)
$http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36")
$http.Timeout = [TimeSpan]::FromSeconds(30)

function Get-FormToken($html) {
    $rx = [System.Text.RegularExpressions.Regex]
    $m = $rx::Match($html, '<input[^>]+name="_token"[^>]+value="([^"]+)"', "IgnoreCase")
    if ($m.Success) { return $m.Groups[1].Value }
    $m = $rx::Match($html, '<input[^>]+value="([^"]+)"[^>]+name="_token"', "IgnoreCase")
    if ($m.Success) { return $m.Groups[1].Value }
    return $null
}
function Get-PageTitle($html) {
    $m = [System.Text.RegularExpressions.Regex]::Match($html, '<title[^>]*>([^<]+)</title>', "IgnoreCase")
    if ($m.Success) { return $m.Groups[1].Value.Trim() } else { return "(no title)" }
}
function Get-InputNames($html) {
    $ms = [System.Text.RegularExpressions.Regex]::Matches($html, '<input[^>]+name="([^"]+)"', "IgnoreCase")
    return ($ms | ForEach-Object { $_.Groups[1].Value }) -join ", "
}
function Get-CookieSummary() {
    $cs = $jar.GetCookies([Uri]$BASE_URL)
    return ($cs | ForEach-Object { "$($_.Name)(len=$($_.Value.Length))" }) -join "  "
}
function Get-Xsrf() {
    $c = $jar.GetCookies([Uri]$BASE_URL)["XSRF-TOKEN"]
    if ($c) { return [Uri]::UnescapeDataString($c.Value) } else { return "" }
}
function Build-FormBody($kvList) {
    # Build application/x-www-form-urlencoded body as a plain string
    $parts = @()
    foreach ($entry in $kvList) {
        $k = [Uri]::EscapeDataString($entry[0])
        $v = [Uri]::EscapeDataString($entry[1])
        $parts += "$k=$v"
    }
    return $parts -join "&"
}
function New-StringFormContent($body) {
    return New-Object System.Net.Http.StringContent($body, [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded")
}

# ── 2. GET login page ─────────────────────────────────────────────────────────
Write-Host "`n[2] GET $BASE_URL$LOGIN_PATH"
$r = $http.GetAsync($BASE_URL + $LOGIN_PATH).Result
$h = $r.Content.ReadAsStringAsync().Result
$h | Out-File "$OutDir\01-login-page.html" -Encoding UTF8
$tok = Get-FormToken $h
$xsrf = Get-Xsrf
Write-Host "  Status:       $($r.StatusCode)"
Write-Host "  Title:        $(Get-PageTitle $h)"
Write-Host "  Input names:  $(Get-InputNames $h)"
Write-Host "  _token found: $($null -ne $tok)  len=$($tok.Length)"
Write-Host "  XSRF cookie:  len=$($xsrf.Length)"
Write-Host "  Cookies:      $(Get-CookieSummary)"

$credField = "email"
foreach ($m in [System.Text.RegularExpressions.Regex]::Matches($h, '<input[^>]+name="([^"]+)"', "IgnoreCase")) {
    if ($m.Groups[1].Value.ToLower() -eq "username") { $credField = "username" }
}
Write-Host "  Cred field:   $credField"

# ── 3. POST login ─────────────────────────────────────────────────────────────
Write-Host "`n[3] POST $BASE_URL$LOGIN_PATH"
$formTok = if ($tok) { $tok } else { $xsrf }
$loginKvs = @(@("_token", $formTok), @($credField, $rsEmail), @("password", $rsPassword))
$loginBody = Build-FormBody $loginKvs
Write-Host "  POST body:    $loginBody"
$req = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::Post, $BASE_URL + $LOGIN_PATH)
$req.Headers.TryAddWithoutValidation("X-XSRF-TOKEN", $xsrf) | Out-Null
$req.Headers.TryAddWithoutValidation("Referer", $BASE_URL + $LOGIN_PATH) | Out-Null
$req.Content = New-StringFormContent $loginBody
$r2 = $http.SendAsync($req).Result
$h2 = $r2.Content.ReadAsStringAsync().Result
$h2 | Out-File "$OutDir\02-login-post-response.html" -Encoding UTF8
Write-Host "  Status:           $($r2.StatusCode)"
Write-Host "  Title:            $(Get-PageTitle $h2)"
Write-Host "  Input names:      $(Get-InputNames $h2)"
Write-Host "  Has otpModal:     $($h2.Contains('otpModal'))"
Write-Host "  Has /bsyc/tfa:    $($h2.Contains('/bsyc/tfa'))"
Write-Host "  Has name=otp[:    $($h2.Contains('name="otp['))"
Write-Host "  Has name=email:   $($h2.Contains('name="email"'))"
Write-Host "  Has name=username:$($h2.Contains('name="username"'))"
Write-Host "  Cookies:          $(Get-CookieSummary)"

$needs2FA = $h2.Contains("otpModal") -or $h2.Contains("/bsyc/tfa") -or $h2.Contains('name="otp[')
Write-Host "  needs2FA:         $needs2FA"

if ($needs2FA) {
    # ── 4. GET TFA page explicitly ────────────────────────────────────────────
    Write-Host "`n[4] GET $BASE_URL$TFA_PATH (explicit)"
    $r3 = $http.GetAsync($BASE_URL + $TFA_PATH).Result
    $h3 = $r3.Content.ReadAsStringAsync().Result
    $h3 | Out-File "$OutDir\03-tfa-page.html" -Encoding UTF8
    $tfaTok = Get-FormToken $h3
    $xsrf2  = Get-Xsrf
    Write-Host "  Status:      $($r3.StatusCode)"
    Write-Host "  Title:       $(Get-PageTitle $h3)"
    Write-Host "  Input names: $(Get-InputNames $h3)"
    Write-Host "  _token found:$($null -ne $tfaTok)  len=$($tfaTok.Length)"
    Write-Host "  Cookies:     $(Get-CookieSummary)"
    # Dump form action and all hidden fields so we can see the exact TFA form structure
    $rxForm = [System.Text.RegularExpressions.Regex]
    $formActionM = $rxForm::Match($h3, '<form\b[^>]*>', "IgnoreCase")
    Write-Host "  Form tag:    $(if ($formActionM.Success) { $formActionM.Value.Substring(0,[Math]::Min(200,$formActionM.Value.Length)) } else { '(none)' })"
    foreach ($hm in $rxForm::Matches($h3, '<input[^>]+type="hidden"[^>]*>', "IgnoreCase")) {
        $nm = $rxForm::Match($hm.Value, 'name="([^"]+)"', "IgnoreCase")
        $vl = $rxForm::Match($hm.Value, 'value="([^"]*)"', "IgnoreCase")
        $nStr = if ($nm.Success) { $nm.Groups[1].Value } else { "(no name)" }
        $vStr = if ($vl.Success) { $vl.Groups[1].Value.Substring(0,[Math]::Min(20,$vl.Groups[1].Value.Length)) + "..." } else { "(no value)" }
        Write-Host "  hidden:      $nStr = $vStr"
    }

    # ── 5. POST TFA ───────────────────────────────────────────────────────────
    Write-Host "`n[5] POST $BASE_URL$TFA_PATH"
    $code = Get-Totp $rsTotpSeed
    Write-Host "  TOTP code:   $code"
    $tfaFormTok = if ($tfaTok) { $tfaTok } else { $xsrf2 }
    $tfaKvs = @(,@("_token", $tfaFormTok))
    foreach ($d in $code.ToCharArray()) { $tfaKvs += ,@("authentication-code[]", $d.ToString()) }
    $tfaBody = Build-FormBody $tfaKvs
    Write-Host "  POST body:   $tfaBody"
    $req2 = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::Post, $BASE_URL + $TFA_PATH)
    $req2.Headers.TryAddWithoutValidation("X-XSRF-TOKEN", $xsrf2) | Out-Null
    $req2.Headers.TryAddWithoutValidation("Referer", $BASE_URL + $TFA_PATH) | Out-Null
    $req2.Content = New-StringFormContent $tfaBody
    $r4 = $http.SendAsync($req2).Result
    $h4 = $r4.Content.ReadAsStringAsync().Result
    $h4 | Out-File "$OutDir\04-tfa-post-response.html" -Encoding UTF8
    Write-Host "  Status:           $($r4.StatusCode)"
    Write-Host "  Title:            $(Get-PageTitle $h4)"
    Write-Host "  Has otpModal:     $($h4.Contains('otpModal'))"
    Write-Host "  Has /bsyc/tfa:    $($h4.Contains('/bsyc/tfa'))"
    Write-Host "  Has name=email:   $($h4.Contains('name="email"'))"
    Write-Host "  Cookies:          $(Get-CookieSummary)"
}

# ── 6. Verify session ─────────────────────────────────────────────────────────
Write-Host "`n[6] GET $BASE_URL/bsyc/ (verify session)"
$r5 = $http.GetAsync($BASE_URL + "/bsyc/").Result
$h5 = $r5.Content.ReadAsStringAsync().Result
$h5 | Out-File "$OutDir\05-verify.html" -Encoding UTF8
$isLoginPage = $h5.Contains('name="email"') -or $h5.Contains('name="username"')
Write-Host "  Status:    $($r5.StatusCode)"
Write-Host "  Title:     $(Get-PageTitle $h5)"
Write-Host "  Is login:  $isLoginPage"
Write-Host "  Cookies:   $(Get-CookieSummary)"

if (-not $isLoginPage) {
    Write-Host "`n[SUCCESS] Session established." -ForegroundColor Green
} else {
    Write-Host "`n[FAILED] Still on login page." -ForegroundColor Red
}

Write-Host "`nHTML files: $OutDir"

if (-not $isLoginPage) {
    # ── 7. Probe season_id variants ───────────────────────────────────────────
    Write-Host "`n[7] Probing members CSV season_id variants…"
    $MEMBERS_BASE = "$BASE_URL/bsyc/members/reports/download?fields[]=parentBodyID&fields[]=name&fields[]=creation_time&report-format=csv"
    foreach ($sid in @("43649", "0", "")) {
        $sidLabel = if ($sid -eq "") { "(empty)" } else { $sid }
        $url = if ($sid -eq "") { $MEMBERS_BASE + "&season_id=" } else { $MEMBERS_BASE + "&season_id=$sid" }
        $req7 = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::Get, $url)
        $req7.Headers.TryAddWithoutValidation("Referer", "$BASE_URL/bsyc/members/reports") | Out-Null
        $r7 = $http.SendAsync($req7).Result
        $ct7 = $r7.Content.Headers.ContentType?.MediaType
        $body7 = $r7.Content.ReadAsStringAsync().Result
        $lines7 = ($body7 -split "`n").Count
        $first7 = ($body7 -split "`n" | Select-Object -First 1).Substring(0, [Math]::Min(120, ($body7 -split "`n" | Select-Object -First 1).Length))
        Write-Host "  season_id=$sidLabel  status=$($r7.StatusCode)  type=$ct7  lines=$lines7"
        Write-Host "    header: $first7"
    }
}

$http.Dispose()
