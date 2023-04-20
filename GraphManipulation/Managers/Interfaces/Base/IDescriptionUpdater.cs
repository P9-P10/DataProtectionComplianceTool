namespace GraphManipulation.Managers.Interfaces.Base;

public interface IDescriptionUpdater<T>
{
    public void UpdateDescription(T key, string description);
}