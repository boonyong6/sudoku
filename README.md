# Sudoku
[![codecov](https://codecov.io/github/boonyong6/sudoku/graph/badge.svg?token=0PY68EXOKC)](https://codecov.io/github/boonyong6/sudoku)

## Commands

**Code Coverage Overview**

`coverlet bin\Debug\net6.0\Sudoku.Tests.dll --target "dotnet" --targetargs "test --no-build"`

**Test Results Output (cobertura)**

`dotnet test --collect:"XPlat Code Coverage"`

**Generate Code Coverage Report (Web Pages)**

`reportgenerator -reports:"TestResults\{guid}\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html`
