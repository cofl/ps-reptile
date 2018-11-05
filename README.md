# PS-Reptile (a MAML generator for binary PowerShell modules)

Unlike script modules, authoring help content for binary modules often involves manually authoring a `.xml` help file in [MAML](https://en.wikipedia.org/wiki/Microsoft_Assistance_Markup_Language) (Microsoft Assistance Markup Language) format.
If, like me, you think this [sounds incredibly tedious](https://msdn.microsoft.com/en-us/library/bb525433.aspx#code-snippet-1), then perhaps this tool will be useful. It examines the primary assembly for a binary module (and, optionally, its XML documentation comments) and generates the corresponding MAML help content.

## Custom attribute example 1 (inline help)

```csharp
[Cmdlet(VerbsCommon.Get, "Foo")]
[OutputType(typeof(FooConnectionProfile))]
[CmdletHelp("Retrieve information about one or more Foo connection profiles.")]
public class GetFooConnection
{
    [Parameter(HelpMessage = "Retrieve all connection profiles")]
    public SwitchParameter All { get; set; }
}
```

## Custom attribute example 1 (inline help)

```csharp
[Cmdlet(VerbsCommon.Get, "Foo")]
[OutputType(typeof(FooConnectionProfile))]
[CmdletHelp(
    Synopsis = "Retrieve information about one or more Foo connection profiles",
    Description = @"
        This Cmdlet retrieves information about Foo connection profiles.
        The connection profiles are persisted in $HOME/.foo/connection-profiles.json.
    "
)]
public class GetFooConnection
{
    [Parameter]
    public SwitchParameter All { get; set; }
}
```

## XML documentation comments example

```csharp
/// <summary>Retrieve information about one or more Foo connection profiles.</summary>
/// <returns>
///    <see cref="FooConnectionProfile">A foo connection profile.</see>
/// </returns>
[Cmdlet(VerbsCommon.Get, "Foo")]
public class GetFooConnection
{
    /// <summary>Retrieve all connection profiles.</summary>
    [Parameter]
    public SwitchParameter All { get; set; }
}
```

## Mix-and-match example

```csharp
/// <summary>Retrieve information about one or more Foo connection profiles.</summary>
/// <returns>
///    <see cref="FooConnectionProfile">A connection profile type.</see>
///    <para>A foo connection profile.</para>
/// </returns>
[Cmdlet(VerbsCommon.Get, "Foo")]
[OutputType(typeof(FooConnectionProfile))]
public class GetFooConnection
{
    [Parameter(HelpMessage = "Retrieve all connection profiles")]
    public SwitchParameter All { get; set; }
}
```

# Usage

Install package `PSReptile` into your project; it contains only attribute definitions that you can apply to your Cmdlets. The generator is part of the `PSReptile.Generator` package, or just add `PSReptile.Build` as a design-time package to automatically generate help when you build your project.

# Notes

This project is very much a work-in-progress:

* If you have questions or comments, feel free to raise an issue.
* If you'd like to pitch in, any and all assistence will be greatly appreciated :-)

Eventually, we'll also implement a `dotnet` command-line plugin to invoke this tool.
