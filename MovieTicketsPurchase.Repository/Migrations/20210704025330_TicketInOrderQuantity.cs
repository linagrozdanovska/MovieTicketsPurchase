﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieTicketsPurchase.Repository.Migrations
{
    public partial class TicketInOrderQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "TicketInOrder",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "TicketInOrder");
        }
    }
}
