using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregandoTablaLugar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LugarId",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lugares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identificador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CocheraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lugares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lugares_Cocheras_CocheraId",
                        column: x => x.CocheraId,
                        principalTable: "Cocheras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_LugarId",
                table: "Tickets",
                column: "LugarId");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_CocheraId",
                table: "Lugares",
                column: "CocheraId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Lugares_LugarId",
                table: "Tickets",
                column: "LugarId",
                principalTable: "Lugares",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Lugares_LugarId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Lugares");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_LugarId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "LugarId",
                table: "Tickets");
        }
    }
}
