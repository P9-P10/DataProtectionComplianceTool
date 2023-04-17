using System;
using GraphManipulation.DataAccess.Mappers;
using Xunit;

namespace Test.DataAccess;

public class MapperTest
{
    public class Insert
    {
        [Fact]
        public void ThrowsArgumentNullExceptionIfArgumentIsNull()
        {
            Mapper<object> mapper = new Mapper<object>(null);
            Assert.Throws<ArgumentNullException>(() => mapper.Insert(null));
        }
    }
    
    public class Update
    {
        [Fact]
        public void ThrowsArgumentNullExceptionIfArgumentIsNull()
        {
            Mapper<object> mapper = new Mapper<object>(null);
            Assert.Throws<ArgumentNullException>(() => mapper.Update(null));
        }
    }
    
    public class Delete
    {
        [Fact]
        public void ThrowsArgumentNullExceptionIfArgumentIsNull()
        {
            Mapper<object> mapper = new Mapper<object>(null);
            Assert.Throws<ArgumentNullException>(() => mapper.Delete(null));
        }
    }
}