using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturacionElectronicaSRI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTblEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PathLogo",
                table: "TblEmpresa",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PathCertificado",
                table: "TblEmpresa",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "TblRutasXML",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    RutaGenerados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RutaFirmados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RutaAutorizados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaveAcceso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoRecepcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathXMLPDF = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblRutasXML", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblRutasXML_TblEmpresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "TblEmpresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblRutasXML_IdEmpresa",
                table: "TblRutasXML",
                column: "IdEmpresa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblRutasXML");

            migrationBuilder.AlterColumn<string>(
                name: "PathLogo",
                table: "TblEmpresa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PathCertificado",
                table: "TblEmpresa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
