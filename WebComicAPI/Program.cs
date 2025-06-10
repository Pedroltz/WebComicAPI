using Microsoft.EntityFrameworkCore;
using WebComicAPI.Data;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using AutoMapper;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(WebComicAPI.Helpers.MappingProfile));

// Configurar o DbContext com SQL Server
builder.Services.AddDbContext<WebComicContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configurar CORS para o Frontend (Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Inicializar Firebase
var firebaseCredentialPath = builder.Configuration["Firebase:CredentialPath"];
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebaseCredentialPath)
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");
app.UseStaticFiles();
var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
if (!Directory.Exists(imagePath))
    Directory.CreateDirectory(imagePath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagePath),
    RequestPath = "/images"
});
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
