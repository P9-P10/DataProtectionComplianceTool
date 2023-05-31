using System.CommandLine;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Binders;

public class OriginBinder : BaseBinder<string, Origin>
{
    public OriginBinder(Option<string> keyOption, Option<string> descriptionOption) : base(keyOption, descriptionOption)
    {
    }
}