using System.Text.Json.Serialization;

namespace AICustomerReviewResponser.Models;

public class SentimentAnalysisResponseDto
{
    [JsonPropertyName("review_response")]
    public string? AgentResponse { get; set; }

    [JsonPropertyName("sentiment_positive_score")]
    public double? PositivityScore { get; set; }

    [JsonPropertyName("sentiment_negative_score")]
    public double? NegativityScore { get; set; }

    [JsonPropertyName("sentiment_neutral_score")]
    public double? NeutralScore { get; set; }

    [JsonPropertyName("overall_sentiment")]
    public string? OverallSentiment { get; set; }

    [JsonPropertyName("case_number")]
    public string? CaseNumber { get; set; }
}
