namespace AICustomerReviewResponser.Services;

using System.Reflection;

public static class EmbeddedResource
{
    public static string Read(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath =
            assembly
                .GetManifestResourceNames()
                .FirstOrDefault(name =>
                    name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase)
                ) ?? throw new FileNotFoundException($"Resource '{resourceName}' not found.");
        using var stream =
            assembly.GetManifestResourceStream(resourcePath)
            ?? throw new FileNotFoundException($"Resource stream for '{resourceName}' is null.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
