using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Managers",
                keyColumn: "ManagerId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$duokN496kvwV9HIkHO7rqeucf8xm2zoP9Z3hRAmHd46SbVr5e1TpS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Managers",
                keyColumn: "ManagerId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$eSHBBPmQe5XDOY0y1/xgTOHATcIEqnYEIRFJGlMGWGT3E0aFNHlKe");
        }
    }
}
