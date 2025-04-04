using System;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OllamaIntegration
{
    


    class Program
    {
        static async Task Main(string[] args)
        {
            List<string> test = new List<string>();
            // Set up the HTTP client
            HttpClient client = new HttpClient();

            // Ollama API endpoint
            string ollamaApiUrl = "http://localhost:11434/api/chat";

            // The request body
            var requestBody = new
            {
                model = "mistral",
                messages = new[]
                {
                new { role = "user", content = "how many letter in the word hello" }
            },
                max_tokens = 150
            };

            // Serialize the body as JSON
            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(ollamaApiUrl, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from Ollama:");
                //Console.WriteLine(responseBody);
                test.Add(responseBody);
                Console.WriteLine(test);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            foreach (var item in test)
            {
                Console.WriteLine(item);
            }
        }
    }
}