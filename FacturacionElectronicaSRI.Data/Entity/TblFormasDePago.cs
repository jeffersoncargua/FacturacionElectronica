using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    /// <summary>
    /// Esta table permite definir las formas de pago que acepta el SRI para las facturas electrónicas.
    /// Estas Pueden ser: 01-Sin utilizacion del Sistema Financiero, 15-Compensacion de Deudas, 16-Tarjeta de débito, 17-Dinero electrónico, entre otras.
    /// </summary>
    public class TblFormasDePago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Descripcion { get; set; }

        public required string Codigo { get; set; }
    }
}