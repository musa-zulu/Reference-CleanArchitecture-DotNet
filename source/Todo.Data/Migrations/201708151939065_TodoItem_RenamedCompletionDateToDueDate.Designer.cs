// <auto-generated />
namespace Todo.Data.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class TodoItem_RenamedCompletionDateToDueDate : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(TodoItem_RenamedCompletionDateToDueDate));
        
        string IMigrationMetadata.Id
        {
            get { return "201708151939065_TodoItem_RenamedCompletionDateToDueDate"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
