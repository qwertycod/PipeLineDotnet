using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using StudentCI.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

internal class Program
{
    private static bool checked1 = false;
    private static bool checked2 = false;
    private static string dbName = "my_db";
   // private const string CONNECTION_STRING = "Host=localhost:5455;" + "Username=postgres;" + "Password=example;" + "Database=my_db";
    private const string CONNECTION_STRING1 = "Host=localhost:5455;" + "Username=postgres;" + "Password=example;";
    private const string CONNECTION_STRING2 = "Host=localhost:5455;" + "Username=postgres;" + "Password=example;" + "Database=my_db";
    private static string CONNECTION1 = "Host=db;Username=postgres;Password=example";
    private static string CONNECTION2 = "Host=db;Database=my_db;Username=postgres;Password=example";
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
                CreateDatabase(CONNECTION1);
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
                    checked2 = true;
                }

                //cmd.CommandText = $"DROP TABLE IF EXISTS birds";   
                //using var command1 = new NpgsqlCommand(cmd.CommandText, connection);
                //command1.ExecuteNonQuery();

                var checkTableCommand = "SELECT EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'students') AS table_existence;";

                try
                {
                    NpgsqlCommand sqlCmd1 = new NpgsqlCommand(checkTableCommand, connection);

                    var res1 = (bool)sqlCmd1.ExecuteScalar();

                    if (res1 == false)
                    {
                        var text = "CREATE TABLE students (Id SERIAL PRIMARY KEY," +
                                    "LastName VARCHAR(255)," +
                                    "FirstMidName VARCHAR(255)";

                        using var command3 = new NpgsqlCommand(text, connection);
                        command3.ExecuteNonQuery();
                        checked2 = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}