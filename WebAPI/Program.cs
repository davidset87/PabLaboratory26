using AppCore.Interfaces;
using AppCore.Modules;
using AppCore.Repositories;
using Infrastructure.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
builder.Services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();
builder.Services.AddSingleton<IOrganizationRepository, MemoryOrganizationRepository>();
builder.Services.AddSingleton<IContactRepository, MemoryContactRepository>();
builder.Services.AddSingleton<ICustomerService, MemoryCustomerService>();

// Register Unit of Work
builder.Services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();

// Register Services
builder.Services.AddSingleton<IPersonService, MemoryPersonService>();

// Dodajemy moduł Contacts z walidatorami i AutoMapper
builder.Services.AddContactsModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();