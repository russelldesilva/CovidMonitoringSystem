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

        public SHNFacility(string n, int c, int v, double a, double s, double l)
        {
            FacilityName = n;
            FacilityCapacity = c;
            FacilityVacancy = v;
            DistFromAirCheckpoint = a;
            DistFromSeaCheckpoint = s;
            DistFromLandCheckpoint = l;
        }
        
        /*public double CalculateTravelCost()
        {
            
        }*/

        public bool IsAvailable()
        {
            if (FacilityVacancy < FacilityCapacity)
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
            return "Facility Name: " + FacilityName + "\tFacility Capacity: " + FacilityCapacity + "\tFacility Vacancy: " + FacilityVacancy + "\tDistance from air checkpoint: " + DistFromAirCheckpoint
                + "\tDistance from sea checkpoint: " + DistFromSeaCheckpoint + "\tDistance from land checkpoint: " + DistFromLandCheckpoint;
        }
    }
}
