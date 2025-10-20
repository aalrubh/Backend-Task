using System.Data;
using Microsoft.Data.SqlClient;

namespace MyApp.Data;

public class SqlServerConnectionProvider : ISqlServerConnectionProvider
{
    private readonly string _connectionString;
    
    public SqlServerConnectionProvider(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TestConnection")!;
    }
    
    public IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}