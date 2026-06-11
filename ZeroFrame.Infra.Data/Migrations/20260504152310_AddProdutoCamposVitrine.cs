using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeroFrame.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProdutoCamposVitrine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "produtos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "produtos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origem",
                table: "produtos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoOriginal",
                table: "produtos",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "Marca",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "Origem",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "PrecoOriginal",
                table: "produtos");
        }
    }
}
