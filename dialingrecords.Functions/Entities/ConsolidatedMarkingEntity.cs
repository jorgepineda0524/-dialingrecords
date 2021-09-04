using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace dialingrecords.Functions.Entities
{
    public class ConsolidatedMarkingEntity : TableEntity
    {
        public int IdEmployee { get; set; }
        public DateTime Date { get; set; }
        public double MinutesWorked { get; set; }
    }
}
