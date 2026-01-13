using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturacionElectronicaSRI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableTblProductosAddColumnPathFileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathFileImagen",
                table: "TblProducto",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathFileImagen",
                table: "TblProducto");
        }
    }
}
