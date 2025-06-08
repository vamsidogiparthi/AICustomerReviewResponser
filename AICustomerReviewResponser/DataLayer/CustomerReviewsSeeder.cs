namespace AICustomerReviewResponser.DataLayer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

public interface ICustomerReviewDataSeeder
{
    Task SeedAsync(int count = 100);
}

public class CustomerReviewDataSeeder(ICustomerReviewDataStore customerReviewDataStore)
    : ICustomerReviewDataSeeder
{
    private static readonly string[] ProductNames =
    [
        "Laptop",
        "Smartphone",
        "Headphones",
        "Smartwatch",
        "Tablet",
        "Keyboard",
        "Mouse",
        "Monitor",
        "Printer",
        "Webcam",
    ];

    public async Task SeedAsync(int count = 100)
    {
        // Check if the collection exists and is empty.  If not empty, seeding is skipped.

        Console.WriteLine("Seeding CustomerReviews collection...");

        var reviews = GenerateCustomerReviews(count);

        await customerReviewDataStore.AddCustomerReviewAsync(reviews);

        Console.WriteLine($"Seeded {count} customer reviews.");
    }

    private static List<CustomerReview> GenerateCustomerReviews(int count)
    {
        var random = new Random();
        var reviews = new List<CustomerReview>();

        for (int i = 1; i <= count; i++)
        {
            // Randomly select a product name
            string productName = ProductNames[random.Next(ProductNames.Length)];

            // Determine sentiment (Positive, Negative, Neutral) with a random probability
            double sentimentProbability = random.NextDouble();
            string overallSentiment;
            if (sentimentProbability < 0.3)
            {
                overallSentiment = "Negative";
            }
            else if (sentimentProbability < 0.7)
            {
                overallSentiment = "Neutral";
            }
            else
            {
                overallSentiment = "Positive";
            }

            // Generate the review text based on the sentiment and product
            string reviewText = GenerateReviewText(productName, overallSentiment, i);

            // Generate the Sentiment Score (consistent with the overall sentiment)
            // Generate an agent response for some of the reviews

            reviews.Add(
                new CustomerReview
                {
                    Id = i,
                    ReviewText = reviewText,
                    AgentResponse = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)), // Reviews from the last 30 days
                }
            );
        }

        return reviews;
    }

    private static string GenerateReviewText(string productName, string overallSentiment, int id)
    {
        string review = overallSentiment switch
        {
            "Positive" =>
                $"I love my new {productName}! It's everything I hoped for. Great product! Review {id}",
            "Negative" =>
                $"I'm very disappointed with the {productName}. It broke after only a week. Don't buy it! Review {id}",
            "Neutral" =>
                $"The {productName} is okay. It does what it's supposed to do, but nothing special. Review {id}",
            _ => $"Review {id} for the {productName}.",
        };
        return review;
    }
}
