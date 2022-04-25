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

        public string DbConnectionString => _config.GetValue<bool>(MyTemplateDbConstants.SqlServerOptions.UseAdminConnectionString)
            ? _config.GetConnectionString(MyTemplateDbConstants.ConnectionStringName.Admin)
            : _config.GetConnectionString(MyTemplateDbConstants.ConnectionStringName.Default);

        public int SqlServerMaxRetryCount => _config.GetValue(MyTemplateDbConstants.SqlServerOptions.MaxRetryCountValue, 20);
        public int SqlServerMaxDelay => _config.GetValue(MyTemplateDbConstants.SqlServerOptions.MaxDelayValue, 500);
        public bool UseRetries => _config.GetValue(MyTemplateDbConstants.SqlServerOptions.UseRetries, false);
    }
}