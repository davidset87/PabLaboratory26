using AppCore.Interfaces;
using AppCore.Modules;
using Infrastructure;
using Infrastructure.Security;
using WebAPI.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register JWT Settings
var jwtSettings = new JwtSettings(builder.Configuration);
builder.Services.AddSingleton(jwtSettings);

// Register exception handling
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

// Add EF module
builder.Services.AddContactsEfModule(builder.Configuration);

// Add JWT authentication and authorization
builder.Services.AddJwt(jwtSettings);

// Add Core module (validators and AutoMapper)
builder.Services.AddContactsModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Run Seeders
    using (var scope = app.Services.CreateScope())
    {
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>()
            .OrderBy(s => s.Order);

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }
    }
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

app.Run();