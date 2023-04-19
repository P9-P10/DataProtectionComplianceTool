using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;

namespace GraphManipulation.Managers.Interfaces;

public interface IIndividualsManager : IGetter<Individual, int>
{
    public void SetIndividualsSource(TableColumnPair source);
}