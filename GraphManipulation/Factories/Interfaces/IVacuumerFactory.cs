using GraphManipulation.Vacuuming;

namespace GraphManipulation.Factories.Interfaces;

public interface IVacuumerFactory
{
    public IVacuumer CreateVacuumer();
}