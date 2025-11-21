using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturacionElectronicaSRI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterTblComprobanteVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subtotal12",
                table: "TblComprobanteVenta",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "Subtotal",
                table: "TblComprobanteVenta",
                newName: "Subtotal15");

            migrationBuilder.AddColumn<decimal>(
                name: "Propina",
                table: "TblComprobanteVenta",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Propina",
                table: "TblComprobanteVenta");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "TblComprobanteVenta",
                newName: "Subtotal12");

            migrationBuilder.RenameColumn(
                name: "Subtotal15",
                table: "TblComprobanteVenta",
                newName: "Subtotal");
        }
    }
}
