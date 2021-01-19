﻿//============================================================
// Student Number : S10203242, S10206172
// Student Name : Russell de Silva, Ayken Lee Kang
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace COVID_Monitoring_System
{
    class Visitor : Person
    {
		private string passportNo;

		public string PassportNo
		{
			get { return passportNo; }
			set { passportNo = value; }
		}

		private string nationality;

		public string Nationality
		{
			get { return nationality; }
			set { nationality = value; }
		}

		public Visitor(string name, string passportNo, string nationality) : base(name)
		{
			this.passportNo = passportNo;
			this.nationality = nationality;
		}

		public override double CalculateSHNCharges()
		{
			double swabTest = 200;
			double transportation = 0;
			double SDF = 0;
			foreach (TravelEntry t in TravelEntryList)
			{ 
				if ((t.SHNEndDate.CompareTo(DateTime.Now) >= 0) && (!t.IsPaid))
				{
					if ((t.SHNEndDate - t.EntryDate).Days <= 7)
					{
						transportation = 80;
					}
					else if ((t.SHNEndDate - t.EntryDate).Days == 14)
					{
						transportation = t.SHNStay.CalculateTravelCost(t.EntryMode, t.EntryDate);
						SDF = 2000;
					}
				}
			}
			double totalCost = swabTest + transportation + SDF;
			return totalCost;
		}

	}
}
