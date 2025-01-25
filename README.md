[![CI](https://github.com/libanvl/uuid/actions/workflows/libanvl-dotnet-ci.yml/badge.svg?branch=main)](https://github.com/libanvl/uuid/actions/workflows/libanvl-dotnet-ci.yml)
[![CodeQL](https://github.com/libanvl/uuid/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/libanvl/uuid/actions/workflows/github-code-scanning/codeql)
[![NuGet (with prereleases)](https://img.shields.io/nuget/vpre/libanvl.uuid?label=libanvl.uuid)](https://www.nuget.org/packages/libanvl.uuid/)

# libanvl.UUID

libanvl.UUID is an immutable, endian-aware UUID library for .NET. It supports generating Version IV (4), Version V (5), Version VII (7), and Version VIII (8) UUIDs. This library is designed to be highly performant and easy to use, providing seamless integration with .NET applications.

## Requirements

- [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)

## Installation

You can install the libanvl.UUID library via NuGet:

dotnet add package libanvl.uuid

For CI builds, you can use the GitHub feed:

dotnet nuget add source --username USERNAME --password TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/libanvl/index.json"
dotnet add package libanvl.uuid --prerelease

## Releases

- NuGet packages are available on [NuGet.org](https://www.nuget.org/packages/libanvl.uuid)
  - Embedded debug symbols
  - Source Link enabled
- NuGet packages from CI builds are available on the [libanvl GitHub feed](https://github.com/libanvl/uuid/packages/)

## Features

- [X] Immutable
- [X] Endian-aware
- [X] Generates Version IV (4) "Random" UUIDs
- [X] Generates Version V (5) Namespaced UUIDs
- [X] Generates Version VII (7) "Unix Timestamp" UUIDs
- [X] Generates Version VIII (8) "Custom" UUIDs
- [X] Implicit conversion to and from `System.Guid`
  - [X] Conversion to `System.Guid` always follows platform endianess
- [X] Implicit conversion from `byte[]` and `ReadOnlyMemory<byte>`
- [X] Copy to new `byte[]`
- [X] Property access to all five UUID records
- [X] No signed ints in the API
- [X] Enumerable as a sequence of bytes 
- [X] More constructors than you can shake a stick at

## Usage

Here are some examples of how to use the libanvl.UUID library:

### Generate a Namespaced UUID

```csharp
public Guid GetWindowsTerminalNamespacedProfileGuid(string profileName)
{
    Guid terminalNamespace = new("2BDE4A90-D05F-401C-9492-E40884EAD1D8");
    return UUID.V(terminalNamespace, profileName);
}
```

### Generate a Fragmented Namespaced UUID

```csharp
public Guid GetWindowsTerminalNamespacedFragmentProfileGuid(string fragmentName, string profileName)
{
    Guid fragmentNamespace = new("f65ddb7e-706b-4499-8a50-40313caf510a");
    // Guid can be implicitly converted to UUID with endianess that matches the platform
    UUID fragmentUUID = UUID.V(fragmentNamespace, fragmentName);
    return UUID.V(fragmentUUID, profileName);
}
```

### Convert to Big Endian UUID

```csharp
public UUID GetBigEndianUUID(UUID value)
{
    if (value.IsLittleEndian)
    {
        value = value.EndianSwap()
    }

    return value;
}
```
