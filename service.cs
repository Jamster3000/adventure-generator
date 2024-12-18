﻿using System;
using System.Text.Json;
using System.Text.RegularExpressions;

public interface ITitleGeneratorService
{
    Task InitializeAsync();
    Task<string> GenerateTitle(bool randomizeOptions);
    Task<string> GenerateOpeningScene();
    Task<string> GenerateNonPlayerCharacter();
    Task<string> GenerateTemperament();
    Task<string> GenerateQuirks();
    Task<string> GenerateCult();
    Task<string> GenerateOpponent();
    Task<string> GenerateNationalities();
    Task<string> GenerateHooksAndDraws();
    Task<string> GeneratePlots();
    Task<string> GenerateAntagonist();
    Task<string> GeneratePlotFulfillment();
    Task<string> GenerateObstaclesAndTwists();
    Task<string> GenerateGoalsAndObjectives();

    Task<string> GenerateArtifact();
    Task<string> GenerateLocation();
    Task<string> GenerateBuilding();
    Task<string> GenerateMilitary();
    Task<string> GenerateKnowledgePerson();
    Task<string> GenerateRuralMarine();
    Task<string> GenerateOldCreature();
    Task<string> GenerateLegendaryCharacter();
    Task<string> GenerateLegendaryArtifact();
}

public class TitleGeneratorService : ITitleGeneratorService
{
    private readonly HttpClient _httpClient;
    private Dictionary<string, JsonElement> _data;
    private Tables _tables;

