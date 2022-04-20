using Microsoft.Extensions.Configuration;

namespace MyTemplate.Data
{
    public class DataSettings
    {
        private readonly IConfiguration _config;

        public DataSettings(IConfiguration config)
        {
            _config = config;
        }

        public string DbConnectionString => _config.GetValue<bool>(DemoDbConstants.SqlServerOptions.UseAdminConnectionString)
            ? _config.GetConnectionString(DemoDbConstants.ConnectionStringName.Admin)
            : _config.GetConnectionString(DemoDbConstants.ConnectionStringName.Default);

        public int SqlServerMaxRetryCount => _config.GetValue(DemoDbConstants.SqlServerOptions.MaxRetryCountValue, 20);
        public int SqlServerMaxDelay => _config.GetValue(DemoDbConstants.SqlServerOptions.MaxDelayValue, 500);
        public bool UseRetries => _config.GetValue(DemoDbConstants.SqlServerOptions.UseRetries, false);
    }
}