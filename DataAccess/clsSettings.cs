using Microsoft.Extensions.Configuration;

namespace Portfolio.DataAccess
{
    public static class clsSettings
    {
        public static string ConnectionString { get; set; } = string.Empty;

        public static void SetConnectionString(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
    }
}
