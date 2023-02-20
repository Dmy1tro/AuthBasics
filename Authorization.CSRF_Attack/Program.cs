using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

// Watch video about CSRF attack to get what is happening here =)
// https://www.youtube.com/watch?v=80S8h5hEwTY&t=181s&ab_channel=WebDevSimplified

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Disabling validation for testing purposes.
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateActor = false,
            ValidateIssuerSigningKey = false,
            ValidateLifetime = false,
            ValidateTokenReplay = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key-key-key-key-key-key"))
        };

        // For some reason it could be null =(
        options.Events ??= new JwtBearerEvents();
        options.Events.OnMessageReceived = context =>
        {
            // Get token from cookie.
            if (context.Request.Cookies.TryGetValue("access_token", out var token))
            {
                context.Token = token.ToString();
            }

            return Task.CompletedTask;
        };
    });
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

// Disable https for testing purposes.
// app.UseHttpsRedirection();

app.UseAuthentication();
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
            Name = "X-CSRF-Key",
            In = ParameterLocation.Header,
            Required = false
        });
    }
}