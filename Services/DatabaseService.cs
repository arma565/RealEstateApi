using Microsoft.Data.SqlClient;

public class DatabaseService
{
    private readonly string _connectionString;
    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ShopSqlConnection")!;
    }

    /// <summary>
    /// Reset identity(id) column to 0
    /// </summary>
    /// <param name="tableName">
    /// Table name in db
    /// </param>
    public void ResetIdentity(string tableName){
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand($"DBCC CHECKIDENT ('{tableName}', RESEED, 0)" , connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}