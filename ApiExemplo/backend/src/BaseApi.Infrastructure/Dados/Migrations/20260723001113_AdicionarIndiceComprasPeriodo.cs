using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApi.Infrastructure.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarIndiceComprasPeriodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Carrinhos",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "SenhaHash",
                value: "$2a$11$I1xE4nCafftsJ6yUeS/vgeaLh1wH1Eu3WLEZi1/KSyC.7SlL00fta");

            migrationBuilder.CreateIndex(
                name: "IX_Carrinhos_UsuarioId_Status_CriadoEm",
                table: "Carrinhos",
                columns: new[] { "UsuarioId", "Status", "CriadoEm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carrinhos_UsuarioId_Status_CriadoEm",
                table: "Carrinhos");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Carrinhos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "SenhaHash",
                value: "$2a$11$uwyRiibWO2kMz62HSZIbZ.LeTt5pnjiE8Upqg/RwzhQguO2318euW");
        }
    }
}
