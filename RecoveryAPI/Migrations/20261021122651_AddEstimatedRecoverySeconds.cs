using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecoveryAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimatedRecoverySeconds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EstimatedRecoverySeconds",
                table: "Service",
                type: "REAL",
                nullable: false,
                defaultValue: 30.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedRecoverySeconds",
                table: "Service");
        }
    }
}
