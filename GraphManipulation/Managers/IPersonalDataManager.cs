using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public interface IPersonalDataManager
{
    public PersonalDataColumn AddPersonalData(string table, string column, string joinCondition, string description);
    public PersonalDataColumn UpdatePersonalDataDescription(string table, string column, string description);
    public void DeletePersonalData(string table, string column);
    public IEnumerable<PersonalDataColumn> GetAllPersonalData();
    public PersonalDataColumn? GetPersonalData(string table, string column);

    public PersonalDataColumn AddPurpose(string table, string column, string purposeName);
    public PersonalDataColumn RemovePurpose(string table, string column, string purposeName);

    public void SetOrigin(string table, string column, int individualsId, string originName);
    public Origin GetOriginOf(string table, string column, int individualsId);
}