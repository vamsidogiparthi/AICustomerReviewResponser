using System.ComponentModel;
using AICustomerReviewResponser.Services;
using Microsoft.SemanticKernel;

namespace AICustomerReviewResponser.Plugins.FunctionPlugins;

public class CustomerCaseManagerPlugin
{
    [KernelFunction("generate_customer_case")]
    [Description("generates customer case when the customer review sentiment is negative")]
    public string GenerateCaseNumber(
        [Description("Customer Review Text")] string customerReview,
        [Description("Customer Review ID")] int customerReviewId
    )
    {
        var generate_customer_case = CaseNumberGenerator.GenerateUniqueCaseNumber(
            "CAS-YYYYMMDD-NNNN"
        );
        return generate_customer_case;
    }
}
