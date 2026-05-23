using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeroFrame.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminDashboardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cor",
                table: "produtos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Desconto",
                table: "produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "produtos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoCusto",
                table: "produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SecaoVitrine",
                table: "produtos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TamanhosDisponiveis",
                table: "produtos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoDesconto",
                table: "produtos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "nenhum");

            migrationBuilder.AddColumn<string>(
                name: "TipoTamanho",
                table: "produtos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEntrega",
                table: "pedidos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnderecoId",
                table: "pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrevisaoEntrega",
                table: "pedidos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusEntrega",
                table: "pedidos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Pendente");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoCustoUnitario",
                table: "itemPedidos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "enderecos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "enderecos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AvaliacoesProdutos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Nota = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesProdutos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaliacoesProdutos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvaliacoesProdutos_produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_EnderecoId",
                table: "pedidos",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesProdutos_ProdutoId",
                table: "AvaliacoesProdutos",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesProdutos_UsuarioId_ProdutoId",
                table: "AvaliacoesProdutos",
                columns: new[] { "UsuarioId", "ProdutoId" },
                unique: true,
                filter: "[Ativo] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_pedidos_enderecos_EnderecoId",
                table: "pedidos",
                column: "EnderecoId",
                principalTable: "enderecos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pedidos_enderecos_EnderecoId",
                table: "pedidos");

            migrationBuilder.DropTable(
                name: "AvaliacoesProdutos");

            migrationBuilder.DropIndex(
                name: "IX_pedidos_EnderecoId",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "Cor",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "Desconto",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "Genero",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "PrecoCusto",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "SecaoVitrine",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "TamanhosDisponiveis",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "TipoDesconto",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "TipoTamanho",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "DataEntrega",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "PrevisaoEntrega",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "StatusEntrega",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "PrecoCustoUnitario",
                table: "itemPedidos");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "enderecos");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "enderecos");
        }
    }
}
