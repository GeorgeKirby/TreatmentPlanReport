using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreatmentPlanReport.Models;
using VMS.TPS.Common.Model.API;

namespace TreatmentPlanReport.ViewModels
{
    public class PatientViewModel
    {
        public PatientModel PatientInfo { get; set; }
        public PatientViewModel(Patient patient)
        {
            PatientInfo = new PatientModel
            {
                Id = patient.Id,
                Name = $"{patient.LastName}, {patient.FirstName}",
                DateOfBirth = patient.DateOfBirth == null ? "No DOB"
                : ((DateTime)patient.DateOfBirth).ToShortDateString(),
                Hospital = patient.Hospital.Id
            };
        }
    }
}
