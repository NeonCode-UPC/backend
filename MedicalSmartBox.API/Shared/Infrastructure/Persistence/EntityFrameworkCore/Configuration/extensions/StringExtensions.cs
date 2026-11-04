namespace MedicalSmartBox.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Humanizer;

public static class StringExtensions
{
    public static string ToSnakeCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        var snakeCase = System.Text.RegularExpressions.Regex.Replace(text.Trim(), "([A-Z]+)([A-Z][a-z])", "$1_$2");
        snakeCase = System.Text.RegularExpressions.Regex.Replace(snakeCase, "([a-z0-9])([A-Z])", "$1_$2");
        return snakeCase.Replace('-', '_').Replace(' ', '_').ToLowerInvariant();
    }

    public static string ToPlural(this string text)
    {
        return text.Pluralize(false);
    }
}
