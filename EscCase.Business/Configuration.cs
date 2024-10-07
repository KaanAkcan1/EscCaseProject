using EscCase.Common.Entities.App;
using Microsoft.Extensions.Configuration;

namespace EscCase.Business
{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                var connectionString = string.Empty;

                ConfigurationManager configurationManager = new ConfigurationManager();
                configurationManager.AddJsonFile("appsettings.json");

                if (configurationManager != null)
                    connectionString = configurationManager.GetConnectionString("MSSQL") ?? "";

                return connectionString;
            }
        }

        static public JwtOptions JwtOption
        {
            get
            {
                ConfigurationManager configurationManager = new ConfigurationManager();
                configurationManager.SetBasePath(Directory.GetCurrentDirectory());
                configurationManager.AddJsonFile("appsettings.json");

                var result = new JwtOptions()
                {
                    AccessTokenExpirationMinutes = Convert.ToDouble(configurationManager.GetSection("JwtSettings:AccessTokenExpirationMinutes").Value),
                    AccessTokenSecret = configurationManager.GetSection("JwtSettings:AccessTokenSecret").Value ?? "",
                    Audience = configurationManager.GetSection("JwtSettings:Audience").Value ?? "",
                    Issuer = configurationManager.GetSection("JwtSettings:Issuer").Value ?? "",
                    RefreshTokenExpirationMinutes = Convert.ToDouble(configurationManager.GetSection("JwtSettings:RefreshTokenExpirationMinutes").Value)
                };

                return result;
            }
        }

    }
}
