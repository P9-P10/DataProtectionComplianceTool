using Timer = System.Timers.Timer;

namespace GraphManipulation.Vacuuming;

public class IntervalBasedVacuuming
{
    private readonly int _intervalInMinutes;
    private readonly IVacuumer _vacuumer;

    public IntervalBasedVacuuming(IVacuumer vacuumer, int intervalInMinutes = 5)
    {
        _vacuumer = vacuumer;
        _intervalInMinutes = intervalInMinutes;

        Timer timer = new(_intervalInMinutes * 6000);
        timer.Elapsed += (_, _) => ExecuteWrapper();
        timer.Start();
    }

    private void ExecuteWrapper()
    {
        _vacuumer.Execute();
    }
}