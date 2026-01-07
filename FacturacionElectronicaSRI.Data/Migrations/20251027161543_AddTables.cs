using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturacionElectronicaSRI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblEmpresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ruc = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreComercial = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DireccionMatriz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObligadoLlevarContabilidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ambiente = table.Column<int>(type: "int", nullable: false),
                    PathCertificado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contraseña = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PathLogo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonoEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlRecepcionPrueba = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlRecepcionProduccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlAutorizacionPrueba = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlAutorizacionProduccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblEmpresa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblProducto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoPrincipal = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CodigoAuxiliar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblProducto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblComprobanteVenta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoComprobante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtotal0 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal12 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalIva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DocSri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblComprobanteVenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblComprobanteVenta_TblCliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "TblCliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblComprobanteVenta_TblEmpresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "TblEmpresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblDetalleVenta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdComprobanteVenta = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VentaIva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDetalleVenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblDetalleVenta_TblComprobanteVenta_IdComprobanteVenta",
                        column: x => x.IdComprobanteVenta,
                        principalTable: "TblComprobanteVenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblDetalleVenta_TblProducto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "TblProducto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblCliente_Email",
                table: "TblCliente",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TblCliente_IdEmpresa",
                table: "TblCliente",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_TblComprobanteVenta_IdCliente",
                table: "TblComprobanteVenta",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_TblComprobanteVenta_IdEmpresa",
                table: "TblComprobanteVenta",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_TblDetalleVenta_IdComprobanteVenta",
                table: "TblDetalleVenta",
                column: "IdComprobanteVenta");

            migrationBuilder.CreateIndex(
                name: "IX_TblDetalleVenta_IdProducto",
                table: "TblDetalleVenta",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_TblEmpresa_NombreComercial",
                table: "TblEmpresa",
                column: "NombreComercial");

            migrationBuilder.CreateIndex(
                name: "IX_TblEmpresa_Ruc",
                table: "TblEmpresa",
                column: "Ruc");

            migrationBuilder.CreateIndex(
                name: "IX_TblProductos_CodigoPrincipal",
                table: "TblProducto",
                column: "CodigoPrincipal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblDetalleVenta");

            migrationBuilder.DropTable(
                name: "TblComprobanteVenta");

            migrationBuilder.DropTable(
                name: "TblProducto");

            migrationBuilder.DropTable(
                name: "TblCliente");

            migrationBuilder.DropTable(
                name: "TblEmpresa");
        }
    }
}