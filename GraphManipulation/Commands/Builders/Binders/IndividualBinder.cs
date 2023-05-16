using System.CommandLine;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

public class IndividualBinder : BaseBinder<int, Individual>
{
    public IndividualBinder(Option<int> keyOption, Option<string> descriptionOption) : base(keyOption, descriptionOption)
    {
    }
}