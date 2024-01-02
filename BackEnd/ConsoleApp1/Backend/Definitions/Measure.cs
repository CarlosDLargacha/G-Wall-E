
using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices; 
using System.Collections.Generic;

namespace G_Wall_E;
public class Measure : IComparable<Measure>
{
    public double Value { get; set; }
    public string Color { get; private set; }
    public string Name { get; private set; }
    public Point P1 { get; set; }
    public Point P2 { get; set; }

    public int CompareTo(Measure other)
    {
        if(other == null)
        {
            return -1;
        }

        return Value.CompareTo(other.Value);
    }

    public Measure(string color, string name, Point p1, Point p2)
    {
        Color = color;
        Name = name;
        P1 = p1;
        P2 = p2;

        double xDiff = P1.X - P2.X;
        double yDiff = P1.Y - P2.Y;
        Value = Math.Sqrt(Convert.ToDouble(xDiff * xDiff + yDiff * yDiff));
    }

    public double Execute()
    {
        return Value;
    }

    public static Measure operator +(Measure m1, Measure m2)
    {
        Measure measure = new Measure("","", new Point("","",0,0),new Point("","",0,m1.Execute() + m2.Execute()));
        return measure;
    }

    public static Measure operator -(Measure m1, Measure m2)
    {
        Measure measure = new Measure("","", new Point("","",0,0),new Point("","",0,m1.Execute() - m2.Execute()));
        return measure;
    }

    public static Measure operator *(Measure m1, Measure m2)
    {
        Measure measure = new Measure("","", new Point("","",0,0),new Point("","",0,m1.Execute() * m2.Execute()));
        return measure;
    }

    public static Measure operator /(Measure m1, Measure m2)
    {
        Measure measure = new Measure("","", new Point("","",0,0),new Point("","",0,m1.Execute() / m2.Execute()));
        return measure;
    }

    public static Measure operator +(double m1, Measure m2)
    {
        Measure measure = new Measure("","", new Point("","",0,0),new Point("","",0,m1 + m2.Execute()));
        return measure;
    }

    public static Measure operator -(double m1, Measure m2)
    {
        Measure measure = new Measure("","", new Point("","",0,0),new Point("","",0,m1 - m2.Execute()));
        return measure;
    }

    public static Measure operator *(double m1, Measure m2)
    {
        Measure measure = new Measure("","", new Point("","",0,0),new Point("","",0,Math.Abs(Convert.ToInt32(m1 * m2.Execute()))));
        return measure;
    }

    public static Measure operator /(double m1, Measure m2)
    {
        Measure measure = new Measure("","", new Point("","",0,0),new Point("","",0,m1 / m2.Execute()));
        return measure;
    }


}
