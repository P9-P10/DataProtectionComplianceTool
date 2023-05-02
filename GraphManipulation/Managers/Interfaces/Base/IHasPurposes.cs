namespace GraphManipulation.Managers.Interfaces.Base;

public interface IHasPurposes<TKey, TPurposeKey>
{
    public void AddPurpose(TKey key, TPurposeKey purposeKey);
    public void RemovePurpose(TKey key, TPurposeKey purposeKey);
}