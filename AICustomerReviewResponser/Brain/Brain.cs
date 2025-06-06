using AICustomerReviewResponser.options;
using AICustomerReviewResponser.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace AICustomerReviewResponser.Brain
{
    public interface IBrain
    {
        void ProcessInput(string input);
        Task<string> GenerateReviewResponse();
    }

    public class Brain(
        ILogger<Brain> logger,
        // IOptions<OpenAIConfiguration> options,
        [FromKernelServices("CustomerReviewResponser")]
            Kernel kernel
    ) : IBrain
    {
        public async Task<string> GenerateReviewResponse()
        {
            logger.LogInformation("Started the customer review processor");
            var chatHistory = new ChatHistory();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };

            var template = EmbeddedResource.Read("ChatBotPromptTemplate.txt");

            var templateFactory = new HandlebarsPromptTemplateFactory();
            var promptTemplateConfig = new PromptTemplateConfig()
            {
                Template = template,
                TemplateFormat = "handlebars",
                Name = "ContosoChatPrompt",
            };

            var arguments = new KernelArguments()
            {
                // { "userMessage", userMessage },
                // { "history", summary.ToString() },
            };
            // Render the prompt
            var promptTemplate = templateFactory.Create(promptTemplateConfig);
            var renderedPrompt = await promptTemplate.RenderAsync(kernel, arguments);

            chatHistory.AddSystemMessage(renderedPrompt);
            var chatMessage = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                openAIPromptExecutionSettings,
                kernel: kernel
            );
            logger.LogInformation("Response > {chatMessage}", chatMessage);
            chatHistory.AddAssistantMessage(chatMessage.Content ?? string.Empty);
            return chatMessage.Content ?? string.Empty;
        }

        public void ProcessInput(string input)
        {
            throw new NotImplementedException();
        }
    }
}
