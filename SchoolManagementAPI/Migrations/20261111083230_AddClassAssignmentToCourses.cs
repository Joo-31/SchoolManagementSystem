using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddClassAssignmentToCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ClassId",
                table: "Courses",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Classes_ClassId",
                table: "Courses",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Classes_ClassId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ClassId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Courses");
        }
    }
}
