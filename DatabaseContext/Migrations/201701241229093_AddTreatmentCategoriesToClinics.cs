namespace PureSmileUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTreatmentCategoriesToClinics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicToTreatmentCategory",
                c => new
                    {
                        ClinicId = c.Int(nullable: false),
                        TreatmentCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ClinicId, t.TreatmentCategoryId })
                .ForeignKey("dbo.Clinics", t => t.ClinicId)
                .ForeignKey("dbo.TreatmentCategories", t => t.TreatmentCategoryId)
                .Index(t => t.ClinicId)
                .Index(t => t.TreatmentCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClinicToTreatmentCategory", "TreatmentCategoryId", "dbo.TreatmentCategories");
            DropForeignKey("dbo.ClinicToTreatmentCategory", "ClinicId", "dbo.Clinics");
            DropIndex("dbo.ClinicToTreatmentCategory", new[] { "TreatmentCategoryId" });
            DropIndex("dbo.ClinicToTreatmentCategory", new[] { "ClinicId" });
            DropTable("dbo.ClinicToTreatmentCategory");
        }
    }
}
