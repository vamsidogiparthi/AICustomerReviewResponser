namespace AICustomerReviewResponser.DataLayer;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CustomerReview
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("reviewText")]
    public string ReviewText { get; set; } = string.Empty;

    [BsonElement("agentResponse")]
    public string? AgentResponse { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("sentiment_positive_score")]
    public double? PositivityScore { get; set; }

    [BsonElement("sentiment_negative_score")]
    public double? NegativityScore { get; set; }

    [BsonElement("sentiment_neutral_score")]
    public double? NeutralScore { get; set; }

    [BsonElement("overallSentiment")]
    public string? OverallSentiment { get; set; } // Optional overall sentiment (e.g., Positive, Negative, Neutral)

    // Additional properties can be added as needed
}
