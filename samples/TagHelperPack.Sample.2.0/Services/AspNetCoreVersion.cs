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
                    var appAssembly = Assembly.Load(new AssemblyName(_env.ApplicationName));
                    _version = DependencyContext.Load(appAssembly)
                        .RuntimeLibraries
                        .FirstOrDefault(l => string.Equals(l.Name, "Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase))
                        .Version;

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
