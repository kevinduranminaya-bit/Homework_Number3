try
{
    Dictionary<int, string> names = new Dictionary<int, string>();
    Dictionary<int, string> categories = new Dictionary<int, string>();
    Dictionary<int, string> states = new Dictionary<int, string>();
    Dictionary<int, string> owners = new Dictionary<int, string>();
    List<int> ids = new List<int>();
    List<string> users = new List<string>();
    List<string> logOwners = new List<string>();
    List<string> logEntries = new List<string>();

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
            Console.WriteLine("3. Delete worker");
            Console.WriteLine("4. Exit program");
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

                        logOwners.Add(userKey);
                        logEntries.Add($"{DateTime.Now:HH:mm:ss} Logged in");

                        bool sessionActive = true;

                        while (sessionActive)
                        {
                            try
                            {
                                Console.WriteLine($"\n<=== Inventory of {displayName} ===>");
                                Console.WriteLine("1. Add tool");
                                Console.WriteLine("2. View my tools");
                                Console.WriteLine("3. Search my tools");
                                Console.WriteLine("4. Edit tool");
                                Console.WriteLine("5. Change state");
                                Console.WriteLine("6. Delete tool");
                                Console.WriteLine("7. Add category");
                                Console.WriteLine("8. View categories");
                                Console.WriteLine("9. Inventory summary");
                                Console.WriteLine("10. Filter my tools by state");
                                Console.WriteLine("11. Filter my tools by category");
                                Console.WriteLine("12. Transfer tool to another worker");
                                Console.WriteLine("13. Mark all my tools as available");
                                Console.WriteLine("14. View my activity log");
                                Console.WriteLine("15. Log out");
                                Console.WriteLine("16. Exit program");
                                Console.Write("Select an option: ");

                                int choice = Convert.ToInt32(Console.ReadLine());

                                switch (choice)
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

                                            logOwners.Add(userKey);
                                            logEntries.Add($"{DateTime.Now:HH:mm:ss} Added tool '{name}' with ID {id}");

                                            Console.WriteLine("Tool registered with ID " + id);
                                            break;
                                        }

                                    case 2:
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

                                    case 3:
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

                                    case 4:
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
                                            string newCategory = categoryCatalog[categoryNumber - 1];

                                            names[id] = newName;
                                            categories[id] = newCategory;

                                            logOwners.Add(userKey);
                                            logEntries.Add($"{DateTime.Now:HH:mm:ss} Edited tool ID {id}: '{currentName}' is now '{newName}' ({newCategory})");

                                            Console.WriteLine("Tool updated successfully.");
                                            break;
                                        }

                                    case 5:
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
                                            string newState = stateCatalog[stateNumber - 1];

                                            states[id] = newState;

                                            logOwners.Add(userKey);
                                            logEntries.Add($"{DateTime.Now:HH:mm:ss} Changed state of '{names[id]}' from '{currentState}' to '{newState}'");

                                            Console.WriteLine("State updated successfully.");
                                            break;
                                        }

                                    case 6:
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
                                                    logOwners.Add(userKey);
                                                    logEntries.Add($"{DateTime.Now:HH:mm:ss} Deleted tool '{name}' with ID {id}");
                                                    Console.WriteLine("Tool deleted.");
                                                    break;
                                                default:
                                                    Console.WriteLine("Deletion cancelled.");
                                                    break;
                                            }
                                            break;
                                        }

                                    case 7:
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

                                            logOwners.Add(userKey);
                                            logEntries.Add($"{DateTime.Now:HH:mm:ss} Added category '{newCategory}'");

                                            Console.WriteLine("Category added.");
                                            break;
                                        }

                                    case 8:
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

                                    case 9:
                                        {
                                            List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                            Console.WriteLine($"\n<=== Inventory summary of {displayName} ===>");
                                            Console.WriteLine("Total tools: " + myTools.Count);

                                            Console.WriteLine("\nBy state:");
                                            foreach (var stateName in stateCatalog)
                                            {
                                                int amount = myTools.Count(t => states[t] == stateName);
                                                Console.WriteLine($"  {stateName,-20}{amount}");
                                            }

                                            Console.WriteLine("\nBy category:");
                                            foreach (var categoryName in categoryCatalog)
                                            {
                                                int amount = myTools.Count(t => categories[t] == categoryName);
                                                Console.WriteLine($"  {categoryName,-20}{amount}");
                                            }
                                            break;
                                        }

                                    case 10:
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

                                    case 11:
                                        {
                                            Console.WriteLine("Categories:");
                                            for (int i = 0; i < categoryCatalog.Count; i++)
                                            {
                                                Console.WriteLine($"  {i + 1}. {categoryCatalog[i]}");
                                            }
                                            Console.Write("Choose a number: ");
                                            int categoryNumber = Convert.ToInt32(Console.ReadLine());
                                            string selectedCategory = categoryCatalog[categoryNumber - 1];

                                            List<int> filtered = ids.Where(t => owners[t] == userKey && categories[t] == selectedCategory).ToList();

                                            Console.WriteLine($"\nTools of {displayName} in category '{selectedCategory}'");
                                            Console.WriteLine($"{"Id",-5}{"Name",-25}{"Category",-15}State");
                                            Console.WriteLine("--------------------------------------------------------------");
                                            foreach (var toolId in filtered)
                                            {
                                                Console.WriteLine($"{toolId,-5}{names[toolId],-25}{categories[toolId],-15}{states[toolId]}");
                                            }
                                            Console.WriteLine("Total: " + filtered.Count);
                                            break;
                                        }

                                    case 12:
                                        {
                                            List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                            Console.WriteLine($"{"Id",-5}{"Name",-25}{"Category",-15}State");
                                            Console.WriteLine("--------------------------------------------------------------");
                                            foreach (var toolId in myTools)
                                            {
                                                Console.WriteLine($"{toolId,-5}{names[toolId],-25}{categories[toolId],-15}{states[toolId]}");
                                            }

                                            Console.Write("Enter the ID of the tool to transfer: ");
                                            int id = Convert.ToInt32(Console.ReadLine());

                                            string owner = owners[id];

                                            switch (owner == userKey)
                                            {
                                                case false:
                                                    throw new KeyNotFoundException("That tool does not belong to you.");
                                            }

                                            List<string> others = users.Where(u => u != userKey).ToList();

                                            switch (others.Count > 0)
                                            {
                                                case false:
                                                    throw new ArgumentException("There are no other workers registered.");
                                            }

                                            Console.WriteLine("Workers:");
                                            for (int i = 0; i < others.Count; i++)
                                            {
                                                Console.WriteLine($"  {i + 1}. {textInfo.ToTitleCase(others[i])}");
                                            }
                                            Console.Write("Choose a number: ");
                                            int workerNumber = Convert.ToInt32(Console.ReadLine());
                                            string target = others[workerNumber - 1];

                                            string toolName = names[id];

                                            bool duplicated = ids.Any(t => owners[t] == target && names[t].Equals(toolName, comparison));

                                            switch (duplicated)
                                            {
                                                case true:
                                                    throw new ArgumentException("That worker already has a tool with the same name.");
                                            }

                                            owners[id] = target;

                                            logOwners.Add(userKey);
                                            logEntries.Add($"{DateTime.Now:HH:mm:ss} Transferred '{toolName}' to {textInfo.ToTitleCase(target)}");
                                            logOwners.Add(target);
                                            logEntries.Add($"{DateTime.Now:HH:mm:ss} Received '{toolName}' from {displayName}");

                                            Console.WriteLine("Tool transferred successfully.");
                                            break;
                                        }

                                    case 13:
                                        {
                                            List<int> myTools = ids.Where(t => owners[t] == userKey).ToList();

                                            Console.WriteLine($"This will set {myTools.Count} tool(s) to '{stateCatalog[0]}'. 1. Yes, 2. No");
                                            int confirmation = Convert.ToInt32(Console.ReadLine());

                                            switch (confirmation)
                                            {
                                                case 1:
                                                    foreach (var toolId in myTools)
                                                    {
                                                        states[toolId] = stateCatalog[0];
                                                    }
                                                    logOwners.Add(userKey);
                                                    logEntries.Add($"{DateTime.Now:HH:mm:ss} Marked {myTools.Count} tool(s) as available");
                                                    Console.WriteLine("All your tools are now available.");
                                                    break;
                                                default:
                                                    Console.WriteLine("Operation cancelled.");
                                                    break;
                                            }
                                            break;
                                        }

                                    case 14:
                                        {
                                            List<string> myLog = logEntries.Where((entry, index) => logOwners[index] == userKey).ToList();

                                            Console.WriteLine($"\n<=== Activity log of {displayName} ===>");
                                            for (int i = 0; i < myLog.Count; i++)
                                            {
                                                Console.WriteLine($"{i + 1,-5}{myLog[i]}");
                                            }
                                            Console.WriteLine("Total entries: " + myLog.Count);
                                            break;
                                        }

                                    case 15:
                                        {
                                            int savedAmount = ids.Count(t => owners[t] == userKey);
                                            logOwners.Add(userKey);
                                            logEntries.Add($"{DateTime.Now:HH:mm:ss} Logged out");
                                            Console.WriteLine($"\nSession closed. {savedAmount} tool(s) were saved under the name {displayName}.");
                                            sessionActive = false;
                                            break;
                                        }

                                    case 16:
                                        {
                                            int savedAmount = ids.Count(t => owners[t] == userKey);
                                            logOwners.Add(userKey);
                                            logEntries.Add($"{DateTime.Now:HH:mm:ss} Logged out");
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
                    {
                        Console.WriteLine("\nRegistered workers:");
                        for (int i = 0; i < users.Count; i++)
                        {
                            Console.WriteLine($"  {i + 1}. {textInfo.ToTitleCase(users[i])}");
                        }

                        Console.Write("Name of the worker to delete: ");
                        string removeName = (Console.ReadLine() ?? string.Empty).Trim();

                        switch (removeName.Length > 0 && removeName.All(c => char.IsLetter(c) || c == ' '))
                        {
                            case false:
                                throw new ArgumentException("The worker name can only contain letters and cannot be empty.");
                        }

                        string removeKey = removeName.ToLower();

                        switch (users.Contains(removeKey))
                        {
                            case false:
                                throw new KeyNotFoundException("That worker does not exist.");
                        }

                        List<int> removeTools = ids.Where(t => owners[t] == removeKey).ToList();

                        Console.WriteLine($"Delete {textInfo.ToTitleCase(removeKey)} and their {removeTools.Count} tool(s)? 1. Yes, 2. No");
                        int confirmation = Convert.ToInt32(Console.ReadLine());

                        switch (confirmation)
                        {
                            case 1:
                                foreach (var toolId in removeTools)
                                {
                                    names.Remove(toolId);
                                    categories.Remove(toolId);
                                    states.Remove(toolId);
                                    owners.Remove(toolId);
                                    ids.Remove(toolId);
                                }

                                for (int i = logOwners.Count - 1; i >= 0; i--)
                                {
                                    switch (logOwners[i] == removeKey)
                                    {
                                        case true:
                                            logOwners.RemoveAt(i);
                                            logEntries.RemoveAt(i);
                                            break;
                                    }
                                }

                                users.Remove(removeKey);
                                Console.WriteLine("Worker deleted.");
                                break;
                            default:
                                Console.WriteLine("Deletion cancelled.");
                                break;
                        }
                        break;
                    }

                case 4:
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