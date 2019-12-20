# TagHelperPack [![Build status](https://damianedwards.visualstudio.com/TagHelperPack/_apis/build/status/build/TagHelperPack-ASP.NET%20Core-CI)](https://damianedwards.visualstudio.com/TagHelperPack/_build/latest?definitionId=1) [![NuGet](https://img.shields.io/nuget/v/TagHelperPack?logo=nuget)](https://www.nuget.org/packages/TagHelperPack/)

A set of useful, and possibly opinionated, Tag Helpers for ASP.NET Core (all versions).

## Included Tag Helpers & Examples
See the examples page at https://taghelperpack.net

Supports ASP.NET Core 2.1.x, 2.2.x, and 3.0.x

## Installing
1. Add a reference to the package from the cmd line:
    ```
    MyGreatProject> dotnet add package TagHelperPack
    ```
1. Restore:
    ```
    MyGreatProject> dotnet restore
    ```
1. Register the Tag Helpers in your application's `_ViewImports.cshtml` file:
    ```
   @addTagHelper *, TagHelperPack
    ```
