namespace PureSmileUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSignOfActivityToTreatmentsAndItsCategories : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Treatments", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.TreatmentCategories", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TreatmentCategories", "IsActive");
            DropColumn("dbo.Treatments", "IsActive");
        }
    }
}
