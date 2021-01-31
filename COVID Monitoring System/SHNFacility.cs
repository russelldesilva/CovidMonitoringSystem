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
    class SHNFacility
    {
        private string facilityName;

        public string FacilityName
        {
            get { return facilityName; }
            set { facilityName = value; }
        }

        private int facilityCapacity;

        public int FacilityCapacity
        {
            get { return facilityCapacity; }
            set { facilityCapacity = value; }
        }

        private int facilityVacancy;

        public int FacilityVacancy
        {
            get { return facilityVacancy; }
            set { facilityVacancy = value; }
        }

        private double distFromAirCheckpoint;

        public double DistFromAirCheckpoint
        {
            get { return distFromAirCheckpoint; }
            set { distFromAirCheckpoint = value; }
        }


        private double distfromSeaCheckpoint;

        public double DistFromSeaCheckpoint
        {
            get { return distfromSeaCheckpoint; }
            set { distfromSeaCheckpoint = value; }
        }

        private double distFromLandCheckpoint;

        public double DistFromLandCheckpoint
        {
            get { return distFromLandCheckpoint; }
            set { distFromLandCheckpoint = value; }
        }

        public SHNFacility() { }

        public SHNFacility(string n, int c, double a, double s, double l)
        {
            FacilityName = n;
            FacilityCapacity = c;
            FacilityVacancy = c;
            DistFromAirCheckpoint = a;
            DistFromSeaCheckpoint = s;
            DistFromLandCheckpoint = l;
        }
        
        public double CalculateTravelCost(string mode, DateTime entry)
        {
            double baseCost = 0;
            double surcharge = 1;
            TimeSpan entryTime = entry.TimeOfDay;
            if (mode == "Air")
            {
                baseCost = 50 + DistFromAirCheckpoint * 0.22;
            }
            else if (mode == "Sea")
            {
                baseCost = 50 + DistFromSeaCheckpoint * 0.22;
            }
            else if (mode == "Land")
            {
                baseCost = 50 + DistFromLandCheckpoint * 0.22;
            }
            if ((entryTime > Convert.ToDateTime("06:00").TimeOfDay && entryTime < Convert.ToDateTime("08:59").TimeOfDay) || (entryTime > Convert.ToDateTime("18:00").TimeOfDay && entryTime < Convert.ToDateTime("23:59").TimeOfDay))
            {
                surcharge += 0.25;
            }
            else if (entryTime > Convert.ToDateTime("00:00").TimeOfDay && entryTime < Convert.ToDateTime("05:59").TimeOfDay)
            {
                surcharge += 0.5;
            }
            double totalCost = baseCost * surcharge;
            return totalCost;
        }

        public bool IsAvailable()
        {
            if (FacilityVacancy > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
           return "\nFacility Name: " + FacilityName + "\nFacility Capacity: " + FacilityCapacity + "\nFacility Vacancy: " + FacilityVacancy + "\nDistance from air checkpoint: " + DistFromAirCheckpoint
                + "\nDistance from sea checkpoint: " + DistFromSeaCheckpoint + "\nDistance from land checkpoint: " + DistFromLandCheckpoint;
        }
    }
}
