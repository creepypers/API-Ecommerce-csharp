using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECOM_PanierMicroservice.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Variant",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Variant",
                table: "CartItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
