using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyModel;

namespace TagHelperPack.Sample.Services
{
    public class AspNetCoreVersion
    {
#if NETCOREAPP3_1 || NET5_0
        private readonly IHostEnvironment _env;
#else
        private readonly IHostingEnvironment _env;
#endif
        private string _version;

#if NETCOREAPP3_1 || NET5_0
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

                    // .NET Core 1.x and 2.x both report themselves as .NET Core 4.x :(
                    if (framework.StartsWith(".NET Framework") || !framework.StartsWith(".NET Core 4."))
                    {
                        _version += " on " + framework;
                    }
                    else
                    {
                        _version += " on " + ".NET Core " + GetPre30CoreFrameworkVersion();
                    }
                }

                return _version;
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

        private string GetPre30CoreFrameworkVersion()
        {
#if !NET461
            // Try and get version directly from AppContext data
            var fxProductVersion = AppContext.GetData("FX_PRODUCT_VERSION") as string;
            if (!string.IsNullOrEmpty(fxProductVersion))
            {
                return fxProductVersion;
            }

            // Try and parse version from shared framework folder
            var fxDepsFile = AppContext.GetData("FX_DEPS_FILE") as string;
            if (!string.IsNullOrEmpty(fxDepsFile))
            {
                try
                {
                    var frameworkDirPath = Path.GetDirectoryName(fxDepsFile);
                    return GetSharedFrameworkVersion(frameworkDirPath);
                }
                catch (Exception) { }

                // Fallback to just getting version from dependency context
                var appAssembly = Assembly.Load(new AssemblyName(_env.ApplicationName));
                return DependencyContext.Load(appAssembly)
                    .RuntimeLibraries
                    .FirstOrDefault(l => string.Equals(l.Name, "Microsoft.NETCore.App", StringComparison.OrdinalIgnoreCase))
                    .Version;
            }
#endif
            return null;
        }
    }
}
