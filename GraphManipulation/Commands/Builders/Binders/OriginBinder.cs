using System.CommandLine;
using System.CommandLine.Binding;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

public class OriginBinder : BaseBinder<string, Origin>
{
    private readonly Option<string> _descriptionOption;

    public OriginBinder(Option<string> nameOption, Option<string> descriptionOption) : base(nameOption)
    {
        _descriptionOption = descriptionOption;
    }

    protected override Origin GetBoundValue(BindingContext bindingContext)
    {
        var origin = base.GetBoundValue(bindingContext);

        origin.Description = bindingContext.ParseResult.GetValueForOption(_descriptionOption);

        return origin;
    }
}