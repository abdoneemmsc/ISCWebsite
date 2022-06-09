using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Web.Migrations
{
    public partial class Description : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PostTypes",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "PostTypes",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PostTypes");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "PostTypes");
        }
    }
}
