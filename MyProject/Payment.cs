using MyProject;


    class Payment
    {
        public string TenantName { get; set; }
        public string PropertyAddress { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }

        public int PaymentId { get; set; }

        public override string ToString()
        {
            return $"{PaymentId},{TenantName},{PropertyAddress},{AmountPaid},{PaymentDate}";
        }

        public static Payment FromString(string paymentData)
        {
            var data = paymentData.Split(',');
            return new Payment { PaymentId = int.Parse(data[0]), TenantName = data[1], PropertyAddress = data[2], AmountPaid = decimal.Parse(data[3]), PaymentDate = DateTime.Parse(data[4]) };
        }
    }



    // PAYMENT


    class PaymentManagement : ManagementBase
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

                Console.Write("   Enter Property Number: ");
                int num = int.Parse(Console.ReadLine());
                Property prop = Program.properties.Find(p => p.Propertynumber == num);

                if (prop == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Property Number not found!");
                    Console.ReadKey();
                    return;
                }
                Console.Write("   Enter payment amount: ");
                decimal amountPaid = decimal.Parse(Console.ReadLine());
                Console.Write("   Enter payment date (yyyy-mm-dd): ");
                DateTime paymentDate = DateTime.Parse(Console.ReadLine());

                tenant.MakePayment(amountPaid);
                decimal rentAmount = prop.Rent;
                Save<Tenant>.SaveToFile(Program.tenants, Program.TenantFilePath);


                RecalculatepayIds();
                
                Payment payment = new Payment
                {
                    PaymentId = Program.payments.Count + 1, 
                    TenantName = tenantName,
                    PropertyAddress = propertyAddress,
                    AmountPaid = amountPaid,
                    PaymentDate = paymentDate
                };

                Program.payments.Add(payment);
                Save<Payment>.SaveToFile(Program.payments, Program.PaymentFilePath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n   Payment recorded successfully!");
                PrintReceipt(payment, rentAmount, tenant);
            }
            catch (FormatException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   Invalid input format. Please ensure that the payment amount and date are in the correct format. Error: {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   Null argument encountered. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   An unexpected error occurred: {ex.Message}");
            }

            Console.ReadKey();
        }


        private void RecalculatepayIds()
        {
            for (int i = 0; i < Program.payments.Count; i++)
            {
                Program.payments[i].PaymentId = i + 1;
            }
        }
       

        public override void Delete()
        {
            try
            {
                

                Console.Write("\n   Enter Payment Id: ");
                int id = int.Parse(Console.ReadLine());
                Payment Payid = Program.payments.Find(p => p.PaymentId == id);

                if (Payid == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Payment Number not found.");
                    Console.ReadKey();
                    return;

                }
                RecalculatepayIds();

                Program.payments.Remove(Payid);
                Save<Payment>.SaveToFile(Program.payments, Program.PaymentFilePath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n   Payment deleted successfully!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   An unexpected error occurred: {ex.Message}");
            }

            Console.ReadKey();
        }


        //view


        public override void View()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                CenterText("  PAYMENTS  ");

                Console.ForegroundColor = ConsoleColor.White;

                if (Program.payments == null || !Program.payments.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    CenterText("No payments found.");
                }
                else
                {
                  
                    CenterText("┌───────────────┬─────────────────────────┬─────────────────────────┬───────────────┬─────────────────┐");
                    CenterText("│ Payment ID    │ Tenant Name             │ Property Address        │ Amount Paid   │ Payment Date    │");
                  

                   
                    foreach (var payment in Program.payments)
                    {
                        string line = string.Format("│ {0,-13} │ {1,-23} │ {2,-23} │ {3,-13} │ {4,-15} │",
                            payment.PaymentId,
                            payment.TenantName,
                            payment.PropertyAddress,
                            payment.AmountPaid,
                            payment.PaymentDate.ToString("yyyy-MM-dd"));
                        CenterText("├───────────────┼─────────────────────────┼─────────────────────────┼───────────────┼─────────────────┤");

                        CenterText(line);

                       
                    }

                 
                    CenterText("└───────────────┴─────────────────────────┴─────────────────────────┴───────────────┴─────────────────┘");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAn error occurred while viewing payments: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
                Console.ReadKey();
            }
        }



        public void ViewTenantsByBalance()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            CenterText("  Tenants Sorted by Balance (High to Low)  ");

            if (Program.tenants == null || !Program.tenants.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No tenants found.");
                return;
            }

            
            var sortedTenants = Program.tenants.OrderByDescending(t => t.Balance).ToList();

            CenterText("┌───────┬───────────────────────────┬───────────────────────┬──────────────┐");
            CenterText("│ ID    │ Name                      │ Contact               │ Balance      │");


            foreach (var tenant in sortedTenants)
            {
                CenterText("├───────┼───────────────────────────┼───────────────────────┼──────────────┤");
                CenterText($"│ {tenant.Id,-5} │ {tenant.Name,-25} │ {tenant.Contact,-21} │ {tenant.Balance,12:N2} │");


            }

            CenterText("└───────┴───────────────────────────┴───────────────────────┴──────────────┘");
            Console.ReadLine();
        }




        private void PrintReceipt(Payment payment, decimal rentAmount, Tenant tenant)
        {
            Console.Clear();
            Console.ForegroundColor= ConsoleColor.White;
            Console.WriteLine("                                         -------------------------------------");
            Console.WriteLine("                                            ------- Payment Receipt -------");
            Console.WriteLine("                                         -------------------------------------");
            Console.WriteLine($"                                         Payment No.    : {payment.PaymentId}");
            Console.WriteLine($"                                         Tenant Name    : {payment.TenantName}");
            Console.WriteLine($"                                         Property Addr. : {payment.PropertyAddress}");
            Console.WriteLine("                                         -------------------------------------");
            Console.WriteLine($"                                         Rent Amount    : Php {rentAmount}");
            Console.WriteLine($"                                         Amount Paid    : Php {payment.AmountPaid}");
            string formattedDate = payment.PaymentDate.ToString("M/d/yyyy");
            Console.WriteLine($"                                         Remaining Bal  : {tenant.Balance}");
            Console.WriteLine($"                                         Payment Date   : {formattedDate}");
            Console.WriteLine("                                         -------------------------------------");
            Console.WriteLine("                                              Thank you for your payment!");
            Console.WriteLine("                                         -------------------------------------");

    }



    //search


    public override void Search()
    {
        try
        {
            Console.Write("\n   Enter tenant name to search payment: ");
            string tenantName = Console.ReadLine()?.Trim();

            if (Program.payments == null || !Program.payments.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n   No payments found in the system.");
                return;
            }

            // Perform a case-insensitive search using StringComparison.OrdinalIgnoreCase
            var payments = Program.payments
                .Where(p => p.TenantName?.Contains(tenantName, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            if (payments.Any())
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n   Payments found:");

                CenterText("┌───────────────┬─────────────────────────┬─────────────────────────┬───────────────┬─────────────────┐");
                CenterText("│ Payment ID    │ Tenant Name             │ Property Address        │ Amount Paid   │ Payment Date    │");

                foreach (var payment in payments)
                {
                    string line = string.Format("│ {0,-13} │ {1,-23} │ {2,-23} │ {3,-13:N2} │ {4,-15} │",
                        payment.PaymentId,
                        payment.TenantName,
                        payment.PropertyAddress,
                        payment.AmountPaid,
                        payment.PaymentDate.ToString("yyyy-MM-dd"));
                    CenterText("├───────────────┼─────────────────────────┼─────────────────────────┼───────────────┼─────────────────┤");
                    CenterText(line);
                }

                CenterText("└───────────────┴─────────────────────────┴─────────────────────────┴───────────────┴─────────────────┘");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                CenterText("\n    No payments found for the given tenant.");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CenterText($"\n    An error occurred while searching for payments: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
            Console.ReadKey();
        }
    }






    public static void CenterText(string text)
        {
            int consoleWidth = Console.WindowWidth;  
            int textLength = text.Length;           
            int spaces = (consoleWidth / 2) + (textLength / 2); 
            Console.WriteLine(text.PadLeft(spaces)); 
        }


    }

