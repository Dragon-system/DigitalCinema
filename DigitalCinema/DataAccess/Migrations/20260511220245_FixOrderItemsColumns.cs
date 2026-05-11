using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderItemsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Tickets_TicketId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "OrderItems",
                newName: "ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_TicketId",
                table: "OrderItems",
                newName: "IX_OrderItems_ShowId");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Shows_ShowId",
                table: "OrderItems",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Shows_ShowId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ShowId",
                table: "OrderItems",
                newName: "TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ShowId",
                table: "OrderItems",
                newName: "IX_OrderItems_TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Tickets_TicketId",
                table: "OrderItems",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
