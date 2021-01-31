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
            Console.WriteLine("================ Menu ================");
            for (int i = 0; i < menu.Length; i++)
            {
                Console.WriteLine("[" + (i + 1) + "] " + menu[i]);
            }
            Console.WriteLine("[0] Quit the program");
            Console.WriteLine("======================================\n");
        }
        //Basic Feature 1
        static List<string[]> FiletoList(string path) //convert each line of file to string array; string path = name of file
        {
            List<string[]> DataList = new List<string[]>();
            using (StreamReader sr = new StreamReader(path))
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
        //Basic Feature 2
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
                    foreach (SHNFacility s in SHNList)
                    {
                        s.FacilityVacancy = s.FacilityCapacity;
                    }
                }
                return SHNList;
            }
        }
            //Basic Feature 3
            static void DisplayVisitors(List<Visitor> visitorList)
            {
                foreach (Visitor visitor in visitorList)
                {
                    Console.WriteLine(visitor.ToString());
                    Console.WriteLine();
                }
            }
            //Basic Feature 4
            static void DisplayPersonDetails(List<Person> personList)
            {
                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Person p = InitPerson(personList, name);
                while (p == null)
                {
                    Console.WriteLine("Name not found!\n");
                    Console.Write("Enter name: ");
                    name = Console.ReadLine();
                    p = InitPerson(personList, name);
                }
                Console.WriteLine(p.ToString());
                if (p is Resident)
                {
                    Resident r = (Resident)p;
                    if (r.Token.ExpiryDate != DateTime.MinValue)
                    {
                        Console.WriteLine(r.Token.ToString());
                    }
                }
                Console.WriteLine();
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
            //Basic Feature 5
            static void AssignToken(List<Person> personList)
            {
                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Person p = InitPerson(personList, name);
                while (p == null)
                {
                    Console.WriteLine("Name not found!\n");
                    Console.Write("Enter name: ");
                    name = Console.ReadLine();
                    p = InitPerson(personList, name);
                }
                if (p is Resident)
                {
                    DateTime today = DateTime.Today;
                    Resident resident = (Resident)p;
                    if (resident.Token.SerialNo is null | today.AddMonths(1) > resident.Token.ExpiryDate)
                    {
                        string serialNo = GenerateSerialNo(personList);
                        resident.Token.SerialNo = serialNo;
                        resident.Token.CollectionLocation = "Kallang CC";
                        today = today.AddMonths(6);
                        resident.Token.ExpiryDate = today;
                        Console.WriteLine("You have been assigned a token!\n");
                    }
                    else
                    {
                        Console.WriteLine("Your token expires on {0}. You are not eligible for a replacement.\n", resident.Token.ExpiryDate);
                    }
                }
                else
                {
                    Console.WriteLine("Visitors are not allowed to have tokens!\n");
                }
            }
            //Basic Feature 6
            static void DisplayBusinessLocation(List<BusinessLocation> bizList)
            {
                Console.WriteLine("{0,-20}{1,-15}{2,-15}{3,-15}", "Business Name", "Branch Code", "Max Capacity", "Visitors Now");
                foreach (BusinessLocation b in bizList)
                {
                    Console.WriteLine("{0,-20}{1,-15}{2,-15}{3,-15}", b.BusinessName, b.BranchCode, b.MaximumCapacity, b.VisitorsNow);
                }
                Console.WriteLine();
            }
            //Basic Feature 7
            static void EditBusinessLocation(List<BusinessLocation> bizList)
            {
                DisplayBusinessLocation(bizList);
                Console.Write("Enter Buiness Location: ");
                string bizName = Console.ReadLine();
                BusinessLocation biz = InitBussinessLocation(bizList, bizName);
                while (biz == null)
                {
                    Console.WriteLine("Business not found!\n");
                    DisplayBusinessLocation(bizList);
                    Console.Write("Enter Buiness Location: ");
                    bizName = Console.ReadLine();
                    biz = InitBussinessLocation(bizList, bizName);
                }
                Console.WriteLine("Edit Maximum Capacity: ");
                int maximumCapacity = Convert.ToInt32(Console.ReadLine());
                biz.MaximumCapacity = maximumCapacity;
                Console.WriteLine();
            }
            //Basic Feature 8
            static void CheckIn(List<Person> personList, List<BusinessLocation> bizList)
            {
                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Person p = InitPerson(personList, name);
                while (p == null)
                {
                    Console.WriteLine("Name not found!\n");
                    Console.Write("Enter name: ");
                    name = Console.ReadLine();
                    p = InitPerson(personList, name);
                }
                DisplayBusinessLocation(bizList);
                Console.Write("Enter Buiness Location: ");
                string bizName = Console.ReadLine();
                BusinessLocation biz = InitBussinessLocation(bizList, bizName);
                while (biz == null)
                {
                    Console.WriteLine("Business not found!\n");
                    DisplayBusinessLocation(bizList);
                    Console.Write("Enter Buiness Location: ");
                    bizName = Console.ReadLine();
                    biz = InitBussinessLocation(bizList, bizName);
                }
                if (!biz.IsFull())
                {
                    bool checkIn = false;
                    foreach (SafeEntry s in p.SafeEntryList)
                    {
                        if (s.Location.BusinessName == biz.BusinessName && s.CheckOut == DateTime.MinValue)
                        {
                            Console.WriteLine("You are already checked in.");
                            checkIn = true;
                            break;
                        }
                    }
                    if (!checkIn)
                    {
                        DateTime today = DateTime.Now;
                        SafeEntry safeEntry = new SafeEntry(today, biz);
                        p.AddSafeEntry(safeEntry);
                        biz.VisitorsNow += 1;
                        Console.WriteLine("Successfully checked in.\n");
                    }
                }
                else
                {
                    Console.WriteLine("Business is full!\n");
                }
            }
            //Basic Feature 9
            static void CheckOut(List<Person> personList, List<BusinessLocation> bizList)
            {
                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Person p = InitPerson(personList, name);
                while (p == null)
                {
                    Console.WriteLine("Name not found!\n");
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
                DisplayBusinessLocation(bizList);
                Console.Write("Enter Buiness Location: ");
                string bizName = Console.ReadLine();
                BusinessLocation biz = InitBussinessLocation(bizList, bizName);
                while (biz == null)
                {
                    Console.WriteLine("Business not found!\n");
                    DisplayBusinessLocation(bizList);
                    Console.Write("Enter Buiness Location: ");
                    bizName = Console.ReadLine();
                    biz = InitBussinessLocation(bizList, bizName);
                }
                bool checkout = false;
                foreach (SafeEntry safeEntry in p.SafeEntryList)
                {
                    if (bizName == safeEntry.Location.BusinessName)
                    {
                        safeEntry.performCheckOut();
                        checkout = true;
                        Console.WriteLine("Successfully checked out.\n");
                        break;
                    }
                }
                if (checkout == false)
                {
                    Console.WriteLine("Unable to check out (You need to check in first).\n");
                }
            }
            //Basic Feature 10
            static void DisplaySHNFacilities(List<SHNFacility> SHNList)//display SHN facilities
            {
                Console.WriteLine("{0,-15}{1,10}{2,10}{3,25}{4,25}{5,25}", "Name", "Capacity", "Vacancy", "Dist from Air checkpoint", "Dist from Sea checkpoint", "Dist from Land checkpoint");
                foreach (SHNFacility s in SHNList)
                {
                    Console.WriteLine("{0,-15}{1,10}{2,10}{3,25}{4,25}{5,25}", s.FacilityName, s.FacilityCapacity, s.FacilityVacancy, s.DistFromAirCheckpoint, s.DistFromSeaCheckpoint, s.DistFromLandCheckpoint);
                }
                Console.WriteLine();
            }
            //Basic Feature 11
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
                Console.WriteLine("New visitor created.\n");
            }
            //Basic Feature 12
            static void NewTravelEntry(List<Person> personList, List<SHNFacility> SHNList)
            {
                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Person p = InitPerson(personList, name);
                while (p == null)
                {
                    Console.WriteLine("Name not found!\n");
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
                    Console.WriteLine("Invalid entry mode! (Land, Sea or Air only)\n");
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
                    while (s == null)
                    {
                        Console.WriteLine("Invalid facility name!\n");
                        DisplaySHNFacilities(SHNList);
                        Console.Write("Enter name of facility: ");
                        SHNname = Console.ReadLine();
                        s = InitSHNFacility(SHNList, SHNname);
                    }
                    if (s.IsAvailable())
                    {
                        newEntry.AssignSHNFacility(s);
                        p.AddTravelEntry(newEntry);
                        Console.WriteLine("Travel entry added. Welcome to Singapore!\n");
                    }
                    else
                    {
                        Console.WriteLine("Facility is full!");
                    }
                }
                else if ((newEntry.SHNEndDate - DateTime.Now).Days + 1 == 7)
                {
                    Console.WriteLine("You are required to do SHN at your own accomodation.");
                    p.AddTravelEntry(newEntry);
                    Console.WriteLine("Travel entry added. Welcome to Singapore!\n");
                }
            }
            //Basic feature 13
            static void CalculateSHNCharges(List<Person> personList)
            {

                bool entryFound = false;
                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Person p = InitPerson(personList, name);
                while (p == null)
                {
                    Console.WriteLine("Name not found!\n");
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
                        Console.WriteLine("Total Charge: ${0}\n", SHNcharge.ToString("0.00"));
                        t.IsPaid = true;
                        break;
                    }
                }
                if (!entryFound)
                {
                    Console.WriteLine("{0} has no unpaid travel entries!\n", name);
                }
            }
            //Advanced Feature 1
            static void ContactTracingReport(List<Person> personList, List<BusinessLocation> bizList)
            {
                Console.Write("Enter Buiness Location: ");
                string bizLocation = Console.ReadLine();
                while (InitBussinessLocation(bizList, bizLocation) == null)
                {
                    Console.WriteLine("Business not found!\n");
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
                Console.WriteLine("Report generated!\n");
            }

            //Advanced Feature 2
            static void SHNStatusReport(List<Person> personList)
            {
                DateTime parsedDate = ValidDate("date");
            using (StreamWriter sw = new StreamWriter("SHNStatus.csv", false))
            {
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
            Console.WriteLine("Report generated!\n");
            }
            static int ValidOption()
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
                        Console.WriteLine("Enter an integer (0-13)!\n");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid input!\n");
                    }
                }
                return option;
            }
            static DateTime ValidDate(string dateType)
            {
                Console.Write("Enter {0}: ", dateType);
                string date = Console.ReadLine();
                bool dateParsed = DateTime.TryParse(date, out DateTime parsedDate);
                while (!dateParsed)
                {
                    Console.WriteLine("Invalid {0}!", dateType);
                    Console.Write("Enter {0}: ", dateType);
                    date = Console.ReadLine();
                    dateParsed = DateTime.TryParse(date, out parsedDate);
                }
                return parsedDate;
            }

            static BusinessLocation InitBussinessLocation(List<BusinessLocation> bizList, string name)
            {
                foreach (BusinessLocation b in bizList)
                {
                    if (name == b.BusinessName)
                    {
                        return b;
                    }
                }
                return null;
            }
            static SHNFacility InitSHNFacility(List<SHNFacility> SHNList, string name)
            {
                foreach (SHNFacility s in SHNList)
                {
                    if (name == s.FacilityName)
                    {

                        return s;
                    }
                }
                return null;
            }
            static Person InitPerson(List<Person> personList, string name)
            {
                foreach (Person p in personList)
                {
                    if (name == p.Name)
                    {

                        return p;
                    }
                }
                return null;
            }
            static void Main(string[] args)
            {
                List<Person> personList = new List<Person>();
                List<Visitor> visitorList = new List<Visitor>();
                List<BusinessLocation> bizList = new List<BusinessLocation>();
                List<SHNFacility> SHNList = SHNAPI();
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
                        if (data[9] != "")
                        {
                            TravelEntry t = new TravelEntry(data[9], data[10], Convert.ToDateTime(data[11]))
                            {
                                SHNEndDate = Convert.ToDateTime(data[12]),
                                IsPaid = Convert.ToBoolean(data[13])
                            };
                            if (data[14] != "")
                            {
                                t.AssignSHNFacility(InitSHNFacility(SHNList, data[14]));
                            }
                            resident.AddTravelEntry(t);
                        }
                        personList.Add(resident); //Populate list
                    }
                    else if (data[0] == "visitor")
                    {
                        Visitor visitor = new Visitor(data[1], data[4], data[5]); //create new Visitor object from string[]

                        if (data[9] != "")
                        {
                            TravelEntry t = new TravelEntry(data[9], data[10], Convert.ToDateTime(data[11]))
                            {
                                SHNEndDate = Convert.ToDateTime(data[12]),
                                IsPaid = Convert.ToBoolean(data[13])
                            };
                            if (data[14] != "")
                            {
                                t.AssignSHNFacility(InitSHNFacility(SHNList, data[14]));
                            }
                            visitor.AddTravelEntry(t);
                        }
                        personList.Add(visitor); //Populate list
                        visitorList.Add(visitor);
                    }
                }
                foreach (string[] data in FiletoList("BusinessLocation.csv")) //convert BusinessLocation.csv into list of string[] and process each item
                {
                    BusinessLocation bizLocation = new BusinessLocation(data[0], data[1], Convert.ToInt32(data[2])); //create new BusinessLocation object from string[]
                    bizList.Add(bizLocation); //Populate list
                }
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
                        CheckOut(personList, bizList);
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
