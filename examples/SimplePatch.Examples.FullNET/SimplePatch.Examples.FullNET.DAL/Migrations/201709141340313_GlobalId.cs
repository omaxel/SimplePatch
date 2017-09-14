namespace SimplePatch.Examples.FullNET.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GlobalId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "GlobalId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "GlobalId");
        }
    }
}
