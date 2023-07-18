using Azure.Core;
using Azure.Identity;
using Muljin.B2CMagicLink;
using Muljin.B2CMagicLink.Example.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//setup credentials
AzureAdOptions azureAdOptions = new AzureAdOptions();
builder.Configuration.GetSection("AzureAd").Bind(azureAdOptions);

builder.Services.Configure<AzureAdOptions>(
    builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<Muljin.B2CMagicLink.Example.Models.AzureAdB2cOptions>(builder.Configuration.GetSection("AzureAdB2c"));

//setup credentials
TokenCredential creds = new ClientSecretCredential(azureAdOptions!.TenantId,
    azureAdOptions!.ClientId, azureAdOptions!.ClientSecret);


//add magic link
builder.Services.AddB2CMagicLink(builder.Configuration);
builder.Services.AddB2CMagicLinkKeyVault(builder.Configuration, creds);

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

