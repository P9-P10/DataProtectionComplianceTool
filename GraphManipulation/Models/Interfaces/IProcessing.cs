using GraphManipulation.Managers;
using GraphManipulation.Managers.Archive;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IProcessing : INamedEntity, IDescribedEntity, IListable
{
    public IPurpose GetPurpose();
    public TableColumnPair GetPersonalDataTableColumnPair();
}