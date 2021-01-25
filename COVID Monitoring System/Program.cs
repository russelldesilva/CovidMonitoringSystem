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
            Console.WriteLine("======================================");
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
                }
            }
            return SHNList;
        }
        //Basic Feature 3
        static void DisplayVisitors(List<Visitor> visitorList)
        {
            foreach (Visitor visitor in visitorList)
            {
                Console.WriteLine(visitor.ToString());
            }
        }
        //Basic Feature 4
        static void DisplayPersonDetails(List<Person> personList)
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
                    }
                }

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
        //Basic Feature 5
        static void AssignToken(List<Person> personList)
        {
            Console.WriteLine("Enter Name: ");
            string name = Console.ReadLine();
            foreach (Person person in personList)
            {
                if (name == person.Name)
                {
                    if (person is Resident)

                    {
                        DateTime today = DateTime.Today;
                        Resident resident = (Resident)person;
                        if (resident.Token.SerialNo is null || today > resident.Token.ExpiryDate.AddMonths(-1))
                        {
                            string serialNo = GenerateSerialNo(personList);
                            resident.Token.SerialNo = serialNo;
                            resident.Token.CollectionLocation = "Kallang CC";

                            today = today.AddMonths(6);

                            resident.Token.ExpiryDate = today;
                        }

                    }

                }
            }
        }
        //Basic Feature 6
        static void DisplayBusinessLocation(List<BusinessLocation> bizList)
        {
            foreach (BusinessLocation businessLocation in bizList)
            {
                Console.WriteLine(businessLocation.ToString());
            }
        }
        //Basic Feature 7
        static void EditBusinessLocation(List<BusinessLocation> bizList)
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

        }
        //Basic Feature 8
        static void CheckIn(List<Person> personList, List<BusinessLocation> bizList)
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
                                DateTime today = DateTime.Today;
                                SafeEntry safeEntry = new SafeEntry(today, location);
                                person.AddSafeEntry(safeEntry);
                                location.VisitorsNow += 1;
                            }
                        }
                    }
                }
            }
        }
        //Basic Feature 9
        static void CheckOut(List<Person> personList)
        {
            Console.WriteLine("Enter Name: ");
            string name = Console.ReadLine();
            foreach (Person person in personList)
            {
                if (name == person.Name)
                {
                    foreach (SafeEntry safeEntry in person.SafeEntryList)
                    {
                        DateTime checkOut = safeEntry.CheckOut;
                        if (checkOut == DateTime.MinValue)
                        {
                            Console.WriteLine(safeEntry.ToString());
                        }

                    }
                    Console.WriteLine("Select Location to check out: ");
                    string location = Console.ReadLine();
                    foreach (SafeEntry safeEntry in person.SafeEntryList)
                    {
                        if (location == safeEntry.Location.BusinessName)
                        {
                            safeEntry.performCheckOut();
                        }
                    }
                }
            }
        }




        //Basic Feature 10
        static void DisplaySHNFacilities(List<SHNFacility> SHNList)//display SHN facilities
        {
            Console.WriteLine("{0,-15}{1,10}{2,30}{3,30}{4,30}", "Name", "Capacity", "Dist from Air checkpoint", "Dist from Sea checkpoint", "Dist from Land checkpoint");
            foreach (SHNFacility s in SHNList)
            {
                Console.WriteLine("{0,-15}{1,10}{2,30}{3,30}{4,30}", s.FacilityName, s.FacilityCapacity, s.DistFromAirCheckpoint, s.DistFromSeaCheckpoint, s.DistFromLandCheckpoint);
            }
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
            Console.WriteLine("New visitor created.");
        }
        //Basic Feature 12
        static void NewTravelEntry(List<Person> personList, List<SHNFacility> SHNList)
        {
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            foreach (Person p in personList)
            {
                if (p.Name == name)
                {
                    Console.Write("Enter last country of embarkation: ");
                    string lastCountry = Console.ReadLine();
                    Console.Write("Enter mode of entry: ");
                    string entryMode = Console.ReadLine();
                    TravelEntry newEntry = new TravelEntry(lastCountry, entryMode, DateTime.Now);
                    newEntry.CalculateSHNDuration();
                    if ((newEntry.SHNEndDate - DateTime.Now).Days + 1 == 14)
                    {
                        Console.WriteLine("You are required to do SHN at a SDF.\nPlease choose your preferred facility: ");
                        DisplaySHNFacilities(SHNList);
                        Console.Write("Enter name of facility: ");
                        string SHNname = Console.ReadLine();
                        foreach (SHNFacility s in SHNList)
                        {
                            if (s.FacilityName == SHNname)
                            {
                                if (s.IsAvailable())
                                {
                                    s.FacilityVacancy -= 1;
                                    newEntry.AssignSHNFacility(s);
                                }
                            }
                        }
                    }
                    else if ((newEntry.SHNEndDate - DateTime.Now).Days + 1 == 7)
                    {
                        Console.WriteLine("You are required to do SHN at your own accomodation.");
                    }
                    p.AddTravelEntry(newEntry);
                    Console.WriteLine("Travel entry added. Welcome to Singapore!");
                }
            }
        }
        //Basic feature 13
        static void CalculateSHNCharges(List<Person> personList)
        {
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            foreach (Person p in personList)
            {
                if (p.Name == name)
                {
                    foreach (TravelEntry t in p.TravelEntryList)
                    {
                        if ((t.SHNEndDate.CompareTo(DateTime.Now) >= 0) && (!t.IsPaid))
                        {
                            double SHNcharge = p.CalculateSHNCharges() * 1.07;
                            Console.WriteLine("Total Charge: ${0}", SHNcharge.ToString("0.00"));
                            t.IsPaid = true;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        //Advanced Feature 1
        static void ContactTracingReport(List<Person> personList)
        {
            Console.Write("Enter Buiness Location: ");
            string bizLocation = Console.ReadLine();
            Console.Write("Enter Date: ");
            string date = Console.ReadLine();
            Console.Write("Enter Start Time: ");
            string startTime = Console.ReadLine();
            Console.Write("Enter End Time: ");
            string endTime = Console.ReadLine();
            DateTime startDate = Convert.ToDateTime(date +  " " + startTime);
            DateTime endDate = Convert.ToDateTime(date +  " "  + endTime);
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
                                string[] contactTracingData = { person.Name, safeEntry.CheckIn.ToString(), safeEntry.CheckOut.ToString() };

                                foreach (string s in contactTracingData)
                                {
                                    sw.Write(s);
                                }
                            }

                        }
                    }
                }
            }
            
        }

        //Advanced Feature 2
        static void SHNStatusReport(List<Person> personList)
        {
            Console.Write("Enter date: ");
            DateTime date = Convert.ToDateTime(Console.ReadLine());
            using (StreamWriter sw = new StreamWriter("SHNStatus.csv", false))
            foreach (Person p in personList)
            {
                foreach (TravelEntry t in p.TravelEntryList)
                {
                    if (t.SHNEndDate.CompareTo(date) >= 0)
                    {
                            string[] SHNStatusData = { p.Name, t.SHNStay.FacilityName, t.SHNEndDate.ToString() };
                            foreach (string s in SHNStatusData)
                            {
                                sw.Write(s);
                            }
                    }
                }
            }
        }

        static void Main(string[] args)
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
            /*DisplaySHNFacilities(SHNList);
            CreateVisitor(personList);
            NewTravelEntry(personList, SHNList);
            CalculateSHNCharges(personList);
            DisplayVisitors(visitorList);
            DisplayPersonDetails(personList);
            AssignToken(personList);
            DisplayBusinessLocation(bizList);
            EditBusinessLocation(bizList);
            checkIn(personList, bizList);
            checkOut(personList);*/

            DisplayMenu();
            Console.Write("\nEnter option: ");
            int option = Convert.ToInt32(Console.ReadLine());
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
                    ContactTracingReport(personList);
                }
                else if (option == 13)
                {
                    SHNStatusReport(personList);
                }
                DisplayMenu();
                Console.Write("\nEnter option: ");
                option = Convert.ToInt32(Console.ReadLine());
            }
            Console.WriteLine("Bye!");
        }
    }
}
