using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

public interface ITitleGeneratorService
{
    Task InitializeAsync();
    Task<string> GenerateTitle();
}

public class TitleGeneratorService : ITitleGeneratorService
{
    private readonly HttpClient _httpClient;
    private Dictionary<string, JsonElement> data;
    private Tables tables;

    public TitleGeneratorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("data/conanData.json");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);
                tables = new Tables(data);
            }
            else
            {
                Console.WriteLine($"Failed to load data. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    public async Task<string> GenerateTitle()
    {
        if (data == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateTitle();
    }

    private class Tables
    {
        private Random random = new Random();
        private HashSet<string> usedObjects = new HashSet<string>();
        private HashSet<string> usedEntity = new HashSet<string>();
        private Dictionary<string, JsonElement> data;

        public Tables(Dictionary<string, JsonElement> data)
        {
            this.data = data;
        }

        public (string, int) Object(int randomRange)
        {
            int randomNum = random.Next(1, randomRange + 1);
            while (true)
            {
                foreach (var obj in data["object"].EnumerateArray())
                {
                    foreach (var property in obj.EnumerateObject())
                    {
                        string key = property.Name;
                        string value = property.Value.GetString();
                        int[] range = value.Split('-').Select(int.Parse).ToArray();
                        if (range[0] <= randomNum && randomNum <= range[1])
                        {
                            if (usedObjects.Contains(key))
                            {
                                randomNum = random.Next(1, randomRange + 1);
                            }
                            else
                            {
                                if (key.Contains("pick"))
                                {
                                    string[] options = key.Split(": ")[1].Split(", ");
                                    string selectedOption = options[random.Next(options.Length)];
                                    usedObjects.Add(key);
                                    return (selectedOption, randomNum);
                                }
                                else
                                {
                                    usedObjects.Add(key);
                                    return (key, randomNum);
                                }
                            }
                        }
                    }
                }
            }
        }

        public (string, int) Entity(int randomRange)
        {
            int randomNum = random.Next(1, randomRange + 1);
            while (true)
            {
                foreach (var ent in data["entity"].EnumerateArray())
                {
                    foreach (var property in ent.EnumerateObject())
                    {
                        string key = property.Name;
                        string value = property.Value.GetString();
                        int[] range = value.Split('-').Select(int.Parse).ToArray();
                        if (range[0] <= randomNum && randomNum <= range[1])
                        {
                            if (usedEntity.Contains(key))
                            {
                                randomNum = random.Next(1, randomRange + 1);
                            }
                            else
                            {
                                if (key.Contains("pick"))
                                {
                                    string[] options = key.Split(": ")[1].Split(", ");
                                    string selectedOption = options[random.Next(options.Length)];
                                    usedEntity.Add(key);
                                    return (selectedOption, randomNum);
                                }
                                else
                                {
                                    usedEntity.Add(key);
                                    return (key, randomNum);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string TitleStructure()
        {
            int randomNum = random.Next(1, 21);
            foreach (var title in data["title structure"].EnumerateArray())
            {
                foreach (var property in title.EnumerateObject())
                {
                    string key = property.Name;
                    string value = property.Value.GetString();
                    int[] range = value.Split('-').Select(int.Parse).ToArray();
                    if (range[0] <= randomNum && randomNum <= range[1])
                    {
                        return key;
                    }
                }
            }
            return string.Empty;
        }

        public string GenerateTitle()
        {
            string title = TitleStructure();
            while (title.Contains("[object]"))
            {
                if (title.Contains("𖤍"))
                {
                    var (obj, _) = Object(6);
                    title = title.Replace("𖤍[object]", "[" + obj + "]");
                }
                else
                {
                    var (obj, _) = Object(20);
                    title = title.Replace("[object]", "[" + obj + "]");
                }
            }

            while (title.Contains("[entity]"))
            {
                if (title.Contains("𖤍"))
                {
                    var (ent, _) = Entity(6);
                    title = title.Replace("𖤍[entity]", "[" + ent + "]");
                }
                else
                {
                    var (ent, _) = Entity(20);
                    title = title.Replace("[entity]", "[" + ent + "]");
                }
            }
            return title;
        }
    }
}