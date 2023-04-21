namespace GraphManipulation.Vacuuming;

public class IntervalBasedVacuuming
{
    private int _intervalInMinutes;
    private IVacuumer _vacuumer;

    public IntervalBasedVacuuming(IVacuumer vacuumer, int intervalInMinutes = 5)
    {
        _vacuumer = vacuumer;
        _intervalInMinutes = intervalInMinutes;

        System.Timers.Timer timer = new(_intervalInMinutes * 6000);
        timer.Elapsed += (_, _) => ExecuteWrapper();
        timer.Start();
    }

    private void ExecuteWrapper()
    {
        _vacuumer.Execute();
    }
}