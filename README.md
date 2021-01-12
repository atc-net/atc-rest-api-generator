### General Project Info
[![Github top language](https://img.shields.io/github/languages/top/atc-net/atc-rest-api-generator)](https://github.com/atc-net/atc-rest-api-generator)
[![Github stars](https://img.shields.io/github/stars/atc-net/atc-rest-api-generator)](https://github.com/atc-net/atc-rest-api-generator)
[![Github forks](https://img.shields.io/github/forks/atc-net/atc-rest-api-generator)](https://github.com/atc-net/atc-rest-api-generator)
[![Github size](https://img.shields.io/github/repo-size/atc-net/atc-rest-api-generator)](https://github.com/atc-net/atc-rest-api-generator)
[![Issues Open](https://img.shields.io/github/issues/atc-net/atc-rest-api-generator.svg?logo=github)](https://github.com/atc-net/atc-rest-api-generator/issues)

### Packages
[![Github Version](https://img.shields.io/static/v1?logo=github&color=blue&label=github&message=latest)](https://github.com/orgs/atc-net/packages?repo_name=atc-rest-api-generator)
[![NuGet Version](https://img.shields.io/nuget/v/atc-api-gen.svg?logo=nuget)](https://www.nuget.org/profiles/atc-net)

### Build Status
![Pre-Integration](https://github.com/atc-net/atc-rest-api-generator/workflows/Pre-Integration/badge.svg)
![Post-Integration](https://github.com/atc-net/atc-rest-api-generator/workflows/Post-Integration/badge.svg)
![Release](https://github.com/atc-net/atc-rest-api-generator/workflows/Release/badge.svg)

### Code Quality
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=atc-rest-api-generator&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=atc-rest-api-generator)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=atc-rest-api-generator&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=atc-rest-api-generator)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=atc-rest-api-generator&metric=security_rating)](https://sonarcloud.io/dashboard?id=atc-rest-api-generator)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=atc-rest-api-generator&metric=bugs)](https://sonarcloud.io/dashboard?id=atc-rest-api-generator)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=atc-rest-api-generator&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=atc-rest-api-generator)

# ATC.Net

## Projects in the repository

|Project|Target Framework|Description|Docs|Nuget Download Link|
|---|---|---|---|---|
|[Atc.Rest.ApiGenerator](src/Atc.Rest.ApiGenerator)|netstandard2.1|Atc.Rest.ApiGenerator is a WebApi C# code generator using a OpenApi 3.0.x specification YAML file.|[References](docs/CodeDoc/Atc.Rest.ApiGenerator/Index.md)<br/>[References extended](docs/CodeDoc/Atc.Rest.ApiGenerator/IndexExtended.md)|[![Nuget](https://img.shields.io/nuget/dt/Atc.Rest.ApiGenerator?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Atc.Rest.ApiGenerator)|
|[Atc.Rest.ApiGenerator.CLI](src/Atc.Rest.ApiGenerator.CLI)|netcoreapp3.1|A CLI tool that use Atc.Rest.ApiGenerator to create/update a project specified by a OpenApi 3.0.x specification YAML file.||[![Nuget](https://img.shields.io/nuget/dt/atc-api-gen?logo=nuget&style=flat-square)](https://www.nuget.org/packages/atc-api-gen)|

## CLI Tools

REST API generator, please go to [Atc.Rest.ApiGenerator.CLI](src/Atc.Rest.ApiGenerator.CLI)

# The workflow setup for this repository
[Read more on Git-Flow](docs/GitFlow.md)

# Contributing

Please refer to each project's style and contribution guidelines for submitting patches and additions. In general, we follow the "fork-and-pull" Git workflow. [Read more here](https://gist.github.com/Chaser324/ce0505fbed06b947d962).

 1. **Fork** the repo on GitHub
 2. **Clone** the project to your own machine
 3. **Commit** changes to your own branch
 4. **Push** your work back up to your fork
 5. Submit a **Pull request** so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Coding Guidelines

This repository is adapting the [ATC-Coding-Rules](https://github.com/atc-net/atc-coding-rules) which is defined and based on .editorconfig's and a range of Roslyn Analyzers.
