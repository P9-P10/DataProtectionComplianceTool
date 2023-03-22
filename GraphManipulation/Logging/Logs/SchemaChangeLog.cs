using GraphManipulation.Extensions;
using VDS.RDF;

namespace GraphManipulation.Logging.Logs;

public class SchemaChangeLog : Log
{
    public SchemaChangeLog(IGraph message) : base(LogType.SchemaChange, LogMessageFormat.Turtle, message.ToStorageString())
    {
    }
}