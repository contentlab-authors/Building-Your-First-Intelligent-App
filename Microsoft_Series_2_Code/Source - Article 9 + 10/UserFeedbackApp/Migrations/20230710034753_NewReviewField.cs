using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserFeedbackApp.Migrations
{
    /// <inheritdoc />
    public partial class NewReviewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomSentiment",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomSentiment",
                table: "Reviews");
        }
    }
}
