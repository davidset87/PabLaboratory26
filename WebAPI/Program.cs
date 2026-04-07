using AppCore.Modules;
using Infrastructure;
using WebAPI.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register exception handling
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

// Add EF module (or Memory module for testing)
builder.Services.AddContactsEfModule(builder.Configuration);
// builder.Services.AddContactsMemoryModule(); // Uncomment to use Memory repositories

// Add Core module (validators and AutoMapper)
builder.Services.AddContactsModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

app.Run();