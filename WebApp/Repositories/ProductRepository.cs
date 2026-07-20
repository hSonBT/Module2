using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Repositories;

public class ProductRepository : BaseRepository
{
    public ProductRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public IEnumerable<Product> GetProducts()
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        return connection.Query<Product>("SELECT * FROM product");
    }

    /// <summary>
    /// Add product via call store procedure
    /// </summary>
    public int Add(Product product)
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        return connection.Execute("AddProduct", new
        {
            product.CategoryId,
            product.ProductName,
            product.Description,
            product.Content,
            product.Price,
            product.ImageUrl,
            product.Quantity
        }, commandType: CommandType.StoredProcedure);
    }

    public Product? GetProduct(int id)
    {
        const string sql = "Select * from Product where ProductId = @id";
        using IDbConnection connection = new SqlConnection(connectionString);
        return connection.QuerySingleOrDefault<Product>(sql, new { id });
    }


    public int Delete(int id)
    {
        const string sql = "Delete Product WHERE ProductId =  @id";
        using IDbConnection connection = new SqlConnection(connectionString);
        return connection.Execute(sql, new { id });
    }

    public int Edit(Product product)
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        return connection.Execute("UpdateProduct", new
        {
            product.ProductId,
            product.CategoryId,
            product.ProductName,
            product.Description,
            product.Content,
            product.ImageUrl,
            product.Price,
            product.Quantity
        }, commandType: CommandType.StoredProcedure);
    }
}