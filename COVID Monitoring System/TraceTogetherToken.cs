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
    class TraceTogetherToken
    {
        private string serialNo;

        public string SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }

        private string collectionLocation;

        public string CollectionLocation
        {
            get { return collectionLocation; }
            set { collectionLocation = value; }
        }

        private DateTime expiryDate;

        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }

        public TraceTogetherToken() { }

        public TraceTogetherToken(string sn, string cl, DateTime e)
        {
            SerialNo = sn;
            CollectionLocation = cl;
            ExpiryDate = e;
        }

        public bool IsEligibleForReplacement()
        {
            if (DateTime.Now.AddMonths(1) > ExpiryDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ReplaceToken(string sn, string cl)
        {
            SerialNo = sn;
            CollectionLocation = cl;
        }
    }
}
