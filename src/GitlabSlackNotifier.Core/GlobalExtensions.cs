using System.Reflection;
using System.Text;

namespace GitlabSlackNotifier.Core;

public static class GlobalExtensions
{
    public static string ToSha256(this string randomString)
    {
        var crypt = new System.Security.Cryptography.SHA256Managed();
        var hash = new StringBuilder();
        byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
        foreach (byte theByte in crypto)
            hash.Append(theByte.ToString("x2"));

        return hash.ToString();
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
    
    
}