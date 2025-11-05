using Facturacion_Electronica.Mapping;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Repository.Repository;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

// builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IRutasFacturacionRepository, RutasFacturacionRepository>();

// builder.Services.AddScoped<IDetalleVentaRepository, DetalleVentaRepository>();
// builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivos>();

// Add Services SRI and Certificate Services
// builder.Services.AddScoped<ICertificadoService, CertificadoService>();
// builder.Services.AddScoped<IServiceSRI, ServiceSRI>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();