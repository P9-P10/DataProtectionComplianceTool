namespace GraphManipulation.Helpers;

public interface IConfigManager
{
    public string GetValue(string key);
    public void UpdateValue(string key, string value);

    public List<string> GetEmptyKeys();
}