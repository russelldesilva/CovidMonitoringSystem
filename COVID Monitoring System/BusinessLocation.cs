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
    class BusinessLocation
    {
		private string businessName;

		public string BusinessName
		{
			get { return businessName; }
			set { businessName = value; }
		}

		private string branchCode;

		public string BranchCode
		{
			get { return branchCode; }
			set { branchCode= value; }
		}

		private int maximumCapacity;

		public int MaximumCapacity
		{
			get { return maximumCapacity; }
			set { maximumCapacity = value; }
		}

		private int visitorsNow;

		public int VisitorsNow
		{
			get { return visitorsNow; }
			set { visitorsNow = value; }
		}

		public BusinessLocation() { }

		public BusinessLocation(string businessName, string branchCode, int maximumCapacity)
		{
			this.businessName = businessName;
			this.branchCode = branchCode;
			this.maximumCapacity = maximumCapacity;

		}

		public bool IsFull()
		{
			if(visitorsNow < maximumCapacity)
			{
				return false;
			}

			else
			{
				return true;
			}
		}

		public override string ToString()
		{
			return "Business Name: " + businessName + "Branch Code: " + branchCode + "Max Capacity: " + maximumCapacity + "Current number of visitors: " + visitorsNow;

		}




	}
}
