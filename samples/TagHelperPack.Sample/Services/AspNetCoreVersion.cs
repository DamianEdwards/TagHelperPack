using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;

namespace TagHelperPack.Sample.Services;

public class AspNetCoreVersion
{
#if NET6_0_OR_GREATER
    private readonly IHostEnvironment _env;
#else
    private readonly IHostingEnvironment _env;
#endif
    private string _version;
    private string _osName;

#if NET6_0_OR_GREATER
    public AspNetCoreVersion(IHostEnvironment env)
#else
    public AspNetCoreVersion(IHostingEnvironment env)
#endif
    {
        _env = env;
    }

    public string CurrentVersion
    {
        get
        {
            if (_version == null)
            {
                _version = GetAspNetCoreVersion();

                var framework = RuntimeInformation.FrameworkDescription;

                _version += " on " + framework;
            }

            return _version;
        }
    }

    public string OperatingSystemName
    {
        get
        {
            if (_osName == null)
            {
#if NET471_OR_GREATER
                _osName = Environment.OSVersion.Platform.ToString();
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    _osName = "Windows";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    _osName = "Linux";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    _osName = "macOS";
                }
                else
                {
                    _osName = "unknown";
                }
#endif
            }

            return _osName;
        }
    }

    private static string GetSharedFrameworkVersion(string frameworkPath)
    {
        var versionFilePath = Path.Combine(frameworkPath, ".version");
        if (File.Exists(versionFilePath))
        {
            var versionFile = new FileInfo(versionFilePath);

            using (var fileStream = new StreamReader(versionFile.OpenRead()))
            {
                // Version is 2nd line of the .version file
                fileStream.ReadLine();
                var secondLine = fileStream.ReadLine();
                return secondLine;
            }
        }
        return string.Empty;
    }

    private string GetAspNetCoreVersion()
    {
        var aspNetCoreAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => string.Equals(a.GetName().Name, "Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase));

        // Get AssemblyInformationalVersion
        var assemblyInformationalVersion = aspNetCoreAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (assemblyInformationalVersion != null)
        {
            var indexOfPlus = assemblyInformationalVersion.IndexOf('+');
            return indexOfPlus >= 0
                ? assemblyInformationalVersion.Substring(0, indexOfPlus)
                : assemblyInformationalVersion;
        }

        // Try shared framework directory name
        try
        {    
            var aspNetCorePath = aspNetCoreAssembly.Location;
            if (aspNetCorePath.IndexOf($"dotnet{Path.DirectorySeparatorChar}shared{Path.DirectorySeparatorChar}Microsoft.AspNetCore.App", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return GetSharedFrameworkVersion(Path.GetDirectoryName(aspNetCorePath));
            }
        }
        catch (Exception) { }

        // Just use the version of the Microsoft.AspNetCore assembly
        return aspNetCoreAssembly.GetName().Version.ToString();
    }
}
