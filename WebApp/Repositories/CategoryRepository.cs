using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Repositories;

public class CategoryRepository
{
    string connectionString;

    public CategoryRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("Shop") ?? throw new Exception("Not found Shop");
    }

    public IEnumerable<Category> GetCategories()
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        
        return connection.Query<Category>("Select * From Category");
    }
}