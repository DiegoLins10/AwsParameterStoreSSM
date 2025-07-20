using AwsParameterStoreSSM.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var enviroment = builder.Environment.EnvironmentName.ToLower();

//adiciona os atributos do ssm
builder.Configuration.AddSystemsManager($"/{enviroment}/ssm");

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add parameter configuration to IConfig
builder.Services.Configure<AwsParameterCommon>(builder.Configuration.GetSection("Settings"));


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
