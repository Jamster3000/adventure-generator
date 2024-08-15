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
    Task<string> GeneratePlotFullfillment();
    Task<string> GenerateObstaclesAndTwists();
    Task<string> GenerateGoalsAndObjectives();
}

public class TitleGeneratorService : ITitleGeneratorService
{
    private readonly HttpClient _httpClient;
    private Dictionary<string, JsonElement> data = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> openSceneData = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> non_player_character = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> quirks = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> temperament = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> nationality = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> opponent = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> cult = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> hooksAndDraw = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> plots = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> antagonist = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> plotFullfillment = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> obstacles = new Dictionary<string, JsonElement>();
    private Dictionary<string, JsonElement> goals = new Dictionary<string, JsonElement>();
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

    public async Task<string> GenerateHooksAndDraws()
    {
        if (hooksAndDraw == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateHooksAndDraws();
    }

    public async Task<string> GeneratePlots()
    {
        if (plots == null)
        {
            await InitializeAsync();
        }
        return tables.GeneratePlots();
    }

    public async Task<string> GenerateAntagonist()
    {
        if (antagonist == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateAntagonist();
    }

    public async Task<string> GeneratePlotFullfillment()
    {
        if (plotFullfillment == null)
        {
            await InitializeAsync();
        }
        return tables.GeneratePlotFullfillment();
    }

    public async Task<string> GenerateObstaclesAndTwists()
    {
        if (obstacles == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateObstaclesAndTwists();
    }

    public async Task<string> GenerateGoalsAndObjectives()
    {
        if (goals == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateGoalsAndObjectives();
    }

    public async Task<string> GenerateOpeningScene()
    {
        if (openSceneData == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateOpeningScene();
    }

    public async Task<string> GenerateCult()
    {
        if (cult == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateCult();
    }

    public async Task<string> GenerateNationalities()
    {
        if (nationality == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateNationalities();
    }

    public async Task<string> GenerateOpponent()
    {
        if (opponent == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateOpponent();
    }

    public async Task<string> GenerateTemperament()
    {
        if (temperament == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateTemperament();
    }

    public async Task<string> GenerateQuirks()
    {
        if (quirks == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateQuirks();
    }

    public async Task<string> GenerateNonPlayerCharacter()
    {
        if (non_player_character == null)
        {
            await InitializeAsync();
        }
        return tables.GenerateNonPlayerCharacter();
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

        public string GenerateGoalsAndObjectives()
        {
            var twist = data["goals_and_objectives"].EnumerateArray().ToList();

            // First roll for the hook
            int twistRoll = random.Next(1, 21);
            var selectedPlot = twist.FirstOrDefault(item =>
            {
                var value = item.EnumerateObject().First().Value.GetString();
                return int.Parse(value) == twistRoll;
            });

            if (selectedPlot.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate hook";
            }

            string hook = selectedPlot.EnumerateObject().First().Name;

            // Second roll for the draw
            int twistRoll2 = random.Next(1, 21);
            var selectedTwist = twist[twistRoll2 - 1].GetProperty("Objective").GetString();

            return $"{hook}***{selectedTwist}";
        }

        public string GenerateObstaclesAndTwists()
        {
            var twist = data["obstacles_and_twists"].EnumerateArray().ToList();

            // First roll for the hook
            int twistRoll = random.Next(1, 21);
            var selectedPlot = twist.FirstOrDefault(item =>
            {
                var value = item.EnumerateObject().First().Value.GetString();
                return int.Parse(value) == twistRoll;
            });

            if (selectedPlot.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate hook";
            }

            string hook = selectedPlot.EnumerateObject().First().Name;

            // Second roll for the draw
            int twistRoll2 = random.Next(1, 21);
            var selectedTwist = twist[twistRoll2 - 1].GetProperty("Plot Twist").GetString();

            return $"{hook}***{selectedTwist}";
        }

        public string GeneratePlotFullfillment()
        {
            var antagonist = data["plot_fulfillment_and_location"].EnumerateArray().ToList();

            // First roll for the hook
            int plotRoll = random.Next(1, 21);
            var selectedPlot = antagonist.FirstOrDefault(item =>
            {
                var value = item.EnumerateObject().First().Value.GetString();
                return int.Parse(value) == plotRoll;
            });

            if (selectedPlot.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate hook";
            }

            string hook = selectedPlot.EnumerateObject().First().Name;

            // Second roll for the draw
            int plotRoll2 = random.Next(1, 21);
            var seletedPlot2 = antagonist[plotRoll2 - 1].GetProperty("From/Where").GetString();

            return $"{hook}***{seletedPlot2}";
        }

        public string GenerateAntagonist()
        {
            var antagonist = data["primary_antagonist"].EnumerateArray().ToList();

            // First roll for the hook
            int antagonistRoll = random.Next(1, 21);
            var selectedAntagonist = antagonist.FirstOrDefault(item =>
            {
                var value = item.EnumerateObject().First().Value.GetString();
                return int.Parse(value) == antagonistRoll;
            });

            if (selectedAntagonist.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate hook";
            }

            string hook = selectedAntagonist.EnumerateObject().First().Name;

            // Second roll for the draw
            int antagonistRoll2 = random.Next(1, 21);
            var selectedAntagonist2 = antagonist[antagonistRoll2 - 1].GetProperty("Trait").GetString();

            return $"{hook}***{selectedAntagonist2}";
        }

        public string GeneratePlots()
        {
            var plotConcept = data["plot_concept"].EnumerateArray().ToList();

            // First roll for the hook
            int plotRoll = random.Next(1, 21);
            var selectedPlot = plotConcept.FirstOrDefault(item =>
            {
                var value = item.EnumerateObject().First().Value.GetString();
                return int.Parse(value) == plotRoll;
            });

            if (selectedPlot.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate hook";
            }

            string hook = selectedPlot.EnumerateObject().First().Name;

            // Second roll for the draw
            int motivationRoll = random.Next(1, 21);
            var selecctedMotivation = plotConcept[motivationRoll - 1].GetProperty("Motivation").GetString();

            return $"{hook}***{selecctedMotivation}";
        }

        public string GenerateHooksAndDraws()
        {
            var hooksAndDraws = data["hooks_and_draws"].EnumerateArray().ToList();

            // First roll for the hook
            int hookRoll = random.Next(1, 21);
            var selectedHook = hooksAndDraws.FirstOrDefault(item =>
            {
                var value = item.EnumerateObject().First().Value.GetString();
                return int.Parse(value) == hookRoll;
            });

            if (selectedHook.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate hook";
            }

            string hook = selectedHook.EnumerateObject().First().Name;

            // Second roll for the draw
            int drawRoll = random.Next(1, 21);
            var selectedDraw = hooksAndDraws[drawRoll - 1].GetProperty("Draw").GetString();

            Console.WriteLine(selectedDraw);
            Console.WriteLine(hook);

            return $"{hook}***{selectedDraw}";
        }

        public string GenerateNationalities()
        {
            var nationalityList = data["nationalities"].EnumerateArray().ToList();
            int roll = random.Next(2, 41);  // Roll between 2 and 40 inclusive

            var selectedNationality = nationalityList.FirstOrDefault(nationality =>
            {
                var value = nationality.EnumerateObject().First().Value.GetString();
                if (value.Contains("-"))
                {
                    var range = value.Split('-').Select(int.Parse).ToArray();
                    return range[0] <= roll && roll <= range[1];
                }
                else
                {
                    return int.Parse(value) == roll;
                }
            });

            if (selectedNationality.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate nationality";
            }

            string nationalityType = selectedNationality.EnumerateObject().First().Name;
            return nationalityType;
        }

        public string GenerateNonPlayerCharacter()
        {
            var npcTypes = data["non_player_characters"].EnumerateArray().ToList();

            int roll = random.Next(1, 21);
            var selectedNpc = npcTypes.FirstOrDefault(npc =>
            {
                var range = npc.EnumerateObject().First().Value.GetString().Split('-').Select(int.Parse).ToArray();
                return range[0] <= roll && roll <= range[1];
            });

            if (selectedNpc.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate NPC";
            }

            string npcType = selectedNpc.EnumerateObject().First().Name;

            // Special handling for Clergy and double roll
            if (npcType == "Clergy")
            {
                string religiousTenet = GenerateReligiousTenet(); // You'll need to implement this
                return $"Clergy ({religiousTenet})";
            }
            else if (npcType == "Roll twice, with a separate roll on Temperament table for each")
            {
                string npc1 = GenerateNonPlayerCharacter();
                string npc2 = GenerateNonPlayerCharacter();
                string temperament1 = GenerateTemperament(); // You'll need to implement this
                string temperament2 = GenerateTemperament();
                return $"{npc1} ({temperament1}) and {npc2} ({temperament2})";
            }

            return npcType;
        }

        public string GenerateTemperament()
        {
            var tempermantType = data["temperament"].EnumerateArray().ToList();

            int roll = random.Next(1, 21);
            var selectedTempermant = tempermantType.FirstOrDefault(npc =>
            {
                var range = npc.EnumerateObject().First().Value.GetString().Split('-').Select(int.Parse).ToArray();
                return range[0] <= roll && roll <= range[1];
            });

            if (selectedTempermant.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate temperament";
            }

            string tempermantTypes = selectedTempermant.EnumerateObject().First().Name;

            return tempermantTypes;
        }

        public string GenerateCult()
        {
            var cultType = data["cult"].EnumerateArray().ToList();

            int roll = random.Next(1, 21);
            var selectedCult = cultType.FirstOrDefault(npc =>
            {
                var range = npc.EnumerateObject().First().Value.GetString().Split('-').Select(int.Parse).ToArray();
                return range[0] <= roll && roll <= range[1];
            });

            if (selectedCult.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate cult";
            }

            string cultTypes = selectedCult.EnumerateObject().First().Name;

            return cultTypes;
        }

        public string GenerateCreature()
        {
            var creatureType = data["ancient_or_otherworldly_creature"].EnumerateArray().ToList();

            int roll = random.Next(1, 21);
            var selectedCreature = creatureType.FirstOrDefault(npc =>
            {
                var range = npc.EnumerateObject().First().Value.GetString().Split('-').Select(int.Parse).ToArray();
                return range[0] <= roll && roll <= range[1];
            });

            if (selectedCreature.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate creature";
            }

            string creatureTypes = selectedCreature.EnumerateObject().First().Name;

            return creatureTypes;
        }

        public string GenerateOpponent()
        {
            var opponentList = data["opponent"].EnumerateArray().ToList();
            int roll = random.Next(1, 21);  // Roll between 1 and 20 inclusive

            var selectedOpponent = opponentList.FirstOrDefault(opponent =>
            {
                var value = opponent.EnumerateObject().First().Value.GetString();
                return value == roll.ToString() || value.StartsWith($"{roll} ");
            });

            if (selectedOpponent.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate opponent";
            }

            var opponentEntry = selectedOpponent.EnumerateObject().First();
            string opponentType = opponentEntry.Name;
            string opponentValue = opponentEntry.Value.GetString();

            // Special handling for Otherworldly creature
            if (opponentValue == "20 (Roll on Ancient or Otherworldly Creature table)")
            {
                string creature = GenerateCreature();
                return $"Otherworldly Creature: {creature}";
            }

            return opponentType;
        }

        public string GenerateQuirks()
        {
            var quirksList = data["quirks"].EnumerateArray().ToList();
            int roll = random.Next(2, 41);  // Roll between 2 and 40 inclusive

            var selectedQuirk = quirksList.FirstOrDefault(quirk =>
            {
                var value = int.Parse(quirk.EnumerateObject().First().Value.GetString());
                return value == roll;
            });

            if (selectedQuirk.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate quirk";
            }

            string quirkDescription = selectedQuirk.EnumerateObject().First().Name;
            return quirkDescription;
        }

        public string GenerateReligiousTenet()
        {
            var religionType = data["religious_tenets"].EnumerateArray().ToList();

            int roll = random.Next(1, 21);
            var selectedReligion = religionType.FirstOrDefault(npc =>
            {
                var range = npc.EnumerateObject().First().Value.GetString().Split('-').Select(int.Parse).ToArray();
                return range[0] <= roll && roll <= range[1];
            });

            if (selectedReligion.ValueKind == JsonValueKind.Undefined)
            {
                return "Failed to generate NPC";
            }

            string religionTypes = selectedReligion.EnumerateObject().First().Name;

            return religionTypes;
        }

        public string GenerateOpeningScene()
        {
            var openingSettings = data["opening_setting"].EnumerateArray().ToList();

            //rolling for the setting from the openingin_setting table
            int settingRoll = random.Next(1, 21);
            var settingEntry = openingSettings[settingRoll - 1];
            string setting = settingEntry.EnumerateObject().First().Name;

            //rolling for the descriptor from the opening_setting table
            int descriptorRoll = random.Next(1, 21);
            var descriptorEntry = openingSettings[descriptorRoll - 1];
            string descriptor = descriptorEntry.GetProperty("Descriptor").GetString();

            if (descriptor == "Roll twice and apply both")
            {
                int secondRoll = random.Next(1, 21);
                while (secondRoll == descriptorRoll)
                {
                    secondRoll = random.Next(1, 21);
                }
                var secondDescriptorEntry = openingSettings[secondRoll - 1];
                string secondDescriptor = secondDescriptorEntry.GetProperty("Descriptor").GetString();
                descriptor = $"{descriptor} and {secondDescriptor}";
            }
            return $"{descriptor}***{setting}";
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