using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _20211129078_butunleme_ozanuysal.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToRecipient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Messages",
                newName: "RecipientId");

            migrationBuilder.DropColumn(
                 name: "FullName",
                table: "AspNetUsers"
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
                );

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Messages",
                newName: "ReceiverId");
        }
    }
}
