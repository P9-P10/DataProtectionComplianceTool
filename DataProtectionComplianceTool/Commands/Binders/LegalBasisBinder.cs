using System.CommandLine;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Binders;

public class LegalBasisBinder : BaseBinder<string, LegalBasis>
{
    public LegalBasisBinder(Option<string> keyOption, Option<string> descriptionOption) : base(keyOption,
        descriptionOption)
    {
    }
}