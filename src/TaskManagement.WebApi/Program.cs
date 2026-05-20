using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Seeding;
using TaskManagement.WebApi.Extensions;
using TaskManagement.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiVersioningSupport()
    .AddSwaggerDocumentation()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .Services
    .AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<MasterDataSeederOrchestrator>();
    await seeder.SeedAsync();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}
else
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
