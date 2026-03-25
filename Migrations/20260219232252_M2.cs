using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfAppT.Migrations
{
    /// <inheritdoc />
    public partial class M2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Specialists_SpecialistId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "CarBrand",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "Records");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Specialists",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Records",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Records",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Records",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    LicensePlate = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.LicensePlate);
                    table.ForeignKey(
                        name: "FK_Cars_CarBrands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "CarBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cars_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Specialists_PhotoId",
                table: "Specialists",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_CustomerId",
                table: "Records",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_LicensePlate",
                table: "Records",
                column: "LicensePlate");

            migrationBuilder.CreateIndex(
                name: "IX_Records_PhotoId",
                table: "Records",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_BrandId",
                table: "Cars",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CustomerId",
                table: "Cars",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Cars_LicensePlate",
                table: "Records",
                column: "LicensePlate",
                principalTable: "Cars",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Customers_CustomerId",
                table: "Records",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Photos_PhotoId",
                table: "Records",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Specialists_SpecialistId",
                table: "Records",
                column: "SpecialistId",
                principalTable: "Specialists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Specialists_Photos_PhotoId",
                table: "Specialists",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Cars_LicensePlate",
                table: "Records");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Customers_CustomerId",
                table: "Records");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Photos_PhotoId",
                table: "Records");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Specialists_SpecialistId",
                table: "Records");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialists_Photos_PhotoId",
                table: "Specialists");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "CarBrands");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Specialists_PhotoId",
                table: "Specialists");

            migrationBuilder.DropIndex(
                name: "IX_Records_CustomerId",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_LicensePlate",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_PhotoId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Specialists");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Records");

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Records",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CarBrand",
                table: "Records",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "Records",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Specialists_SpecialistId",
                table: "Records",
                column: "SpecialistId",
                principalTable: "Specialists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
