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
    class TravelEntry
    {
        private string lastCountryOfEmbarkation;

        public string LastCountryOfEmbarkation
        {
            get { return lastCountryOfEmbarkation; }
            set { lastCountryOfEmbarkation = value; }
        }

        private string entryMode;

        public string EntryMode
        {
            get { return entryMode; }
            set { entryMode = value; }
        }

        private DateTime entryDate;

        public DateTime EntryDate
        {
            get { return entryDate; }
            set { entryDate = value; }
        }

        private DateTime shnEndDate;

        public DateTime SHNEndDate
        {
            get { return shnEndDate; }
            set { shnEndDate = value; }
        }

        private SHNFacility shnStay;

        public SHNFacility SHNStay
        {
            get { return shnStay; }
            set { shnStay = value; }
        }

        private bool isPaid;

        public bool IsPaid
        {
            get { return isPaid; }
            set { isPaid = value; }
        }


        public TravelEntry() { }

        public TravelEntry(string l, string m, DateTime entry)
        {
            LastCountryOfEmbarkation = l;
            EntryMode = m;
            EntryDate = entry;
            SHNEndDate = new DateTime();
            SHNStay = new SHNFacility();
            IsPaid = false;
        }

        public void AssignSHNFacility(SHNFacility s)
        { 
            SHNStay = s;
        }
        //Calculate SHNDuration
        public override string ToString()
        {
            return "Last Country of Embarkation: " + LastCountryOfEmbarkation + "\tEntry mode: " + EntryMode + "\tEntry Date: " + EntryDate + "" +
                "\tSHN End Date: " + SHNEndDate + "\tSHN Facility: " + SHNStay + "\tPaid: " + IsPaid;
        }
    }
}
