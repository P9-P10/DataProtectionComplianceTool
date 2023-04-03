using System.Globalization;
using System.Text.RegularExpressions;

namespace GraphManipulation.Vacuuming.Components;

public class Purpose
{
    private readonly string _timeFormat;
    private readonly string _name;
    private readonly string _origin;

    public Purpose(string name, string ttl, string expirationCondition, string origin, bool legallyRequired,
        string timeFormat = "yyyy-M-d h:m")
    {
        _name = name;
        _timeFormat = timeFormat;
        GetExpirationDate = ExpirationDateCalculator(ttl);
        ExpirationCondition = expirationCondition.Replace("#", "'");
        _origin = origin;
        GetLegallyRequired = legallyRequired;
    }

    public string ExpirationCondition { get; }

    public string GetExpirationDate { get; }

    public DateTime GetExpirationDateAsDateTime => ExpirationDateAsDatetime();

    public bool GetLegallyRequired { get; }

    /// <summary>
    ///     Calculates the expiration date for data, by parsing the input string.
    ///     This function is however no longer used in the code, as the same functionality can be achieved through SQL.
    ///     However, different SQL engines, have different ways, so we might choose to use this method later.
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    private string ExpirationDateCalculator(string inputString)
    {
        int years = 0, months = 0, days = 0, hours = 0, minutes = 0;

        var compontents = inputString.Split(" ");

        foreach (var component in compontents)
        {
            var componentNumber = GetNumberFromComponentGetNumberFromComponent(component);
            switch (component)
            {
                case { } when component.Contains('y'):
                    years += componentNumber;
                    break;
                case { } when component.Contains('m'):
                    months += componentNumber;
                    break;
                case { } when component.Contains('d'):
                    days += componentNumber;
                    break;
                case { } when component.Contains('h'):
                    hours += componentNumber;
                    break;
                case { } when component.Contains('M'):
                    minutes += componentNumber;
                    break;
            }
        }

        var expirationDate = DateTime.Now.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours)
            .AddMinutes(-minutes);
        return expirationDate.ToString(_timeFormat, CultureInfo.InvariantCulture);
    }

    private int GetNumberFromComponentGetNumberFromComponent(string inputString)
    {
        var match = Regex.Match(inputString, @"\d");
        return match.Success ? Convert.ToInt32(match.Value) : 0;
    }

    private DateTime ExpirationDateAsDatetime()
    {
        return DateTime.ParseExact(GetExpirationDate, _timeFormat, CultureInfo.InvariantCulture);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Purpose);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name, ExpirationCondition, _origin, GetLegallyRequired, GetExpirationDate,
            _timeFormat);
    }

    private bool Equals(Purpose? other)
    {
        return other != null && GetExpirationDate == other.GetExpirationDate &&
               GetLegallyRequired == other.GetLegallyRequired && other._name == _name &&
               other.ExpirationCondition == ExpirationCondition;
    }
}