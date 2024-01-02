using G_Wall_E;
using GeoWall_E;
using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;

public partial class WGraphEdit : GraphEdit
{
    List<IDrawable> list = new List<IDrawable>();
    List<Measure> measures = new List<Measure>();
    public void Create()
    {
        HSplitContainer ParentNode = (HSplitContainer)GetParent();

        VSplitContainer SiblingNode = (VSplitContainer)ParentNode.GetChild(0);

        GraphEdit map = (GraphEdit)ParentNode.GetChild(1);

        int ChildCount = map.GetChildCount();

        for (int i = ChildCount - 1; i >= 0; i--) { var child = map.GetChild(i); child.Free(); }

        //Se invoca el nodo en donde se va a introducir el código
        CodeEdit inputBox = (CodeEdit)SiblingNode.GetChild(0);

        string input = inputBox.Text;

        List<IDrawable> newlist = Link.Start(input);

        for (int index = 0; index < newlist.Count; index++)
        {
            if (newlist[index].Name != "_;") { newlist[index] = IsAlredyDrawed(newlist[index]); }

            //Se verifica el objeto es un punto
            if (newlist[index].GetType() == typeof(G_Wall_E.Point))
            {
                G_Wall_E.Point point = (G_Wall_E.Point)newlist[index];
                Vector2 vector = new Vector2();
                vector.X = (float)point.X; vector.Y = (float)point.Y;

                //Se obtiene el color del punto
                Godot.Color color = GetColor(newlist[index].Color);

                //Se carga el nuevo punto
                var newPoint = ResourceLoader.Load<PackedScene>("res://point.tscn").Instantiate();

                //Se añade el punto al mapa
                AddChild(newPoint);

                string name = index.ToString();

                newPoint.Name = index.ToString();

                WPoint Child = (WPoint)GetNode(name);

                //Se asigna la posición del punto
                Child.PositionOffset = vector;

                //Se aigna el color del punto
                Child.Color = color;

                if (point.Msg != null) { Child.SelfModulate = new Godot.Color(1, 1, 1, 1); Child.Title = point.Msg; }

                continue;
            }

            //Se verifica si el objeto es un segmento
            if (newlist[index].GetType() == typeof(G_Wall_E.Segment))
            {
                G_Wall_E.Segment segment = (G_Wall_E.Segment)newlist[index];

                foreach (var c in list)
                {
                    if (c.GetType() == typeof(G_Wall_E.Point))
                    {
                        G_Wall_E.Point temp = (G_Wall_E.Point)c;
                        if(segment.P1.Name == temp.Name) { segment.P1 = temp; }
                        if(segment.P2.Name == temp.Name) { segment.P2 = temp; }
                    }
                }

                G_Wall_E.Point point1 = segment.P1;
                G_Wall_E.Point point2 = segment.P2;

                //Se crean los puntos del segmento
                Vector2 vector1 = new Vector2((float)point1.X, (float)point1.Y);
                Vector2 vector2 = new Vector2((float)point2.X, (float)point2.Y);

                //Se obtiene el color de la recta
                Godot.Color color = GetColor(segment.Color);

                //Se carga el nuevo segmento
                WLine newSegment = (WLine)ResourceLoader.Load<PackedScene>("res://Line.tscn").Instantiate();

                newSegment.Color = color;
                newSegment.PositionOffset = new Vector2((vector1.X + vector2.X) / 2, (vector1.Y + vector2.Y) / 2);
                newSegment.Point1 = vector1 - newSegment.PositionOffset;
                newSegment.Point2 = vector2 - newSegment.PositionOffset;

                //Se añade la recta al mapa
                AddChild(newSegment);

                if (segment.Msg != null) { newSegment.SelfModulate = new Godot.Color(1, 1, 1, 1); newSegment.Title = segment.Msg; }

                continue;
            }

            //Se verifica si el objeto es una recta
            if (newlist[index].GetType() == typeof(G_Wall_E.Line))
            {
                G_Wall_E.Line line = (G_Wall_E.Line)newlist[index];

                foreach (var c in list)
                {
                    if (c.GetType() == typeof(G_Wall_E.Point))
                    {
                        G_Wall_E.Point temp = (G_Wall_E.Point)c;
                        if (line.P1.Name == temp.Name) { line.P1 = temp; }
                        if (line.P2.Name == temp.Name) { line.P2 = temp; }
                    }
                }

                G_Wall_E.Point point1 = line.P1;
                G_Wall_E.Point point2 = line.P2;

                //Se crean los puntos por donde pasará la recta
                Vector2 vector1 = new Vector2((float)point1.X, (float)point1.Y);
                Vector2 vector2 = new Vector2((float)point2.X, (float)point2.Y);

                //Se obtiene el color de la recta
                Godot.Color color = GetColor(line.Color);

                //Se carga el nuevo segmento
                WLine newline = (WLine)ResourceLoader.Load<PackedScene>("res://Line.tscn").Instantiate();

                newline.Color = color;
                newline.PositionOffset = new Vector2((vector1.X + vector2.X) / 2, (vector1.Y + vector2.Y) / 2);
                Vector2 direction = vector2 - vector1;
                vector1 = new Vector2(vector1.X + (direction.X * 1000), vector1.Y + (direction.Y * 1000));
                vector2 = new Vector2(vector2.X + (direction.X * (-1000)), vector2.Y + (direction.Y * (-1000)));
                newline.Point1 = vector1 - newline.PositionOffset;
                newline.Point2 = vector2 - newline.PositionOffset;

                //Se añade la recta al mapa
                AddChild(newline);

                if (line.Msg != null) { newline.SelfModulate = new Godot.Color(1, 1, 1, 1); newline.Title = line.Msg; }

                continue;
            }

            //Se verifica si el objeto es una semirecta
            if (newlist[index].GetType() == typeof(G_Wall_E.Ray))
            {
                G_Wall_E.Ray ray = (G_Wall_E.Ray)newlist[index];

                foreach (var c in list)
                {
                    if (c.GetType() == typeof(G_Wall_E.Point))
                    {
                        G_Wall_E.Point temp = (G_Wall_E.Point)c;
                        if (ray.P1.Name == temp.Name) { ray.P1 = temp; }
                        if (ray.P2.Name == temp.Name) { ray.P2 = temp; }
                    }
                }

                G_Wall_E.Point point1 = ray.P1;
                G_Wall_E.Point point2 = ray.P2;

                //Se crean los puntos por donde pasará la semirecta
                Vector2 vector1 = new Vector2((float)point1.X, (float)point1.Y);
                Vector2 vector2 = new Vector2((float)point2.X, (float)point2.Y);

                //Se obtiene el color de la semirecta
                Godot.Color color = GetColor(ray.Color);

                //Se carga la nueva semirecta
                WLine newRay = (WLine)ResourceLoader.Load<PackedScene>("res://Line.tscn").Instantiate();

                newRay.Color = color;
                newRay.PositionOffset = new Vector2((vector1.X + vector2.X) / 2, (vector1.Y + vector2.Y) / 2);
                Vector2 direction = vector2 - vector1;
                vector2 = new Vector2(vector2.X + (direction.X * 1000), vector2.Y + (direction.Y * 1000));
                newRay.Point1 = vector1 - newRay.PositionOffset;
                newRay.Point2 = vector2 - newRay.PositionOffset;

                //Se añade la semirecta al mapa
                AddChild(newRay);

                if (ray.Msg != null) { newRay.SelfModulate = new Godot.Color(1, 1, 1, 1); newRay.Title = ray.Msg; }

                continue;
            }

            //Se verifica si el objeto es una circunferencia
            if (newlist[index].GetType() == typeof(G_Wall_E.Circle))
            {
                G_Wall_E.Circle circle = (G_Wall_E.Circle)newlist[index];

                foreach (var c in list)
                {
                    if (c.GetType() == typeof(G_Wall_E.Point))
                    {
                        if (circle.P1.Name == c.Name) { circle.P1 = (G_Wall_E.Point)c; }
                        if (circle.Radius.P1.Name == c.Name) { circle.Radius.P1 = (G_Wall_E.Point)c; }
                        if (circle.Radius.P2.Name == c.Name) { circle.Radius.P2 = (G_Wall_E.Point)c; }
                    }
                }

                for (int i = 0; i <= newlist.Count; i++)
                {
                    if (i == newlist.Count) { newlist.Add(circle.P1); }
                    if (circle.P1.Name == newlist[i].Name) { break; }
                }

                G_Wall_E.Point center = circle.P1;

                double x = (circle.Radius.P1.X - circle.Radius.P2.X) * (circle.Radius.P1.X - circle.Radius.P2.X);
                double y = (circle.Radius.P1.Y - circle.Radius.P2.Y) * (circle.Radius.P1.Y - circle.Radius.P2.Y);

                //Se crea el centro y el radio de la circunferencia
                Vector2 Center = new Vector2((float)center.X, (float)center.Y);
                float Radius = (float)(Math.Sqrt(x + y));

                //Se obtiene el color de la circunferencia
                Godot.Color color = GetColor(circle.Color);

                //Se carga la nueva circunferencia
                Circunference newCircle = (Circunference)ResourceLoader.Load<PackedScene>("res://Circunference.tscn").Instantiate();

                newCircle.Radius = Radius;
                newCircle.Color = color;
                newCircle.Center = Center;

                //Se añade la nueva circunferencia al mapa
                AddChild(newCircle);

                if (circle.Msg != null) { newCircle.SelfModulate = new Godot.Color(1, 1, 1, 1); newCircle.Title = circle.Msg; }

                measures.Add(circle.Radius);

                continue;
            }

            //Se verifica si el objeto es un arco
            if (newlist[index].GetType() == typeof(G_Wall_E.Arc))
            {
                G_Wall_E.Arc arc = (G_Wall_E.Arc)newlist[index];
                foreach (var c in list)
                {
                    if (c.GetType() == typeof(G_Wall_E.Point))
                    {
                        if (arc.P1.Name == c.Name) { arc.P1 = (G_Wall_E.Point)c; }
                        if (arc.P2.Name == c.Name) { arc.P2 = (G_Wall_E.Point)c; }
                        if (arc.P3.Name == c.Name) { arc.P3 = (G_Wall_E.Point)c; }
                        if (arc.Distance.P1.Name == c.Name) { arc.Distance.P1 = (G_Wall_E.Point)c; }
                        if (arc.Distance.P2.Name == c.Name) { arc.Distance.P2 = (G_Wall_E.Point)c; }
                    }
                }

                for (int i = 0; i <= newlist.Count; i++)
                {
                    if (i == newlist.Count) { newlist.Add(arc.P1); }
                    if (arc.P1.Name == newlist[i].Name) { break; }
                }

                for (int i = 0; i <= newlist.Count; i++)
                {
                    if (i == newlist.Count) { newlist.Add(arc.P2); }
                    if (arc.P2.Name == newlist[i].Name) { break; }
                }

                for (int i = 0; i <= newlist.Count; i++)
                {
                    if (i == newlist.Count) { newlist.Add(arc.P3); }
                    if (arc.P3.Name == newlist[i].Name) { break; }
                }

                //Se halla la distancia del arco
                double x = (arc.Distance.P1.X - arc.Distance.P2.X) * (arc.Distance.P1.X - arc.Distance.P2.X);
                double y = (arc.Distance.P1.Y - arc.Distance.P2.Y) * (arc.Distance.P1.Y - arc.Distance.P2.Y);
                float Distance = (float)(Math.Sqrt(x + y));

                //Se hallan los ángulos del arco
                Vector2 vectorStart = new Vector2((float)(arc.P2.X - arc.P1.X), (float)(arc.P2.Y - arc.P1.Y));
                Vector2 vectorEnd = new Vector2((float)(arc.P3.X - arc.P1.X), (float)(arc.P3.Y - arc.P1.Y));

                float starAngle = (float)Math.Atan(vectorStart.Y / vectorStart.X);
                float endAngle = (float)Math.Atan(vectorEnd.Y / vectorEnd.X);

                //Se obtiene el color del arco
                Godot.Color color = GetColor(arc.Color);

                //Se carga el nuevo arco
                WArc newArc = (WArc)ResourceLoader.Load<PackedScene>("res://Arc.tscn").Instantiate();

                newArc.PositionOffset = new Vector2((float)arc.P2.X, (float)arc.P2.Y);

                //Se crea el centro del arco
                Vector2 Center = new Vector2((float)(arc.P1.X - arc.P2.X), (float)(arc.P1.Y - arc.P2.Y));

                //Se le asignan los valores a las propiedades del arco
                newArc.Center = Center;
                newArc.StartAngle = starAngle;
                newArc.EndAngle = endAngle;
                newArc.Distance = Distance;
                newArc.Color = color;

                //Se añadel el nuevo arco
                AddChild(newArc);

                if (arc.Msg != null) { newArc.SelfModulate = new Godot.Color(1, 1, 1, 1); newArc.Title = arc.Msg; }

                measures.Add(arc.Distance);

                continue;
            }
        }

        list.Clear();
        list = newlist;
        foreach(var x in list) { if(x.Name == "_;") { list.Remove(x); } }

        IDrawable IsAlredyDrawed(IDrawable figure)
        {
            foreach(var item in list) { if (item.Name == figure.Name) { item.Msg = figure.Msg; return item; } }
            return figure;
        }

        Godot.Color GetColor(string color)
        {
            if (color == "black") { return new Godot.Color(0, 0, 0, 1); }
            if (color == "red") { return new Godot.Color(1, 0, 0, 1); }
            if (color == "blue") { return new Godot.Color(0, 0, 1, 1); }
            if (color == "green") { return new Godot.Color(0, 1, 0, 1); }
            if (color == "magenta") { return new Godot.Color(1, 0, 1, 1); }
            if (color == "cyan") { return new Godot.Color(0, 1, 1, 1); }
            if (color == "yellow") { return new Godot.Color(1, 1, 0, 1); }
            if (color == "gray") { return new Godot.Color(0.5f, 0.5f, 0.5f, 1); }
            return new Godot.Color(1, 1, 1, 1);
        }
    }

    public override void _Ready()
    {

    }
}
