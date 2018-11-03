# TagHelperPack [![Build status](https://ci.appveyor.com/api/projects/status/6943lgsvtg2c8jcu/branch/master?svg=true)](https://ci.appveyor.com/project/DamianEdwards/taghelperpack/branch/master) [![MyGet Pre Release](https://img.shields.io/myget/taghelperpack-ci/vpre/TagHelperPack.svg)](https://www.myget.org/gallery/taghelperpack-ci)

A set of useful, and possibly opinionated, Tag Helpers for ASP.NET Core (all versions).

## Included Tag Helpers & Examples
See the examples page at https://taghelperpack.azurewebsites.net

Supports ASP.NET Core 1.0.x, 1.1.x, and 2.1.x

## Installing
Add the MyGet feed to your project's NuGet configuration:
1. Update (or create if it doesn't exist) your solution's `nuget.config` file (next to the .sln file) to include the [MyGet feed](https://www.myget.org/gallery/taghelperpack-ci):
    ``` xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
    <packageSources>
        <add key="TagHelperPack-CI" value="https://www.myget.org/F/taghelperpack-ci/api/v3/index.json" protocolVersion="3" />
    </packageSources>
    </configuration>
    ```
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
