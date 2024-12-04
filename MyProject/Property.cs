using MyProject;

class Property
{
    public int Propertynumber { get; set; }
    public string Type { get; set; }
    public string Address { get; set; }
    public decimal Rent { get; set; }
    public string Status { get; set; } 

    
    public Property()
    {
        Status = "Available";
    }

    public override string ToString()
    {
        return $"{Propertynumber},{Type},{Address},{Rent},{Status}";
    }

    public static Property FromString(string propertyData)
    {
        var data = propertyData.Split(',');
        try
        {
            return new Property
            {
                Propertynumber = int.Parse(data[0]),
                Type = data[1],
                Address = data[2],
                Rent = decimal.Parse(data[3]),
                Status = data[4] 
            };
        }
        catch (FormatException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Invalid data format.");
            return null;
        }
    }
}



//PROPERTY


class PropertyManagement : ManagementBase
{

    //add
    public override void Add()
    {
        try
        {
            Console.Write("\n   Enter property type: ");
            string type = Console.ReadLine();
            Console.Write("   Enter address: ");
            string address = Console.ReadLine();
            Console.Write("   Enter rent amount: ");
            decimal rent = decimal.Parse(Console.ReadLine());

            Recalculatepropnum();

            Property property = new Property
            {
                Propertynumber = Program.properties.Count + 1,
                Type = type,
                Address = address,
                Rent = rent
            };
            Program.properties.Add(property);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n   Property added successfully!");
            Save<Property>.SaveToFile(Program.properties, Program.PropertyFilePath);
        }
        catch (FormatException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n   Error: Invalid input format. Please ensure all inputs are correct.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n   An unexpected error occurred: {ex.Message}");
        }
        Console.ReadKey();
    }


    private void Recalculatepropnum()
    {
        for (int i = 0; i < Program.properties.Count; i++)
        {
            Program.properties[i].Propertynumber = i + 1;
        }
    }

    //delete

    public override void Delete()
    {
        try
        {
            Console.Write("\n   Enter property address to delete: ");
            string address = Console.ReadLine();
            Property propertyToDelete = Program.properties.Find(p => p.Address == address);

            if (propertyToDelete == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Property not found.");
                Console.ReadKey();
                return;
            }

            Console.Write("   Enter property number to confirm deletion: ");
            int propnum = int.Parse(Console.ReadLine());
            if (propertyToDelete.Propertynumber != propnum)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n   Property number does not match.");
                Console.ReadKey();
                return;
            }

            Recalculatepropnum();

            Program.properties.Remove(propertyToDelete);
            Save<Property>.SaveToFile(Program.properties, Program.PropertyFilePath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n   Property deleted successfully!");
        }
        catch (FormatException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n   Error: Invalid property number format.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n   An unexpected error occurred: {ex.Message}");
        }
        Console.ReadKey();
    }


    //delete


