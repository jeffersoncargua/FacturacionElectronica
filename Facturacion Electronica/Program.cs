using Facturacion_Electronica.Mapping;
using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Repository.Repository;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using FacturacionElectronicaSRI.Repository.Service;
using FacturacionElectronicaSRI.Repository.Service.IService;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add Provier and configuration to allow CORS Configuration
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();

// Add service for Cors configuration
builder.Services.AddCors(options =>
{
    var frontEndUrl = configuration.GetValue<string>("FrontUrl");

    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(frontEndUrl)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddControllers();

// Add SQL Services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionString:DefaultConnection"]));

// Add Automapper Services
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Add IRepository and Repository Services
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IComprobanteVentaRepository, ComprobanteVentaRepository>();
builder.Services.AddScoped<IRutasFacturacionRepository, RutasFacturacionRepository>();
builder.Services.AddScoped<IDetalleVentaRepository, DetalleVentaRepository>();
builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivos>();

// Add Services SRI and Certificate Services
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<ICertificadoService, CertificadoService>();
builder.Services.AddScoped<IServiceSRI, ServiceSRI>();
builder.Services.AddScoped<IEmailService, EmailService>();

// please kindly ensure what license is appropriate for your project - Permite utilizar el generador QuestPdf de forma gratuita
QuestPDF.Settings.License = LicenseType.Community;

// Add Email Configuration services
var emailConfiguration = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfig>();
builder.Services.AddSingleton(emailConfiguration);

// Add ApplicationURL configuration services
var appConfig = builder.Configuration.GetSection("ApplicationURL").Get<ApplicationURL>();
builder.Services.AddSingleton(appConfig);

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

app.UseHttpsRedirection();

app.UseCors();

app.UseStaticFiles(); // Permite que se pueda acceder a los archivos de la carpeta wwwroot

app.UseAuthorization();

app.MapControllers();

app.Run();