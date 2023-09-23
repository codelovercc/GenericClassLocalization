using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace GenericClassLocalization.Localization;

public static class LocalizationServiceCollectionExtensions
{
    public static IServiceCollection AddGenericTypeSupportStringLocalizerFactory(this IServiceCollection collection)
    {
        return collection.Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, 
            GenericTypeSupportResourceManagerStringLocalizerFactory>());
    }
}