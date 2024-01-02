using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices; 
using System.Collections.Generic;

namespace G_Wall_E;
public class Segment : IDrawable
{
    public string Color { get; private set; }
    public string Name { get; private set; }
    public Point P1 { get; set; }
    public Point P2 { get; set; }
    public string Msg { get; set; }

    public Segment(string name, string color)
    {
        Name = name;
        Color = color;
        P1 = new Point(name, color);
        P2 = new Point(name, color);
    }

    public Segment(string name, string color, Point p1, Point p2)
    {
        Name = name;
        Color = color;
        P1 = p1;
        P2 = p2;
    }

    public DrawableProperties Export()
    {
        return new DrawableProperties {
            Name = Name,
            Color = Color,
            Type = "segment",
            Msg = Msg,
            P1 = P1.Export(),
            P2 = P2.Export()
        };
    }
}
