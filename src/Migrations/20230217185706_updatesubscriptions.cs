using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kmums.Migrations
{
    public partial class updateSubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonTrans",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransId",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonTrans",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "TransId",
                table: "Subscriptions");
        }
    }
}
