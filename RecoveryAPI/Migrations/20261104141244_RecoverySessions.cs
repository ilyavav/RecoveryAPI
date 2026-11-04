using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecoveryAPI.Migrations
{
    /// <inheritdoc />
    public partial class RecoverySessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecoveryAttempt_Service_ServiceId",
                table: "RecoveryAttempt");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDependency_ServiceId",
                table: "ServiceDependency");

            migrationBuilder.DropIndex(
                name: "IX_RecoveryAttempt_ServiceId",
                table: "RecoveryAttempt");

            migrationBuilder.DropColumn(
                name: "EstimatedRecoverySeconds",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "IsSuccessful",
                table: "RecoveryAttempt");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Service",
                newName: "DesiredReplicas");

            migrationBuilder.RenameColumn(
                name: "Criticality",
                table: "Service",
                newName: "DefaultPriority");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "RecoveryAttempt",
                newName: "Status");

            migrationBuilder.AddColumn<DateTime>(
                name: "ObservedAt",
                table: "Service",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReadyReplicas",
                table: "Service",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "RecoveryAttempt",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "RecoveryAttempt",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecoverySessionServiceId",
                table: "RecoveryAttempt",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RecoverySession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoverySession", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecoverySessionService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecoverySessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetReplicas = table.Column<int>(type: "INTEGER", nullable: false),
                    PriorityAtStart = table.Column<int>(type: "INTEGER", nullable: true),
                    FinalReadyReplicas = table.Column<int>(type: "INTEGER", nullable: true),
                    FinalObservedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoverySessionService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecoverySessionService_RecoverySession_RecoverySessionId",
                        column: x => x.RecoverySessionId,
                        principalTable: "RecoverySession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecoverySessionService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDependency_ServiceId_DependsOnServiceId",
                table: "ServiceDependency",
                columns: new[] { "ServiceId", "DependsOnServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryAttempt_RecoverySessionServiceId",
                table: "RecoveryAttempt",
                column: "RecoverySessionServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RecoverySessionService_RecoverySessionId_ServiceId",
                table: "RecoverySessionService",
                columns: new[] { "RecoverySessionId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecoverySessionService_ServiceId",
                table: "RecoverySessionService",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecoveryAttempt_RecoverySessionService_RecoverySessionServiceId",
                table: "RecoveryAttempt",
                column: "RecoverySessionServiceId",
                principalTable: "RecoverySessionService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecoveryAttempt_RecoverySessionService_RecoverySessionServiceId",
                table: "RecoveryAttempt");

            migrationBuilder.DropTable(
                name: "RecoverySessionService");

            migrationBuilder.DropTable(
                name: "RecoverySession");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDependency_ServiceId_DependsOnServiceId",
                table: "ServiceDependency");

            migrationBuilder.DropIndex(
                name: "IX_RecoveryAttempt_RecoverySessionServiceId",
                table: "RecoveryAttempt");

            migrationBuilder.DropColumn(
                name: "ObservedAt",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ReadyReplicas",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "RecoveryAttempt");

            migrationBuilder.DropColumn(
                name: "RecoverySessionServiceId",
                table: "RecoveryAttempt");

            migrationBuilder.RenameColumn(
                name: "DesiredReplicas",
                table: "Service",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "DefaultPriority",
                table: "Service",
                newName: "Criticality");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "RecoveryAttempt",
                newName: "ServiceId");

            migrationBuilder.AddColumn<double>(
                name: "EstimatedRecoverySeconds",
                table: "Service",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "RecoveryAttempt",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccessful",
                table: "RecoveryAttempt",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDependency_ServiceId",
                table: "ServiceDependency",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryAttempt_ServiceId",
                table: "RecoveryAttempt",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecoveryAttempt_Service_ServiceId",
                table: "RecoveryAttempt",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
