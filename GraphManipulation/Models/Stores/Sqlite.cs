using GraphManipulation.Models.Connections;

namespace GraphManipulation.Models.Stores;

public class Sqlite : Relational
{
    public Sqlite(string name) : base(name)
    {
    }

    public override Connection GetConnection()
    {
        throw new NotImplementedException();
    }

    public override string ToRdf()
    {
        throw new NotImplementedException();
    }

    public override void FromRdf()
    {
        throw new NotImplementedException();
    }
}