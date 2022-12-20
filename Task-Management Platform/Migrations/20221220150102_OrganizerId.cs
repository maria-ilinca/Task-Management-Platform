using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Management_Platform.Migrations
{
    public partial class OrganizerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_ApplicationUserId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Tasks",
                newName: "OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ApplicationUserId",
                table: "Tasks",
                newName: "IX_Tasks_OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_OrganizerId",
                table: "Tasks",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_OrganizerId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "OrganizerId",
                table: "Tasks",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_OrganizerId",
                table: "Tasks",
                newName: "IX_Tasks_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_ApplicationUserId",
                table: "Tasks",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
