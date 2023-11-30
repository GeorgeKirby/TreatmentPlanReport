using DVHPlot.ViewModels;
using Microsoft.Win32;
using OxyPlot.Wpf;
using PDFtoAria;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using TreatmentPlanReport.Views;
using VMS.TPS.Common.Model.API;

namespace TreatmentPlanReport.ViewModels
{
    public class MainViewModel
    {
        private Patient _patient;
        private User _user;

        public DelegateCommand PrintCommand { get; set; }
        public DelegateCommand ARIAPostCommand { get; set; }
        public MainViewModel(PatientViewModel patientViewModel, 
            PlanViewModel planViewModel, 
            FieldViewModel fieldViewModel, 
            DVHViewModel dVHViewModel, 
            DVHSelectionViewModel dVHSelectionViewModel,
            Patient patient,
            User user)
        {
            _patient = patient;
            _user = user;
            PatientViewModel = patientViewModel;
            PlanViewModel = planViewModel;
            FieldViewModel = fieldViewModel;
            DVHViewModel = dVHViewModel;
            DVHSelectionViewModel = dVHSelectionViewModel;
            PrintCommand = new DelegateCommand(ONPrint);
            ARIAPostCommand = new DelegateCommand(OnARIAPost);
        }

        private void OnARIAPost()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "PDF (*.pdf)|*.pdf";
            if (file.ShowDialog() == true)
            {
                var BinaryContent = File.ReadAllBytes(file.FileName);

                CustomInsertDocumentsParameter.PostDocumentData(_patient.Id, _user,
                    BinaryContent, "Plan Report",
                    new VMS.OIS.ARIALocal.WebServices.Document.Contracts.DocumentType { DocumentTypeDescription = "Consent" });
            }
        }

        private void ONPrint()
        {
            FlowDocument fd = new FlowDocument { FontSize = 12, FontFamily = new System.Windows.Media.FontFamily("Franklin Gothic") };
            fd.Blocks.Add(new Paragraph(new Run("Treatment Plan Report")));
            fd.Blocks.Add(new BlockUIContainer(new PatientView { DataContext = PatientViewModel.PatientInfo }));
            fd.Blocks.Add(new BlockUIContainer(new PlanView { DataContext = PlanViewModel.PlanInfo }));
            foreach (var field in FieldViewModel.Fields)
            {
                fd.Blocks.Add(new BlockUIContainer(new FieldDetailsView { DataContext = field }));
            }
            BitmapSource bmp = new PngExporter().ExportToBitmap(DVHViewModel.DVHPlotModel);
            fd.Blocks.Add(new BlockUIContainer(new System.Windows.Controls.Image
            {
                Source = bmp,
                Height = 600,
                Width = 725
            }));
            System.Windows.Controls.PrintDialog printer = new System.Windows.Controls.PrintDialog();
            //printer.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
            fd.PageHeight = 1056;
            fd.PageWidth = 816;
            fd.PagePadding = new System.Windows.Thickness(50);
            fd.ColumnGap = 0;
            fd.ColumnWidth = 816;
            IDocumentPaginatorSource source = fd;
            if (printer.ShowDialog() == true)
            {
                printer.PrintDocument(source.DocumentPaginator, "TreatmentPlanReport");
            }
        }

        public PatientViewModel PatientViewModel { get; }
        public PlanViewModel PlanViewModel { get; }
        public FieldViewModel FieldViewModel { get; }
        public DVHViewModel DVHViewModel { get; }
        public DVHSelectionViewModel DVHSelectionViewModel { get; }
    }
}
