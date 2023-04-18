using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public interface IOriginManager
{
    public Origin AddOrigin(string name, string description);
    public Origin UpdateOriginName(string name, string newName);
    public Origin UpdateOriginDescription(string name, string description);
    public void DeleteOrigin(string name);
    public IEnumerable<Origin> GetAllOrigins();
    public Origin? GetOrigin(string name);

}