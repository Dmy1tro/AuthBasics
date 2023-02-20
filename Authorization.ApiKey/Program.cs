using Authorization.ApiKey.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions<ApiKeyConfig>()
    .Bind(builder.Configuration.GetSection("ApiKeyConfig"))
    .Validate(config =>
    {
        return !string.IsNullOrEmpty(config.ApiKey);
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(conf =>
{
    conf.OperationFilter<SwaggerOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

// It is important to put this middleware before `UseAuthorization`, otherwise we get 401 response.
app.UseMiddleware<PreSharedKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Add possibility to input value for headers in swagger.
class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = PreSharedKeyMiddleware.PreSharedKeyHeaderName,
            In = ParameterLocation.Header,
            Required = false
        });
    }
}