---
_layout: landing
---

# Welcome to libanvl.UUID

Immutable, endian-aware UUID library for .NET. Also generates Version V (5) and Version IV (4) UUIDs.

## Introduction

libanvl.UUID is a powerful library designed to handle UUIDs with ease. It supports both Version V (5) Namespaced UUIDs and Version IV (4) "Random" UUIDs. The library is immutable and endian-aware, ensuring compatibility across different platforms.

## Features

- Immutable
- Endian-aware
- Generates Version V (5) Namespaced UUIDs
- Generates Version IV (4) "Random" UUIDs
- Implicit conversion to and from `System.Guid`
  - Conversion to `System.Guid` always follows platform endianess
- Implicit conversion from `byte[]`and `ReadOnlyMemory<byte>`
- Copy to new `byte[]`
- Property access to all five UUID records
- No signed ints in the API
- Enumerable as a sequence of bytes
- More constructors than you can shake a stick at

## Resources

- [NuGet Packages](https://www.nuget.org/packages/libanvl.uuid)
- [GitHub Repository](https://github.com/libanvl/uuid)
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
