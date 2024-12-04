using MyProject;

    
   class Tenant
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public int Id { get; set; }
        public decimal Balance { get; set; }

       

        public override string ToString()
        {
            return $"{Id},{Name},{Contact},{Balance}";
        }

        public static Tenant FromString(string tenantData)
        {
            try
            {
                var data = tenantData.Split(',');
                return new Tenant
                {
                    Id = int.Parse(data[0]),
                    Name = data[1],
                    Contact = data[2],
                    Balance = decimal.Parse(data[3])
                };
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error parsing tenant data: {ex.Message}");
                return null;
            }
        }

       
        public void MakePayment(decimal amount)
        {
            Balance -= amount;
        }

       
        public void AddCharge(decimal Rent)
        {
            Balance += Rent;
        }
    }

   


//TENANT



    class TenantManagement : ManagementBase
    {
        LeaseAgreement leaseagreement = new LeaseAgreement();


        //add
        public override void Add()
        {
            try
            {
                Console.Write("\n   Enter tenant name: ");
                string name = Console.ReadLine();

                string contact;

                while (true)
                {
                    
                    Console.Write("   Enter Contact number: ");
                    contact = Console.ReadLine();

                    // Validate the contact number
                    if (IsValidContact(contact))
                    {
                        break; // Exit loop if valid
                    }
                    else
                    {
                       
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n   Invalid contact number. Please enter a valid 11-digit number starting with 09.");
                        Console.ResetColor();
                        Console.ReadLine();
                        return;
                    }
                }

                RecalculateTenantIds();


                Tenant tenant = new Tenant { Id = Program.tenants.Count + 1, Name = name, Contact = contact };
                Program.tenants.Add(tenant);
                Save<Tenant>.SaveToFile(Program.tenants, Program.TenantFilePath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n   Tenant added successfully!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   An error occurred while adding the tenant: {ex.Message}");
            }
            Console.ReadKey();
        }


        public bool IsValidContact(string contact)
        {
            
            return contact.Length == 11 && contact.StartsWith("09") && long.TryParse(contact, out _);
        }

        private void RecalculateTenantIds()
        {
            for (int i = 0; i < Program.tenants.Count; i++)
            {
                Program.tenants[i].Id = i + 1;
            }
        }



    public override void Delete()
    {
        try
        {
            Console.Write("\n   Enter tenant name to delete: ");
            string name = Console.ReadLine();
            Tenant tenant = Program.tenants.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (tenant == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n   Tenant not found.");
                Console.ReadKey();
                return;
            }

            Console.Write("\n   Enter tenant ID to confirm deletion: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
               
                if (tenant.Id != id)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   ID does not match.");
                    Console.ReadKey();
                    return;
                }

                
                Program.tenants.Remove(tenant);
                Save<Tenant>.SaveToFile(Program.tenants, Program.TenantFilePath);

              
                List<LeaseAgreement> leasesToRemove = Program.leaseAgreements.FindAll(lease => lease.Tenant.Id == tenant.Id);
                foreach (var lease in leasesToRemove)
                {
                   
                    Property property = lease.Property;

                    if (property != null)
                    {
                        
                        property.Status = "Available";
                    }
                }

                
                Program.leaseAgreements.RemoveAll(lease => lease.Tenant.Id == tenant.Id);
                Save<LeaseAgreement>.SaveToFile(Program.leaseAgreements, Program.LeaseAgreementFilePath);
                Save<Property>.SaveToFile(Program.properties, Program.PropertyFilePath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n   Tenant, associated lease agreements, and property status updated successfully!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n   Invalid tenant ID.");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n   An error occurred while deleting the tenant: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
            Console.ReadKey();
        }
    }







    public override void View()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                CenterText("  All Tenants  ");
                Console.ForegroundColor = ConsoleColor.White;

               
                if (Program.tenants.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    CenterText("No tenants found.");
                }
                else
                {
                   
                    CenterText("┌───────┬───────────────────────────┬────────────────────────┐");
                    CenterText("│ ID    │ Name                      │ Contact                │");

                   
                    foreach (var tenant in Program.tenants)
                    {
                      
                        string line = string.Format("│ {0,-5} │ {1,-25} │ {2,-22} │",
                            tenant.Id,      
                            tenant.Name,     
                            tenant.Contact);

                        CenterText("├───────┼───────────────────────────┼────────────────────────┤");
                        CenterText(line);
                    }

                    
                    CenterText("└───────┴───────────────────────────┴────────────────────────┘");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                CenterText($"\nAn error occurred while viewing tenants: {ex.Message}");
            }

            Console.ResetColor();
            Console.ReadKey();
        }



        public void ViewTenantsByStartDate()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                CenterText("  Tenants by Lease Start Date  \n");
                Console.ForegroundColor = ConsoleColor.White;

                
                if (Program.leaseAgreements == null || !Program.leaseAgreements.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    CenterText("No lease agreements found.");
                    Console.ResetColor();
                    Console.ReadKey();
                    return;
                }

               
                var sortedAgreements = Program.leaseAgreements
                    .Where(agreement => agreement.Tenant != null && agreement.StartDate != DateTime.MinValue)
                    .OrderBy(agreement => agreement.StartDate)
                    .ToList();

              
                if (sortedAgreements.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    CenterText(" No tenants with valid lease agreements found.");
                }
                else
                {
                   
                    CenterText("┌───────┬───────────────────────────┬────────────────────┐");
                    CenterText("│ ID    │ Tenant Name               │ Start Date         │");

                   
                    foreach (var agreement in sortedAgreements)
                    {
                        string line = string.Format("│ {0,-5} │ {1,-25} │ {2,-18} │",
                            agreement.Tenant.Id,
                            agreement.Tenant.Name,
                            agreement.StartDate.ToString("yyyy-MM-dd"));

                        CenterText("├───────┼───────────────────────────┼────────────────────┤");
                        CenterText(line);
                    }

                  
                    CenterText("└───────┴───────────────────────────┴────────────────────┘");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                CenterText($"An error occurred while viewing tenants by start date: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        //search

        public override void Search()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" ~SEARCH TENANT~ ");
                Console.ResetColor();

                Console.Write("   Enter tenant name or ID to search: ");
                string searchInput = Console.ReadLine();

               
                List<Tenant> searchResults;
                if (int.TryParse(searchInput, out int tenantId))
                {
                   
                    searchResults = Program.tenants.Where(t => t.Id == tenantId).ToList();
                }
                else
                {
                   
                    searchResults = Program.tenants
                        .Where(t => t.Name.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

               
                if (searchResults.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    CenterText("No matching tenants found.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    CenterText(" SEARCH RESULTS ");
                    Console.ForegroundColor = ConsoleColor.White;

                  
                    CenterText("┌───────┬───────────────────────────┬────────────────────┬─────────────┐");
                    CenterText("│ ID    │ Name                      │ Contact            │ Balance     │");

                    
                    foreach (var tenant in searchResults)
                    {
                        string line = string.Format("│ {0,-5} │ {1,-25} │ {2,-18} │ {3,11:N2} │",
                            tenant.Id,       
                            tenant.Name,      
                            tenant.Contact,   
                            tenant.Balance);  

                        CenterText("├───────┼───────────────────────────┼────────────────────┼─────────────┤");
                        CenterText(line);
                    }

                    
                    CenterText("└───────┴───────────────────────────┴────────────────────┴─────────────┘");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                CenterText($"An error occurred while searching tenants: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
                Console.ReadKey();
            }
        }


    // Edit a tenant's details
    public void Edit()
    {
        try
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" ~EDIT TENANT DETAILS~ ");
            Console.ResetColor();

            // Prompt for tenant ID to edit
            Console.Write("\n   Enter tenant ID to edit: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                // Find the tenant by ID
                Tenant tenant = Program.tenants.Find(t => t.Id == id);

                if (tenant == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Tenant not found.");
                    Console.ReadKey();
                    return;
                }

                // Display current tenant details
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n   Current Details: \n   Name: {tenant.Name}\n   Contact: {tenant.Contact}\n   Balance: {tenant.Balance:C}");
                Console.ResetColor();

                // Edit tenant name
                Console.Write("\n   Enter new name (leave blank to keep current): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    tenant.Name = newName;
                    
                }

                // Edit tenant contact
                Console.Write("   Enter new contact (leave blank to keep current): ");
                string newContact = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newContact))
                {
                    // Validate new contact number
                    if (IsValidContact(newContact))
                    {
                        tenant.Contact = newContact;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n   Invalid contact number. Must be 11 digits starting with '09'.");
                        Console.ResetColor();
                        return;
                    }
                }

                // Edit tenant balance
                Console.Write("   Enter new balance (leave blank to keep current): ");
                string balanceInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(balanceInput) && decimal.TryParse(balanceInput, out decimal newBalance))
                {
                    tenant.Balance = newBalance;
                }

                Save<LeaseAgreement>.SaveToFile(Program.leaseAgreements, Program.LeaseAgreementFilePath);

                // Save changes to file
                Save<Tenant>.SaveToFile(Program.tenants, Program.TenantFilePath);

                

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n   Tenant details updated successfully!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n   Invalid tenant ID.");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n   An error occurred while editing tenant details: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
            Console.ReadKey();
        }
    }





    //para ma center ang text


    public static void CenterText(string text)
        {
            int consoleWidth = Console.WindowWidth; 
            int textLength = text.Length;            
            int spaces = (consoleWidth / 2) + (textLength / 2); 
            Console.WriteLine(text.PadLeft(spaces)); 
        }
    }

