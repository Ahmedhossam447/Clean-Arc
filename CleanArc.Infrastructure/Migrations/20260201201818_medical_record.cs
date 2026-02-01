using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class medical_record : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_medical_record_Animals_Animalid",
                table: "medical_record");

            migrationBuilder.DropTable(
                name: "VaccinationNeededs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_medical_record",
                table: "medical_record");

            migrationBuilder.DropIndex(
                name: "IX_medical_record_Animalid",
                table: "medical_record");

            migrationBuilder.RenameTable(
                name: "medical_record",
                newName: "MedicalRecords");

            migrationBuilder.RenameColumn(
                name: "Animalid",
                table: "MedicalRecords",
                newName: "AnimalId");

            migrationBuilder.RenameColumn(
                name: "injurys",
                table: "MedicalRecords",
                newName: "Injuries");

            migrationBuilder.RenameColumn(
                name: "Recordid",
                table: "MedicalRecords",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "MedicalRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "MedicalRecords",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MedicalHistoryNotes",
                table: "MedicalRecords",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "MedicalRecords",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalRecords",
                table: "MedicalRecords",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Vaccinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalRecordId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateGiven = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vaccinations_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_AnimalId",
                table: "MedicalRecords",
                column: "AnimalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_MedicalRecordId",
                table: "Vaccinations",
                column: "MedicalRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Animals_AnimalId",
                table: "MedicalRecords",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "AnimalId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Animals_AnimalId",
                table: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Vaccinations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalRecords",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_AnimalId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "MedicalHistoryNotes",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "MedicalRecords");

            migrationBuilder.RenameTable(
                name: "MedicalRecords",
                newName: "medical_record");

            migrationBuilder.RenameColumn(
                name: "AnimalId",
                table: "medical_record",
                newName: "Animalid");

            migrationBuilder.RenameColumn(
                name: "Injuries",
                table: "medical_record",
                newName: "injurys");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "medical_record",
                newName: "Recordid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_medical_record",
                table: "medical_record",
                column: "Recordid");

            migrationBuilder.CreateTable(
                name: "VaccinationNeededs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Medicalid = table.Column<int>(type: "int", nullable: true),
                    VaccineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationNeededs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaccinationNeededs_medical_record_Medicalid",
                        column: x => x.Medicalid,
                        principalTable: "medical_record",
                        principalColumn: "Recordid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medical_record_Animalid",
                table: "medical_record",
                column: "Animalid");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationNeededs_Medicalid",
                table: "VaccinationNeededs",
                column: "Medicalid");

            migrationBuilder.AddForeignKey(
                name: "FK_medical_record_Animals_Animalid",
                table: "medical_record",
                column: "Animalid",
                principalTable: "Animals",
                principalColumn: "AnimalId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
