//============================================================
// Student Number : S10203242, S10206172
// Student Name : Russell de Silva, Ayken Lee Kang
// Module Group : T04
//============================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace COVID_Monitoring_System
{
    class Program
    {
        //Creating a menu to access features
        static void DisplayMenu()
        {
            string[] menu = {
                "List all Visitors",
                "List Person Details",
                "Assign/Replace TraceTogether Token",
                "List all Business Locations",
                "Edit Business Location Capacity",
                "SafeEntry Check-in",
                "SafeEntry Check-out",
                "List all SHN Facilities",
                "Create new Visitor",
                "Create Travel entry record",
                "Calculate SHN charges",
                "Generate Contact Tracing Report",
                "Generate SHN Status Report"
            };
            Console.WriteLine("================ Menu ================");//printing the menu
            for (int i = 0; i < menu.Length; i++)
            {
                Console.WriteLine("[" + (i + 1) + "] " + menu[i]);
            }
            Console.WriteLine("[0] Quit the program");
            Console.WriteLine("======================================");//stop the program 
        }
        //Basic Feature 1 Load person and business location data
        static List<string[]> FiletoList(string path) //convert each line of file to string array; string path = name of file
        {
            List<string[]> DataList = new List<string[]>();
            using (StreamReader sr = new StreamReader(path))//Read excel file 
            {
                string header = sr.ReadLine();
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(',');
                    DataList.Add(data);
                }
            }
            return DataList;
        }
        //Basic Feature 2 load SHN facility data 
        static List<SHNFacility> SHNAPI() //call API and store JSON as List of SHNFacility objects
        {
            List<SHNFacility> SHNList = new List<SHNFacility>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://covidmonitoringapiprg2.azurewebsites.net");
                Task<HttpResponseMessage> responseTask = client.GetAsync("/facility");
                responseTask.Wait();
                HttpResponseMessage result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Task<string> readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    string data = readTask.Result;
                    SHNList = JsonConvert.DeserializeObject<List<SHNFacility>>(data);
                }
            }
            return SHNList;
        }
        //Basic Feature 3 List all visitors 
        static void DisplayVisitors(List<Visitor> visitorList) // Displaying content of the list 
        {
            foreach (Visitor visitor in visitorList)
            {
                Console.WriteLine(visitor.ToString());
            }
        }
        //Basic Feature 4 List person deatails 
        static void DisplayPersonDetails(List<Person> personList)
        {
            while (true)
            {
                Console.WriteLine("Enter Name: ");
                string name = Console.ReadLine();
                foreach (Person person in personList)
                {
                    if (name == person.Name)
                    {
                        Console.WriteLine(person.ToString());
                        if (person is Resident)
                        {
                            Resident resident = (Resident)person;
                            Console.WriteLine(resident.Token.ToString());
                            break;
                        }
                       
                    }
                }
                Console.WriteLine("Invalid name entered. Please try again");
                continue;
            }

            

        }
        //Generate random and unique Serial Number for Token
        static string GenerateSerialNo(List<Person> personList)
        {
            string serialNo = "";
            while (true)
            {
                Random rnd = new Random();
                int randomNo = rnd.Next(10000, 100000);
                serialNo = "T" + Convert.ToString(randomNo);
                bool exist = false;
                foreach (Person person in personList)
                {
                    if (person is Resident)
                    {
                        Resident resident = (Resident)person;
                        if (serialNo == resident.Token.SerialNo)
                        {
                            exist = true;
                        }
                    }

                }
                if (!exist)
                    break;
            }
            return serialNo;
        }
        //Basic Feature 5 Assign/Replace TraceTogether Token
        static void AssignToken(List<Person> personList)
        {
 
            {
                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Person p = InitPerson(personList, name);
                while (p.Name == null)
                {
                    Console.WriteLine("Name not found!");
                    Console.Write("Enter name: ");
                    name = Console.ReadLine();
                    p = InitPerson(personList, name);
                }
 
                if (p is Resident)

                {
                    DateTime today = DateTime.Today;
                    Resident resident = (Resident)p;
                            
                    if (resident.Token.SerialNo is null | today > resident.Token.ExpiryDate.AddMonths(1))
                    {                               
                        string serialNo = GenerateSerialNo(personList);
                        resident.Token.SerialNo = serialNo;
                        resident.Token.CollectionLocation = "Kallang CC";

                        today = today.AddMonths(6);

                        resident.Token.ExpiryDate = today;
                        Console.WriteLine("You have been assigned a token!");
                    }
                          
                }

              
            }
            
        }
        //Basic Feature 6 List all business location 
        static void DisplayBusinessLocation(List<BusinessLocation> bizList)
        {
            foreach (BusinessLocation businessLocation in bizList)
            {
                Console.WriteLine(businessLocation.ToString());
            }
        }
        //Basic Feature 7 Edit business location capacity
        static void EditBusinessLocation(List<BusinessLocation> bizList)
        {
            while (true)
            {
                Console.WriteLine("Enter Details: ");
                string details = Console.ReadLine();
                foreach (BusinessLocation businessLocation in bizList)
                {
                    if (details == businessLocation.BusinessName)
                    {
                        Console.WriteLine("Edit Maximum Capacity: ");
                        int maximumCapacity = Convert.ToInt32(Console.ReadLine());
                        businessLocation.MaximumCapacity = maximumCapacity;
                    }

                }
                Console.WriteLine("Invalid location entered. Please try again");
                continue;
            }
            

        }
        //Basic Feature 8 SafeEtry Check-In
        static void CheckIn(List<Person> personList, List<BusinessLocation> bizList)
        {
            while (true)
            {
                Console.WriteLine("Enter Name: ");
                string name = Console.ReadLine();
                foreach (Person person in personList)
                {
                    if (name == person.Name)
                    {
                        DisplayBusinessLocation(bizList);
                        Console.WriteLine("Choose Business Location: ");
                        string locationName = Console.ReadLine();
                        foreach (BusinessLocation location in bizList)
                        {
                            if (locationName == location.BusinessName)
                            {
                                if (!location.IsFull())
                                {
                                    DateTime today = DateTime.Now;
                                    SafeEntry safeEntry = new SafeEntry(today, location);
                                    person.AddSafeEntry(safeEntry);
                                    location.VisitorsNow += 1;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("Invalid name entered. Please try again");
                continue;
            }
            
        }
        //Basic Feature 9 SafeEntry Check-Out 
        static void CheckOut(List<Person> personList)
        {
            
            {
                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Person p = InitPerson(personList, name);
                while (p.Name == null)
                {
                    Console.WriteLine("Name not found!");
                    Console.Write("Enter name: ");
                    name = Console.ReadLine();
                    p = InitPerson(personList, name);
                }
  
                foreach (SafeEntry safeEntry in p.SafeEntryList)
                {
                    DateTime checkOut = safeEntry.CheckOut;
                    if (checkOut == DateTime.MinValue)
                    {
                        Console.WriteLine(safeEntry.ToString());
                    }

                }
                Console.WriteLine("Select Location to check out: ");
                string location = Console.ReadLine();
                bool checkout = false;

                foreach (SafeEntry safeEntry in p.SafeEntryList)
                {
                    if (location == safeEntry.Location.BusinessName)
                    {
                        safeEntry.performCheckOut();
                        checkout = true;
                    }
                            
                }
                if(checkout == false)
                {
                    Console.WriteLine("Unable to CheckOut (You need to CheckIn first)");
                }

                
            }
            
        }




        //Basic Feature 10 List all SHN Facilities
        static void DisplaySHNFacilities(List<SHNFacility> SHNList)//display SHN facilities
        {
            Console.WriteLine("{0,-15}{1,10}{2,30}{3,30}{4,30}", "Name", "Capacity", "Dist from Air checkpoint", "Dist from Sea checkpoint", "Dist from Land checkpoint");
            foreach (SHNFacility s in SHNList)
            {
                Console.WriteLine("{0,-15}{1,10}{2,30}{3,30}{4,30}", s.FacilityName, s.FacilityCapacity, s.DistFromAirCheckpoint, s.DistFromSeaCheckpoint, s.DistFromLandCheckpoint);
            }
        }
        //Basic Feature 11 Create Visitors 
        static void CreateVisitor(List<Person> personList) //Create a new visitor object based on user inputs
        {
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            Console.Write("Enter passport No: ");
            string passNo = Console.ReadLine();
            Console.Write("Enter nationality: ");
            string nationality = Console.ReadLine();
            Visitor newVisitor = new Visitor(name, passNo, nationality);
            personList.Add(newVisitor);
            Console.WriteLine("New visitor created.");
        }
        //Basic Feature 12 Create TraveEntry Record
        static void NewTravelEntry(List<Person> personList, List<SHNFacility> SHNList)
        {
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            Person p = InitPerson(personList, name);
            while (p.Name == null)
            {
                Console.WriteLine("Name not found!");
                Console.Write("Enter name: ");
                name = Console.ReadLine();
                p = InitPerson(personList, name);
            }
            Console.Write("Enter last country of embarkation: ");
            string lastCountry = Console.ReadLine();
            Console.Write("Enter mode of entry: ");
            string entryMode = Console.ReadLine();
            while (entryMode != "Sea" && entryMode != "Land" && entryMode != "Air")
            {
                Console.WriteLine("Invalid entry mode! (Land, Sea or Air only)");
                Console.Write("Enter mode of entry: ");
                entryMode = Console.ReadLine();
            }
            TravelEntry newEntry = new TravelEntry(lastCountry, entryMode, DateTime.Now);
            newEntry.CalculateSHNDuration();
            if ((newEntry.SHNEndDate - DateTime.Now).Days + 1 == 14)
            {
                Console.WriteLine("You are required to do SHN at a SDF.\nPlease choose your preferred facility: ");
                DisplaySHNFacilities(SHNList);
                Console.Write("Enter name of facility: ");
                string SHNname = Console.ReadLine();
                SHNFacility s = InitSHNFacility(SHNList, SHNname);
                while (s.FacilityName == null)
                {
                    Console.WriteLine("Invalid facility name!");
                    DisplaySHNFacilities(SHNList);
                    Console.Write("Enter name of facility: ");
                    SHNname = Console.ReadLine();
                    s = InitSHNFacility(SHNList, SHNname);
                }
                if (s.IsAvailable())
                {
                    s.FacilityVacancy -= 1;
                    newEntry.AssignSHNFacility(s);
                }

                            
            }
            else if ((newEntry.SHNEndDate - DateTime.Now).Days + 1 == 7)
            {
                Console.WriteLine("You are required to do SHN at your own accomodation.");
            }
            p.AddTravelEntry(newEntry);
            Console.WriteLine("Travel entry added. Welcome to Singapore!");
        }
        //Basic feature 13 Calculate SHN Charges
        static void CalculateSHNCharges(List<Person> personList)
        {
            
            bool entryFound = false;
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            Person p = InitPerson(personList, name);
            while (p.Name == null)
            {
                Console.WriteLine("Name not found!");
                Console.Write("Enter name: ");
                name = Console.ReadLine();
                p = InitPerson(personList, name);
            }
            foreach (TravelEntry t in p.TravelEntryList)
            {
                if (!t.IsPaid)
                {
                    entryFound = true;
                    double SHNcharge = p.CalculateSHNCharges() * 1.07;
                    Console.WriteLine("Total Charge: ${0}", SHNcharge.ToString("0.00"));
                    t.IsPaid = true;
                    break;
                }
            }
            if (!entryFound)
            {
                Console.WriteLine("{0} has no unpaid travel entries!", name);
            }
        }
        //Advanced Feature 1 Contact Tracing Reporting 
        static void ContactTracingReport(List<Person> personList, List<BusinessLocation> bizList)
        {
            Console.Write("Enter Buiness Location: ");
            string bizLocation = Console.ReadLine();
            while (!InitBussinessLocation(bizList, bizLocation))
            {
                Console.WriteLine("Business not found!");
                Console.Write("Enter Buiness Location: ");
                bizLocation = Console.ReadLine();
            }
            DateTime parsedDate = ValidDate("Date");
            DateTime startTime = ValidDate("Start time");
            DateTime startDate = parsedDate.Date + startTime.TimeOfDay;
            DateTime endTime = ValidDate("End time");
            DateTime endDate = parsedDate.Date + endTime.TimeOfDay;
            using (StreamWriter sw = new StreamWriter("ContactTracing.csv", false))
            {
                foreach (Person person in personList)
                {
                    foreach (SafeEntry safeEntry in person.SafeEntryList)
                    {
                        if (safeEntry.Location.BusinessName == bizLocation)
                        {
                            if (safeEntry.CheckIn < endDate && safeEntry.CheckIn > startDate || safeEntry.CheckOut < endDate && safeEntry.CheckOut > startDate)
                            {
                                string[] contactTracingData = { person.Name, bizLocation, safeEntry.CheckIn.ToString(), safeEntry.CheckOut.ToString() };
                                sw.WriteLine("{0},{1},{2},{3}", contactTracingData[0], contactTracingData[1], contactTracingData[2], contactTracingData[3]);
                            }
                        }
                        break;
                    }
                }
            }
        }

        //Advanced Feature 2 SHN Status Reporting 
        static void SHNStatusReport(List<Person> personList)
        {
            Console.Write("Enter date: ");
            string unparsedDate = Console.ReadLine();
            bool parsed = DateTime.TryParse(unparsedDate, out DateTime parsedDate);
            while (!parsed)
            {
                Console.WriteLine("Invalid Date!");
                Console.Write("Enter date: ");
                unparsedDate = Console.ReadLine();
                parsed = DateTime.TryParse(unparsedDate, out parsedDate);
            }
            using (StreamWriter sw = new StreamWriter("SHNStatus.csv", false))
            foreach (Person p in personList)
            {
                foreach (TravelEntry t in p.TravelEntryList)
                {
                    if (t.SHNEndDate.CompareTo(parsedDate) >= 0)
                    {
                        string[] SHNStatusData = { p.Name, t.SHNStay.FacilityName, t.SHNEndDate.ToString() };
                        sw.WriteLine("{0},{1},{2}", SHNStatusData[0], SHNStatusData[1], SHNStatusData[2]);
                    }
                }
            }
        }
        static int ValidOption() //Check if the option entered is valid 
        {
            int option = 256;
            bool invalid = true;
            while (invalid)
            {
                try
                {
                    DisplayMenu();
                    Console.Write("Enter option: ");
                    option = Convert.ToInt32(Console.ReadLine());
                    invalid = false;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Enter an integer (0-13)!");
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input!");
                }
            }
            return option;
        }
        static DateTime ValidDate(string dateType) //check if datetime entered is valid
        {
            Console.Write("Enter {0}: ",dateType);
            string date = Console.ReadLine();
            bool dateParsed = DateTime.TryParse(date, out DateTime parsedDate);
            while (!dateParsed)
            {
                Console.WriteLine("Invalid {0}!",dateType);
                Console.Write("Enter {0}: ",dateType);
                date = Console.ReadLine();
                dateParsed = DateTime.TryParse(date, out parsedDate);
            }
            return parsedDate;
        }

        static bool InitBussinessLocation(List<BusinessLocation> bizList, string name) // Initialize business location 
        {
            foreach (BusinessLocation b in bizList)
            {
                if (name == b.BusinessName)
                {
                    return true;
                }
            }
            return false;
        }
        static SHNFacility InitSHNFacility(List<SHNFacility> SHNList, string name)// initialize SHN facility
        {
            foreach (SHNFacility s in SHNList)
            {
                if (name == s.FacilityName)
                {

                    return s;
                }
            }
            return new SHNFacility();
        }
        static Person InitPerson(List<Person> personList, string name) // initialize person list 
        {
            foreach (Person p in personList)
            {
                if (name == p.Name)
                {

                    return p;
                }
            }
            return new Resident();
        }
        static void Main(string[] args) // main program 
        {
            List<Person> personList = new List<Person>();
            List<Visitor> visitorList = new List<Visitor>();
            List<BusinessLocation> bizList = new List<BusinessLocation>();
            foreach (string[] data in FiletoList("Person.csv")) //convert Person.csv into list of string[] and process each item
            {

                if (data[0] == "resident")
                {
                    Resident resident = new Resident(data[1], data[2], Convert.ToDateTime(data[3])); //create new Resident object from string[]
                    if (data[6] != "")
                    {
                        resident.Token.SerialNo = data[6];
                        resident.Token.CollectionLocation = data[7];
                        resident.Token.ExpiryDate = Convert.ToDateTime(data[8]);

                    }
                    personList.Add(resident); //Populate list
                }
                else if (data[0] == "visitor")
                {
                    Visitor visitor = new Visitor(data[1], data[4], data[5]); //create new Visitor object from string[]
                    personList.Add(visitor); //Populate list
                    visitorList.Add(visitor);
                }
            }
            foreach (string[] data in FiletoList("BusinessLocation.csv")) //convert BusinessLocation.csv into list of string[] and process each item
            {
                BusinessLocation bizLocation = new BusinessLocation(data[0], data[1], Convert.ToInt32(data[2])); //create new BusinessLocation object from string[]
                bizList.Add(bizLocation); //Populate list
            }
            List<SHNFacility> SHNList = SHNAPI();
            int option = ValidOption();
            while (option != 0)
            {
                if (option == 1)
                {
                    DisplayVisitors(visitorList);
                }
                else if (option == 2)
                {
                    DisplayPersonDetails(personList);
                }
                else if (option == 3)
                {
                    AssignToken(personList);
                }
                else if (option == 4)
                {
                    DisplayBusinessLocation(bizList);
                }
                else if (option == 5)
                {
                    EditBusinessLocation(bizList);
                }
                else if (option == 6)
                {
                    CheckIn(personList, bizList);
                }
                else if (option == 7)
                {
                    CheckOut(personList);
                }
                else if (option == 8)
                {
                    DisplaySHNFacilities(SHNList);
                }
                else if (option == 9)
                {
                    CreateVisitor(personList);
                }
                else if (option == 10)
                {
                    NewTravelEntry(personList, SHNList);
                }
                else if (option == 11)
                {
                    CalculateSHNCharges(personList);
                }
                else if (option == 12)
                {
                    ContactTracingReport(personList, bizList);
                }
                else if (option == 13)
                {
                    SHNStatusReport(personList);
                }
                else
                {
                    Console.WriteLine("Enter a number 0-13!");
                }
                option = ValidOption();
            }
            Console.WriteLine("Bye!");
        }
    }
}
