namespace GraphManipulation.Logging.Logs;

public class VacuumingLog : Log
{
    public VacuumingLog(string message) : base(LogType.Vacuuming, LogMessageFormat.Plaintext, message)
    {
        
    }
}