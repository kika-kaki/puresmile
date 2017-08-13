using System;
using System.Linq;
using DatabaseContext.Models;

namespace DatabaseContext.Managers
{
    public class AppointmentBlockManager
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        private const int BlockDurationInMinutes = 15;

        public bool CheckForBlocks(DateTimeOffset selectedStart, int clinicId, int treatmentId)
        {
            var time = selectedStart.DateTime;
            var block =_context.AppointmentBlocks.FirstOrDefault(x => x.ClinicId == clinicId && x.TreatmentId == treatmentId && x.SelectedTime == time );
            if (block==null)
            {
                return true;
            }
            if (block.EndBlock < DateTime.UtcNow)
            {
                _context.AppointmentBlocks.Remove(block);
                _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public void ClearBlockAsync(DateTimeOffset selectedStart, int treatmentId, int clinicId)
        {
            var block =_context.AppointmentBlocks.FirstOrDefault(x => x.ClinicId == clinicId && x.TreatmentId == treatmentId && x.SelectedTime == selectedStart);
            if (block !=null)
            {
                _context.AppointmentBlocks.Remove(block);
                _context.SaveChangesAsync();
            }
        }

        public void SetBlockAsync(DateTime selectedTime, int clinicId, int treatmentId)
        {

            AppointmentBlock block = new AppointmentBlock()
            {
                ClinicId = clinicId,
                TreatmentId = treatmentId,
                SelectedTime = selectedTime,
                StartBlock = DateTime.UtcNow,
                EndBlock = DateTime.UtcNow.AddMinutes(BlockDurationInMinutes)

            };
            _context.AppointmentBlocks.Add(block);
            _context.SaveChanges();

        }
    }
}