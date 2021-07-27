﻿namespace MessageSending
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public class MessageSendingRequestVerifiyer : IMessageSendingRequestVerifiyer
    {
        private readonly HttpClient _httpClient;
        private readonly string _verifyServiceSecretKey;

        public MessageSendingRequestVerifiyer(
            HttpClient httpClient,
            IVerifyServiceConfiguration verifyServiceConfiguration)
        {
            _httpClient = httpClient;
            _verifyServiceSecretKey = verifyServiceConfiguration.SecretKey;
        }

        public async Task<bool> VerifiyMessageSendingRequest(string token, string remoteIPAddress)
        {
            var content = JsonContent.Create(new RecaptchaVerifyRequest
            {
                Secret = _verifyServiceSecretKey,
                Response = token,
                Remoteip = remoteIPAddress,
            });
            var httpResponse =
                await _httpClient
                    .PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var recaptchaVerifyResponse =
                await httpResponse.Content
                    .ReadFromJsonAsync<RecaptchaVerifyResponse>();
            return recaptchaVerifyResponse.Success;
        }

        class RecaptchaVerifyRequest
        {
            public string Secret { get; set; }
            public string Response { get; set; }
            public string Remoteip { get; set; }
        }

        class RecaptchaVerifyResponse
        {
            public bool Success { get; set; }
            public string Challenge_ts { get; set; }
            public string Hostname { get; set; }
        }
    }
}