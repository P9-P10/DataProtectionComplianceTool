using System.Globalization;
using System.Text.RegularExpressions;

namespace GraphManipulation.Vacuuming.Components;

public class Purpose
{
    private string _name;
    private string _expirationCondition;
    private string _origin;
    private bool _legallyRequired;
    private string _expiration_date;

    public string ExpirationCondition => _expirationCondition;
    public string GetExpirationDate => _expiration_date;

    public DateTime GetExpirationDateAsDateTime => ExpirationDateAsDatetime();

    private readonly string _timeFormat;

    public bool GetLegallyRequired => _legallyRequired;

    public Purpose(string name, string ttl, string expirationCondition, string origin, bool legallyRequired,
        string timeFormat = "yyyy-M-d h:m")
    {
        _name = name;
        _timeFormat = timeFormat;
        _expiration_date = ExpirationDateCalculator(ttl);
        _expirationCondition = expirationCondition.Replace("#", "'");
        _origin = origin;
        _legallyRequired = legallyRequired;
    }

    private string ExpirationDateCalculator(string inputString)
    {
        int years = 0, months = 0, days = 0, hours = 0, minutes = 0;

        string[] compontents = inputString.Split(" ");

        foreach (string component in compontents)
        {
            int componentNumber = GetNumberFromComponentGetNumberFromComponent(component);
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

        DateTime expirationDate = DateTime.Now.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours)
            .AddMinutes(-minutes);
        return expirationDate.ToString(_timeFormat, CultureInfo.InvariantCulture);
    }

    private int GetNumberFromComponentGetNumberFromComponent(string inputString)
    {
        Match match = Regex.Match(inputString, @"\d");
        return match.Success ? Convert.ToInt32(match.Value) : 0;
    }

    private DateTime ExpirationDateAsDatetime()
    {
        return DateTime.ParseExact(_expiration_date, _timeFormat, CultureInfo.InvariantCulture);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Purpose);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name, _expirationCondition, _origin, _legallyRequired, _expiration_date, _timeFormat);
    }

    private bool Equals(Purpose? other)
    {
        return other != null && GetExpirationDate == other.GetExpirationDate &&
               GetLegallyRequired == other.GetLegallyRequired && other._name == _name &&
               other._expirationCondition == _expirationCondition;
    }
}