using Microsoft.Extensions.Configuration;

namespace CertiBlock.Services.Certificates.Core.DAL;

public static class Extensions
{
    private const string SectionName = "mongo";
    
    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);

        return options;
    }
}