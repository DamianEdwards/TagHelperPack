using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyModel;

namespace TagHelperPack.Sample.Services
{
    public class AspNetCoreVersion
    {
        private readonly IHostingEnvironment _env;
        private string _version;

        public AspNetCoreVersion(IHostingEnvironment env)
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
                    if (framework.StartsWith(".NET Framework"))
                    {
                        _version += " on " + framework;
                    }
                    else
                    {

                        _version += " on " + ".NET Core " + GetCoreFrameworkVersion();
                    }
                }

                return _version;
            }
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
                    // Assemly path contains the shared framework name, the version is the dir above the assembly
                    var parentDirName = Directory.GetParent(aspNetCorePath).Name;
                    if (Version.TryParse(parentDirName, out Version aspNetCoreVersion))
                    {
                        return aspNetCoreVersion.ToString();
                    }
                }
            }
            catch (Exception) { }
            
            
            // Just use the version of the Microsoft.AspNetCore assembly
            return aspNetCoreAssembly.GetName().Version.ToString();
        }

        private string GetCoreFrameworkVersion()
        {
#if !NET461
            var fxDepsFile = AppContext.GetData("FX_DEPS_FILE") as string;

            if (!string.IsNullOrEmpty(fxDepsFile))
            {
                try
                {
                    var frameworkDir = new FileInfo(fxDepsFile) // Microsoft.NETCore.App.deps.json
                        .Directory; // (version)

                    if (Version.TryParse(frameworkDir.Name, out Version frameworkVersion))
                    {
                        return frameworkVersion.ToString();
                    }
                }
                catch (Exception)
                {
                    var appAssembly = Assembly.Load(new AssemblyName(_env.ApplicationName));
                    return DependencyContext.Load(appAssembly)
                        .RuntimeLibraries
                        .FirstOrDefault(l => string.Equals(l.Name, "Microsoft.NETCore.App", StringComparison.OrdinalIgnoreCase))
                        .Version;
                }
            }
#endif
            return null;
        }
    }
}
