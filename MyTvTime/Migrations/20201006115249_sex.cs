using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTvTime.Migrations
{
    public partial class sex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "sex",
                table: "User",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "sex",
                table: "User",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
