# .NET Repo Template

## Workflows

Be sure to check the workflows and remove or adjust as needed.

* `.github/workflows/libanvl-dotnet-ci.yml`
* `.github/workflows/libanvl-nuget-release.yml`
* `.github/workflows/libanvl-docfx.yml`

The default workflows use reusable workflows and composite actions from [libanvl/ci](https://github.com/libanvl/ci/)

## Required Repo or Org Secrets

* CODECOV_TOKEN for uploading coverage to [Codecov](https://about.codecov.io/)
* NUGET_PUSH_KEY for pushing to [NuGet Gallery](https://www.nuget.org/)
