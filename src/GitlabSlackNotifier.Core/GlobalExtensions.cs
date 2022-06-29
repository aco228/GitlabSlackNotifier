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
}