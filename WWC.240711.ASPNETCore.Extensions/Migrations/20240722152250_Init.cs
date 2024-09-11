using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWC._240711.ASPNETCore.Extensions.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationInfos",
                columns: table => new
                {
                    ConfigKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "varchar(3000)", maxLength: 3000, nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationInfos", x => x.ConfigKey);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationInfos");
        }
    }
}
