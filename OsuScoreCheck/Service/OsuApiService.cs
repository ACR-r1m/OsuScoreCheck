using OsuScoreCheck.Models.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OsuScoreCheck.Service
{
    public class OsuApiService
    {
        private readonly string _tokenUrl = "https://osu.ppy.sh/oauth/token";
        private readonly string _apiBaseUrl = "https://osu.ppy.sh/api/v2";

        public async Task<string?> GetAccessTokenAsync(string clientId, string clientSecret)
        {
            var (token, _) = await GetAccessTokenWithErrorAsync(clientId, clientSecret);
            return token;
        }

        public async Task<(string? token, string? errorKey)> GetAccessTokenWithErrorAsync(string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                return (null, "ApiNotSaved");
            }

            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(6);

                var formData = new MultipartFormDataContent
                {
                    { new StringContent("client_credentials"), "grant_type" },
                    { new StringContent(clientId), "client_id" },
                    { new StringContent(clientSecret), "client_secret" },
                    { new StringContent("public"), "scope" }
                };

                var requestTask = client.PostAsync(_tokenUrl, formData) ;
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(6));
                var completedTask = await Task.WhenAny(requestTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    return (null, "Timeout");
                }

                var response = await requestTask;

                if (!response.IsSuccessStatusCode)
                {
                    return (null, "HttpError");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<OAuthResponse>(jsonResponse);

                if (string.IsNullOrEmpty(tokenResponse?.AccessToken))
                {
                    return (null, "AccessTokenError");
                }

                return (tokenResponse.AccessToken, null);
            }
            catch (TaskCanceledException ex)
            {
                return (null, "NetworkError");
            }
            catch (HttpRequestException ex)
            {
                return (null, "NoInternetConnection");
            }
            catch (Exception ex)
            {
                return (null, "UnknownError");
            }
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId, string accessToken)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string url = $"{_apiBaseUrl}/users/{userId}";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound) // 404
                {
                    return null; // Пользователь не найден
                }
                throw new InvalidOperationException($"Error: Ошибка запроса: {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UserProfile>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<List<MostPlayedBeatmap>> GetPlayedMapsBatchAsync(int userId, string accessToken, int limit, int offset)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string url = $"{_apiBaseUrl}/users/{userId}/beatmapsets/most_played?limit={limit}&offset={offset}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error: Ошибка запроса: {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            return System.Text.Json.JsonSerializer.Deserialize<List<MostPlayedBeatmap>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<MostPlayedBeatmap>();
        }

        public async Task<List<ScoreData>?> GetBestScoreDataAsync(int beatmapId, int userId, string accessToken)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string url = $"{_apiBaseUrl}/beatmaps/{beatmapId}/scores/users/{userId}/all";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Ошибка запроса: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var scoreApiResponse = JsonSerializer.Deserialize<ScoreApiResponse>(jsonResponse);

            return scoreApiResponse?.Scores;
        }
    }
}
