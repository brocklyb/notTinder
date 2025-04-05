using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OllamaIntegration
{
    class Program
    {
        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }

        public class OllamaResponse
        {
            public string model { get; set; }
            public string created_at { get; set; }
            public Message? message { get; set; }
            public bool done { get; set; }
        }

        static async Task Main(string[] args)
        {
            List<OllamaResponse> responses = new List<OllamaResponse>();
            StringBuilder fullResponse = new StringBuilder();

            HttpClient client = new HttpClient();
            string ollamaApiUrl = "http://localhost:11434/api/chat";

            Console.WriteLine("What would you like to ask? ");
            string askAQuestion = Console.ReadLine();

            var requestBody = new
            {
                model = "mistral",
                messages = new[]
                {
                    new { role = "user", content = askAQuestion  }
                },
                stream = true,
                max_tokens = 150
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, ollamaApiUrl)
                {
                    Content = content
                };

                HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);

                Console.WriteLine("AI response:\n");

                while (!reader.EndOfStream)
                {
                    string? line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var chunk = JsonConvert.DeserializeObject<OllamaResponse>(line);
                        if (chunk?.message?.content != null)
                        {
                            responses.Add(chunk);
                            fullResponse.Append(chunk.message.content);
                            Console.Write(chunk.message.content); // Optional: stream output live
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"⚠️ Skipped malformed JSON: {line}");
                        Console.WriteLine($"   Error: {ex.Message}");
                    }
                }

                //Console.WriteLine("\n\nFinal Full Response:");
                //Console.WriteLine(fullResponse.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }
}
