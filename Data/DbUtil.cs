using Microsoft.EntityFrameworkCore;

namespace FightArchive.Data;

public class DbUtil
{
    public static async Task InitDb(DbContextOptions<DataContext> options)
    {
        LoggerFactory factory = new();
        DbContextOptionsBuilder<DataContext> builder = new DbContextOptionsBuilder<DataContext>(options)
            .UseLoggerFactory(factory);

        using DataContext context = new(builder.Options);

        if (!Path.Exists("./db"))
            Directory.CreateDirectory("./db");

        await context.Database.EnsureCreatedAsync();
    }
}