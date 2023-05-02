using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IIndividualsManager : IGetter<IIndividual, int>
{
    public void SetIndividualsSource(TableColumnPair source);
    public TableColumnPair GetIndividualsSource();
}