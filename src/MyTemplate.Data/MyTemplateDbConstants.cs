namespace MyTemplate.Data
{
    public static class MyTemplateDbConstants
    {
        public static class SchemaName
        {
            public const string Default = "dbo";
            public const string Integration = "integration";
        }
        public static class Migrations
        {
            public const string TableName = "__MigrationsHistory";
            public const string AssemblyName = "Template.Data";
        }

        public static class ConnectionStringName
        {
            public const string Default = "MyTemplateDbContext";
            public const string Admin = "AdminTemplateDbContext";
        }

        public static class SqlServerOptions
        {
            public const string MaxRetryCountValue = "sqlServer:maxRetryCount";
            public const string MaxDelayValue = "sqlServer:maxDelay";
            public const string UseAdminConnectionString = "sqlServer:useAdminConnectionString";
            public const string UseRetries = "sqlServer:useRetries";
        }
    }
}