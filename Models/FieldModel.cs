﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TreatmentPlanReport.Models
{
    public class FieldModel
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string Technique { get; set; }
        public string Energy { get; set; }
        public double FieldX { get; set; }
        public double FieldY { get; set; }
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public string Machine { get; set; }
        public string Isocenter { get; set; }
        public double MU { get; set; }
        public string MLCPlanType { get; set; }
        public int NumControlPoints { get; set; }
        public double SSD { get; set; }
        public double EffectiveDepth { get; set; }
        public double DoseRate { get; set; }
        public string Gantry { get; set; }
        public double Collimator { get; set; }
        public string ToleranceTable { get; set; }
        public double CouchAngle { get; set; }
        public BitmapSource DRR { get; set; }
    }
}
