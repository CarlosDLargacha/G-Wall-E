using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices; 
using System.Collections.Generic;

namespace G_Wall_E;

public interface IDrawable
{
    public string Msg { get; set; }
    public string Color { get; }
    public string Name { get; }

    public DrawableProperties Export();
}

public class DrawableProperties
    {
        public string Msg { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public double? X { get; set; }
        public double? Y { get; set; }
        public DrawableProperties P1 { get; set; }
        public DrawableProperties P2 { get; set; }
        public DrawableProperties P3 { get; set; }
        public double? Radius { get; set; }
    }