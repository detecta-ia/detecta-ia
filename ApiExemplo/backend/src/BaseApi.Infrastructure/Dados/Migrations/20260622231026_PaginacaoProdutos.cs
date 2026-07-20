using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApi.Infrastructure.Dados.Migrations
{
    /// <inheritdoc />
    public partial class PaginacaoProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "SenhaHash",
                value: "$2a$11$uwyRiibWO2kMz62HSZIbZ.LeTt5pnjiE8Upqg/RwzhQguO2318euW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "SenhaHash",
                value: "$2a$11$De/pVb9fR7a8m9EPOvtAcu.cNADBxHWb1YVGYxLvR/uXKa1coSHyW");
        }
    }
}
