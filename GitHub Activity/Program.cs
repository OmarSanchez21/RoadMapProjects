using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace GitHubActivityCLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: github-activity <username>");
                return;
            }

            string username = args[0];
            await FetchGitHubActivity(username);
        }

        static async Task FetchGitHubActivity(string username)
        {
            string apiUrl = $"https://api.github.com/users/{username}/events";
            
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // GitHub requires a user-agent header
                    client.DefaultRequestHeaders.Add("User-Agent", "GitHubActivityCLI");

                    // Fetch the data from GitHub API
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        
                        // Parse the JSON and display activity
                        var activities = JsonSerializer.Deserialize<JsonElement[]>(jsonResponse);
                        DisplayActivity(activities);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch activity. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        static void DisplayActivity(JsonElement[] activities)
        {
            if (activities.Length == 0)
            {
                Console.WriteLine("No recent activity found.");
                return;
            }

            foreach (var activity in activities)
            {
                string type = activity.GetProperty("type").GetString();
                string repoName = activity.GetProperty("repo").GetProperty("name").GetString();
                
                string action = type switch
                {
                    "PushEvent" => $"Pushed commits to {repoName}",
                    "PullRequestEvent" => $"Opened a pull request in {repoName}",
                    "IssuesEvent" => $"Opened an issue in {repoName}",
                    "WatchEvent" => $"Starred {repoName}",
                    _ => $"{type.Replace("Event", "")} in {repoName}"
                };

                Console.WriteLine(action);
            }
        }
    }
}
