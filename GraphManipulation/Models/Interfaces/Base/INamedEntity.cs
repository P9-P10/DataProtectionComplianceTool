namespace GraphManipulation.Models.Interfaces.Base;

public interface INamedEntity
{
    public string Name { get; set; }
    public string GetName();

    public void SetName(string NewName) => Name = NewName;
}