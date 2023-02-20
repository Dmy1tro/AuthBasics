
using System.Security.Cryptography;

// Generate private and public key.
if (!File.Exists("rsa_private_key"))
{
    using var rsa = RSA.Create();

    var privateKey = rsa.ExportRSAPrivateKey();
    File.WriteAllBytes("rsa_private_key", privateKey);

    var publicKey = rsa.ExportRSAPublicKey();
    File.WriteAllBytes("rsa_public_key", publicKey);
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();