// <auto-generated />
namespace Todo.Data.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class Comment_AddedNotNullAndAdjustedMaxLengthForCommentField : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(Comment_AddedNotNullAndAdjustedMaxLengthForCommentField));
        
        string IMigrationMetadata.Id
        {
            get { return "201710160803496_Comment_AddedNotNullAndAdjustedMaxLengthForCommentField"; }
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
