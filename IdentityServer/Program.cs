using IdentityServer.Configs;

var builder = WebApplication.CreateBuilder(args);

// Add Identity Server and set configuration.
builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources())
    .AddInMemoryApiScopes(InMemoryConfiguration.GetApiScopes())
    .AddInMemoryClients(InMemoryConfiguration.GetClients())
    .AddDeveloperSigningCredential();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseIdentityServer();

app.MapControllers();

app.Run();
