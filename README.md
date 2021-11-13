# libanvl.dotnet
Some dotnet utilities

[![.NET 6](https://github.com/libanvl/libanvl.dotnet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/libanvl/libanvl.dotnet/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/libanvl/libanvl.dotnet/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/libanvl/libanvl.dotnet/actions/workflows/codeql-analysis.yml)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/libanvl.opt?label=libanvl.opt)](https://www.nuget.org/packages/libanvl.opt/)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/libanvl.uuid?label=libanvl.uuid)](https://www.nuget.org/packages/libanvl.uuid/)

## libanvl.Opt

A null-free optional value library for #nullabe enable contexts

```csharp
class Car
{
	public string Driver { get; set;}
}

public void AcceptOptionalValue(Opt<Car> optCar, Opt<string> optName)
{
	if (optCar is Opt<Car>.Some someCar)
	{
		someCar.Value.Driver = optName.SomeOrDefault("Default Driver");
	}

	if (optCar.IsNone)
	{
		throw new Exception();
	}

	// or use Unwrap() to throw for None

	Car bcar = optCar.Unwrap();
}

public void RunCarOperations()
{
	var acar = new Car();
	AcceptOptionalValue(acar, "Rick");

	Car? nocar = null;
	AcceptOptionalValue(nocar.WrapOpt(), None.String)

	// use Select to project to an Opt of an inner property
	Opt<string> driver = acar.Select(x => x.Driver);
}

public void OptsOfEnumerablesAreIterable<T>(Opt<List<T>> optList)
{
	// if optList is None, the enumerable is empty, not null
	foreach (T item in optList)
	{
		Console.WriteLine(item);
	}

	// this is equivalent
	foreach (T item in optList.SomeOrEmpty())
	{
		Console.WriteLine(item);
	}
}
```

## libanvl.UUID

```csharp

public Guid GetWindowsTerminalNamespacedProfileGuid(string profileName)
{
    Guid TerminalNamespace = new("2BDE4A90-D05F-401C-9492-E40884EAD1D8");
	return UUID.V(TerminalNamespace, profileName);
}
```
