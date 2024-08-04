using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public interface ITitleGeneratorService
{
    Task InitializeAsync();
    Task<string> GenerateTitle(bool randomizeOptions);
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

    public async Task<string> GenerateTitle(bool randomizeOptions)
    {
        if (data == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateTitle(randomizeOptions);
    }

    private class Tables
    {
        private Random random = new Random();
        private Dictionary<string, HashSet<string>> usedItems = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, JsonElement> data;

        public Tables(Dictionary<string, JsonElement> data)
        {
            this.data = data;
            foreach (var key in new[] { "object", "entity", "location", "disposition", "religious_tenets" })
            {
                usedItems[key] = new HashSet<string>();
            }
        }

        private (string, int) GetRandomItem(string category, int randomRange)
        {
            int randomNum = random.Next(1, randomRange + 1);
            var item = data[category].EnumerateArray()
                .SelectMany(obj => obj.EnumerateObject())
                .Where(prop =>
                {
                    var range = prop.Value.GetString().Split('-').Select(int.Parse).ToArray();
                    return range[0] <= randomNum && randomNum <= range[1];
                })
                .FirstOrDefault(prop => !usedItems[category].Contains(prop.Name));

            if (item.Value.ValueKind == JsonValueKind.Undefined)
            {
                usedItems[category].Clear();
                return GetRandomItem(category, randomRange);
            }

            string key = item.Name;
            usedItems[category].Add(key);

            if (key.StartsWith("pick: "))
            {
                string[] options = key[6..].Split(", ");
                return (options[random.Next(options.Length)], randomNum);
            }

            return (key, randomNum);
        }

        public string TitleStructure()
        {
            int attempts = 0;
            while (attempts < 100)
            {
                int randomNum = random.Next(1, 21);
                var matchingStructure = data["title structure"].EnumerateArray()
                    .SelectMany(obj => obj.EnumerateObject())
                    .FirstOrDefault(prop =>
                    {
                        var range = prop.Value.GetString().Split('-').Select(int.Parse).ToArray();
                        return range[0] <= randomNum && randomNum <= range[1];
                    });

                if (matchingStructure.Name != null)
                {
                    return matchingStructure.Name;
                }
                attempts++;
            }

            return "The [object] of the [entity]";
        }

        public string GenerateTitle(bool randomizeOptions)
        {
            string title;
            Dictionary<string, string> usedReplacements = new Dictionary<string, string>();
            int attempts = 0;
            const int maxAttempts = 100;  // Prevent infinite loop

            do
            {
                attempts++;
                if (attempts > maxAttempts)
                {
                    return "Failed to generate a suitable title";
                }

                title = TitleStructure();
                usedReplacements.Clear();
                foreach (var category in new[] { "object", "entity", "location", "disposition", "religious_tenets" })
                {
                    usedItems[category].Clear();
                }

                foreach (var category in new[] { "object", "entity", "location", "disposition", "religious_tenets" })
                {
                    string pattern = $"[{category}]";
                    string misspelledPattern = category == "religious_tenets" ? "[religous tenets]" : pattern;

                    while (title.Contains(pattern) || title.Contains(misspelledPattern))
                    {
                        var (item, _) = GetRandomItem(category, title.Contains("𖤍") ? 6 : 20);

                        if (item == "[religious_tenets]" || item == "[religous tenets]")
                        {
                            var (religiousTenet, _) = GetRandomItem("religious_tenets", 20);
                            item = religiousTenet;
                        }

                        string replacement = category switch
                        {
                            "disposition" => item,
                            "location" => item.Contains("/") ? $"[{item}]" : item,
                            _ => $"[{item}]"
                        };

                        if (usedReplacements.ContainsValue(replacement))
                        {
                            // If we're using the same replacement, regenerate the title
                            title = "";
                            break;
                        }

                        usedReplacements[pattern] = replacement;
                        title = title.Replace(title.Contains("𖤍") ? $"𖤍{pattern}" : pattern, replacement);
                        title = title.Replace(title.Contains("𖤍") ? $"𖤍{misspelledPattern}" : misspelledPattern, replacement);
                    }

                    if (string.IsNullOrEmpty(title)) break; // Restart the generation process
                }

                if (title.Contains("A**An"))
                {
                    title = title.Replace("A**An", "[A/An]");
                }

                if (randomizeOptions)
                {
                    title = findOptions(title);
                }

                title = ApplyTheRule(title);

            } while (HasRepeatedTerms(title) || IsTitlePotentiallyProblematic(title));

            return title;
        }

        public string findOptions(string title)
        {
            Random random = new Random();

            string ReplaceMatch(Match match)
            {
                string options = match.Value.Trim('[', ']');
                string[] choices = options.Split('/');
                return choices[random.Next(choices.Length)];
            }

            string pattern = @"\[(?:[^\[\]]+)\]|(?:\b\w+(?:/\w+)+\b)";
            string result = Regex.Replace(title, pattern, ReplaceMatch);

            // Add spaces between options and following words
            result = Regex.Replace(result, @"(\w)([A-Z])", "$1 $2");

            // Handle A/An
            if (result.Contains("A/An"))
            {
                var vowels = new List<char> { 'A', 'E', 'I', 'O', 'U' };
                string[] resultSplit = result.Split(" ", 2);

                if (resultSplit.Length > 1)
                {
                    var firstLetter = resultSplit[1][0];
                    var firstLetterCapitalized = char.ToUpper(firstLetter);
                    var beginsWithVowel = vowels.Contains(firstLetterCapitalized);

                    result = (beginsWithVowel ? "An " : "A ") + resultSplit[1];
                }
            }

            return result;
        }

        public string ApplyTheRule(string title)
        {
            int roll = random.Next(1, 7);  // Roll a d6

            if (title.StartsWith("The "))
            {
                if (roll <= 2)
                {
                    // "The" remains
                    return title;
                }
                else if (roll == 3 || roll == 4)
                {
                    // Switch "The" with "A" or "An"
                    string restOfTitle = title.Substring(4);
                    return (IsVowel(restOfTitle[0]) ? "An " : "A ") + restOfTitle;
                }
                else
                {
                    // Remove "The" and add a Descriptor
                    string restOfTitle = title.Substring(4);
                    string descriptor = GetRandomDescriptor();
                    return $"{descriptor} {restOfTitle}";
                }
            }

            // If title doesn't start with "The", still add Descriptor on 5 or 6
            if (roll >= 5)
            {
                string descriptor = GetRandomDescriptor();
                return $"{descriptor} {title}";
            }

            return title;
        }

        private string GetRandomDescriptor()
        {
            int randomNum = random.Next(1, 21);
            var descriptorItem = data["descriptor"].EnumerateArray()
                .SelectMany(obj => obj.EnumerateObject())
                .FirstOrDefault(prop =>
                {
                    var range = prop.Value.GetString().Split('-').Select(int.Parse).ToArray();
                    return range[0] <= randomNum && randomNum <= range[1];
                });

            if (descriptorItem.Name != null)
            {
                string[] options = descriptorItem.Name.Split('/');
                return options[random.Next(options.Length)];
            }

            return "Mysterious"; // Default descriptor if something goes wrong
        }

        private bool IsVowel(char c)
        {
            return "AEIOUaeiou".IndexOf(c) >= 0;
        }

        private bool HasRepeatedTerms(string title)
        {
            var words = title.Split(' ');
            return words.Distinct().Count() != words.Length;
        }

        private bool IsTitlePotentiallyProblematic(string title)
        {
            // Add checks here, e.g., for very short titles
            return title.Split(' ').Length < 3;
        }
    }
}