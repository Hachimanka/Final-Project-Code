using MyProject;


    class LeaseAgreement
    {
        public Tenant Tenant { get; set; }
        public Property Property { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RentAmount => Property.Rent; 

        public override string ToString()
        {
            return $"{Tenant.Name},{Property.Address},{RentAmount},{StartDate:yyyy-MM-dd},{EndDate:yyyy-MM-dd}";
        }

        public static LeaseAgreement FromString(string agreementData, List<Tenant> tenants, List<Property> properties)
        {
            try
            {
                var data = agreementData.Split(',');
                Tenant tenant = tenants.Find(t => t.Name == data[0]);
                Property property = properties.Find(p => p.Address == data[1]);

                if (tenant == null || property == null)
                    throw new Exception("Tenant or Property not found.");

                return new LeaseAgreement
                {
                    Tenant = tenant,
                    Property = property,
                    StartDate = DateTime.Parse(data[3]),
                    EndDate = DateTime.Parse(data[4])
                };
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error parsing lease agreement: {ex.Message}");
                return null;
            }
        }
    }


    //LEASEAGREEMENT

    class LeaseAgreementManagement : ManagementBase
    {

        //add
        public override void Add()
        {
            try
            {
                Console.Write("\n   Enter tenant name: ");
                string tenantName = Console.ReadLine();
                Tenant tenant = Program.tenants.Find(t => t.Name == tenantName);

                if (tenant == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Tenant not found!");
                    Console.ReadKey();
                    return;
                }

                Console.Write("   Enter property address: ");
                string propertyAddress = Console.ReadLine();
                Property property = Program.properties.Find(p => p.Address == propertyAddress);

                if (property == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Property not found!");
                    Console.ReadKey();
                    return;
                }

                Console.Write("   Enter Property No: ");
                int num;
                if (!int.TryParse(Console.ReadLine(), out num))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Invalid property number format.");
                    Console.ReadKey();
                    return;
                }

               
                Property prop = Program.properties.Find(p => p.Propertynumber == num);
                if (prop == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Property number not found!");
                    Console.ReadKey();
                    return;
                }

               
                if (prop.Status == "Rented")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   This property is already occupied!");
                    Console.ReadKey();
                    return;
                }

                Console.Write("   Enter lease start date (yyyy-mm-dd): ");
                DateTime startDate;
                if (!DateTime.TryParse(Console.ReadLine(), out startDate))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Invalid start date format.");
                    Console.ReadKey();
                    return;
                }

                Console.Write("   Enter lease end date (yyyy-mm-dd): ");
                DateTime endDate;
                if (!DateTime.TryParse(Console.ReadLine(), out endDate))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Invalid end date format.");
                    Console.ReadKey();
                    return;
                }

                if (startDate >= endDate)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   End date must be after the start date.");
                    Console.ReadKey();
                    return;
                }

              
                LeaseAgreement agreement = new LeaseAgreement
                {
                    Tenant = tenant,
                    Property = prop,
                    StartDate = startDate,
                    EndDate = endDate
                };
                Program.leaseAgreements.Add(agreement);
                Save<LeaseAgreement>.SaveToFile(Program.leaseAgreements, Program.LeaseAgreementFilePath);

                prop.Status = "Rented";
                Save<Property>.SaveToFile(Program.properties, Program.PropertyFilePath);

               
               

               
                ApplyMonthlyCharge(startDate, endDate, tenant, agreement);

              
                Save<Tenant>.SaveToFile(Program.tenants, Program.TenantFilePath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n   Lease agreement added successfully!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   An error occurred while adding the lease agreement: {ex.Message}");
                Console.ReadKey();
            }
        }



        public void ApplyMonthlyCharge(DateTime startDate, DateTime endDate, Tenant tenant, LeaseAgreement agreement)
        {
            try
            {
                // Akun gi Calculate ang number sang full months between sa start date and the end date


                int months = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;


                if (endDate.Day < startDate.Day)
                {
                    months--;
                }

                
                if (months >= 1)
                {
                    tenant.AddCharge(agreement.RentAmount); 
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred while applying charges: {ex.Message}");
            }
        }




        //delete


        public override void Delete()
        {
            try
            {
                Console.Write("\n   Enter tenant name to delete lease agreement: ");
                string tenantName = Console.ReadLine();
                LeaseAgreement agreementToDelete = Program.leaseAgreements.Find(l => l.Tenant.Name == tenantName);

               

                if (agreementToDelete == null)

                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Lease agreement not found.");
                    Console.ReadLine();
                    return;
                   
                }

                Console.Write("   Enter Property No.: ");
                int num = int.Parse(Console.ReadLine());

                Property prop = Program.properties.Find(p => p.Propertynumber == num);

                prop.Status = "Available";

                Save<Property>.SaveToFile(Program.properties, Program.PropertyFilePath);

                if(prop == null)
                {
                    Console.WriteLine("\n   Property number not found!");
                }

                
                    Program.leaseAgreements.Remove(agreementToDelete);
                    Save<LeaseAgreement>.SaveToFile(Program.leaseAgreements, Program.LeaseAgreementFilePath);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n   Lease agreement deleted successfully!");
                
                
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   An error occurred while deleting the lease agreement: {ex.Message}");
                Console.ReadKey();
            }

           
        }



    //view

    public override void View()
    {
        try
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            CenterText("LEASE AGREEMENTS");
            Console.ForegroundColor = ConsoleColor.White;

            if (Program.leaseAgreements == null || !Program.leaseAgreements.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                CenterText("No lease agreements found.");
            }
            else
            {
                
                CenterText("┌──────┬──────────────────────────────┬──────┬────────────────────────────────┬────────────┬────────────┐");
                CenterText("│ ID   │ Tenant                       │Prop# │ Property Address               │ Start Date │ End Date   │");

                foreach (var agreement in Program.leaseAgreements)
                {
                    if (agreement.Tenant == null)
                    {
                        CenterText("│ Lease agreement has no tenant information.                                        │");
                        continue;
                    }

                    if (agreement.Property == null)
                    {
                        CenterText($"│ Tenant: {agreement.Tenant.Name}, Lease agreement has no property information.  │");
                        continue;
                    }

                 
                    string line = string.Format("│ {0,-4} │ {1,-28} │ {2,-4} │ {3,-30} │ {4,-10} │ {5,-10} │",
                        agreement.Tenant.Id,
                        agreement.Tenant.Name,                   
                        agreement.Property.Propertynumber,        
                        agreement.Property.Address,               
                        agreement.StartDate.ToString("yyyy-MM-dd"),
                        agreement.EndDate.ToString("yyyy-MM-dd"));

                   
                    CenterText("├──────┼──────────────────────────────┼──────┼────────────────────────────────┼────────────┼────────────┤");
                    CenterText(line);
                }

                
                CenterText("└──────┴──────────────────────────────┴──────┴────────────────────────────────┴────────────┴────────────┘");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nAn error occurred while viewing lease agreements: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
            Console.ReadKey();
        }
    }



    public override void Search()
    {
        try
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n   SEARCH LEASE AGREEMENT");
            Console.ResetColor();

            Console.Write("\n   Enter tenant name or property address to search: ");
            string searchQuery = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n   Invalid search input.");
                Console.ReadKey();
                return;
            }

           
            var matchingAgreements = Program.leaseAgreements.Where(agreement =>agreement.Tenant.Name.ToLower().Contains(searchQuery) 
            || agreement.Property.Address.ToLower().Contains(searchQuery)).ToList();

            if (matchingAgreements == null || matchingAgreements.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n   No lease agreements found matching the search query.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n   Matching Lease Agreements:");
                CenterText("┌──────┬─────────────────────────┬────────────────────────────────┬────────────────────┬────────────────────┐");
                CenterText("│ ID   │ Tenant                  │ Property Address               │ Start Date         │ End Date           │");

                foreach (var agreement in matchingAgreements)
                {
                    string line = string.Format("│ {0,-4} │ {1,-23} │ {2,-30} │ {3,-18} │ {4,-18} │",
                        agreement.Tenant.Id,
                        agreement.Tenant.Name,
                        agreement.Property.Address,
                        agreement.StartDate.ToString("yyyy-MM-dd"),
                        agreement.EndDate.ToString("yyyy-MM-dd"));
                    CenterText("├──────┼─────────────────────────┼────────────────────────────────┼────────────────────┼────────────────────┤");
                    CenterText(line);
                }
                CenterText("└──────┴─────────────────────────┴────────────────────────────────┴────────────────────┴────────────────────┘");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n   An error occurred during the search: {ex.Message}");
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
            int consoleWidth = Console.WindowWidth;  // width
            int textLength = text.Length;           
            int spaces = (consoleWidth / 2) + (textLength / 2);
            Console.WriteLine(text.PadLeft(spaces));  // Print the centered text
        }

    }
