﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasteReductionPlatform.Migrations
{
    /// <inheritdoc />
    public partial class UserPickupRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPickupRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupScheduleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPickupRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPickupRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPickupRequests_PickupSchedules_PickupScheduleId",
                        column: x => x.PickupScheduleId,
                        principalTable: "PickupSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPickupRequests_PickupScheduleId",
                table: "UserPickupRequests",
                column: "PickupScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPickupRequests_UserId",
                table: "UserPickupRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPickupRequests");
        }
    }
}
