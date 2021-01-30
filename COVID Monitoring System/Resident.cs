//============================================================
// Student Number : S10203242, S10206172
// Student Name : Russell de Silva, Ayken Lee Kang
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace COVID_Monitoring_System
{
    class Resident : Person
    {
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private DateTime lastLeftCountry;

        public DateTime LastLeftCountry
        {
            get { return lastLeftCountry; }
            set { lastLeftCountry = value; }
        }

        private TraceTogetherToken token;

        public TraceTogetherToken Token
        {
            get { return token; }
            set { token = value; }
        }


        public Resident() { }

        public Resident(string name, string a, DateTime l) : base(name)
        {
            Address = a;
            LastLeftCountry = l;
            Token = new TraceTogetherToken();
        }

        public override double CalculateSHNCharges()
        {
            double swabTest = 200;
            double transportation = 0;
            double SDF = 0;
            TravelEntry t = TravelEntryList[^1];
            int duration = (t.SHNEndDate - t.EntryDate).Days;
            if (duration == 7)
            {

                transportation = 20;
            }
            else if (duration == 14)
            {
                transportation = 20;
                SDF = 1000;
            }
            double totalCost = swabTest + transportation + SDF;
            return totalCost;
        }

        public override string ToString()
        {
            return base.ToString() +
                "Address: " + Address + "\tLast left country on: " + LastLeftCountry;
        }
    }
}
