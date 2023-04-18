using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public interface IIndividualsManager
{
    public IEnumerable<Individual> GetAllIndividuals();
    public Individual? GetIndividual(int individualId);
}