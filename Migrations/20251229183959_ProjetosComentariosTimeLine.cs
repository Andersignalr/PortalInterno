using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PortalInterno.Migrations
{
    /// <inheritdoc />
    public partial class ProjetosComentariosTimeLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Projetos_ProjetoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProjetoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProjetoId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ComentariosProjeto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjetoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosProjeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentariosProjeto_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComentariosProjeto_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjetoMembros",
                columns: table => new
                {
                    ProjetoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    Papel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjetoMembros", x => new { x.ProjetoId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_ProjetoMembros_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjetoMembros_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjetoTimeline",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjetoId = table.Column<int>(type: "integer", nullable: false),
                    Evento = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjetoTimeline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjetoTimeline_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosProjeto_ProjetoId",
                table: "ComentariosProjeto",
                column: "ProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosProjeto_UsuarioId",
                table: "ComentariosProjeto",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjetoMembros_UsuarioId",
                table: "ProjetoMembros",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjetoTimeline_ProjetoId",
                table: "ProjetoTimeline",
                column: "ProjetoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComentariosProjeto");

            migrationBuilder.DropTable(
                name: "ProjetoMembros");

            migrationBuilder.DropTable(
                name: "ProjetoTimeline");

            migrationBuilder.AddColumn<int>(
                name: "ProjetoId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProjetoId",
                table: "AspNetUsers",
                column: "ProjetoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Projetos_ProjetoId",
                table: "AspNetUsers",
                column: "ProjetoId",
                principalTable: "Projetos",
                principalColumn: "Id");
        }
    }
}
