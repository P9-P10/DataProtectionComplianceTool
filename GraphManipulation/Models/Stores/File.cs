using System.Security.Cryptography;

namespace GraphManipulation.Models.Stores;

public abstract class File : DataStore
{
    protected File(string name, HashAlgorithm algorithm) : base(name, algorithm)
    {
    }
}