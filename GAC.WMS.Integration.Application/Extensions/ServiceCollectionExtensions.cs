using Polly.Extensions.Http;
using Polly;

namespace GAC.WMS.Integration.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddResiliencePolicies(this IServiceCollection services, IConfiguration configuration)
        {
            var retryCount = configuration.GetValue<int>("HttpClient:RetryCount", 3);
            var retryDelay = configuration.GetValue<int>("HttpClient:RetryDelayInMilliseconds", 200);
            var timeout = configuration.GetValue<int>("HttpClient:TimeoutInSeconds", 15);

            // Retry policy with exponential backoff
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount, retryAttempt =>
                    TimeSpan.FromMilliseconds(retryDelay * Math.Pow(2, retryAttempt)));

            // Circuit breaker policy
            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );

            // Timeout policy
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);

            services.AddHttpClient("ResilientClient")
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy)
                .AddPolicyHandler(timeoutPolicy);

            return services;
        }
    }
}
