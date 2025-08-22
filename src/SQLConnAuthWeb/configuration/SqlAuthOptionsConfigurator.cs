using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sorling.SqlConnAuthWeb.authentication;
using Sorling.SqlConnAuthWeb.helpers;

namespace Sorling.SqlConnAuthWeb.configuration;

/// <summary>
/// Binds the AllowedIPAddresses property from configuration (array or comma-separated string) to IPAddressRangeList.
/// </summary>
public sealed class SqlAuthOptionsConfigurator(IConfiguration configuration) : IConfigureOptions<SqlAuthOptions>
{
   private readonly IConfiguration _configuration = configuration;
   private const string _sectionName = "SqlAuthOptions";

   public void Configure(SqlAuthOptions options) {
      IConfigurationSection section = _configuration.GetSection(_sectionName);
      section.Bind(options);
      // Custom binding for AllowedIPAddresses
      IConfigurationSection iplistsection = section.GetSection(nameof(SqlAuthOptions.AllowedIPAddresses));
      if (iplistsection.Exists())
      {
         IPAddressRangeList iplist = [];
         string[] entries = iplistsection.Get<string[]>() ?? [];
         foreach (string entry in entries)
         {
            if (!string.IsNullOrWhiteSpace(entry))
               iplist.Add(entry);
         }

         options.AllowedIPAddresses = iplist;
      }
      // Custom binding for IncludeDatabaseFilter
      IConfigurationSection includedbsection = section.GetSection(nameof(SqlAuthOptions.IncludeDatabaseFilter));
      if (includedbsection.Exists())
      {
         CaseInsensitiveStringSet includeset = [];
         string[] entries = includedbsection.Get<string[]>() ?? [];
         foreach (string entry in entries)
         {
            if (!string.IsNullOrWhiteSpace(entry))
               _ = includeset.Add(entry);
         }

         options.IncludeDatabaseFilter = includeset;
      }
      // Custom binding for ExcludeDatabaseFilter
      IConfigurationSection excludedbsection = section.GetSection(nameof(SqlAuthOptions.ExcludeDatabaseFilter));
      if (excludedbsection.Exists())
      {
         CaseInsensitiveStringSet excludeset = new();
         string[] entries = excludedbsection.Get<string[]>() ?? [];
         foreach (string entry in entries)
         {
            if (!string.IsNullOrWhiteSpace(entry))
               _ = excludeset.Add(entry);
         }

         options.ExcludeDatabaseFilter = excludeset;
      }
   }
}
