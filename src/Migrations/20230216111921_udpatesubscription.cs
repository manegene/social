using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kmums.Migrations
{
    public partial class udpateSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Customers_UserModelId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_PublicProfile_UserPublicModelId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "UserPublicModelId",
                table: "Subscriptions",
                newName: "UserPublicProfileId");

            migrationBuilder.RenameColumn(
                name: "UserModelId",
                table: "Subscriptions",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_UserPublicModelId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_UserPublicProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_UserModelId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Customers_UserProfileId",
                table: "Subscriptions",
                column: "UserProfileId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_PublicProfile_UserPublicProfileId",
                table: "Subscriptions",
                column: "UserPublicProfileId",
                principalTable: "PublicProfile",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Customers_UserProfileId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_PublicProfile_UserPublicProfileId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "UserPublicProfileId",
                table: "Subscriptions",
                newName: "UserPublicModelId");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "Subscriptions",
                newName: "UserModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_UserPublicProfileId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_UserPublicModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_UserProfileId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Customers_UserModelId",
                table: "Subscriptions",
                column: "UserModelId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_PublicProfile_UserPublicModelId",
                table: "Subscriptions",
                column: "UserPublicModelId",
                principalTable: "PublicProfile",
                principalColumn: "Id");
        }
    }
}
