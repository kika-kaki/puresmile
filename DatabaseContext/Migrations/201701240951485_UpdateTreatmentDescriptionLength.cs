namespace PureSmileUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTreatmentDescriptionLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Treatments", "Description", c => c.String(nullable: false, maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Treatments", "Description", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
