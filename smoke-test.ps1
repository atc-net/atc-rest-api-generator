Write-Host "Download Swagger Petstore v3 OpenAPI spec`n" -ForegroundColor Green
curl -O https://petstore3.swagger.io/api/v3/openapi.yaml

Write-Host "`nGenerate code`n" -ForegroundColor Green
Set-Location src/Atc.Rest.ApiGenerator.CLI
dotnet run -- generate server all -p "Swagger Petstore v3" -s ../../openapi.yaml --outputSlnPath ../../petstore3/ --outputSrcPath ../../petstore3/src --outputTestPath ../../petstore3/test -v true

Set-Location ../../
dotnet build ./petstore3

Remove-Item ./petstore3 -Recurse -Force
Remove-Item openapi.yaml