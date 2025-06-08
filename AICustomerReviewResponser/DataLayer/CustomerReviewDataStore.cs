using AICustomerReviewResponser.options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AICustomerReviewResponser.DataLayer;

public interface ICustomerReviewDataStore
{
    Task<CustomerReview?> GetCustomerReviewAsync(int id);
    Task<IEnumerable<CustomerReview>> GetAllCustomerReviewsAsync();
    Task AddCustomerReviewAsync(CustomerReview review);
    Task AddCustomerReviewAsync(IEnumerable<CustomerReview> reviews);
    Task UpdateCustomerReviewAsync(CustomerReview review);
    Task DeleteCustomerReviewAsync(int id);
}

public class CustomerReviewDataStore : ICustomerReviewDataStore
{
    public CustomerReviewDataStore(IOptions<CustomerReviewDBOptions> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var database = mongoClient.GetDatabase(options.Value.DatabaseName);
        _customerReviews = database.GetCollection<CustomerReview>(options.Value.CollectionName);
    }

    private readonly IMongoCollection<CustomerReview> _customerReviews;

    public async Task<CustomerReview?> GetCustomerReviewAsync(int id)
    {
        return await _customerReviews.Find(review => review.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<CustomerReview>> GetAllCustomerReviewsAsync()
    {
        return await _customerReviews.Find(_ => true).ToListAsync();
    }

    public async Task AddCustomerReviewAsync(CustomerReview review)
    {
        await _customerReviews.InsertOneAsync(review);
    }

    public async Task UpdateCustomerReviewAsync(CustomerReview review)
    {
        await _customerReviews.ReplaceOneAsync(r => r.Id == review.Id, review);
    }

    public async Task DeleteCustomerReviewAsync(int id)
    {
        await _customerReviews.DeleteOneAsync(review => review.Id == id);
    }

    public async Task AddCustomerReviewAsync(IEnumerable<CustomerReview> reviews)
    {
        await _customerReviews.InsertManyAsync(reviews);
    }
}
