using Microsoft.Rest;
using System;

namespace BookingService.Client
{
    public partial class BookingServiceClient : BookingServiceAPI
    {
        public BookingServiceClient()
        {
            BaseUri = new Uri("http://localhost:6010");
            //Initialize();
        }

        //public void Initialize<TCredentials>(IApiClientInitializer<TCredentials> initializer)
        //{
        //    InitializeHttpClient(initializer.HttpClient, null);
        //    if (!initializer.EnableRetryPolicy)
        //    {
        //        SetRetryPolicy(null); // by default retry policy is enabled. Need to set NULL value to disable it
        //    }
        //    Credentials = initializer.Credentials as ServiceClientCredentials;
        //}
    }
}
