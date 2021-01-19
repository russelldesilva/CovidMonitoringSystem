﻿//============================================================
// Student Number : S10203242, S10206172
// Student Name : Russell de Silva, Ayken Lee Kang
// Module Group : T04
//============================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace COVID_Monitoring_System
{
    class SafeEntry
    {
		private DateTime checkIn;

		public DateTime CheckIn
		{
			get { return checkIn; }
			set { checkIn = value; }
		}

		private DateTime checkOut;

		public DateTime CheckOut
		{
			get { return checkOut; }
			set { checkOut = value; }
		}

		private BusinessLocation location;

		public BusinessLocation Location 
		{
			get { return location; }
			set { location = value; }
		}


		public SafeEntry () { }

		public SafeEntry(DateTime checkIn, BusinessLocation location)
		{
			this.checkIn = checkIn;
			this.location = location;
		}

		public void performCheckOut()
		{
			checkOut = DateTime.Now;
		}

		public string ToString()
		{
			return ""
		}




	}
}
