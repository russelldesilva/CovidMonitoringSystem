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
            List <SHNFacility> SHNList = new List<SHNFacility>();
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
        static void CreateVisitor(List<Visitor> visList)
        {
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            Console.Write("Enter passport No: ");
            string passNo = Console.ReadLine();
            Console.Write("Enter nationality: ");
            string nationality = Console.ReadLine();
            Visitor newVisitor = new Visitor(name, passNo, nationality);
            visList.Add(newVisitor);
            Console.WriteLine("New visitor created.");
        }
        static void Main(string[] args)
        {
            List<Resident> resList = new List<Resident>();
            List<Visitor> visList = new List<Visitor>();
            List<BusinessLocation> bizList = new List<BusinessLocation>();
            foreach (string[] data in FiletoList("Person.csv")) //convert Person.csv into list of string[] and process each item
            {
                if (data[0] == "resident") 
                {
                    Resident resident = new Resident(data[1], data[2], Convert.ToDateTime(data[3])); //create new Resident object from string[]
                    Console.WriteLine(resident.ToString());
                    resList.Add(resident); //Populate list
                }
                else if (data[0] == "visitor")
                {
                    Visitor visitor = new Visitor(data[1], data[4], data[5]); //create new Visitor object from string[]
                    Console.WriteLine(visitor.ToString());
                    visList.Add(visitor); //Populate list
                }
            }
            foreach (string[] data in FiletoList("BusinessLocation.csv")) //convert BusinessLocation.csv into list of string[] and process each item
            {
                BusinessLocation bizLocation = new BusinessLocation(data[0], data[1], Convert.ToInt32(data[2])); //create new BusinessLocation object from string[]
                Console.WriteLine(bizLocation.ToString());
                bizList.Add(bizLocation); //Populate list
            }
            List<SHNFacility> SHNList = SHNAPI();
            DisplaySHNFacilities(SHNList);
            CreateVisitor(visList);
        }
    }
}
