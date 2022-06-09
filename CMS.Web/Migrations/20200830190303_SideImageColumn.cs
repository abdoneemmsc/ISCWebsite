using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Web.Migrations
{
    public partial class SideImageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlignSideImageToLeft",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Posts",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlignSideImageToLeft",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Posts");
        }
    }
}
