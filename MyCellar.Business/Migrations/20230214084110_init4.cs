using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCellar.Business.Migrations
{
    public partial class init4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "users");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$mQr3sWOu6J.DaSPYwkkwTOGLwizLAc3FlEy9dslx229wilUIFtRJ.");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$qzgQtkIox3S6gKZV.rLIOet98Q2ArIQodvGT.P5UcDHTA/6KErbba");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$ukzIdRAq.KA.H5zlxuCPw.9OQhCIYnmfWUmk3lN3PqSUk6.ts7ZMS");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$KyOtcIV6FJV4a6avyP2lmOjtenEb.BxYqWowbfrqaZLVbbjQ3uwcW");
        }
    }
}
