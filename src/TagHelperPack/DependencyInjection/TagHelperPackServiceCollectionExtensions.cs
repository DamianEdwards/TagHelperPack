using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using TagHelperPack;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions for TagHelperPack.
    /// </summary>
    public static class TagHelperPackServiceCollectionExtensions
    {
        /// <summary>
        /// Add optional services to optimize TagHelperPack.
        /// <list type="bullet">
        ///   <item>Registers <see cref="ModelHtmlHelper"/> as <see cref="IHtmlHelper"/> and <see cref="IModelHtmlHelper"/>.</item>
        /// </list>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns><paramref name="services"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
        public static IServiceCollection AddTagHelperPack(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IHtmlHelper, ModelHtmlHelper>();
            services.AddTransient<IModelHtmlHelper, ModelHtmlHelper>();

            return services;
        }
    }
}
