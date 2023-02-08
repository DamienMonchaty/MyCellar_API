using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCellar.Business.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Sexe = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Role = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "Email", "Password", "Role", "Sexe", "Token", "UserName" },
                values: new object[] { 1, "email1@email.fr", "$2a$11$HRA8Cf.6uwboYxhP1i8Mee9NWFw7.ejvgvVsV6Zr9DCNsWANdKYb.", "User", "sexe1", null, "prenom1" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "Email", "Password", "Role", "Sexe", "Token", "UserName" },
                values: new object[] { 2, "email2@email.fr", "$2a$11$A34xXogGMKgcdhWxkTjNM.QLlRCWOZuyX2eEX9wwT5kbRq7y3RVlu", "Admin", "sexe1", null, "prenom2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
