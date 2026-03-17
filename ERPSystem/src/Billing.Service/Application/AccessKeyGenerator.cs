using System.Security.Cryptography;
using System.Text;

namespace Billing.Service.Application;

public static class AccessKeyGenerator
{
    public static string Generate(string ruc, DateOnly date, string sequence)
    {
        var baseKey = $"{date:ddMMyyyy}01{ruc}1{sequence.PadLeft(9, '0')}123456781";
        var verifier = Mod11(baseKey);
        return baseKey + verifier;
    }

    private static int Mod11(string value)
    {
        var multipliers = new[] { 2, 3, 4, 5, 6, 7 };
        var sum = 0;
        for (int i = value.Length - 1, m = 0; i >= 0; i--, m++)
        {
            sum += (value[i] - '0') * multipliers[m % multipliers.Length];
        }

        var result = 11 - (sum % 11);
        return result switch { 11 => 0, 10 => 1, _ => result };
    }
}
