using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace MultiDates.Fields
{
    public class MultiDateField : ContentField
    {
        public string Value
        {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value ?? String.Empty); }
        }

        public DateTime[] Dates { get; set; }

    }
}