    public override void View()
    {
        try
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            CenterText(" PROPERTIES ");
            Console.ResetColor();

            
            CenterText("   ┌──────────┬───────────────┬────────────────────────────────┬────────────┬────────────┐");
            CenterText("   │ Prop #   │ Type          │ Address                        │ Rent       │ Status     │");
           

            
            foreach (var property in Program.properties)
            {
                
                string row = string.Format("                  │ {0,-8} │ {1,-13} │ {2,-30} │ {3,10:N2} │ ",
                    property.Propertynumber,
                    property.Type,
                    property.Address,
                    property.Rent);
                CenterText("   ├──────────┼───────────────┼────────────────────────────────┼────────────┼────────────┤");
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(row);

               
                if (property.Status.Equals("Available", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Green; 
                }
                else if (property.Status.Equals("Rented", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Blue; 
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }

               
                Console.Write($"{property.Status,-10}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" │ ");
                Console.ResetColor();

               
                
            }

           
            CenterText("   └──────────┴───────────────┴────────────────────────────────┴────────────┴────────────┘");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterText($"\nAn unexpected error occurred while viewing properties: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
            Console.ReadKey();
        }
    }






    // Search Method


    public override void Search()
    {
        try
        {
            int choice = 1;

            while (true)
            {
                List<Property> searchResults = new List<Property>();

                while (true)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Choose search criteria: \n");
                    Console.ForegroundColor = ConsoleColor.White;


                    Console.WriteLine((choice == 1 ? "→ " : "  ") + " Property Number");
                    Console.WriteLine((choice == 2 ? "→ " : "  ") + " Type");
                    Console.WriteLine((choice == 3 ? "→ " : "  ") + " Address");
                    Console.WriteLine((choice == 4 ? "→ " : "  ") + " Rent");
                    Console.WriteLine((choice == 5 ? "→ " : "  ") + " Status");
                    Console.WriteLine((choice == 6 ? "→ " : "  ") + " Back");


                    ConsoleKey key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            choice = (choice == 1) ? 6 : choice - 1;
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            choice = (choice == 6) ? 1 : choice + 1;
                            break;
                        case ConsoleKey.Enter:

                            switch (choice)
                            {
                                case 1:
                                    Console.Write("   Enter Property Number: ");
                                    int propNum = int.Parse(Console.ReadLine());
                                    searchResults = Program.properties.FindAll(p => p.Propertynumber == propNum);
                                    break;

                                case 2:
                                    Console.Write("   Enter Property Type: ");
                                    string type = Console.ReadLine();
                                    searchResults = Program.properties.FindAll(p => p.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
                                    break;

                                case 3:
                                    Console.Write("   Enter Address: ");
                                    string address = Console.ReadLine();
                                    searchResults = Program.properties.FindAll(p => p.Address.Contains(address, StringComparison.OrdinalIgnoreCase));
                                    break;

                                case 4:
                                    Console.Write("   Enter Rent amount: ");
                                    decimal rent = decimal.Parse(Console.ReadLine());
                                    searchResults = Program.properties.FindAll(p => p.Rent == rent);
                                    break;

                                case 5:
                                    Console.Write("   Enter Status (Available/Rented): ");
                                    string status = Console.ReadLine();
                                    searchResults = Program.properties.FindAll(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                                    break;

                                case 6:
                                    return;
                            }
                            DisplaySearchResults(searchResults);
                            break;
                        case ConsoleKey.Escape:
                            return;
                    }
                }
            }
        }
        catch (FormatException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   Error: Invalid input format.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"   An unexpected error occurred: {ex.Message}");
        }
        Console.ReadKey();
    }



    private void DisplaySearchResults(List<Property> searchResults)
    {
        if (searchResults.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterText("   No matching properties found.");
            Console.ReadKey();
        }
        else
        {

            Console.ForegroundColor = ConsoleColor.Green;
            CenterText("\n SEARCH RESULTS ");
            Console.ResetColor();

            CenterText("   ┌──────────┬───────────────┬────────────────────────────────┬────────────┬────────────┐");
            CenterText("   │ Prop #   │ Type          │ Address                        │ Rent       │ Status     │");



            foreach (var property in searchResults)
            {

                string row = string.Format("                  │ {0,-8} │ {1,-13} │ {2,-30} │ {3,10:N2} │ ",
                    property.Propertynumber,
                    property.Type,
                    property.Address,
                    property.Rent);
                CenterText("   ├──────────┼───────────────┼────────────────────────────────┼────────────┼────────────┤");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(row);


                if (property.Status.Equals("Available", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (property.Status.Equals("Rented", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }


                Console.Write($"{property.Status,-10}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" │ ");
                Console.ResetColor();



            }


            CenterText("   └──────────┴───────────────┴────────────────────────────────┴────────────┴────────────┘");

            Console.ResetColor();
            Console.ReadKey();
        }
    }



    //para ma center and text

    public static void CenterText(string text)
    {
        int consoleWidth = Console.WindowWidth;
        int textLength = text.Length;
        int spaces = (consoleWidth / 2) + (textLength / 2);
        Console.WriteLine(text.PadLeft(spaces));
    }
}
