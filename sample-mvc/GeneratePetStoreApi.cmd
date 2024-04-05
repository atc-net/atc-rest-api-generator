@ECHO off

CLS
SET currentDirectory=%cd%
FOR %%a IN ("%currentDirectory%") DO SET rootDirectory=%%~dpa
SET projectsRootDirectory=%rootDirectory%\sample-mvc\Code
SET slnDirectory=%projectsRootDirectory%
SET srcDirectory=%projectsRootDirectory%\src
SET testDirectory=%projectsRootDirectory%\test
SET generatorFile=%rootDirectory%src\Atc.Rest.ApiGenerator.CLI\bin\Debug\net8.0\atc-rest-api-generator.exe

SET projectName=PetStore
SET specUrl=https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml

%generatorFile% generate server all -p %projectName% -s %specUrl% --outputSlnPath %slnDirectory% --outputSrcPath %srcDirectory% --outputTestPath %testDirectory% -v