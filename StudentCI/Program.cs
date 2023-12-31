using Microsoft.EntityFrameworkCore;
using Npgsql;
using StudentCI.Data;

internal class Program
{
    private static bool checked1 = false;
    private static bool checked2 = false;
    private static string dbName = "testDb";
    // private const string CONNECTION_STRING = "Host=localhost:5455;" + "Username=postgres;" + "Password=example;" + "Database=my_db";
    private const string CONNECTION_STRING1 = "Host=localhost:5455;" + "Username=postgres;" + "Password=example;"; // to run app locally using docker postgres
    private const string CONNECTION_STRING2 = "Host=localhost:5455;" + "Username=postgres;" + "Password=example;" + "Database=my_db"; // to run app locally using docker postgres
    private static string CONNECTION1 = "Host=db;Username=postgres;Password=example";
    private static string CONNECTION2 = "Host=db;Database=testDb;Username=postgres;Password=example";
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        //builder.Services.AddDbContext<SchoolContext>(options =>
        //   options.UseNpgsql(builder.Configuration.GetConnectionString("SchoolContext")));
        try
        {
            builder.Services.AddDbContext<SchoolContext>(options =>
             options.UseNpgsql(CONNECTION2));
            if (!checked1)
            {
                CreateDatabase(CONNECTION2);
                checkProductTable(CONNECTION2);
            }

            // options.UseNpgsql(GetConnectionString(builder.Configuration.GetConnectionString("SchoolContext"))));
            //  options.UseNpgsql(builder.Configuration.GetConnectionString("SchoolContext")));
            //  m1(builder.Configuration.GetConnectionString("SchoolContext"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                // add 10 seconds delay to ensure the db server is up to accept connections
                // this won't be needed in real world application
                Thread.Sleep(10000);
                var context = services.GetRequiredService<SchoolContext>();
                var created = context.Database.EnsureCreated();

            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();


        static void CreateDatabase(string connStr)
        {
            try
            {
                // connStr = "Server=localhost;Host=localhost:5455;Port=5455;User Id=postgres;Password=example;";
                var connection = new NpgsqlConnection(connStr);
                connection.Open();

                var c1 = "SELECT datname FROM pg_database;";
                using var c11 = new NpgsqlCommand(c1, connection);
                var result1 = c11.ExecuteScalar();

                using var checkIfExistsCommand = new NpgsqlCommand($"SELECT 1 FROM pg_catalog.pg_database WHERE datname = '{dbName}'", connection);
                var result = checkIfExistsCommand.ExecuteScalar();

                if (result == null)
                {
                    using var command = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", connection);
                    command.ExecuteNonQuery();
                }

                //var m_createdb_cmd = new NpgsqlCommand(@"CREATE DATABASE IF NOT EXISTS testDb WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;", m_conn);
                //m_conn.Open();
                //m_createdb_cmd.ExecuteNonQuery();
                //m_conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void checkProductTable(string conn)
        {
            var connection = new NpgsqlConnection(conn);
            connection.Open();

            if (!checked2)
            {
                var checkBirdsCommand = "SELECT EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'birds') AS table_existence;";
                NpgsqlCommand sqlCmd = new NpgsqlCommand(checkBirdsCommand, connection);

                var res = (bool)sqlCmd.ExecuteScalar();

                if (res == false)
                {
                    var cmd = new NpgsqlCommand();
                    cmd.CommandText = "CREATE TABLE birds (Id SERIAL PRIMARY KEY," +
                    "Name VARCHAR(255))";
                    using var command = new NpgsqlCommand(cmd.CommandText, connection);
                    command.ExecuteNonQuery();                    
                }

                //cmd.CommandText = $"DROP TABLE IF EXISTS birds";   
                //using var command1 = new NpgsqlCommand(cmd.CommandText, connection);
                //command1.ExecuteNonQuery();

                /////////////////////////////////

                var checkProductsCommand = "SELECT EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'products') AS table_existence;";
                NpgsqlCommand sqlCmdP = new NpgsqlCommand(checkProductsCommand, connection);

                var resP = (bool)sqlCmdP.ExecuteScalar();

                if (resP == false)
                {
                    var cmd = new NpgsqlCommand();
                    cmd.CommandText = "CREATE TABLE products (Id SERIAL PRIMARY KEY," +
                    "name VARCHAR(255)," +
                    "category VARCHAR(255)," +
                    "price INT" +
                    ")";
                    using var command = new NpgsqlCommand(cmd.CommandText, connection);
                    command.ExecuteNonQuery();
                }

                checked2 = true;
                connection.Close();
            }
        }
    }
}