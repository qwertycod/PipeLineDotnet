using Microsoft.EntityFrameworkCore;
using StudentCI.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<SchoolContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("SchoolContext")));
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
        System.Threading.Thread.Sleep(10000);
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