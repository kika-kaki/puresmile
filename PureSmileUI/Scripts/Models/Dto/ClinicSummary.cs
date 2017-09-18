using System;
using System.Collections.Generic;

namespace PureSmileUI.Models.Dto
{
    public class ClinicSummary
    {
        public string ClinicName { get; set; }

        public List<int> TreatmentIds { get; set; }

        public List<DateTime> TreatmentDateTimes { get; set; }

        public int TreatmentCount { get; set; }

        public int CustomersCount { get; set; }

        public decimal MoneySpent { get; set; }

        public decimal MoneyEarned { get; set; }
    }
}