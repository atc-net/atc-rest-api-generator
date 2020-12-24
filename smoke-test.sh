#!/bin/bash

echo "Download Swagger Petstore v3 OpenAPI spec"
curl -O https://petstore3.swagger.io/api/v3/openapi.yaml

echo "Generate code"
cd src/Atc.Rest.ApiGenerator.CLI
dotnet run -- generate server all -p "Swagger Petstore v3" -s ../../openapi.yaml --outputSlnPath ../../petstore3/ --outputSrcPath ../../petstore3/src --outputTestPath ../../petstore3/test -v true
cd ../../
dotnet build ./petstore3

rm -rf ./petstore3
rm openapi.yaml