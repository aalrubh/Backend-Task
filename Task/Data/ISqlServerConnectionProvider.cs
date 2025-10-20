using System.Data;

namespace MyApp.Data;

public interface ISqlServerConnectionProvider
{
    public IDbConnection GetConnection();
}