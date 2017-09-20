using System;
using System.Linq;
using System.Reflection;
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
                        .FirstOrDefault(l => string.Equals(l.Name, "Microsoft.AspNetCore.Hosting", StringComparison.OrdinalIgnoreCase))
                        .Version;
                }
                
                return _version;
            }
        }
    }
}
