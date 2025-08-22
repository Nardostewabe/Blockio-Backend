using System.Data;
using MySql.Data.MySqlClient;

namespace Blockio_Api.Services
{
    public class DatabaseServices
    {
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public DatabaseServices(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("DefaultConnection");
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
}
