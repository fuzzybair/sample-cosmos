using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommandProcessor
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Register controllers and DbContext
      builder.Services.AddControllers();

      // Swagger / OpenAPI
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
      builder.Services.AddDbContext<ApplicationDbContext>(options =>
          // apply snake_case naming via the Npgsql options builder overload
          options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

      var app = builder.Build();

      // Always enable Swagger UI (serves at /swagger)
      app.UseSwagger();
      app.UseSwaggerUI();

      app.MapControllers();
      app.MapGet("/", () => "Hello World!");

      app.MapPost("/", (string json, [FromServices] ILogger<Program> logger) 
        =>
      {
        logger.LogInformation("Received post at root with {@Body}", json);
      });

      // Automatic DB creation strategy:
      // 1) Try Apply Migrations (recommended)
      // 2) If that fails, try EnsureCreated()
      // 3) If that fails, generate SQL create script to a file for manual execution
      using (var scope = app.Services.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
          logger.LogInformation("Attempting to apply EF Core migrations...");
          db.Database.Migrate();
          logger.LogInformation("Migrations applied successfully.");
        }
        catch (Exception migrateEx)
        {
          logger.LogWarning(migrateEx, "Migrate failed, attempting EnsureCreated fallback...");

          try
          {
            var created = db.Database.EnsureCreated();
            logger.LogInformation("EnsureCreated result: {Created}", created);
          }
          catch (Exception ensureEx)
          {
            logger.LogError(ensureEx, "EnsureCreated failed. Attempting to generate SQL create script...");

            try
            {
              var script = db.Database.GenerateCreateScript();
              var fileName = "create_tables_commandprocessor.sql";
              var path = Path.Combine(app.Environment.ContentRootPath, fileName);
              File.WriteAllText(path, script);
              logger.LogInformation("SQL create script written to {Path}", path);
            }
            catch (Exception scriptEx)
            {
              logger.LogError(scriptEx, "Failed to generate/write SQL create script. Manual intervention required.");
            }
          }
        }
      }

      app.Run();
    }
  }
}
