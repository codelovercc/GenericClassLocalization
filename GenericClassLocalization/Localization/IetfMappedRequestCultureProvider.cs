using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;

namespace GenericClassLocalization.Localization;

/// <summary>
/// Map culture alias to IETF RFC 4646
/// </summary>
/// <remarks>
/// etc.: zh-CN to zh-hans-CN <br/>
/// See：https://learn.microsoft.com/zh-cn/dotnet/api/system.globalization.cultureinfo?view=net-7.0#cultureinfo-and-cultural-data
/// Issues: https://github.com/dotnet/msbuild/issues/7331
/// https://github.com/dotnet/runtime/issues/12263
/// https://github.com/dotnet/sdk/issues/11423
/// </remarks>
public sealed class IetfMappedRequestCultureProvider : RequestCultureProvider
{
    /// <summary>
    /// You can use a Options instead of this static field
    /// </summary>
    private static readonly Dictionary<string, string> Mapper = new()
    {
        { "zh-CN", "zh-hans-CN" },
        { "zh-TW", "zh-hant-TW" },
        { "zh-HK", "zh-hant-HK" },
        { "zh-SG", "zh-hans-SG" },
        { "zh-MO", "zh-hant-MO" }
    };

    public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        if (Options == null)
        {
            throw new InvalidOperationException("The RequestLocalizationOptions can not be null.");
        }

        ProviderCultureResult? result = null;
        foreach (var provider in Options.RequestCultureProviders.Where(provider =>
                     provider is not IetfMappedRequestCultureProvider))
        {
            if ((result = await provider.DetermineProviderCultureResult(httpContext)) is not null)
            {
                break;
            }
        }

        if (result is null)
        {
            return result;
        }

        ReplaceWithMappedCulture(result.Cultures, Mapper);
        ReplaceWithMappedCulture(result.UICultures, Mapper);
        return result;
    }

    private static void ReplaceWithMappedCulture(IList<StringSegment> cultures,
        IReadOnlyDictionary<string, string> mapper)
    {
        for (var index = 0; index < cultures.Count; index++)
        {
            var c = cultures[index];
            if (!c.HasValue)
            {
                continue;
            }

            if (mapper.TryGetValue(c.Value.ToLowerInvariant(), out var realCulture))
            {
                cultures[index] = realCulture;
            }
        }
    }
}