using System.Text.RegularExpressions;

namespace InsurancePartners.Web.Validation;

public static class OibValidator
{
    public static bool IsValid(string? oib)
    {
        if (string.IsNullOrWhiteSpace(oib))
        {
            return true;
        }

        oib = oib.Trim();

        if (!Regex.IsMatch(oib, @"^\d{11}$"))
        {
            return false;
        }

        var analysisDigit = 10;

        for (var i = 0; i < 10; i++)
        {
            var digit = oib[i] - '0';

            analysisDigit += digit;
            analysisDigit %= 10;

            if (analysisDigit == 0)
            {
                analysisDigit = 10;
            }

            analysisDigit = analysisDigit * 2 % 11;
        }

        var checksum = 11 - analysisDigit;

        if (checksum == 10)
        {
            checksum = 0;
        }

        var lastDigit = oib[10] - '0';

        return checksum == lastDigit;
    }
}