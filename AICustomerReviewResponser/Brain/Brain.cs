using System.Text.Json;
using AICustomerReviewResponser.DataLayer;
using AICustomerReviewResponser.Models;
using AICustomerReviewResponser.options;
using AICustomerReviewResponser.Services;
using Google.Apis.Util;
using Microsoft.Extensions.DependencyInjection;
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
        Task ProcessReviews();
        Task<string> GenerateReviewResponse(CustomerReview customerReview);
    }

    public class Brain(
        ILogger<Brain> logger,
        // IOptions<OpenAIConfiguration> options,
        [FromKeyedServices("CustomerReviewResponser")]
            Kernel kernel,
        ICustomerReviewDataStore customerReviewDataStore
    ) : IBrain
    {
        public async Task<string> GenerateReviewResponse(CustomerReview customerReview)
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

            var arguments = new KernelArguments() { { "userReview", customerReview } };
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

        public async Task ProcessReviews()
        {
            var customerReviews = await customerReviewDataStore.GetAllCustomerReviewsAsync();
            foreach (CustomerReview customerReview in customerReviews)
            {
                var response = await GenerateReviewResponse(customerReview);
                var responseJson =
                    JsonSerializer.Deserialize<SentimentAnalysisResponseDto>(response)
                    ?? throw new ArgumentNullException(nameof(SentimentAnalysisResponseDto));

                customerReview.AgentResponse = responseJson.AgentResponse;
                customerReview.UpdatedAt = DateTime.UtcNow;
                customerReview.OverallSentiment = responseJson.OverallSentiment;
                customerReview.PositivityScore = responseJson.PositivityScore;
                customerReview.NegativityScore = responseJson.NegativityScore;
                customerReview.NeutralScore = responseJson.NeutralScore;

                await customerReviewDataStore.UpdateCustomerReviewAsync(customerReview);

                logger.LogInformation("Data updated for the customer review {response}", response);
            }
        }
    }
}
