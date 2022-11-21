using System.Security.Cryptography;

namespace GraphManipulation.Models.Structures;

public class FilePath : Structure
{
    public FilePath(string name, HashAlgorithm algorithm, string toHash) : base(name, algorithm)
    {
    }
}