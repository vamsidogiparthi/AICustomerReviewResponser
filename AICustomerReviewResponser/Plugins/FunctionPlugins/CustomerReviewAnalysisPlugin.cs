using System.ComponentModel;
using AICustomerReviewResponser.options;
using AICustomerReviewResponser.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace AICustomerReviewResponser.Plugins.FunctionPlugins;

public class CustomerReviewAnalysisPlugin(
    ILogger<CustomerReviewAnalysisPlugin> logger,
    IOptions<OpenAIConfiguration> options
)
{
    [KernelFunction("analyze_customer_review")]
    [Description(
        "analyze the customer review, perform sentiment analysis, respond to the customer review"
    )]
    public async Task<FunctionResult> AnalyzeCustomerReview(
        [Description("customer review ")] string customerReview,
        Kernel kernel
    )
    {
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
            ModelId = options.Value.Model,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

        var handlebarsPromptYaml = EmbeddedResource.Read("CustomerReviewPromptTemplate.yaml");
        var templateFactory = new HandlebarsPromptTemplateFactory();
        var function = kernel.CreateFunctionFromPromptYaml(handlebarsPromptYaml, templateFactory);

        var arguments = new KernelArguments()
        {
            { "userReview", customerReview },
            { "executionSettings", openAIPromptExecutionSettings },
        };

        var response = await kernel.InvokeAsync(function, arguments);
        logger.LogInformation("function response {response}", response);
        return response;
    }
}
