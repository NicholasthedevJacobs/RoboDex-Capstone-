using Microsoft.EntityFrameworkCore.Migrations;

namespace RoboDex__Capstone_.Migrations
{
    public partial class items : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e9e46d5f-c8ec-4aef-8c8f-1ee75ec96a97");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Inbox",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "27a6d60f-b0b9-49ae-b8f1-9217dc1d2e45", "8f850e5a-5d9f-4084-9f81-cbca4e457c8f", "RoboDexer", "ROBODEXER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "27a6d60f-b0b9-49ae-b8f1-9217dc1d2e45");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Inbox");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e9e46d5f-c8ec-4aef-8c8f-1ee75ec96a97", "52e6e297-944d-47c8-9b22-88c22f438bea", "RoboDexer", "ROBODEXER" });
        }
    }
}