    public TitleGeneratorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("data/conanData.json");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);
            _tables = new Tables(_data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GenerateArtifact() => await EnsureInitializedAndGenerate(_tables.GenerateArtifact);
    public async Task<string> GenerateLocation() => await EnsureInitializedAndGenerate(_tables.GenerateLocation);
    public async Task<string> GenerateBuilding() => await EnsureInitializedAndGenerate(_tables.GenerateBuilding);
    public async Task<string> GenerateMilitary() => await EnsureInitializedAndGenerate(_tables.GenerateMilitary);
    public async Task<string> GenerateKnowledgePerson() => await EnsureInitializedAndGenerate(_tables.GenerateKnowledgePerson);
    public async Task<string> GenerateRuralMarine() => await EnsureInitializedAndGenerate(_tables.GenerateRuralMarine);
    public async Task<string> GenerateOldCreature() => await EnsureInitializedAndGenerate(_tables.GenerateOldCreature);
    public async Task<string> GenerateLegendaryCharacter() => await EnsureInitializedAndGenerate(_tables.GenerateLegendaryCharacter);
    public async Task<string> GenerateLegendaryArtifact() => await EnsureInitializedAndGenerate(_tables.GenerateLegendaryArtifact);
    public async Task<string> GenerateHooksAndDraws() => await EnsureInitializedAndGenerate(_tables.GenerateHooksAndDraws);
    public async Task<string> GeneratePlots() => await EnsureInitializedAndGenerate(_tables.GeneratePlots);
    public async Task<string> GenerateAntagonist() => await EnsureInitializedAndGenerate(_tables.GenerateAntagonist);
    public async Task<string> GeneratePlotFulfillment() => await EnsureInitializedAndGenerate(_tables.GeneratePlotFulfillment);
    public async Task<string> GenerateObstaclesAndTwists() => await EnsureInitializedAndGenerate(_tables.GenerateObstaclesAndTwists);
    public async Task<string> GenerateGoalsAndObjectives() => await EnsureInitializedAndGenerate(_tables.GenerateGoalsAndObjectives);
    public async Task<string> GenerateOpeningScene() => await EnsureInitializedAndGenerate(_tables.GenerateOpeningScene);
    public async Task<string> GenerateCult() => await EnsureInitializedAndGenerate(_tables.GenerateCult);
    public async Task<string> GenerateNationalities() => await EnsureInitializedAndGenerate(_tables.GenerateNationalities);
    public async Task<string> GenerateOpponent() => await EnsureInitializedAndGenerate(_tables.GenerateOpponent);
    public async Task<string> GenerateTemperament() => await EnsureInitializedAndGenerate(_tables.GenerateTemperament);
    public async Task<string> GenerateQuirks() => await EnsureInitializedAndGenerate(_tables.GenerateQuirks);
    public async Task<string> GenerateNonPlayerCharacter() => await EnsureInitializedAndGenerate(_tables.GenerateNonPlayerCharacter);
    public async Task<string> GenerateTitle(bool randomizeOptions) => await EnsureInitializedAndGenerate(() => _tables.GenerateTitle(randomizeOptions));

    private async Task<string> EnsureInitializedAndGenerate(Func<string> generator)
    {
        if (_data == null)
        {
            await InitializeAsync();
        }
        return generator();
    }

    private class Tables
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<string, HashSet<string>> _usedItems = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, JsonElement> _data;

        public Tables(Dictionary<string, JsonElement> data)
        {
            _data = data;
            foreach (var key in new[] { "object", "entity", "location", "disposition", "religious_tenets" })
            {
                _usedItems[key] = new HashSet<string>();
            }
        }

        public string GenerateMilitary() => GenerateRandomItem("military_or_otherworldly_sites");
        public string GenerateRuralMarine() => GenerateRandomItem("rural_or_marine_locations");

        private string GenerateRandomItem(string tableName)
        {
            var table = _data[tableName].EnumerateArray().ToList();
            int firstRoll = _random.Next(1, 20);
            int secondRoll = _random.Next(1, 20);

            // Find the first selected item
            var selectedItem1 = table.FirstOrDefault(item =>
            {
                foreach (var prop in item.EnumerateObject())
                {
                    if (int.Parse(prop.Value.GetString()) == firstRoll)
                        return true;
                }
                return false;
            });

            // Find the second selected item
            var selectedItem2 = table.FirstOrDefault(item =>
            {
                foreach (var prop in item.EnumerateObject())
                {
                    if (int.Parse(prop.Value.GetString()) == secondRoll)
                        return true;
                }
                return false;
            });

            // Extract the property names (e.g., "Command tent") for the selected items
            string selectedItem1Name = selectedItem1.EnumerateObject().FirstOrDefault().Name;
            string selectedItem2Name = selectedItem2.EnumerateObject().FirstOrDefault().Name;

            return $"{selectedItem1Name}***{selectedItem2Name}";
        }


        public string GenerateGoalsAndObjectives() => GenerateFromTable("goals_and_objectives", "Objective");
        public string GenerateObstaclesAndTwists() => GenerateFromTable("obstacles_and_twists", "Plot Twist");
        public string GeneratePlotFulfillment() => GenerateFromTable("plot_fulfillment_and_location", "From/Where");
        public string GenerateAntagonist() => GenerateFromTable("primary_antagonist", "Trait");
        public string GeneratePlots() => GenerateFromTable("plot_concept", "Motivation");
        public string GenerateHooksAndDraws() => GenerateFromTable("hooks_and_draws", "Draw");
        public string GenerateBuilding() => GenerateFromTable("building_type", "Sinister");

        private string GenerateFromTable(string tableName, string secondPropertyName)
        {
            var table = _data[tableName].EnumerateArray().ToList();
            int roll1 = _random.Next(1, 20);
            int roll2 = _random.Next(1, 20);

            // First Roll (Get Item)
            var selectedItem1 = table.FirstOrDefault(item =>
                int.Parse(item.EnumerateObject().First().Value.GetString()) == roll1);

            if (selectedItem1.ValueKind == JsonValueKind.Undefined)
            {
                return $"Failed to generate from {tableName}";
            }

            string firstPart = selectedItem1.EnumerateObject().First().Name;
            string secondPart = table[roll2 - 1].GetProperty(secondPropertyName).GetString();

            // Roll-based transformations for various phrases
            var replacements = new Dictionary<string, Func<string, string>>()
{
    // Objective phrases
    { "an ancient map to (roll on sinister column of building type table)", (s) =>
        {
            return "An ancient map to: " + GenerateFromRangeTable("building_type", 1, 20);  // Sinister column logic
        }
    },
    { "identity of a person (primary antagonist table)", (s) =>
        {
            return "Identity of a person: " + GenerateFromRangeTable("primary_antagonist", 1, 20);
        }
    },
    { "An opponent (Opponents table)", (s) =>
        {
            return "An opponent: " + GenerateFromRangeTable("opponent", 1, 20);
        }
    },
    { "an artifact (artifact descriptors and conditions table)", (s) =>
        {
            // Handle nested object structure properly in the artifact descriptors table
            var artifact = GenerateFromRangeTable("artifact_descriptors_and_conditions", 1, 20);
            var artifactObject = _data["artifact_descriptors_and_conditions"].EnumerateArray().ElementAtOrDefault(roll2 - 1);

            if (artifactObject.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate from artifact_descriptors_and_conditions after 5 attempts.";
            }

            string descriptor = artifactObject.EnumerateObject().First().Value.GetProperty("Descriptor").GetString();
            string condition = artifactObject.EnumerateObject().First().Value.GetProperty("Condition").GetString();

            if (condition == "None") {
                return $"An artifact: {descriptor}";
            }
            else
            {
                return $"An artifact: {descriptor}";
            }
        }
    },

    // Updated person objective to knowledge_or_person_objectives table
    { "roll on person_objectives table", (s) =>
        {
            return "Knowledge or person objective: " + GenerateFromRangeTable("knowledge_or_person_objectives", 1, 20);
        }
    },

    // Plot location and twists
    { "a deadly or secret organisation (cult table)", (s) =>
        {
            return "A deadly or secret organisation: " + GenerateFromRangeTable("cult", 1, 20);
        }
    },
    {
    "a physical location (location choice and appropriate sub-table)", (s) =>
        {
            var locationAndAtmosphere = GenerateLocation();

            var location = locationAndAtmosphere.Split("***")[1];

            return $"A physical location: {location}";
        }
    },
    { "an otherworldly creature’s body part (ancient or otherworldly creatures table)", (s) =>
        {
            return "An otherworldly creature’s body part: " + GenerateFromRangeTable("ancient_or_otherworldly_creature", 1, 20);
        }
    },
    { "An ally (Person Objective table)", (s) =>
        {
            return "Any Ally: " + GenerateFromRangeTable("knowledge_or_person_objectives").Split("***")[0];
        }
    },
    // Plot location (adjusted)
    { "an otherworldly entity’s lair (ancient or otherworldly creature table)", (s) =>
        {
            return "An otherworldly entity’s lair: " + GenerateFromRangeTable("ancient_or_otherworldly_creature", 1, 20);
        }
    },

    // Plot twists
    { "an opponent (opponent table)", (s) =>
        {
            return "An opponent: " + GenerateFromRangeTable("opponent", 1, 20);
        }
    },
    { "an enemy (primary antagonist table)", (s) =>
        {
            return "An enemy: " + GenerateFromRangeTable("primary_antagonist", 1, 20);
        }
    },

    // Twists and additional narrative elements
    { "snake in the grass (a traitor working for an independent faction — roll on non-player characters table)", (s) =>
        {
            return "Snake in the grass: " + GenerateFromRangeTable("non_player_characters", 1, 20);
        }
    },

    // Add more replacements as needed
    { "roll on religious tenets table", (s) =>
        {
            return "Has information on someone relevant who is missing or dead: " + GenerateFromRangeTable("religious_tenets", 1, 20);
        }
    },
    
    // Plot modification: Manipulated by someone/something
    { "Being manipulated by someone or something (roll twice on Primary Antagonist table, with the second roll as the true antagonist)", (s) =>
        {
            string primaryAntagonistFirst = GenerateFromRangeTable("primary_antagonist", 1, 20);
            string primaryAntagonistSecond = GenerateFromRangeTable("primary_antagonist", 1, 20);
            return $"Being manipulated by: {primaryAntagonistFirst}. The true antagonist: {primaryAntagonistSecond}.";
        }
    },

    // Plot modification: Antagonist is a patsy
    { "The antagonist is a patsy for the true villain (roll again on Primary Antagonist table)", (s) =>
        {
            string firstAntagonist = GenerateFromRangeTable("primary_antagonist", 1, 20);
            string secondAntagonist = GenerateFromRangeTable("primary_antagonist", 1, 20);
            return $"{firstAntagonist} is a patsy for the true villain: {secondAntagonist}.";
        }
    },

    // Plot location
    { "The vault of a hidden cult (cult table)", (s) =>
        {
            return "The vault of a hidden cult: " + GenerateFromRangeTable("cult", 1, 20);
        }
    },
    { "A royal vault (ancient or otherworldly creature table)", (s) =>
        {
            return "A royal vault: " + GenerateFromRangeTable("ancient_or_otherworldly_creature");
        }
    },

    // Objectives
    { "Documents or secrets (Knowledge or Person Objective table)", (s) =>
        {
            return "Documents or secrets: " + GenerateFromRangeTable("knowledge_or_person_objectives");
        }
    },

    // Add any additional replacement logic here
};

            // Handle multiple matches in secondPart based on specific phrases
            string sanitizedSecondPart = secondPart.Trim().ToLower();

            foreach (var key in replacements.Keys)
            {
                if (sanitizedSecondPart.Contains(key.ToLower()))
                {
                    secondPart = replacements[key](secondPart);
                    break;  // Apply the first match and exit the loop
                }
            }

            return $"{firstPart}***{secondPart}";
        }

        public string GenerateNationalities() => GenerateFromRangeTable("nationalities", 2, 40);
        public string GenerateNonPlayerCharacter() => GenerateFromRangeTable("non_player_characters", 1, 20);
        public string GenerateTemperament() => GenerateFromRangeTable("temperament", 1, 20);
        public string GenerateCult() => GenerateFromRangeTable("cult", 1, 20);
        public string GenerateOpponent() => GenerateFromRangeTable("opponent", 1, 20);
        public string GenerateQuirks() => GenerateFromRangeTable("quirks", 2, 40);
        public string GenerateReligiousTenet() => GenerateFromRangeTable("religious_tenets", 1, 20);
        public string GenerateOldCreature() => GenerateFromRangeTable("ancient_or_otherworldly_creature", 1, 20);
        public string GenerateLegendaryCharacter() => GenerateFromRangeTable("legendary_character", 1, 20);
        public string GenerateLegendaryArtifact() => GenerateFromRangeTable("legendary_artifact", 1, 20);
        public string GenerateKnowledgePerson() => GenerateFromRangeTable("knowledge_or_person_objectives");

        private string GenerateFromRangeTable(string tableName, int minRoll = 0, int maxRoll = 0)
        {
            const int maxRetries = 5;
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    if (!_data.ContainsKey(tableName))
                    {
                        throw new KeyNotFoundException($"Table '{tableName}' not found");
                    }

                    if (minRoll == 0 && maxRoll == 0)
                    {
                        var table = _data[tableName].EnumerateArray().ToList();
                        int roll = _random.Next(0, table.Count);
                        var selectedItem = table[roll];
                        var key = selectedItem.EnumerateObject().First().Name;
                        var value = selectedItem.EnumerateObject().First().Value;

                        if (value.ToString().ToLower().Contains("historian"))
                        {
                            return $"{key}***Historian: {GenerateFromRangeTable("cult", 1, 20)}";
                        }

                        if (value.ToString().ToLower().Contains("priest or priestess"))
                        {
                            return $"{key}***Priest or Priestess: {GenerateFromRangeTable("religious_tenets", 1, 20)}";
                        }

                        if (value.ToString().ToLower().Contains("roll again"))
                        {
                            return GenerateFromRangeTable(tableName, minRoll, maxRoll); // Reroll
                        }

                        return $"{key}***{value}";
                    }
                    else
                    {
                        var table = _data[tableName].EnumerateArray().ToList();
                        int roll = _random.Next(minRoll, maxRoll + 1);

                        var selectedItem = table.FirstOrDefault(item =>
                        {
                            var rangeString = item.EnumerateObject().First().Value.GetString();
                            if (string.IsNullOrEmpty(rangeString)) return false;
                            var rangeParts = rangeString.Split('-');

                            if (rangeParts.Length == 1) // Single value case
                            {
                                if (int.TryParse(rangeParts[0], out int singleValue))
                                {
                                    return singleValue == roll;
                                }
                            }
                            else if (rangeParts.Length == 2) // Range case
                            {
                                if (int.TryParse(rangeParts[0], out int rangeStart) && int.TryParse(rangeParts[1], out int rangeEnd))
                                {
                                    return rangeStart <= roll && roll <= rangeEnd;
                                }
                            }

                            return false;
                        });

                        if (selectedItem.ValueKind == JsonValueKind.Undefined)
                        {
                            throw new InvalidOperationException($"No matching item found in table '{tableName}' for roll {roll}");
                        }

                        string result = selectedItem.EnumerateObject().First().Name;

                        if (result.ToString().ToLower().Contains("roll again"))
                        {
                            return GenerateFromRangeTable(tableName, minRoll, maxRoll); // Reroll
                        }

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GenerateFromRangeTable on attempt {attempt + 1}: {ex.Message}");
                    if (attempt == maxRetries - 1)
                    {
                        return $"Failed to generate from {tableName} after {maxRetries} attempts: {ex.Message}";
                    }
                }
            }

            return $"Failed to generate from {tableName} due to an unexpected error";
        }




        public string GenerateArtifact() => GenerateArtifactResult();

        public string GenerateArtifactResult()
        {
            try
            {
                var data = _data["artifact_descriptors_and_conditions"].EnumerateArray().ToList();

                // Randomly select an item from the list
                var selectedItem = data[_random.Next(data.Count)];

                // Get the first (and only) property of the selected item
                var artifactProperty = selectedItem.EnumerateObject().First();
                string artifactType = artifactProperty.Name;

                // Get the inner object
                var innerObject = artifactProperty.Value;

                // Get the descriptor from the inner object
                string descriptor = innerObject.GetProperty("Descriptor").GetString();

                return $"{artifactType}***{descriptor}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating artifact: {ex.Message}");
                return "Error: Unable to generate artifact.";
            }
        }

        public string GenerateLocation()
        {
            try
            {
                var data = _data["location_choice_and_atmosphere"].EnumerateArray().ToList();

                // Randomly select an item from the list
                var selectedItem = data[_random.Next(data.Count)];

                // Get the first (and only) property of the selected item
                var locationProperty = selectedItem.EnumerateObject().First();
                string locationType = locationProperty.Name;

                // Get the inner object
                var innerObject = locationProperty.Value;

                // Get the atmosphere from the inner object
                string atmosphere = innerObject.GetProperty("Atmosphere").GetString();
                Console.WriteLine(atmosphere);

                return $"{locationType}***{atmosphere}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating location: {ex.Message}");
                return "Error: Unable to generate location.";
            }
        }

        public string GenerateOpeningScene()
        {
            var openingSettings = _data["opening_setting"].EnumerateArray().ToList();

            int settingRoll = _random.Next(1, 20);
            int descriptorRoll = _random.Next(1, 20);

            string setting = openingSettings[settingRoll - 1].EnumerateObject().First().Name;
            string descriptor = openingSettings[descriptorRoll - 1].GetProperty("Descriptor").GetString();

            if (descriptor == "Roll twice and apply both")
            {
                int secondRoll = _random.Next(1, 20);
                while (secondRoll == descriptorRoll)
                {
                    secondRoll = _random.Next(1, 20);
                }
                string secondDescriptor = openingSettings[secondRoll - 1].GetProperty("Descriptor").GetString();
                descriptor = $"{descriptor} and {secondDescriptor}";
            }

            return $"{descriptor}***{setting}";
        }

        public string GenerateTitle(bool randomizeOptions)
        {
            const int maxAttempts = 100;
            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                string title = GenerateTitleAttempt(randomizeOptions);
                if (!HasRepeatedTerms(title) && !IsTitlePotentiallyProblematic(title))
                {
                    return title;
                }
            }
            return "Failed to generate a suitable title";
        }

        private string GenerateTitleAttempt(bool randomizeOptions)
        {
            string title = TitleStructure();
            Dictionary<string, string> usedReplacements = new Dictionary<string, string>();

            foreach (var category in new[] { "object", "entity", "location", "disposition", "religious_tenets" })
            {
                _usedItems[category].Clear();
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
                        return string.Empty; // Restart the generation process
                    }

                    usedReplacements[pattern] = replacement;
                    title = title.Replace(title.Contains("𖤍") ? $"𖤍{pattern}" : pattern, replacement);
                    title = title.Replace(title.Contains("𖤍") ? $"𖤍{misspelledPattern}" : misspelledPattern, replacement);
                }

                if (string.IsNullOrEmpty(title)) break;
            }

            if (title.Contains("A**An"))
            {
                title = title.Replace("A**An", "[A/An]");
            }

            if (randomizeOptions)
            {
                title = FindOptions(title);
            }

            return ApplyTheRule(title);
        }

        private string TitleStructure()
        {
            int randomNum = _random.Next(1, 20);
            var matchingStructure = _data["title structure"].EnumerateArray()
                .SelectMany(obj => obj.EnumerateObject())
                .FirstOrDefault(prop =>
                {
                    var range = prop.Value.GetString().Split('-').Select(int.Parse).ToArray();
                    return range[0] <= randomNum && randomNum <= range[1];
                });

            return matchingStructure.Name ?? "The [object] of the [entity]";
        }

        private (string, int) GetRandomItem(string category, int randomRange)
        {
            int randomNum = _random.Next(1, randomRange + 1);
            var item = _data[category].EnumerateArray()
                .SelectMany(obj => obj.EnumerateObject())
                .Where(prop =>
                {
                    var range = prop.Value.GetString().Split('-').Select(int.Parse).ToArray();
                    return range[0] <= randomNum && randomNum <= range[1];
                })
                .FirstOrDefault(prop => !_usedItems[category].Contains(prop.Name));

            if (item.Value.ValueKind == JsonValueKind.Undefined)
            {
                _usedItems[category].Clear();
                return GetRandomItem(category, randomRange);
            }

            string key = item.Name;
            _usedItems[category].Add(key);

            if (key.StartsWith("pick: "))
            {
                string[] options = key[6..].Split(", ");
                return (options[_random.Next(options.Length)], randomNum);
            }

            return (key, randomNum);
        }

        private string FindOptions(string title)
        {
            string ReplaceMatch(Match match)
            {
                string options = match.Value.Trim('[', ']');
                string[] choices = options.Split('/');
                return choices[_random.Next(choices.Length)];
            }

            string pattern = @"\[(?:[^\[\]]+)\]|(?:\b\w+(?:/\w+)+\b)";
            string result = Regex.Replace(title, pattern, ReplaceMatch);

            result = Regex.Replace(result, @"(\w)([A-Z])", "$1 $2");

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

        private string ApplyTheRule(string title)
        {
            int roll = _random.Next(1, 7);

            if (title.StartsWith("The "))
            {
                if (roll <= 2)
                {
                    return title;
                }
                else if (roll == 3 || roll == 4)
                {
                    string restOfTitle = title.Substring(4);
                    return (IsVowel(restOfTitle[0]) ? "An " : "A ") + restOfTitle;
                }
                else
                {
                    string restOfTitle = title.Substring(4);
                    string descriptor = GetRandomDescriptor();
                    return $"{descriptor} {restOfTitle}";
                }
            }

            if (roll >= 5)
            {
                string descriptor = GetRandomDescriptor();
                return $"{descriptor} {title}";
            }

            return title;
        }

        private string GetRandomDescriptor()
        {
            int randomNum = _random.Next(1, 20);
            var descriptorItem = _data["descriptor"].EnumerateArray()
                .SelectMany(obj => obj.EnumerateObject())
                .FirstOrDefault(prop =>
                {
                    var range = prop.Value.GetString().Split('-').Select(int.Parse).ToArray();
                    return range[0] <= randomNum && randomNum <= range[1];
                });

            if (descriptorItem.Name != null)
            {
                string[] options = descriptorItem.Name.Split('/');
                return options[_random.Next(options.Length)];
            }

            return "Couldn't get descriptor";
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