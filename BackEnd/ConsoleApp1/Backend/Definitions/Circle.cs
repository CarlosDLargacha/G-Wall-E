
using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices; 
using System.Collections.Generic;

namespace G_Wall_E;
public class Circle : IDrawable
{
    public string Color { get; private set; }
    public string Name { get; private set; }
    public Point P1 { get; set; }
    public Measure Radius { get; set; }
    public string Msg { get; set; }

    public Circle(string name, string color)
    {
        Name = name;
        Color = color;
        P1 = new Point(name, color);
        Point P2 = new Point(name, color);
        Radius = new Measure(color, "measure", P1, P2);
    }

    public Circle(string name, string color, Point p1, Measure radius)
    {
        Name = name;
        Color = color;
        P1 = p1;
        Radius = radius;
    }

    public DrawableProperties Export()
    {
        return new DrawableProperties {
            Name = Name,
            Color = Color,
            Type = "circle",
            Msg = Msg,
            P1 = P1.Export(),
            Radius = Radius.Execute()
        };
    }
}
