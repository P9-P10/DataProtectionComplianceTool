using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class PlaintextLogger : BaseLogger
{
    private int _lineNumber;
    
    public PlaintextLogger(ConfigManager configManager) : base(configManager)
    {
        var path = GetLogPath();
        
        var openWrite = File.OpenWrite(path);
        openWrite.Dispose();
        
        var lastLine = File.ReadLines(path).ToList().LastOrDefault();

        if (lastLine is null)
        {
            _lineNumber = 1;
        }
        else
        {
            var lineNumberString = lastLine.Split(Log.LogDelimiter()).First();
            if (int.TryParse(lineNumberString, out var result))
            {
                _lineNumber = result + 1;
            }
            else
            {
                throw new LoggerException("Line number could not be parsed: " + lineNumberString);
            }
            
        }
    }
    
    public override void Append(ILog log)
    {
        var writer = File.AppendText(GetLogPath());
        writer.WriteLine(_lineNumber + Log.LogDelimiter() + log.LogToString());
        _lineNumber++;
        writer.Dispose();
    }

    public override ILog Read()
    {
        throw new NotImplementedException();
    }

    public override List<ILog> ReadAll()
    {
        throw new NotImplementedException();
    }
}

