param(
    [Parameter(Mandatory=$true)][string]$GitHubOwner,
    [string]$RepositoryName = 'DV-EET',
    [string]$GameDir = 'C:\Program Files (x86)\Steam\steamapps\common\Derail Valley',
    [string]$UnityDir = 'C:\Program Files\Unity\Hub\Editor\2019.4.40f1\Editor'
)
$ErrorActionPreference = 'Stop'
if ($GitHubOwner -notmatch '^[A-Za-z0-9](?:[A-Za-z0-9-]{0,37}[A-Za-z0-9])?$') { throw 'Invalid GitHub owner.' }
if ($RepositoryName -notmatch '^[A-Za-z0-9_.-]+$' -or $RepositoryName -in @('.', '..')) { throw 'Invalid repository name.' }
$source = Join-Path $PSScriptRoot 'Source'
$metaPath = Join-Path $source 'Info.json'
$meta = Get-Content -LiteralPath $metaPath -Raw | ConvertFrom-Json
if ($meta.Id -ne 'DV_EET' -or $meta.Version -notmatch '^\d+\.\d+\.\d+$') { throw 'Unexpected mod ID or version.' }
$version = $meta.Version
$archiveName = "DV_EET-$version.zip"
$repositoryHome = "https://github.com/$GitHubOwner/$RepositoryName"
$raw = "https://raw.githubusercontent.com/$GitHubOwner/$RepositoryName/main/repository.json"
$download = "https://raw.githubusercontent.com/$GitHubOwner/$RepositoryName/main/Releases/$archiveName"
# Compile first; do not alter repository metadata if the build fails.
& (Join-Path $source 'build.ps1') -GameDir $GameDir -UnityDir $UnityDir
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
$meta | Add-Member -NotePropertyName HomePage -NotePropertyValue $repositoryHome -Force
$meta | Add-Member -NotePropertyName Repository -NotePropertyValue $raw -Force
$repository = @{ Releases = @(@{ Id = 'DV_EET'; Version = $version; DownloadUrl = $download }) }
$utf8 = [Text.UTF8Encoding]::new($false)
[IO.File]::WriteAllText($metaPath, ($meta | ConvertTo-Json -Depth 5) + "`n", $utf8)
[IO.File]::WriteAllText((Join-Path $PSScriptRoot 'repository.json'), ($repository | ConvertTo-Json -Depth 5) + "`n", $utf8)
$dist = Join-Path $PSScriptRoot 'dist'
$stage = Join-Path $dist ('stage-' + [guid]::NewGuid().ToString('N'))
$modDir = Join-Path $stage 'DV_EET'
New-Item -ItemType Directory -Force -Path (Join-Path $modDir 'Item') | Out-Null
Copy-Item -LiteralPath (Join-Path $source 'DV_EET.dll') -Destination $modDir
Copy-Item -LiteralPath $metaPath -Destination (Join-Path $modDir 'info.json')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Item/item_info.json'),(Join-Path $PSScriptRoot 'Item/dv_eet_assets') -Destination (Join-Path $modDir 'Item')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Licenses/ZG2-LICENSE.txt') -Destination $modDir
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'README.md') -Destination $modDir
$zip = Join-Path $dist $archiveName
Compress-Archive -Path $modDir -DestinationPath $zip -Force
Write-Output "Prepared $zip"
Write-Output "Commit Source/Info.json, repository.json, and the ZIP as Releases/$archiveName."

