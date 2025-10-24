using FacturacionElectronicaSRI.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace FacturacionElectronicaSRI.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public DbSet<TblEmpresa> TblEmpresa { get; set; }
    }
}