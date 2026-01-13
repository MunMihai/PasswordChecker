using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordChecker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_CodeFirst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    max_checks_per_day = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    varsta = table.Column<int>(type: "int", nullable: true),
                    gen = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordChecks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    score = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordChecks", x => x.id);
                    table.ForeignKey(
                        name: "FK_PasswordChecks_User",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    plan_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Subscription_Plan",
                        column: x => x.plan_id,
                        principalTable: "Plans",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Subscription_User",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChecks_UserId",
                table: "PasswordChecks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_PlanId",
                table: "Subscriptions",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_UserId",
                table: "Subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UX_Subscription_ActiveUser",
                table: "Subscriptions",
                column: "user_id",
                unique: true,
                filter: "([status]='ACTIVE')");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                table: "Users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordChecks");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
