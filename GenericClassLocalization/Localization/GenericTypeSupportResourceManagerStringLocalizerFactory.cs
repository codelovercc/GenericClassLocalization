using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace GenericClassLocalization.Localization;

public class GenericTypeSupportResourceManagerStringLocalizerFactory : ResourceManagerStringLocalizerFactory
{
    public GenericTypeSupportResourceManagerStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions,
        ILoggerFactory loggerFactory) : base(localizationOptions, loggerFactory)
    {
    }

    protected override string GetResourcePrefix(TypeInfo typeInfo, string? baseNamespace, string? resourcesRelativePath)
    {
        if (typeInfo == null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        if (string.IsNullOrEmpty(baseNamespace))
        {
            throw new ArgumentNullException(nameof(baseNamespace));
        }

        if (string.IsNullOrEmpty(typeInfo.FullName))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                "Type '{0}' must have a non-null type name.", typeInfo));
        }

        if (string.IsNullOrEmpty(resourcesRelativePath))
        {
            return typeInfo.FullName;
        }
        
        // Convert the full type name
        // MyClass<T,T1> is MyClass`2 to MyClass_2
        // MyClass<int, double> is MyClass`2[Int,Double] to MyClass_2
        // MyClass<T, T1>.MySubClass<TS, TS1> is MyClass`2+MySubClass2`2 to MyClass_2.MySubClass2_2
        var fullName = ConvertGenericFullName(typeInfo);

        return baseNamespace + "." + resourcesRelativePath + TrimPrefix(fullName, baseNamespace + ".");
    }

    private static string ConvertGenericFullName(Type typeInfo)
    {
        if (string.IsNullOrEmpty(typeInfo.FullName))
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                "Type '{0}' must have a non-null type name.", typeInfo));
        }

        string fullName;
        if (typeInfo.IsGenericType)
        {
            fullName = typeInfo.FullName.AsSpan(0, typeInfo.FullName.IndexOf('[')).ToString().Replace('`', '_')
                .Replace('+', '.');
        }
        else
        {
            fullName = typeInfo.FullName;
        }

        return fullName;
    }

    private static string TrimPrefix(string name, string prefix)
    {
        return name.StartsWith(prefix, StringComparison.Ordinal) ? name[prefix.Length..] : name;
    }
}
