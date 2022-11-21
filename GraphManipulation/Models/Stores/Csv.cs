using System.Security.Cryptography;
using GraphManipulation.Models.Connections;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models.Stores;

public class Csv : File
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

    public override void AddStructure(Structure structure)
    {
        throw new NotImplementedException();
    }

    public Csv(string name, HashAlgorithm algorithm) : base(name, algorithm)
    {
    }
}