using GraphManipulation.Manipulation;
using Xunit;

namespace Test;

public class FunctionParserTest
{
    [Fact]
    public void TestGetParameters()
    {
        var query =
            "MARK http://www.test.com/presentation_database/main/MarketingInformation/email AS PERSONAL DATA";

        var parameters = FunctionParser.GetParametersFromQuery(query);

        Assert.True(parameters.Count == 5);
    }

    [Fact]
    public void TestValidMarkAsPersonalDataQuery_Returns_True_On_Valid_Query()
    {
        var correctQuery =
            "MARK http://www.test.com/presentation_database/main/MarketingInformation/email AS PERSONAL DATA";
        var parameters = FunctionParser.GetParametersFromQuery(correctQuery);

        var result = FunctionParser.IsValidMarkAsPersonalDataQuery(parameters);
        Assert.True(result);
    }

    [Fact]
    public void TestValidMarkAsPersonalDataQuery_Returns_False_On_InValid_Query()
    {
        var incorrectQuery =
            "Not A COrrect Query";

        var parameters = FunctionParser.GetParametersFromQuery(incorrectQuery);
        var result = FunctionParser.IsValidMarkAsPersonalDataQuery(parameters);
        Assert.False(result);
    }
}