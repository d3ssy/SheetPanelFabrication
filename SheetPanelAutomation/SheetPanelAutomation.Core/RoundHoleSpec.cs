﻿using Rhino.Geometry;
using SheetPanelAutomation.Core.Enums;
using SheetPanelAutomation.Core.Interfaces;

namespace SheetPanelAutomation.Core
{
    public class RoundHoleSpec : IHoleSpec
    {
        public HoleType HoleType { get; set; }
        public HoleProfile HoleProfile { get; set; }
        public Plane Location { get; set; }
        public double Diameter { get; set; }
    }
}