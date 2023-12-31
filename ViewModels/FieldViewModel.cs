﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TreatmentPlanReport.Models;
using VMS.TPS.Common.Model.API;

namespace TreatmentPlanReport.ViewModels
{
    public class FieldViewModel : BindableBase
    {
        public ObservableCollection<FieldModel> Fields { get; set; }
        private FieldModel _selectedField;

        public FieldModel SelectedField
        {
            get { return _selectedField; }
            set { SetProperty(ref _selectedField, value); }
        }
        public FieldViewModel(PlanSetup planSetup)
        {
            Fields = new ObservableCollection<FieldModel>();
            GatherFieldInfo(planSetup);
        }

        private void GatherFieldInfo(PlanSetup planSetup)
        {
            foreach (var field in planSetup.Beams.Where(x => !x.IsSetupField).OrderBy(x => x.BeamNumber))
            {
                Fields.Add(new FieldModel
                {
                    FieldId = field.Id,
                    FieldName = field.Name,
                    Technique = field.Technique.Id,
                    Energy = field.EnergyModeDisplayName,
                    FieldX = (field.ControlPoints.Max(x => x.JawPositions.X2) - field.ControlPoints.Min(x => x.JawPositions.X1)) / 10.0,
                    FieldY = (field.ControlPoints.Max(x => x.JawPositions.Y2) - field.ControlPoints.Min(x => x.JawPositions.Y1)) / 10.0,
                    X1 = field.ControlPoints.Min(x => x.JawPositions.X1) / 10.0,
                    X2 = field.ControlPoints.Max(x => x.JawPositions.X2) / 10.0,
                    Y1 = field.ControlPoints.Min(x => x.JawPositions.Y1) / 10.0,
                    Y2 = field.ControlPoints.Max(x => x.JawPositions.Y2) / 10.0,
                    Machine = field.TreatmentUnit.Id,
                    Isocenter = GetIsocenter(planSetup, field),
                    MU = field.Meterset.Value,
                    MLCPlanType = field.MLCPlanType.ToString(),
                    SSD = Math.Round(field.SSD / 10.0, 2),//maybe for arcs average ssd would be the best.
                    DoseRate = field.DoseRate,
                    Gantry = GetGantry(planSetup, field),
                    Collimator = field.CollimatorAngleToUser(field.ControlPoints.First().CollimatorAngle),
                    CouchAngle = field.PatientSupportAngleToUser(field.ControlPoints.First().PatientSupportAngle),
                    ToleranceTable = field.ToleranceTableLabel,
                    DRR = BuildDRRImage(field)
                });
            }
        }

        private BitmapSource BuildDRRImage(Beam field)
        {
            if (field.ReferenceImage == null) { return null; }
            var drr = field.ReferenceImage;
            int[,] pixels = new int[drr.YSize, drr.XSize];
            drr.GetVoxels(0, pixels);//get image pixels out of ESAPI.
            int[] flat_pixels = new int[drr.YSize * drr.XSize];
            //lay out pixels into single array
            for (int i = 0; i < drr.YSize; i++)
            {
                for (int ii = 0; ii < drr.XSize; ii++)
                {
                    flat_pixels[i + drr.XSize * ii] = pixels[i, ii];
                }
            }
            //translate into byte array
            var drr_max = flat_pixels.Max();
            var drr_min = flat_pixels.Min();
            PixelFormat format = PixelFormats.Gray8;//low res image, but only 1 byte per pixel. 
            int stride = (drr.XSize * format.BitsPerPixel + 7) / 8;
            byte[] image_bytes = new byte[stride * drr.YSize];
            for (int i = 0; i < flat_pixels.Length; i++)
            {
                double value = flat_pixels[i];
                image_bytes[i] = Convert.ToByte(255 * ((value - drr_min) / (drr_max - drr_min)));
            }
            //build the bitmapsource.
            return BitmapSource.Create(drr.XSize, drr.YSize, 25.4 / drr.XRes, 25.4 / drr.YRes, format, null, image_bytes, stride);
        }

        private string GetGantry(PlanSetup plan, Beam field)
        {
            if (field.Technique.Id.Contains("ARC"))
            {
                return $"{field.ControlPoints.First().GantryAngle} {field.GantryDirection} {field.ControlPoints.Last().GantryAngle}";
            }
            return field.ControlPoints.First().GantryAngle.ToString();
        }

        private string GetIsocenter(PlanSetup plan, Beam field)
        {
            //var uo = plan.StructureSet.Image.UserOrigin;
            var iso = field.IsocenterPosition;
            var userIso = plan.StructureSet.Image.DicomToUser(iso, plan);
            return $"({userIso.x / 10.0:F1}, {userIso.y / 10.0:F1},{userIso.z / 10.0:F1}";
            //return $"({(iso.x - uo.x) / 10.0:F1}, {(iso.y - uo.y) / 10.0:F1}, {(iso.z - uo.z) / 10.0:F1})";
        }
    }
}
