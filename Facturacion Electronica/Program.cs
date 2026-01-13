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
// var provider = builder.Services.BuildServiceProvider();
// var configuration = provider.GetRequiredService<IConfiguration>();

// Add FrontURL configuration services
var frontEndUrl = builder.Configuration.GetSection("FrontUrl").Get<FrontURL>();
// var frontEndUrl = configuration.GetSection("FrontUrl").Get<FrontURL>();
builder.Services.AddSingleton(frontEndUrl);

// Add Email Configuration services
//var emailConfiguration = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfig>();
// var emailConfiguration = configuration.GetSection("EmailConfiguration").Get<EmailConfig>();
//builder.Services.AddSingleton(emailConfiguration);
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfiguration"));

// Add Kushki Services
//var kushkiConfig = builder.Configuration.GetSection("KushkiConfig").Get<KushkiConfig>();
//var kushkiConfig = configuration.GetSection("KushkiConfig").Get<KushkiConfig>();
//builder.Services.AddSingleton(kushkiConfig);
builder.Services.Configure<KushkiConfig>(builder.Configuration.GetSection("KushkiConfig"));

// Add ApplicationURL configuration services
var appConfig = builder.Configuration.GetSection("ApplicationURL").Get<ApplicationURL>();
// var appConfig = configuration.GetSection("ApplicationURL").Get<ApplicationURL>();
builder.Services.AddSingleton(appConfig);

// Add service for Cors configuration

//builder.Services.AddCors(options =>
//{
//    // var frontEndUrl = configuration.GetValue<string>("FrontUrl");

//    options.AddDefaultPolicy(builder =>
//    {
//        builder.WithOrigins(frontEndUrl.Url)
//        .AllowAnyMethod()
//        .AllowAnyHeader()
//        .AllowCredentials(); // En "produccion" en caso de no tener credenciales, se elimina esta instruccion 
//    });
//});

builder.Services.AddCors(options =>
{
    // var frontEndUrl = configuration.GetValue<string>("FrontUrl");

    options.AddPolicy(name: "AllowSpecificOrigin", policy =>
    {
        // policy.WithOrigins(frontEndUrl.Url)
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();

// Add SQL Services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Add Services: SRI, Certificate, Venta, Email and Kushki Services
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<ICertificadoService, CertificadoService>();
builder.Services.AddScoped<IServiceSRI, ServiceSRI>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IKushkiService, KushkiService>();

// please kindly ensure what license is appropriate for your project - Permite utilizar el generador QuestPdf de forma gratuita
QuestPDF.Settings.License = LicenseType.Community;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Esta seccion se debe comentar cuando este en produccion y descomentar cuando este en desarrollo
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

// Esta seccion se debe descomentar cuando este en modo de produccion y comentar cuando este en modo de desarrollo
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// app.UseCors();
app.UseCors("AllowSpecificOrigin");

// app.UseStaticFiles(); // Permite que se pueda acceder a los archivos de la carpeta wwwroot

app.UseAuthorization();

app.UseStaticFiles(); // Permite que se pueda acceder a los archivos de la carpeta wwwroot

app.MapControllers();

app.Run();