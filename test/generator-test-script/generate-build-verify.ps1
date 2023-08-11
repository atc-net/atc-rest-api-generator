Clear-Host

$originalLocation = Get-Location
$tmpFolderPath = "$(Get-Location)\tmp"
$generatorBasePath = (get-item $originalLocation).parent.parent.FullName
$cliBasePath = "$($generatorBasePath)\src\Atc.Rest.ApiGenerator.CLI"

$oldProgressPreference = $progressPreference;
$global:progressPreference = 'SilentlyContinue';

function HandleProject
{
    param (
        [Parameter(Mandatory = $true)]
        [string] $projectName,

        [Parameter(Mandatory = $true)]
        [bool] $validateStrictMode,

        [Parameter(Mandatory = $true)]
        [string] $fileLocation
    )

    Write-Host "   Creating project root folder for '$($projectName)'" -ForegroundColor Yellow
    $projectRootFolder = "$($tmpFolderPath)\$($projectName)"
    [System.IO.Directory]::CreateDirectory($projectRootFolder) | Out-Null

    if ($fileLocation.StartsWith("http","CurrentCultureIgnoreCase"))
    {
        Write-Host "   Downloading specification file for '$($projectName)'" -ForegroundColor Yellow
        Invoke-WebRequest -Uri $fileLocation -OutFile "$($tmpFolderPath)\$($projectName)\$($projectName).yaml"
    }
    else
    {
        Write-Host "   Copy specification file for '$($projectName)'" -ForegroundColor Yellow
        Copy-Item $fileLocation -Destination "$($tmpFolderPath)\$($projectName)\$($projectName).yaml"
    }

    Write-Host "   Generating server API for '$($projectName)'" -ForegroundColor Yellow
    Set-Location $cliBasePath

    if ($validateStrictMode)
    {
        dotnet run -- `
        generate server all `
        --validate-strictMode `
        -p "$($projectName)" `
        -s "$($projectRootFolder)\$($projectName).yaml" `
        --outputSlnPath "$($projectRootFolder)\$($projectName)" `
        --outputSrcPath "$($projectRootFolder)\$($projectName)\src" `
        --outputTestPath "$($projectRootFolder)\$($projectName)\test" `
        -v
    }
    else
    {
        dotnet run -- `
        generate server all `
        -p "$($projectName)" `
        -s "$($projectRootFolder)\$($projectName).yaml" `
        --outputSlnPath "$($projectRootFolder)\$($projectName)" `
        --outputSrcPath "$($projectRootFolder)\$($projectName)\src" `
        --outputTestPath "$($projectRootFolder)\$($projectName)\test" `
        -v
    }

    Write-Host "   Building '$($projectName)' project" -ForegroundColor Yellow
    Set-Location "$($projectRootFolder)\$($projectName)"

    $buildErrors = dotnet build -c Debug -v q -clp:NoSummary | out-string -stream | select-string "error"
    if ($buildErrors.Length -gt 0)
    {
        Write-Host "   Build FAILED for project '$($projectName)'" -ForegroundColor Red

        ForEach ($buildError In $buildErrors)
        {
            Write-Host "`n$($buildError)" -ForegroundColor Red
        }

        Set-Location $originalLocation
        Break;
    }

    Write-Host "   Running UnitTests for project '$($projectName)'" -ForegroundColor Yellow
    dotnet test --no-restore | Out-Null

    if($LASTEXITCODE -ne 0)
    {
        Set-Location $originalLocation
        Break;
    }
}

$projects = @(
  @{ Name = 'ATCDemo';      ValidateStrictMode = $true;     FileLocation = "$($generatorBasePath)\sample-mvc\Demo.ApiDesign\SingleFileVersion\Api.v1.yaml";},
  @{ Name = 'PetStore';     ValidateStrictMode = $false;    FileLocation = 'https://petstore3.swagger.io/api/v3/openapi.yaml';                         }
)

Write-Host "Creating tmp folder in '$($originalLocation)'" -ForegroundColor Green
[System.IO.Directory]::CreateDirectory($tmpFolderPath) | Out-Null
Set-Location $tmpFolderPath

Write-Host "Cleaning tmp folder" -ForegroundColor Green
Get-ChildItem -Path $tmpFolderPath -Recurse | Remove-Item -force -recurse

foreach ($project in $projects)
{
    Write-Host "Working on $($project.Name)" -ForegroundColor Green
    HandleProject `
        $project.Name `
        $project.ValidateStrictMode `
        $project.FileLocation
}

Write-Host "Cleaning tmp folder" -ForegroundColor Green
Get-ChildItem -Path $tmpFolderPath -Recurse | Remove-Item -force -recurse

Write-Host "Removing tmp folder" -ForegroundColor Green
[System.IO.Directory]::Delete($tmpFolderPath)

Set-Location $originalLocation
$global:progressPreference = $oldProgressPreference