using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands.Factories;

public interface IVacuumerFactory
{
    public IVacuumer CreateVacuumer();
}