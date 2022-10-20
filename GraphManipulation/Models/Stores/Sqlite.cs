using System.Security.Cryptography;
using GraphManipulation.Models.Connections;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models.Stores;

public class Sqlite : Relational
{
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

    public Sqlite(string name) : base(name)
    {
    }
}