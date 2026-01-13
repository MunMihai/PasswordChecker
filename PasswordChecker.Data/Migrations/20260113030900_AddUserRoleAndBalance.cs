using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordChecker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleAndBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "balance",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "balance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "role",
                table: "Users");
        }
    }
}
