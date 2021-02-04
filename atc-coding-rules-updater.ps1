Clear-Host
Write-Host "Updating atc-coding-rules-updater tool to newest version"
dotnet tool update -g atc-coding-rules-updater

$currentPath = Get-Location

Write-Host "Running atc-coding-rules-updater to fetch updated rulesets and configurations"
atc-coding-rules-updater `
    -r $currentPath `
    --optionsPath $currentPath'\atc-coding-rules-updater.json' `
    -v true