using System.CommandLine;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

public class VacuumingRuleBinder : BaseBinder<string, VacuumingRule>
{
    public VacuumingRuleBinder(
        Option<string> keyOption, 
        Option<string> descriptionOption) : base(keyOption, descriptionOption)
    {
    }
}