using FacturacionElectronicaSRI.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace FacturacionElectronicaSRI.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TblEmpresa> TblEmpresa { get; set; }
        public DbSet<TblProductos> TblProducto { get; set; }
        public DbSet<TblCliente> TblCliente { get; set; }
        public DbSet<TblComprobanteVenta> TblComprobanteVenta { get; set; }
        public DbSet<TblDetalleVenta> TblDetalleVenta { get; set; }
        public DbSet<TblRutasXML> TblRutasXML { get; set; }
    }
}