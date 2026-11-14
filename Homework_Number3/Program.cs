try
{
    Dictionary<int, string> names = new Dictionary<int, string>();
    Dictionary<int, string> categories = new Dictionary<int, string>();
    Dictionary<int, string> states = new Dictionary<int, string>();
    Dictionary<int, string> owners = new Dictionary<int, string>();
    List<int> ids = new List<int>();
    List<string> users = new List<string>();

    List<string> categoryCatalog = new List<string>() { "Hand tools", "Power tools", "Measuring" };
    List<string> stateCatalog = new List<string>() { "Available", "In Use", "Under maintenance", "Damaged" };

    var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
    var comparison = StringComparison.OrdinalIgnoreCase;

    bool appRunning = true;

    Console.WriteLine("<=== Workshop Tool Control ===>");

    while (appRunning)
    {
        try
        {
            Console.WriteLine("\n<=== Start Menu ===>");
            Console.WriteLine("1. Log in");
            Console.WriteLine("2. View workers");
            Console.WriteLine("3. Exit program");
            Console.Write("Select an option: ");

            int startChoice = Convert.ToInt32(Console.ReadLine());

            switch (startChoice)
            {
                case 1:
                    {
                        Console.Write("Worker name: ");
                        string workerName = (Console.ReadLine() ?? string.Empty).Trim();

                        switch (workerName.Length > 0 && workerName.All(c => char.IsLetter(c) || c == ' '))
                        {
                            case false:
                                throw new ArgumentException("The worker name can only contain letters and cannot be empty.");
                        }

                        string userKey = workerName.ToLower();
                        string displayName = textInfo.ToTitleCase(userKey);

                        switch (users.Contains(userKey))
                        {
                            case true:
                                int savedTools = ids.Count(t => owners[t] == userKey);
                                Console.WriteLine($"\nWelcome back, {displayName}! You have {savedTools} tool(s) saved.");
                                break;
                            case false:
                                users.Add(userKey);
                                Console.WriteLine($"\nWelcome, {displayName}! Your personal inventory has been created.");
                                break;
                        }

                        bool sessionActive = true;

                        while (sessionActive)
                        {
                            try
                            {
                                Console.WriteLine($"\n<=== Inventory of {displayName} ===>");
                                Console.WriteLine("1. Manage tools");
                                Console.WriteLine("2. View and search tools");
                                Console.WriteLine("3. Categories");
                                Console.WriteLine("4. Log out");
                                Console.WriteLine("5. Exit program");
                                Console.Write("Select an option: ");

                                int choice = Convert.ToInt32(Console.ReadLine());

                                switch (choice)
                                {
                                    case 1:
                                        {
                                            bool toolsActive = true;

                                            while (toolsActive)
                                            {
                                                try
                                                {
                                                    Console.WriteLine("\n<=== Manage Tools ===>");
                                                    Console.WriteLine("1. Add tool");
                                                    Console.WriteLine("2. Edit tool");
                                                    Console.WriteLine("3. Change state");
                                                    Console.WriteLine("4. Delete tool");
                                                    Console.WriteLine("5. Back to inventory menu");
                                                    Console.Write("Select an option: ");

                                                    int toolChoice = Convert.ToInt32(Console.ReadLine());

                                                    switch (toolChoice)
                                                    {
                                                        case 1:
                                                            {
                                                                Console.WriteLine("Let's register a new tool.");

                                                                Console.Write("Tool name: ");
                                                                string name = (Console.ReadLine() ?? string.Empty).Trim();

                                                                switch (name.Length > 0 && name.All(c => char.IsLetter(c) || c == ' '))
                                                                {
                                                                    case false:
                                                                        throw new ArgumentException("The tool name can only contain letters and cannot be empty.");
                                                                }

                                                                bool duplicated = ids.Any(t => owners[t] == userKey && names[t].Equals(name, comparison));

                                                                switch (duplicated)
                                                                {
                                                                    case true:
                                                                        throw new ArgumentException("You already have a tool with that name.");
                                                                }

                                                                Console.WriteLine("Categories:");
                                                                for (int i = 0; i < categoryCatalog.Count; i++)
                                                                {
                                                                    Console.WriteLine($"  {i + 1}. {categoryCatalog[i]}");
                                                                }
                                                                Console.Write("Choose a number: ");
                                                                int categoryNumber = Convert.ToInt32(Console.ReadLine());
                                                                string category = categoryCatalog[categoryNumber - 1];

                                                                int id = ids.DefaultIfEmpty(0).Max() + 1;
                                                                ids.Add(id);
                                                                names.Add(id, name);
                                                                categories.Add(id, category);
                                                                states.Add(id, stateCatalog[0]);
                                                                owners.Add(id, userKey);

                                                                Console.WriteLine("Tool registered with ID " + id);
                                                                break;
                                                            }

                                                        case 2:
                                                            {
                                                                List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                                                Console.WriteLine($"{"Id",-5}{"Name",-25}{"Category",-15}State");
                                                                Console.WriteLine("--------------------------------------------------------------");
                                                                foreach (var toolId in myTools)
                                                                {
                                                                    Console.WriteLine($"{toolId,-5}{names[toolId],-25}{categories[toolId],-15}{states[toolId]}");
                                                                }

                                                                Console.Write("Enter the ID of the tool to edit: ");
                                                                int id = Convert.ToInt32(Console.ReadLine());

                                                                string owner = owners[id];

                                                                switch (owner == userKey)
                                                                {
                                                                    case false:
                                                                        throw new KeyNotFoundException("That tool does not belong to you.");
                                                                }

                                                                string currentName = names[id];
                                                                string currentCategory = categories[id];

                                                                Console.Write($"The name is: {currentName}. Enter the new name: ");
                                                                string newName = (Console.ReadLine() ?? string.Empty).Trim();

                                                                switch (newName.Length > 0 && newName.All(c => char.IsLetter(c) || c == ' '))
                                                                {
                                                                    case false:
                                                                        throw new ArgumentException("The tool name can only contain letters and cannot be empty.");
                                                                }

                                                                bool duplicated = ids.Any(t => t != id && owners[t] == userKey && names[t].Equals(newName, comparison));

                                                                switch (duplicated)
                                                                {
                                                                    case true:
                                                                        throw new ArgumentException("You already have another tool with that name.");
                                                                }

                                                                Console.WriteLine($"The category is: {currentCategory}. Choose the new one:");
                                                                for (int i = 0; i < categoryCatalog.Count; i++)
                                                                {
                                                                    Console.WriteLine($"  {i + 1}. {categoryCatalog[i]}");
                                                                }
                                                                Console.Write("Choose a number: ");
                                                                int categoryNumber = Convert.ToInt32(Console.ReadLine());

                                                                names[id] = newName;
                                                                categories[id] = categoryCatalog[categoryNumber - 1];

                                                                Console.WriteLine("Tool updated successfully.");
                                                                break;
                                                            }

                                                        case 3:
                                                            {
                                                                List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                                                Console.WriteLine($"{"Id",-5}{"Name",-25}{"Category",-15}State");
                                                                Console.WriteLine("--------------------------------------------------------------");
                                                                foreach (var toolId in myTools)
                                                                {
                                                                    Console.WriteLine($"{toolId,-5}{names[toolId],-25}{categories[toolId],-15}{states[toolId]}");
                                                                }

                                                                Console.Write("Enter the ID of the tool: ");
                                                                int id = Convert.ToInt32(Console.ReadLine());

                                                                string owner = owners[id];

                                                                switch (owner == userKey)
                                                                {
                                                                    case false:
                                                                        throw new KeyNotFoundException("That tool does not belong to you.");
                                                                }

                                                                string currentState = states[id];

                                                                Console.WriteLine($"The current state is: {currentState}. Choose the new one:");
                                                                for (int i = 0; i < stateCatalog.Count; i++)
                                                                {
                                                                    Console.WriteLine($"  {i + 1}. {stateCatalog[i]}");
                                                                }
                                                                Console.Write("Choose a number: ");
                                                                int stateNumber = Convert.ToInt32(Console.ReadLine());

                                                                states[id] = stateCatalog[stateNumber - 1];

                                                                Console.WriteLine("State updated successfully.");
                                                                break;
                                                            }

                                                        case 4:
                                                            {
                                                                List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                                                Console.WriteLine($"{"Id",-5}{"Name",-25}{"Category",-15}State");
                                                                Console.WriteLine("--------------------------------------------------------------");
                                                                foreach (var toolId in myTools)
                                                                {
                                                                    Console.WriteLine($"{toolId,-5}{names[toolId],-25}{categories[toolId],-15}{states[toolId]}");
                                                                }

                                                                Console.Write("Enter the ID of the tool to delete: ");
                                                                int id = Convert.ToInt32(Console.ReadLine());

                                                                string owner = owners[id];

                                                                switch (owner == userKey)
                                                                {
                                                                    case false:
                                                                        throw new KeyNotFoundException("That tool does not belong to you.");
                                                                }

                                                                string name = names[id];

                                                                Console.WriteLine($"Are you sure you want to delete '{name}'? 1. Yes, 2. No");
                                                                int confirmation = Convert.ToInt32(Console.ReadLine());

                                                                switch (confirmation)
                                                                {
                                                                    case 1:
                                                                        names.Remove(id);
                                                                        categories.Remove(id);
                                                                        states.Remove(id);
                                                                        owners.Remove(id);
                                                                        ids.Remove(id);
                                                                        Console.WriteLine("Tool deleted.");
                                                                        break;
                                                                    default:
                                                                        Console.WriteLine("Deletion cancelled.");
                                                                        break;
                                                                }
                                                                break;
                                                            }

                                                        case 5:
                                                            toolsActive = false;
                                                            break;

                                                        default:
                                                            Console.WriteLine("Invalid option. Please try again.");
                                                            break;
                                                    }
                                                }
                                                catch (FormatException)
                                                {
                                                    Console.WriteLine("Error: you must enter a valid number.");
                                                }
                                                catch (OverflowException)
                                                {
                                                    Console.WriteLine("Error: the number you entered is too large.");
                                                }
                                                catch (KeyNotFoundException)
                                                {
                                                    Console.WriteLine("Error: the ID you entered does not exist in your inventory. Please try again.");
                                                }
                                                catch (ArgumentOutOfRangeException)
                                                {
                                                    Console.WriteLine("Error: the option you selected is out of range. Please try again.");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Error: " + ex.Message);
                                                }
                                            }
                                            break;
                                        }

                                    case 2:
                                        {
                                            bool viewActive = true;

                                            while (viewActive)
                                            {
                                                try
                                                {
                                                    Console.WriteLine("\n<=== View and Search Tools ===>");
                                                    Console.WriteLine("1. View my tools");
                                                    Console.WriteLine("2. Search my tools");
                                                    Console.WriteLine("3. Filter my tools by state");
                                                    Console.WriteLine("4. Back to inventory menu");
                                                    Console.Write("Select an option: ");

                                                    int viewChoice = Convert.ToInt32(Console.ReadLine());

                                                    switch (viewChoice)
                                                    {
                                                        case 1:
                                                            {
                                                                List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                                                Console.WriteLine($"\nTools of {displayName}");
                                                                Console.WriteLine($"{"Id",-5}{"Name",-25}{"Category",-15}State");
                                                                Console.WriteLine("--------------------------------------------------------------");
                                                                foreach (var toolId in myTools)
                                                                {
                                                                    Console.WriteLine($"{toolId,-5}{names[toolId],-25}{categories[toolId],-15}{states[toolId]}");
                                                                }
                                                                Console.WriteLine("Total: " + myTools.Count);
                                                                break;
                                                            }

                                                        case 2:
                                                            {
                                                                List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                                                Console.WriteLine("Search by: 1. Name  2. Category  3. State  4. ID");
                                                                Console.Write("Criteria: ");
                                                                int criteria = Convert.ToInt32(Console.ReadLine());

                                                                Console.Write("Value to search: ");
                                                                string value = (Console.ReadLine() ?? string.Empty).Trim();

                                                                switch (value.Length > 0)
                                                                {
                                                                    case false:
                                                                        throw new ArgumentException("The search value cannot be empty.");
                                                                }

                                                                bool lettersOnly = value.All(c => char.IsLetter(c) || c == ' ');
                                                                bool digitsOnly = value.All(char.IsDigit);

                                                                bool validValue = criteria switch
                                                                {
                                                                    1 => lettersOnly,
                                                                    2 => lettersOnly,
                                                                    3 => lettersOnly,
                                                                    4 => digitsOnly,
                                                                    _ => throw new ArgumentException("Invalid search criteria.")
                                                                };

                                                                switch (validValue)
                                                                {
                                                                    case false:
                                                                        throw new ArgumentException("Use only letters to search by name, category or state, and only digits to search by ID.");
                                                                }

                                                                List<int> found = criteria switch
                                                                {
                                                                    1 => myTools.Where(t => names[t].Contains(value, comparison)).ToList(),
                                                                    2 => myTools.Where(t => categories[t].Contains(value, comparison)).ToList(),
                                                                    3 => myTools.Where(t => states[t].Contains(value, comparison)).ToList(),
                                                                    _ => myTools.Where(t => t == Convert.ToInt32(value)).ToList()
                                                                };

                                                                Console.WriteLine($"{"Id",-5}{"Name",-25}{"Category",-15}State");
                                                                Console.WriteLine("--------------------------------------------------------------");
                                                                foreach (var toolId in found)
                                                                {
                                                                    Console.WriteLine($"{toolId,-5}{names[toolId],-25}{categories[toolId],-15}{states[toolId]}");
                                                                }
                                                                Console.WriteLine("Results found: " + found.Count);
                                                                break;
                                                            }

                                                        case 3:
                                                            {
                                                                Console.WriteLine("States:");
                                                                for (int i = 0; i < stateCatalog.Count; i++)
                                                                {
                                                                    Console.WriteLine($"  {i + 1}. {stateCatalog[i]}");
                                                                }
                                                                Console.Write("Choose a number: ");
                                                                int stateNumber = Convert.ToInt32(Console.ReadLine());
                                                                string selectedState = stateCatalog[stateNumber - 1];

                                                                List<int> filtered = ids.Where(t => owners[t] == userKey && states[t] == selectedState).ToList();

                                                                Console.WriteLine($"\nTools of {displayName} with state '{selectedState}'");
                                                                Console.WriteLine($"{"Id",-5}{"Name",-25}{"Category",-15}State");
                                                                Console.WriteLine("--------------------------------------------------------------");
                                                                foreach (var toolId in filtered)
                                                                {
                                                                    Console.WriteLine($"{toolId,-5}{names[toolId],-25}{categories[toolId],-15}{states[toolId]}");
                                                                }
                                                                Console.WriteLine("Total: " + filtered.Count);
                                                                break;
                                                            }

                                                        case 4:
                                                            viewActive = false;
                                                            break;

                                                        default:
                                                            Console.WriteLine("Invalid option. Please try again.");
                                                            break;
                                                    }
                                                }
                                                catch (FormatException)
                                                {
                                                    Console.WriteLine("Error: you must enter a valid number.");
                                                }
                                                catch (OverflowException)
                                                {
                                                    Console.WriteLine("Error: the number you entered is too large.");
                                                }
                                                catch (ArgumentOutOfRangeException)
                                                {
                                                    Console.WriteLine("Error: the option you selected is out of range. Please try again.");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Error: " + ex.Message);
                                                }
                                            }
                                            break;
                                        }

                                    case 3:
                                        {
                                            bool categoriesActive = true;

                                            while (categoriesActive)
                                            {
                                                try
                                                {
                                                    Console.WriteLine("\n<=== Categories ===>");
                                                    Console.WriteLine("1. Add category");
                                                    Console.WriteLine("2. View categories");
                                                    Console.WriteLine("3. Back to inventory menu");
                                                    Console.Write("Select an option: ");

                                                    int categoryChoice = Convert.ToInt32(Console.ReadLine());

                                                    switch (categoryChoice)
                                                    {
                                                        case 1:
                                                            {
                                                                Console.Write("New category name: ");
                                                                string newCategory = (Console.ReadLine() ?? string.Empty).Trim();

                                                                switch (newCategory.Length > 0 && newCategory.All(c => char.IsLetter(c) || c == ' '))
                                                                {
                                                                    case false:
                                                                        throw new ArgumentException("The category name can only contain letters and cannot be empty.");
                                                                }

                                                                bool exists = categoryCatalog.Any(existing => existing.Equals(newCategory, comparison));

                                                                switch (exists)
                                                                {
                                                                    case true:
                                                                        throw new ArgumentException("That category already exists.");
                                                                }

                                                                categoryCatalog.Add(newCategory);
                                                                Console.WriteLine("Category added.");
                                                                break;
                                                            }

                                                        case 2:
                                                            {
                                                                List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                                                Console.WriteLine("\nCategories:");
                                                                Console.WriteLine($"{"No.",-5}{"Category",-20}My tools");
                                                                Console.WriteLine("------------------------------------");
                                                                for (int i = 0; i < categoryCatalog.Count; i++)
                                                                {
                                                                    string categoryName = categoryCatalog[i];
                                                                    int amount = myTools.Count(t => categories[t] == categoryName);
                                                                    Console.WriteLine($"{i + 1,-5}{categoryName,-20}{amount}");
                                                                }
                                                                Console.WriteLine("Total categories: " + categoryCatalog.Count);
                                                                break;
                                                            }

                                                        case 3:
                                                            categoriesActive = false;
                                                            break;

                                                        default:
                                                            Console.WriteLine("Invalid option. Please try again.");
                                                            break;
                                                    }
                                                }
                                                catch (FormatException)
                                                {
                                                    Console.WriteLine("Error: you must enter a valid number.");
                                                }
                                                catch (OverflowException)
                                                {
                                                    Console.WriteLine("Error: the number you entered is too large.");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Error: " + ex.Message);
                                                }
                                            }
                                            break;
                                        }

                                    case 4:
                                        {
                                            int savedAmount = ids.Count(t => owners[t] == userKey);
                                            Console.WriteLine($"\nSession closed. {savedAmount} tool(s) were saved under the name {displayName}.");
                                            sessionActive = false;
                                            break;
                                        }

                                    case 5:
                                        {
                                            int savedAmount = ids.Count(t => owners[t] == userKey);
                                            Console.WriteLine($"\n{savedAmount} tool(s) were saved under the name {displayName}.");
                                            sessionActive = false;
                                            appRunning = false;
                                            break;
                                        }

                                    default:
                                        Console.WriteLine("Invalid option. Please try again.");
                                        break;
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Error: you must enter a valid number.");
                            }
                            catch (OverflowException)
                            {
                                Console.WriteLine("Error: the number you entered is too large.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                            }
                        }
                        break;
                    }

                case 2:
                    {
                        Console.WriteLine("\nRegistered workers:");
                        Console.WriteLine($"{"No.",-5}{"Worker",-25}Tools");
                        Console.WriteLine("----------------------------------------");
                        for (int i = 0; i < users.Count; i++)
                        {
                            string worker = users[i];
                            int workerTools = ids.Count(t => owners[t] == worker);
                            Console.WriteLine($"{i + 1,-5}{textInfo.ToTitleCase(worker),-25}{workerTools}");
                        }
                        Console.WriteLine("Total workers: " + users.Count);
                        break;
                    }

                case 3:
                    appRunning = false;
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Error: you must enter a valid number.");
        }
        catch (OverflowException)
        {
            Console.WriteLine("Error: the number you entered is too large.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    Console.WriteLine("Goodbye!");
}
catch (Exception ex)
{
    Console.WriteLine("An unexpected error occurred: " + ex.Message);
}