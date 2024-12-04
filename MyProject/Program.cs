using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace MyProject
{

    //akun base class
    abstract class ManagementBase
    {
        public abstract void Add();
        public abstract void Delete();

        public abstract void View();

        public abstract void Search();



    }

    //mag write sa file

    class Save<T> 
    {

        public static void SaveToFile(List<T> list, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var item in list)
                {
                    writer.WriteLine(item.ToString());
                }
            }
        }
    }

    

    internal class Program
    {
       
        public static List<Property> properties = new List<Property>();
        public static List<Tenant> tenants = new List<Tenant>();
        public static List<LeaseAgreement> leaseAgreements = new List<LeaseAgreement>();
        public static List<Payment> payments = new List<Payment>();

        public static string PropertyFilePath = "C:\\Users\\laizame forrosuelo\\OneDrive\\Desktop\\MyProject\\properties.txt";
        public static string TenantFilePath = "C:\\Users\\laizame forrosuelo\\OneDrive\\Desktop\\MyProject\\tenants.txt";
        public static string LeaseAgreementFilePath = "C:\\Users\\laizame forrosuelo\\OneDrive\\Desktop\\MyProject\\leaseAgreements.txt";
        public static string PaymentFilePath = "C:\\Users\\laizame forrosuelo\\OneDrive\\Desktop\\MyProject\\payments.txt";

        static void Main(string[] args)
        {
            Manprop manprop = new Manprop();
            Mantent mantent = new Mantent();
            Manlease manlease = new Manlease();
            Manpay manpay = new Manpay();


            LoadData();

            try
            {
                int choice = 1; // Default starting option
                while (true)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\r\n█▀█ █▀▀ █▄░█ ▀█▀ ▄▀█ █░░   █▀▄▀█ ▄▀█ █▄░█ ▄▀█ █▀▀ █▀▀ █▀▄▀█ █▀▀ █▄░█ ▀█▀   █▀ █▄█ █▀ ▀█▀ █▀▀ █▀▄▀█\r");
                        Console.WriteLine("█▀▄ ██▄ █░▀█ ░█░ █▀█ █▄▄   █░▀░█ █▀█ █░▀█ █▀█ █▄█ ██▄ █░▀░█ ██▄ █░▀█ ░█░   ▄█ ░█░ ▄█ ░█░ ██▄ █░▀░█\n");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine((choice == 1 ? "→ " : "  ") + " Manage Properties");
                    Console.WriteLine((choice == 2 ? "→ " : "  ") + " Manage Tenants");
                    Console.WriteLine((choice == 3 ? "→ " : "  ") + " Manage Lease Agreements");
                    Console.WriteLine((choice == 4 ? "→ " : "  ") + " Manage Payments");
                    Console.WriteLine((choice == 5 ? "→ " : "  ") + " Exit");

                    ConsoleKey key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        // Use arrow keys for navigation
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            choice = (choice == 1) ? 5 : choice - 1;
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            choice = (choice == 5) ? 1 : choice + 1;
                            break;
                        case ConsoleKey.Enter:
                            // Handle selected option
                            switch (choice)
                            {
                                case 1:
                                    manprop.manprop();
                                    break;
                                case 2:
                                    mantent.mantent();
                                    break;
                                case 3:
                                    manlease.manlease();
                                    break;
                                case 4:
                                    manpay.manpay();
                                    break;
                                case 5:
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Thank you for using our system<3");
                                    Environment.Exit(0);
                                    break;
                            }
                            break;

                        case ConsoleKey.Escape:
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Invalid input format. Please enter a valid number.");
                Console.ResetColor();
            }
        }



        //for reading file


        static void LoadData()
        {
            try
            {
                if (File.Exists(PropertyFilePath))
                {
                    foreach (var line in File.ReadAllLines(PropertyFilePath))
                    {
                        properties.Add(Property.FromString(line));
                    }
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error reading property file: {ioEx.Message}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while loading properties: {ex.Message}");
            }

            try
            {
                if (File.Exists(TenantFilePath))
                {
                    foreach (var line in File.ReadAllLines(TenantFilePath))
                    {
                        tenants.Add(Tenant.FromString(line));
                    }
                }
            }
           
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error reading tenant file: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while loading properties: {ex.Message}");
            }


            try
            {
                if (File.Exists(LeaseAgreementFilePath))
                {
                    foreach (var line in File.ReadAllLines(LeaseAgreementFilePath))
                    {
                        leaseAgreements.Add(LeaseAgreement.FromString(line, tenants, properties));
                    }
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error reading leaseagreement file: {ioEx.Message}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while loading lease agreements: {ex.Message}");
            }

            try
            {
                if (File.Exists(PaymentFilePath))
                {
                    foreach (var line in File.ReadAllLines(PaymentFilePath))
                    {
                        payments.Add(Payment.FromString(line));
                    }
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error reading payment file: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while loading payments: {ex.Message}");
            }
        }

    }

    //The switch cases for like menu para limpyo ang main program


    class Manprop
    {
        PropertyManagement propertyManagement = new PropertyManagement();
        public void manprop()
        {
            int choice = 1;
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" ~ MANAGE PROPERTIES ~\n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine((choice == 1 ? "→ " : "  ") + " Add Property");
                Console.WriteLine((choice == 2 ? "→ " : "  ") + " Delete Property");
                Console.WriteLine((choice == 3 ? "→ " : "  ") + " View Properties");
                Console.WriteLine((choice == 4 ? "→ " : "  ") + " Search Properties");
                Console.WriteLine((choice == 5 ? "→ " : "  ") + " Back");

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        choice = (choice == 1) ? 5 : choice - 1;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        choice = (choice == 5) ? 1 : choice + 1;
                        break;
                    case ConsoleKey.Enter:
                        switch (choice)
                        {
                            case 1: propertyManagement.Add(); break;
                            case 2: propertyManagement.Delete(); break;
                            case 3: propertyManagement.View(); break;
                            case 4: propertyManagement.Search(); break;
                            case 5: return;
                        }
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
    }


    class Mantent
    {
        TenantManagement tenantManagement = new TenantManagement();
        ViewTenant view = new ViewTenant();

        int choice = 1;
        public void mantent()
        {
            while (true)
            {

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" ~ MANAGE TENANTS ~\n");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine((choice == 1 ? "→ " : "  ") + " Add Tenant");
                Console.WriteLine((choice == 2 ? "→ " : "  ") + " Delete Tenant");
                Console.WriteLine((choice == 3 ? "→ " : "  ") + " View Tenants");
                Console.WriteLine((choice == 4 ? "→ " : "  ") + " Search Tenant");
                Console.WriteLine((choice == 5 ? "→ " : "  ") + " Edit Tenant");
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
                            case 1: tenantManagement.Add(); break;
                            case 2: tenantManagement.Delete(); break;
                            case 3: view.view(); break;
                            case 4: tenantManagement.Search(); break;
                            case 5: tenantManagement.Edit(); break;
                            case 6: return;
                        }
                        break;
                    case ConsoleKey.Escape:
                        return;

                }
            }
        }
    }

    class Manlease
    {
        LeaseAgreementManagement leaseAgreementManagement = new LeaseAgreementManagement();
        public void manlease()
        {
            int choice = 1;
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" ~ MANAGE LEASEAGREEMENT ~\n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine((choice == 1 ? "→ " : "  ") + " Add Lease Agreement");
                Console.WriteLine((choice == 2 ? "→ " : "  ") + " Delete Lease Agreement");
                Console.WriteLine((choice == 3 ? "→ " : "  ") + " View Lease Agreements");
                Console.WriteLine((choice == 4 ? "→ " : "  ") + " Search Lease Agreement");
                Console.WriteLine((choice == 5 ? "→ " : "  ") + " Back");

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        choice = (choice == 1) ? 5 : choice - 1;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        choice = (choice == 5) ? 1 : choice + 1;
                        break;
                    case ConsoleKey.Enter:
                        switch (choice)
                        {
                            case 1: leaseAgreementManagement.Add(); break;
                            case 2: leaseAgreementManagement.Delete(); break;
                            case 3: leaseAgreementManagement.View(); break;
                            case 4: leaseAgreementManagement.Search(); break;
                            case 5: return;
                        }
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
    }



    class Manpay
    {
        PaymentManagement paymentTracking = new PaymentManagement();
        public void manpay()
        {
            int choice = 1;

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" ~ MANAGE PAYMENT ~\n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine((choice == 1 ? "→ " : "  ") + " Record Payment");
                Console.WriteLine((choice == 2 ? "→ " : "  ") + " Delete Payment");
                Console.WriteLine((choice == 3 ? "→ " : "  ") + " View Payment");
                Console.WriteLine((choice == 4 ? "→ " : "  ") + " Search Payment");
                Console.WriteLine((choice == 5 ? "→ " : "  ") + " Back");

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        choice = (choice == 1) ? 5 : choice - 1;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        choice = (choice == 5) ? 1 : choice + 1;
                        break;
                    case ConsoleKey.Enter:

                        switch (choice)
                        {
                            case 1:
                                paymentTracking.Add();
                                break;
                            case 2:
                                paymentTracking.Delete();
                                break;
                            case 3:
                                paymentTracking.View();
                                break;
                            case 4:
                                paymentTracking.Search();
                                break;
                            case 5:
                                return;
                        }
                        break;
                    case ConsoleKey.Escape:
                        return;



                }
            }
        }
    }


    



    class ViewTenant
    {
        LeaseAgreementManagement lease = new LeaseAgreementManagement();
        TenantManagement tenantManagement = new TenantManagement();
        PaymentManagement payment = new PaymentManagement();
       
        public void view()
        {
            int choice = 1;
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" ~VIEW TENANTS~ \n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine((choice == 1 ? "→ " : "  ") + "  all Tenants");
                Console.WriteLine((choice == 2 ? "→ " : "  ") + "  Tenants by Date");
                Console.WriteLine((choice == 3 ? "→ " : "  ") + "  Tenants by Balance");
                Console.WriteLine((choice == 4 ? "→ " : "  ") + "  Back");

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        choice = (choice == 1) ? 4 : choice - 1;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        choice = (choice == 4) ? 1 : choice + 1;
                        break;
                    case ConsoleKey.Enter:

                        switch (choice)
                        {
                            case 1:
                                tenantManagement.View();
                                break;
                            case 2:
                                tenantManagement.ViewTenantsByStartDate();
                                break;
                            case 3:
                                payment.ViewTenantsByBalance();
                                break;
                            case 4:
                                return;

                        }
                        break;

                    case ConsoleKey.Escape:
                        return;

                }
            }
        }

    }
}
