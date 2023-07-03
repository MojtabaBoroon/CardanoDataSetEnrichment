using CardanoDataSetEnrichment.Application;
using CardanoDataSetEnrichment.Application.Abstractios;
using CardanoDataSetEnrichment.Infrastructure.ExternalServices;
using CardanoDataSetEnrichment.Infrastructure.ExternalServices.Abstractions;
using CardanoDataSetEnrichment.Infrastructure.FileReader.Abstractios;
using CardanoDataSetEnrichment.Infrastructure.FileReader.InputParsers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<ITransactionEnrichment, TransactionEnrichment>();
builder.Services.AddTransient<IGLeifApiClient, GLeifApiClient>();
builder.Services.AddTransient<ITransactionParser, TransactionParser>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }