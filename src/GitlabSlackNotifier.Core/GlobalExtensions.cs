using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace GitlabSlackNotifier.Core;

public static class GlobalExtensions
{
    public static string ToSha256(this string randomString)
    {
        var crypt = new System.Security.Cryptography.SHA256Managed();
        var hash = new StringBuilder();
        var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
        foreach (byte theByte in crypto)
            hash.Append(theByte.ToString("x2"));
        return hash.ToString();
    }

    public static bool GetJiraTicket(this string input, out string value)
    {
        var match = new Regex(GlobalConstants.Jira.JiraTicketRegex).Match(input);
        value = match.Success ? match.Value : string.Empty;
        return match.Success;
    }
    
    public static List<(PropertyInfo Info, T? Attribute)> GetPropertyWithAttribute<T>(this Type type) where T : Attribute
        => type
            .GetProperties()
            .Select(x => (Info: x, Attribute: GetAttribute<T>(x)))
            .Where(x => x.Attribute != default)
            .ToList();

    private static T? GetAttribute<T>(PropertyInfo info) where T : Attribute
        => (T?)info
            .GetCustomAttributes(typeof(T), true)
            .FirstOrDefault();
    
    public static string ToSlackLink(this string text, string link)
        => $"<{link}|{text}>";
    
    
}