using DVHPlot.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreatmentPlanReport.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(PatientViewModel patientViewModel, PlanViewModel planViewModel, FieldViewModel fieldViewModel, DVHViewModel dVHViewModel, DVHSelectionViewModel dVHSelectionViewModel)
        {
            PatientViewModel = patientViewModel;
            PlanViewModel = planViewModel;
            FieldViewModel = fieldViewModel;
            DVHViewModel = dVHViewModel;
            DVHSelectionViewModel = dVHSelectionViewModel;
        }

        public PatientViewModel PatientViewModel { get; }
        public PlanViewModel PlanViewModel { get; }
        public FieldViewModel FieldViewModel { get; }
        public DVHViewModel DVHViewModel { get; }
        public DVHSelectionViewModel DVHSelectionViewModel { get; }
    }
}
