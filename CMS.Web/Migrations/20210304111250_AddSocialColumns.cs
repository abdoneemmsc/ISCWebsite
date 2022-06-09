using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Web.Migrations
{
    public partial class AddSocialColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "Settings",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "Settings",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Snapchat",
                table: "Settings",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "Settings",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Whatsapp",
                table: "Settings",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Youtube",
                table: "Settings",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Snapchat",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Whatsapp",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Youtube",
                table: "Settings");
        }
    }
}
