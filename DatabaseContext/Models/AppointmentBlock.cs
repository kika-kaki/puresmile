using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    public class AppointmentBlock
    {
        [Key]
        public int BlockId { get; set; }
        public int ClinicId { get; set; }
        public int TreatmentId { get; set; }
        public DateTime StartBlock { get; set; }
        public DateTime EndBlock { get; set; }
        public DateTime SelectedTime { get; set; }
        
        //public virtual Clinic Clinics { get; set; } 
    }

}