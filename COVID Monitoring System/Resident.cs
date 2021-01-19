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
            
        }

        public override string ToString()
        {
            return base.ToString() + "\tAddress: " + Address + "\tLast left country on: " + LastLeftCountry + Token.ToString();
        }
    }
}
