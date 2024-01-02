using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices; 
using System.Collections.Generic;

namespace G_Wall_E;

public class Point : IDrawable
{
    public string Color { get; private set; }
    public string Name { get; private set; }
    
    [JsonPropertyName("coordinateX")]
    public double X { get; set; }
    
    [JsonPropertyName("coordinateY")]
    public double Y { get; set; }
    public string Msg { get; set; }

    public Point(string name, string color)
    {
        Name = name;
        Color = color;
        X = new Random().Next() % 525;
        Y = new Random().Next() % 600;
    }
    
    public Point(string name, string color, double x, double y)
    {
        Name = name;
        Color = color;
        X = x;
        Y = y;
    }

    public DrawableProperties Export()
    {
        return new DrawableProperties {
            Name = Name,
            Color = Color,
            Type = "point",
            Msg = Msg,
            X = X,
            Y = Y
        };
    }
}
