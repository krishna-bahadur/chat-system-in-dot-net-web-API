using ChatHub.BLL.Services.Implementation;
using ChatHub.DAL.Datas;
using ChatHub.Dependencies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.ConfigureDbContext(configuration);

builder.Services.AddSignalR();

// Adding Authentication
builder.Services.ConfigureJwtBearerAuthentication(configuration);

//Indentity password 
builder.Services.ConfigureIdentityPassword();

builder.Services.ConfigureCors();

builder.Services.ConfigureServiceDependencies();

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatServices>("/hamrochathub");

app.MapControllers();

app.Run();
