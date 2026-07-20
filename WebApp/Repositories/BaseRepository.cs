namespace WebApp.Repositories;

public abstract class BaseRepository
{
    protected string connectionString;

    public BaseRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("Shop") ?? throw new Exception("Not found db Shop"); 
    }
}