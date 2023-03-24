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
        _expiration_date = expiration_date_calculator(ttl);
        _expirationCondition = expirationCondition;
        _origin = origin;
        _legallyRequired = legallyRequired;
    }

    private string expiration_date_calculator(string inputString)
    {
        int years = 0, months = 0, days = 0, hours = 0, minutes = 0;

        string[] compontents = inputString.Split(" ");

        foreach (string component in compontents)
        {
            int componentNumber = get_number_from_component(component);
            switch (component)
            {
                case { } a when a.Contains('y'):
                    years += componentNumber;
                    break;
                case { } b when b.Contains('m'):
                    months += componentNumber;
                    break;
                case { } c when c.Contains('d'):
                    days += componentNumber;
                    break;
                case { } d when d.Contains('h'):
                    hours += componentNumber;
                    break;
                case { } e when e.Contains('M'):
                    minutes += componentNumber;
                    break;
            }
        }

        DateTime expirationDate = DateTime.Now.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours)
            .AddMinutes(-minutes);
        return expirationDate.ToString(_timeFormat,CultureInfo.InvariantCulture);
    }

    private int get_number_from_component(string inputString)
    {
        Match match = Regex.Match(inputString, @"\d");
        if (match.Success)
        {
            return Convert.ToInt32(match.Value);
        }

        return 0;
    }

    private DateTime ExpirationDateAsDatetime()
    {
        return DateTime.ParseExact(_expiration_date, _timeFormat, CultureInfo.InvariantCulture);
    }
}