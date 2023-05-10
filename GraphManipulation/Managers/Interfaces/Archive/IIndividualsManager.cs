using GraphManipulation.Managers.Archive;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces.Archive;

public interface IIndividualsManager : IGetter<IIndividual, int>
{
    public void SetIndividualsSource(TableColumnPair source);
    public TableColumnPair GetIndividualsSource();
}