using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapStone.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Swap 0 <-> 1 for Users (ActiveStatus: InActive was 1, now 0; Active was 0, now 1)
            migrationBuilder.Sql("UPDATE Users SET ActiveStatus = CASE WHEN ActiveStatus = 0 THEN 1 ELSE 0 END;");

            // Swap 0 <-> 1 for InsuranceProducts (Status: NotAvailable was 1, now 0; Available was 0, now 1)
            migrationBuilder.Sql("UPDATE InsuranceProducts SET Status = CASE WHEN Status = 0 THEN 1 ELSE 0 END;");

            // Swap 0 <-> 1 for PremiumPayments (PaymentStatus: Failed was 1, now 0; Success was 0, now 1; Pending = 2 unchanged)
            migrationBuilder.Sql("UPDATE PremiumPayments SET PaymentStatus = CASE WHEN PaymentStatus = 0 THEN 1 WHEN PaymentStatus = 1 THEN 0 ELSE PaymentStatus END;");

            // Swap 0 <-> 1 for PolicyPlans (PlanStatus: NotAvailable was 1, now 0; Available was 0, now 1)
            migrationBuilder.Sql("UPDATE PolicyPlans SET PlanStatus = CASE WHEN PlanStatus = 0 THEN 1 ELSE 0 END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Users SET ActiveStatus = CASE WHEN ActiveStatus = 1 THEN 0 ELSE 1 END;");

            migrationBuilder.Sql("UPDATE InsuranceProducts SET Status = CASE WHEN Status = 1 THEN 0 ELSE 1 END;");

            migrationBuilder.Sql("UPDATE PremiumPayments SET PaymentStatus = CASE WHEN PaymentStatus = 1 THEN 0 WHEN PaymentStatus = 0 THEN 1 ELSE PaymentStatus END;");

            migrationBuilder.Sql("UPDATE PolicyPlans SET PlanStatus = CASE WHEN PlanStatus = 1 THEN 0 ELSE 1 END;");
        }
    }
}
