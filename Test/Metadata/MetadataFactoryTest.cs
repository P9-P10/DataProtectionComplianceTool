using GraphManipulation.DataAccess.Entities;
using GraphManipulation.MetadataManagement;
using Xunit;

namespace Test.Metadata;

public class MetadataFactoryTest
{
    public class CreateMetadataTest
    {
        [Fact]
        public void CreatesDefaultInstance()
        {
            GDPRMetadata expected = new GDPRMetadata();
            GDPRMetadata actual = MetadataFactory.CreateMetadata();
            
            Assert.Equal(expected, actual);
        }
    }

    public class ForTest
    {
        [Fact]
        public void AssignsTableAndColumn()
        {
            var column = new ColumnMetadata() { TargetTable = "table", TargetColumn = "column" };
            GDPRMetadata actual = MetadataFactory.CreateMetadata().For(column);
            
            Assert.Equal("table", actual.TargetTable);
            Assert.Equal("column", actual.TargetColumn);
        }

        [Fact]
        public void DoesNothingGivenDefaultColumn()
        {
            GDPRMetadata expected = new GDPRMetadata();
            GDPRMetadata actual = MetadataFactory.CreateMetadata().For(new ColumnMetadata());
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ChangesTableAndColumn()
        {
            GDPRMetadata actual = new GDPRMetadata("originalTable", "originalColumn");
            var column = new ColumnMetadata() { TargetTable = "newTable", TargetColumn = "newColumn" };
            actual.For(column);

            Assert.Equal("newTable", actual.TargetTable);
            Assert.Equal("newColumn", actual.TargetColumn);
        }
    }
}