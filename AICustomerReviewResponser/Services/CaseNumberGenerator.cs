using System;
using System.Collections.Generic;
using System.Linq;

namespace AICustomerReviewResponser.Services;

public static class CaseNumberGenerator
{
    private static readonly Random Random = new();
    private static readonly HashSet<string> GeneratedCaseNumbers = new(); // Store generated numbers for uniqueness

    /// <summary>
    /// Generates a random, unique case number in one of several CRM-like formats.
    /// </summary>
    /// <param name="format">Optional:  Specific format to use. If null, a random format is chosen.</param>
    /// <returns>A unique case number string.</returns>
    public static string GenerateUniqueCaseNumber(string? format = null)
    {
        string caseNumber;
        int maxAttempts = 100; // Prevent infinite loops if uniqueness is impossible

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (string.IsNullOrEmpty(format))
            {
                // Choose a random format
                int formatIndex = Random.Next(0, 4); // Number of formats defined below
                caseNumber = GenerateCaseNumber(formatIndex);
            }
            else
            {
                // Use the specified format
                caseNumber = GenerateCaseNumber(format);
            }

            // Ensure uniqueness
            if (GeneratedCaseNumbers.Add(caseNumber))
            {
                return caseNumber;
            }
        }

        throw new Exception(
            $"Failed to generate a unique case number after {maxAttempts} attempts."
        );
    }

    private static string GenerateCaseNumber(int formatIndex)
    {
        switch (formatIndex)
        {
            case 0: // CAS-YYYYMMDD-NNNN
                return $"CAS-{DateTime.Now:yyyyMMdd}-{Random.Next(1000, 9999)}";
            case 1: // INC-YYYY-NNNNN
                return $"INC-{DateTime.Now:yyyy}-{Random.Next(10000, 99999)}";
            case 2: // CRM-NNNNNN
                return $"CRM-{Random.Next(100000, 999999)}";
            case 3: // CS-YYMM-NNNN
                return $"CS-{DateTime.Now:yyMM}-{Random.Next(1000, 9999)}";
            default:
                throw new ArgumentException("Invalid format index.");
        }
    }

    private static string GenerateCaseNumber(string format)
    {
        // Implement handling for specific formats provided as strings
        // Example: "CUSTOM-{YYYY}-{NNNN}"
        // You'll need to parse the format string and generate the case number accordingly

        switch (format.ToUpper())
        {
            case "CAS-YYYYMMDD-NNNN":
                return $"CAS-{DateTime.Now:yyyyMMdd}-{Random.Next(1000, 9999)}";
            case "INC-YYYY-NNNNN":
                return $"INC-{DateTime.Now:yyyy}-{Random.Next(10000, 99999)}";
            case "CRM-NNNNNN":
                return $"CRM-{Random.Next(100000, 999999)}";
            case "CS-YYMM-NNNN":
                return $"CS-{DateTime.Now:yyMM}-{Random.Next(1000, 9999)}";
            default:
                throw new ArgumentException($"Unsupported format: {format}");
        }
    }
}
