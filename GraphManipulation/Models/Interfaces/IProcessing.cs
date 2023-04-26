using GraphManipulation.Managers;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IProcessing : INamedEntity, IDescribedEntity, IListable
{
    public IPurpose GetPurpose();
    public IPersonalDataColumn GetPersonalDataColumn();
    public TableColumnPair GetPersonalDataTableColumnPair();
}