using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordChecker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionIdToPasswordCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "subscription_id",
                table: "PasswordChecks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChecks_SubscriptionId",
                table: "PasswordChecks",
                column: "subscription_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordChecks_Subscription",
                table: "PasswordChecks",
                column: "subscription_id",
                principalTable: "Subscriptions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordChecks_Subscription",
                table: "PasswordChecks");

            migrationBuilder.DropIndex(
                name: "IX_PasswordChecks_SubscriptionId",
                table: "PasswordChecks");

            migrationBuilder.DropColumn(
                name: "subscription_id",
                table: "PasswordChecks");
        }
    }
}
