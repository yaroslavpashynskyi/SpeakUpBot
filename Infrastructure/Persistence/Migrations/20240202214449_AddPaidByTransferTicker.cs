using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaidByTransferTicker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:english_level", "pre_intermediate,intermediate,upper_intermediate,advanced")
                .Annotation("Npgsql:Enum:payment_status", "pending,to_be_approved,paid_by_card,paid_by_transfer_ticket,to_be_paid_by_cash,cancelled")
                .Annotation("Npgsql:Enum:role", "member,admin")
                .OldAnnotation("Npgsql:Enum:english_level", "pre_intermediate,intermediate,upper_intermediate,advanced")
                .OldAnnotation("Npgsql:Enum:payment_status", "pending,to_be_approved,paid_by_card,to_be_paid_by_cash,cancelled")
                .OldAnnotation("Npgsql:Enum:role", "member,admin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:english_level", "pre_intermediate,intermediate,upper_intermediate,advanced")
                .Annotation("Npgsql:Enum:payment_status", "pending,to_be_approved,paid_by_card,to_be_paid_by_cash,cancelled")
                .Annotation("Npgsql:Enum:role", "member,admin")
                .OldAnnotation("Npgsql:Enum:english_level", "pre_intermediate,intermediate,upper_intermediate,advanced")
                .OldAnnotation("Npgsql:Enum:payment_status", "pending,to_be_approved,paid_by_card,paid_by_transfer_ticket,to_be_paid_by_cash,cancelled")
                .OldAnnotation("Npgsql:Enum:role", "member,admin");
        }
    }
}
