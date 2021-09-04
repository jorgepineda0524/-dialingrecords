using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace dialingrecords.Functions.Entities
{
    public class MarkingEntity : TableEntity
    {
        public int IdEmpleyee { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public bool Consolidated { get; set; }
    }
}
