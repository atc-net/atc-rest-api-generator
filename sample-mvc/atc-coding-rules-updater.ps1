Clear-Host
Write-Host "Update atc-coding-rules"
dotnet tool update -g atc-coding-rules-updater

$currentPath = Get-Location

atc-coding-rules-updater run `
-p $currentPath `
--optionsPath $currentPath'\atc-coding-rules-updater.json' `
--verbose