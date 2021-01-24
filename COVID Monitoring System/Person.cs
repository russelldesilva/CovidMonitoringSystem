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
    abstract class Person
    {
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private List<SafeEntry> safeEntryList;

		public List<SafeEntry> SafeEntryList
		{
			get { return safeEntryList; }
			set { safeEntryList = value; }
		}

		private List<TravelEntry> travelEntryList;

		public List<TravelEntry> TravelEntryList
		{
			get { return travelEntryList; }
			set { travelEntryList = value; }
		}

		public Person() { }

		public Person(string name)
		{
			this.name = name;
			SafeEntryList = new List<SafeEntry>();
			TravelEntryList = new List<TravelEntry>();
		}

		public void AddTravelEntry(TravelEntry travelEntry)
		{
			TravelEntryList.Add(travelEntry);
		}

		public void AddSafeEntry(SafeEntry safeEntry)
		{
			SafeEntryList.Add(safeEntry);
		}

		public abstract double CalculateSHNCharges();

		public override string ToString()
		{
			Console.WriteLine("Name: " + Name);
			foreach (SafeEntry s in SafeEntryList)
			{
				Console.WriteLine(s.ToString());
			}
			foreach (TravelEntry t in TravelEntryList)
			{
				Console.WriteLine(t.ToString());
			}

			return "";
		}
	}
}
