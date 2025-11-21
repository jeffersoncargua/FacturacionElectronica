using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturacionElectronicaSRI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterTblClienteDeleteIdEmpresaAddIndexIndentificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblCliente_IdEmpresa",
                table: "TblCliente");

            migrationBuilder.DropColumn(
                name: "IdEmpresa",
                table: "TblCliente");

            migrationBuilder.AlterColumn<string>(
                name: "DocSri",
                table: "TblComprobanteVenta",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Identificacion",
                table: "TblCliente",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TblCliente_Identificacion",
                table: "TblCliente",
                column: "Identificacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblCliente_Identificacion",
                table: "TblCliente");

            migrationBuilder.AlterColumn<string>(
                name: "DocSri",
                table: "TblComprobanteVenta",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Identificacion",
                table: "TblCliente",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "IdEmpresa",
                table: "TblCliente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TblCliente_IdEmpresa",
                table: "TblCliente",
                column: "IdEmpresa");
        }
    }
}