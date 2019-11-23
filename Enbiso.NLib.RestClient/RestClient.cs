using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Enbiso.NLib.RestClient
{
    public interface IRestClient
    {
        Task<HttpResponseMessage> Post(string service, string path, object payload);
        Task<HttpResponseMessage> Get(string service, string path);
        Task<HttpResponseMessage> Put(string service, string path, object payload);
        Task<HttpResponseMessage> Delete(string service, string path);
        Task<T> Post<T>(string service, string path, object payload);
        Task<T> Get<T>(string service, string path);
        Task<T> Put<T>(string service, string path, object payload);
        Task<T> Delete<T>(string service, string path);
    }
    
    public class RestClient: IRestClient
    {
        private readonly HttpClient _client;
        private readonly RestClientOptions _options;

        public RestClient(IHttpClientFactory clientFactory, IOptions<RestClientOptions> options)
        {
            _client = clientFactory.CreateClient();
            _options = options.Value;
        }

        public Task<HttpResponseMessage> Post(string service, string path, object payload)
            => _retryPerform(service, path,
                endpoint => _client.PostAsync(endpoint,
                    new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")));

        public Task<HttpResponseMessage> Put(string service, string path, object payload)
            => _retryPerform(service, path,
                endpoint => _client.PutAsync(endpoint,
                    new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")));
        
        public Task<HttpResponseMessage> Get(string service, string path)
            => _retryPerform(service, path, endpoint => _client.GetAsync(endpoint));
        
        public Task<HttpResponseMessage> Delete(string service, string path) 
            => _retryPerform(service, path, endpoint => _client.DeleteAsync(endpoint));

        public async Task<T> Post<T>(string service, string path, object payload) =>
            await ParseResponse<T>(await Post(service, path, payload));

        public async Task<T> Get<T>(string service, string path) => await ParseResponse<T>(await Get(service, path));

        public async Task<T> Put<T>(string service, string path, object payload) =>
            await ParseResponse<T>(await Put(service, path, payload));

        public async Task<T> Delete<T>(string service, string path) =>
            await ParseResponse<T>(await Delete(service, path));

        private async Task UpdateToken()
        {
            if (string.IsNullOrEmpty(_options.TokenEndpoint)) return;
            var resp = await _client.PostAsync(_options.TokenEndpoint, new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {"client_id", _options.ClientId},
                    {"client_secret", _options.ClientSecret},
                    {"grant_type", _options.GrantType},
                    {"scope", _options.Scope},
                }));
                
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Unable to fetch user information from Identity Service");
            var content = await resp.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Token>(content);
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
        }

        private async Task<HttpResponseMessage> _retryPerform(string service, string path, Func<string, Task<HttpResponseMessage>> perform)
        {
            if (string.IsNullOrWhiteSpace(_client.DefaultRequestHeaders.Authorization?.Parameter)) 
                await UpdateToken();

            if(!_options.Services.ContainsKey(service))
                throw new Exception($"Service {service} not found");
            
            var endpoint = $"{_options.Services[service]}{path}";

            var resp = await perform.Invoke(endpoint);
            if (resp.StatusCode != HttpStatusCode.Unauthorized) return resp;

            await UpdateToken();
            resp = await perform.Invoke(endpoint);
            return resp;
        }

        private static async Task<T> ParseResponse<T>(HttpResponseMessage resp)
        {
            var content = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new RestRequestFailedException(resp.StatusCode, content);
            return JsonSerializer.Deserialize<T>(content);
        }
        
        private class Token
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            [JsonPropertyName("expires_in")]
            public int ExpireIn { get; set; }
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
        }
    }
}