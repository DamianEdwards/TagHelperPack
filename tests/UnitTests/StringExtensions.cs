namespace System;

internal static class StringExtensions
{
    public static string NormalizeLineEndings(this string text, string newLine = "\n")
    {
        return text.Replace("\r\n", newLine).Replace("\n", newLine);
    }
}
