
using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices; 
using System.Collections.Generic;

namespace G_Wall_E;
public class Arc : IDrawable
{
    public string Color { get; private set; }
    public string Name { get; private set; }
    public Point P1 { get; set; }
    public Point P2 { get; set; }
    public Point P3 { get; set; }
    public Measure Distance { get; set; }
    public string Msg { get; set; }

    public Arc(string name, string color)
    {
        Color = color;
        Name = name;
        P1 = new Point(name, color);
        P2 = new Point(name, color);
        P3 = new Point(name, color);
        Distance = new Measure(color, name, P1, P3);
    } 

    public Arc(string name, string color, Point p1, Point p2, Point p3, Measure distance)
    {
        Color = color;
        Name = name;
        P1 = p1;
        P2 = p2;
        P3 = p3;
        Distance = distance;
    }       

    public DrawableProperties Export()
    {
        return new DrawableProperties {
            Name = Name,
            Color = Color,
            Type = "arc",
            Msg = Msg,
            P1 = P1.Export(),
            P2 = P2.Export(),
            P3 = P3.Export(),
            Radius = Distance.Execute()
        };
    }
}
