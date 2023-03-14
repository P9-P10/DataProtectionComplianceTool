using System.Collections.Generic;
using System.Text.RegularExpressions;
using GraphManipulation.Manipulation;
using Xunit;

namespace Test;

public class FunctionParserTest
{
    [Fact]
    public void TestGetParameters()
    {
        string query =
            "MARK http://www.test.com/presentation_database/main/MarketingInformation/email AS PERSONAL DATA";

        List<string> parameters = FunctionParser.GetParametersFromQuery(query);

        Assert.True(parameters.Count == 5);
    }

    [Fact]
    public void TestValidMarkAsPersonaldDataQuery_Returns_True_On_Valid_Query()
    {
        string correctQuery =
            "MARK http://www.test.com/presentation_database/main/MarketingInformation/email AS PERSONAL DATA";
        List<string> parameters = FunctionParser.GetParametersFromQuery(correctQuery);

        bool result = FunctionParser.IsValidMarkAsPersonaldDataQuery(parameters);
        Assert.True(result);
    }

    [Fact]
    public void TestValidMarkAsPersonaldDataQuery_Returns_False_On_InValid_Query()
    {
        string incorrectQuery =
            "Not A COrrect Query";

        List<string> parameters = FunctionParser.GetParametersFromQuery(incorrectQuery);
        bool result = FunctionParser.IsValidMarkAsPersonaldDataQuery(parameters);
        Assert.False(result);
    }
}