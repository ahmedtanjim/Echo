using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Echo.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSenderAndReceiverIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatRoom_ChatRoomId",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatRoom");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ChatRoomId_SentAt",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ChatRoomId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "ChatMessages");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "ChatMessages");

            migrationBuilder.AddColumn<Guid>(
                name: "ChatRoomId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ChatMessages",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ChatRoom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoom", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatRoomId_SentAt",
                table: "ChatMessages",
                columns: new[] { "ChatRoomId", "SentAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatRoom_ChatRoomId",
                table: "ChatMessages",
                column: "ChatRoomId",
                principalTable: "ChatRoom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
