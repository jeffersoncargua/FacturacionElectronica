using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturacionElectronicaSRI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTblKushkiPayment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblKushkiPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subtotalIva = table.Column<double>(type: "float", nullable: false),
                    subtotalIva0 = table.Column<double>(type: "float", nullable: false),
                    ice = table.Column<double>(type: "float", nullable: false),
                    iva = table.Column<double>(type: "float", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approvalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    approvedTransactionAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bindCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cardCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lastFourDigits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cardHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created = table.Column<long>(type: "bigint", nullable: false),
                    merchantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    merchantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    paymentBrand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    processorBankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requestAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    responseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    responseText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transactionReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transactionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ticketNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblKushkiPayment", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblKushkiPayment");
        }
    }
}