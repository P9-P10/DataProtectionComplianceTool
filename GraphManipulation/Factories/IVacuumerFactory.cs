using GraphManipulation.Vacuuming;

namespace GraphManipulation.Factories;

public interface IVacuumerFactory
{
    public IVacuumer CreateVacuumer();
}