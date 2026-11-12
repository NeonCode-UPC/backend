namespace MedicalSmartBox.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;

public static class StringExtensions
{
    public static string ToKebabCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        var kebabCase = System.Text.RegularExpressions.Regex.Replace(text.Trim(), "([a-z0-9])([A-Z])", "$1-$2");
        kebabCase = System.Text.RegularExpressions.Regex.Replace(kebabCase, "([A-Z])([A-Z][a-z])", "$1-$2");
        return kebabCase.Replace('_', '-').Replace(' ', '-').ToLowerInvariant();
    }
}
