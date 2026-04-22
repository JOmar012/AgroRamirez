using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgropRamirez.Migrations
{
    /// <inheritdoc />
    public partial class AddCarritoPromocion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarritoPromociones",
                columns: table => new
                {
                    CarritoPromocionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarritoId = table.Column<int>(type: "int", nullable: false),
                    PromocionId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoPromociones", x => x.CarritoPromocionId);
                    table.ForeignKey(
                        name: "FK_CarritoPromociones_Carrito_CarritoId",
                        column: x => x.CarritoId,
                        principalTable: "Carrito",
                        principalColumn: "CarritoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarritoPromociones_Promociones_PromocionId",
                        column: x => x.PromocionId,
                        principalTable: "Promociones",
                        principalColumn: "PromocionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarritoPromociones_CarritoId",
                table: "CarritoPromociones",
                column: "CarritoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoPromociones_PromocionId",
                table: "CarritoPromociones",
                column: "PromocionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarritoPromociones");
        }
    }
}
