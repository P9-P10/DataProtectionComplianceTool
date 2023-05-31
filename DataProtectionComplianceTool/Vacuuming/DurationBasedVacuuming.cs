using Timer = System.Timers.Timer;

namespace GraphManipulation.Vacuuming;

public class DurationBasedVacuuming
{
    private readonly int _durationInMinutes;
    private readonly IVacuumer _vacuumer;

    public DurationBasedVacuuming(IVacuumer vacuumer, int durationInMinutes = 5)
    {
        _vacuumer = vacuumer;
        _durationInMinutes = durationInMinutes;

        Timer timer = new(_durationInMinutes * 6000);
        timer.Elapsed += (_, _) => ExecuteWrapper();
        timer.Start();
    }

    private void ExecuteWrapper()
    {
        _vacuumer.Execute();
    }
}