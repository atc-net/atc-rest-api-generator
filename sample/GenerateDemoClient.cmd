@ECHO off

CLS
SET currentDirectory=%cd%
FOR %%a IN ("%currentDirectory%") DO SET rootDirectory=%%~dpa
SET srcDirectory=%rootDirectory%src
SET nswagFile=%currentDirectory%\Demo.ApiDesign\nswag.json

nswag run %nswagFile%
