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
        static List<string[]> FiletoList(string path)
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
        static List<SHNFacility> SHNAPI()
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
        static void DisplaySHNFacilities(List<SHNFacility> SHNList)
        {
            Console.WriteLine("{0,-15}{1,10}{2,30}{3,30}{4,30}", "Name", "Capacity", "Dist from Air checkpoint", "Dist from Sea checkpoint", "Dist from Land checkpoint");
            foreach (SHNFacility s in SHNList)
            {
                Console.WriteLine("{0,-15}{1,10}{2,30}{3,30}{4,30}", s.FacilityName, s.FacilityCapacity, s.DistFromAirCheckpoint, s.DistFromSeaCheckpoint, s.DistFromLandCheckpoint);
            }
        }
        static void Main(string[] args)
        {
            List<Resident> resList = new List<Resident>();
            List<Visitor> visList = new List<Visitor>();
            List<BusinessLocation> bizList = new List<BusinessLocation>();
            foreach (string[] data in FiletoList("Person.csv"))
            {
                if (data[0] == "resident")
                {
                    Resident resident = new Resident(data[1], data[2], Convert.ToDateTime(data[3]));
                    Console.WriteLine(resident.ToString());
                    resList.Add(resident);
                }
                else if (data[0] == "visitor")
                {
                    Visitor visitor = new Visitor(data[1], data[4], data[5]);
                    Console.WriteLine(visitor.ToString());
                    visList.Add(visitor);
                }
            }
            foreach (string[] data in FiletoList("BusinessLocation.csv"))
            {
                BusinessLocation bizLocation = new BusinessLocation(data[0], data[1], Convert.ToInt32(data[2]));
                Console.WriteLine(bizLocation.ToString());
                bizList.Add(bizLocation);
            }
            List<SHNFacility> SHNList = SHNAPI();
            DisplaySHNFacilities(SHNList);
        }
    }
}
