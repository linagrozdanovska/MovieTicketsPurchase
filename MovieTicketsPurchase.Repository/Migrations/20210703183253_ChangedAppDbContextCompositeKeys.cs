using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieTicketsPurchase.Repository.Migrations
{
    public partial class ChangedAppDbContextCompositeKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketsInCart",
                table: "TicketsInCart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketInOrder",
                table: "TicketInOrder");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketsInCart",
                table: "TicketsInCart",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketInOrder",
                table: "TicketInOrder",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInOrder_TicketId",
                table: "TicketInOrder",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketsInCart",
                table: "TicketsInCart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketInOrder",
                table: "TicketInOrder");

            migrationBuilder.DropIndex(
                name: "IX_TicketInOrder_TicketId",
                table: "TicketInOrder");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketsInCart",
                table: "TicketsInCart",
                columns: new[] { "Id", "CartId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketInOrder",
                table: "TicketInOrder",
                columns: new[] { "TicketId", "OrderId" });
        }
    }
}
