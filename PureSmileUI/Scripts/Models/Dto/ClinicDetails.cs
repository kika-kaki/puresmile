namespace PureSmileUI.Models.Dto
{
    public class ClinicDetails
    {
        public string ClinicName { get; set; }

        public int TreatmentId { get; set; }

        public string TreatmentName { get; set; }

        public string TreatmentDateTime { get; set; }

        public string BookingStatus { get; set; }

        public string CustomerName { get; set; }
        
        public string CustomerFeedback { get; set; }

        public decimal MoneySpent { get; set; }

        public decimal MoneyEarned { get; set; }
    }
}