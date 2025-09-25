using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pet_ToDo_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHashIdFieald : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        { 
            migrationBuilder.DropColumn(name: "HashPasswordId",
                table: "Users"
                );
        }
         
    }
}
