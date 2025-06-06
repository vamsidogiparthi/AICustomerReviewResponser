// See https://aka.ms/new-console-template for more information
using AICustomerReviewResponser.Brain;
using AICustomerReviewResponser.options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var kernelBuilder = Kernel.CreateBuilder();

Console.WriteLine("Hello, World!");
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

kernelBuilder.Services.Configure<OpenAIConfiguration>(
    configuration.GetSection(OpenAIConfiguration.SectionName)
);

kernelBuilder.Services.Configure<GoogleAIConfiguration>(
    configuration.GetSection(GoogleAIConfiguration.SectionName)
);

kernelBuilder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    var openAIConfiguration =
        configuration.GetSection(OpenAIConfiguration.SectionName).Get<OpenAIConfiguration>()
        ?? throw new Exception("OpenAI configuration is missing");

    var googleAIConfiguration =
        configuration.GetSection(GoogleAIConfiguration.SectionName).Get<GoogleAIConfiguration>()
        ?? throw new Exception("Google AI configuration is missing");

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    // return new GoogleAIGeminiChatCompletionService(
    //     googleAIConfiguration.Model,
    //     googleAIConfiguration.ApiKey
    // );
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    return new OpenAIChatCompletionService(openAIConfiguration.Model, openAIConfiguration.ApiKey);
});

kernelBuilder.Services.AddSingleton<IBrain, Brain>();

kernelBuilder.Services.AddKeyedTransient(
    "CustomerReviewResponser",
    (sp, key) =>
    {
        KernelPluginCollection kernelFunctions = [];

        return new Kernel(sp, kernelFunctions);
    }
);

var kernel = kernelBuilder.Build();
var logger = kernel.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting Customer Review Responser...");
var brain = kernel.GetRequiredService<IBrain>();
