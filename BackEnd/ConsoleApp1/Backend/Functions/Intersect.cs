using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System;
using System.Collections.Generic;

namespace G_Wall_E
{
    //aqui se guardan todos los metodos necesarios para hallar el intercepto entre dos figuras
    public static class Intersect_Methods
    {
        //PUNTOS
        //con punto
        public static PointSequence Intersect(Point p1, Point p2)
        {
            //si los puntos son iguales retorna cualquiera de los dos puntos
            if (p1.X == p2.X && p1.Y == p2.Y) return new PointSequence(new List<Point>() { p1 });

            //si no lo son retornar secuencia vacia
            else return new PointSequence(new List<Point>());
        }

        //con linea
        public static PointSequence Intersect(Point p, Line l)
        {
            //si intersecta, devuelve el punto
            if (Point_Line(p, l.P1, l.P2)) return new PointSequence(new List<Point>() { p });
            //si no intersecta, devuelve una secuencia vacia
            else return new PointSequence(new List<Point>());
        }

        //con segmento
        public static PointSequence Intersect(Point p, Segment s)
        {
            //hallar la acotacion del segmento en x
            var x_min = Math.Min(s.P1.X, s.P2.X);
            var x_max = Math.Max(s.P1.X, s.P2.X);
            //hallar la acotacion del segmento en y
            var y_min = Math.Min(s.P1.Y, s.P2.Y);
            var y_max = Math.Max(s.P1.Y, s.P2.Y);

            //si esta en la recta y en el rango del segmento es intercepto
            if (Point_Line(p, s.P1, s.P2) && Is_Acot(p.X, x_min, x_max) && Is_Acot(p.Y, y_min, y_max))
            {
                return new PointSequence(new List<Point>() { p });
            }
            //no es intercepto
            else return new PointSequence(new List<Point>());
        }

        //con rayo
        public static PointSequence Intersect(Point p, Ray r)
        {
            //si esta dentro de la recta
            if (Point_Line(p, r.P1, r.P2))
            {
                //si esta acotada, es intercepto
                if (Ray_Acot(r, p.X, p.Y)) return new PointSequence(new List<Point>() { p });
                //si no lo esta, no lo es
                else return new PointSequence(new List<Point>());
            }
            //no es intercepto desde un inicio
            else return new PointSequence(new List<Point>());
        }

        //con arco
        public static PointSequence Intersect(Point p, Arc a)
        {
            //hallando el radio de la circunferencia
            var radius = a.Distance.Execute();
            //hallar la acotacion del arco en x
            var x_min = Math.Min(a.P3.X, a.P2.X);
            var x_max = Math.Max(a.P3.X, a.P2.X);
            //hallar la acotacion del arco en y
            var y_min = Math.Min(a.P3.Y, a.P2.Y);
            var y_max = Math.Max(a.P3.Y, a.P2.Y);

            if (Point_Circle(p, a.P1, radius) && Is_Acot(p.X, x_min, x_max) && Is_Acot(p.Y, y_min, y_max))
            {
                return new PointSequence(new List<Point>() { p });
            }

            else return new PointSequence(new List<Point>());
        }

        //con circunferencia
        public static PointSequence Intersect(Point p, Circle c)
        {
            //hallar el radio
            var radius = c.Radius.Execute();
            //es intercepto
            if (Point_Circle(p, c.P1, radius)) return new PointSequence(new List<Point>() { p });
            //no es intercepto
            else return new PointSequence(new List<Point>());
        }

        //LINEAS
        //con linea
        public static PointSequence Intersect(Line l1, Line l2, string color)
        {
            //hallando ecuación de la recta
            var l1_m = (l1.P2.Y - l1.P1.Y) / (l1.P2.X - l1.P1.X);
            var l1_n = l1.P1.Y - l1_m * l1.P1.X;
            var l2_m = (l2.P2.Y - l2.P1.Y) / (l2.P2.X - l2.P1.X);
            var l2_n = l2.P1.Y - l2_m * l2.P1.X; ;

            //si tienen la misma pendiente son paralelas
            if (l1_m == l2_m)
            {
                //son exactamente iguales
                if (l1_n == l2_n) return new PointSequence(true);
                //no se interceptan
                else return new PointSequence(new List<Point>());
            }
            //se interceptan en un punto
            else
            {
                var xr = (l2_n - l1_n) / (l1_m - l2_m);
                var yr = l1_m * xr + l1_n;
                return new PointSequence(new List<Point>() { new Point("intersect", color, xr, yr) });
            }
        }

        //con segmento
        public static PointSequence Intersect(Line l, Segment s, string color)
        {
            //hallando la ecuacion de la recta
            var l_m = (l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X);
            var l_n = l.P1.Y - l_m * l.P1.X;
            var s_m = (s.P2.Y - s.P1.Y) / (s.P2.X - s.P1.X);
            var s_n = s.P1.Y - s_m * s.P1.X;

            //hallar la acotacion del segmento en x
            var x_min = Math.Min(s.P1.X, s.P2.X);
            var x_max = Math.Max(s.P1.X, s.P2.X);
            //hallar la acotacion del segmento en y
            var y_min = Math.Min(s.P1.Y, s.P2.Y);
            var y_max = Math.Max(s.P1.Y, s.P2.Y);

            //son paralelas
            if (l_m == s_m)
            {
                //son exactamente iguales
                if (l_n == s_n) return new PointSequence(true);
                //no se interceptan
                else return new PointSequence(new List<Point>());
            }
            //se interceptan en un punto
            else
            {
                var xr = (s_n - l_n) / (l_m - s_m);
                var yr = l_m * xr + l_n;

                //si esta acotada intercepta
                if (Is_Acot(xr, x_min, x_max) && Is_Acot(yr, y_min, y_max))
                {
                    return new PointSequence(new List<Point>() { new Point("intersect", color, xr, yr) });
                }
                //no esta acotada
                else return new PointSequence(new List<Point>());
            }
        }

        //con rayo
        public static PointSequence Intersect(Line l, Ray r, string color)
        {
            //hallando ecuacion de la recta
            var l_m = (l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X);
            var l_n = l.P1.Y - l_m * l.P1.X;
            var r_m = (r.P2.Y - r.P1.Y) / (r.P2.X - r.P1.X);
            var r_n = r.P1.Y - r_m * r.P1.X;

            //son paralelas
            if (l_m == r_m)
            {
                //son exactamente iguales
                if (l_n == r_n) return new PointSequence(true);
                //no se cortan
                else return new PointSequence(new List<Point>());
            }
            //se cortan
            else
            {
                var xr = (r_n - l_n) / (l_m - r_m);
                var yr = l_m * xr + l_n;

                //si esta acotada, es intercepto
                if (Ray_Acot(r, xr, yr)) return new PointSequence(new List<Point>() { new Point("intersect", color, xr, yr) });
                //si no lo esta, no lo es
                else return new PointSequence(new List<Point>());
            }
        }

        //con arco
        public static PointSequence Intersect(Line l, Arc a , string color)
        {
            var radius = a.Distance.Execute();
            List<Point> Result = new List<Point>();

            //hallar la acotacion del arco en x
            var x_min = Math.Min(a.P3.X, a.P2.X);
            var x_max = Math.Max(a.P3.X, a.P2.X);
            //hallar la acotacion del arco en y
            var y_min = Math.Min(a.P3.Y, a.P2.Y);
            var y_max = Math.Max(a.P3.Y, a.P2.Y);

            //Si la distancia del centro a la recta es mayor que el radio, no hay intersección
            if (Distancia_Punto_Recta(a.P1, l.P1, l.P2) > radius)
            {
                return new PointSequence(new List<Point>());
            }

            //Si la distancia del punto a la recta es igual o menor al radio, se intersectan en un solo punto o en dos
            else
            {
                //Si no podemos calcular la pendiente por la via trivial, hay que hacerlo de otra forma
                if (l.P2.X - l.P1.X == 0)
                {
                    double X = l.P1.X;
                    if (Is_Acot(X, x_min, x_max))
                    {
                        double Y = a.P1.Y + Math.Sqrt((radius * radius) - ((X - a.P1.X) * (X - a.P1.X)));

                        if (Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));

                        Y = a.P1.Y - Math.Sqrt((radius * radius) - ((X - a.P1.X) * (X - a.P1.X)));

                        if (Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));
                    }

                }
                else
                {
                    //Hallando m y n
                    double m = (l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X);
                    double n = l.P2.Y - (m * l.P2.X);
                    //Parametrizando
                    double A = 1 + (m * m);
                    double B = (2 * m * n) - (2 * a.P1.Y * m) - (2 * a.P1.X);
                    double C = (a.P1.X * a.P1.X) + (a.P1.Y * a.P1.Y) - (radius * radius) - (2 * n * a.P1.Y) + (n * n);
                    double Discriminante = (B * B) - (4 * A * C);
                    //Si el dicriminante es 0, tiene una sola intersección
                    if (Discriminante == 0)
                    {
                        double X = (-B) / (2 * A);
                        double Y = (m * X) + n;
                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max))
                        {
                            Result.Add(new Point("intersection", color, X, Y));
                        }
                    }
                    //Si no es 0, tiene 2 intersecciones
                    else
                    {
                        double X = ((-B) + Math.Sqrt(Discriminante)) / (2 * A);
                        double Y = (m * X) + n;

                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));

                        X = ((-B) - Math.Sqrt(Discriminante)) / (2 * A);
                        Y = (m * X) + n;

                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));
                    }
                }
            }
            return new PointSequence(Result);
        }

        //con circunferencia
        public static PointSequence Intersect(Line l, Circle c , string color)
        {
            var radius = c.Radius.Execute();
            List<Point> Result = new List<Point>();

            //Si la distancia del centro a la recta es mayor que el radio, no hay intersección
            if (Distancia_Punto_Recta(c.P1, l.P1, l.P2) > radius)
            {
                return new PointSequence(new List<Point>());
            }

            //Si la distancia del punto a la recta es igual o menor al radio, se intersectan en un solo punto o en dos
            else
            {
                //Si no podemos calcular la pendiente por la via trivial, hay que hacerlo de otra forma
                if (l.P2.X - l.P1.X == 0)
                {
                    double X = l.P1.X;
                    double Y = c.P1.Y + Math.Sqrt((radius * radius) - ((X - c.P1.X) * (X - c.P1.X)));
                    Result.Add(new Point("intersection", color, X, Y));
                    Y = c.P1.Y - Math.Sqrt((radius * radius) - ((X - c.P1.X) * (X - c.P1.X)));
                    Result.Add(new Point("intersection", color, X, Y));
                }
                else
                {
                    //Hallando m y n
                    double m = (l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X);
                    double n = l.P2.Y - (m * l.P2.X);
                    //Parametrizando
                    double A = 1 + (m * m);
                    double B = (2 * m * n) - (2 * c.P1.Y * m) - (2 * c.P1.X);
                    double C = (c.P1.X * c.P1.X) + (c.P1.Y * c.P1.Y) - (radius * radius) - (2 * n * c.P1.Y) + (n * n);
                    double Discriminante = (B * B) - (4 * A * C);
                    //Si el dicriminante es 0, tiene una sola intersección
                    if (Discriminante == 0)
                    {
                        double X = (-B) / (2 * A);
                        double Y = (m * X) + n;
                        Result.Add(new Point("intersection", color, X, Y));
                    }
                    //Si no es 0, tiene 2 intersecciones
                    else
                    {
                        double X = ((-B) + Math.Sqrt(Discriminante)) / (2 * A);
                        double Y = (m * X) + n;
                        Result.Add(new Point("intersection", color, X, Y));
                        X = ((-B) - Math.Sqrt(Discriminante)) / (2 * A);
                        Y = (m * X) + n;
                        Result.Add(new Point("intersection", color, X, Y));
                    }
                }
            }
            return new PointSequence(Result);
        }

        //SEGMENTOS
        //con segmento
        public static PointSequence Intersect(Segment s1, Segment s2 , string color)
        {
            //hallando la ecuacion de la recta
            var s1_m = (s1.P2.Y - s1.P1.Y) / (s1.P2.X - s1.P1.X);
            var s1_n = s1.P1.Y - s1_m * s1.P1.X;
            var s2_m = (s2.P2.Y - s2.P1.Y) / (s2.P2.X - s2.P1.X);
            var s2_n = s2.P1.Y - s2_m * s2.P1.X;

            //hallar la acotacion del segmento1 en x
            var x_min_1 = Math.Min(s1.P1.X, s1.P2.X);
            var x_max_1 = Math.Max(s1.P1.X, s1.P2.X);
            //hallar la acotacion del segmento1 en y
            var y_min_1 = Math.Min(s1.P1.Y, s1.P2.Y);
            var y_max_1 = Math.Max(s1.P1.Y, s1.P2.Y);

            //hallar la acotacion del segmento2 en x
            var x_min_2 = Math.Min(s2.P1.X, s2.P2.X);
            var x_max_2 = Math.Max(s2.P1.X, s2.P2.X);
            //hallar la acotacion del segmento2 en y
            var y_min_2 = Math.Min(s2.P1.Y, s2.P2.Y);
            var y_max_2 = Math.Max(s2.P1.Y, s2.P2.Y);

            //son paralelas
            if (s1_m == s2_m)
            {
                //son exactamente iguales
                if (s1_n == s2_n) return new PointSequence(true);
                //no se interceptan
                else return new PointSequence(new List<Point>());
            }
            //se interceptan
            else
            {
                var xr = (s2_n - s1_n) / (s1_m - s2_m);
                var yr = s1_m * xr + s1_n;

                //si esta acotada intercepta
                if (Is_Acot(xr, x_min_1, x_max_1) && Is_Acot(yr, y_min_1, y_max_1) && Is_Acot(xr, x_min_2, x_max_2) && Is_Acot(yr, y_min_2, y_max_2))
                {
                    return new PointSequence(new List<Point>() { new Point("intersect", color, xr, yr) });
                }
                //no esta acotada
                else return new PointSequence(new List<Point>());
            }
        }

        //con rayo
        public static PointSequence Intersect(Segment s, Ray r , string color)
        {
            //hallando la ecuacion de la recta
            var l_m = (s.P2.Y - s.P1.Y) / (s.P2.X - s.P1.X);
            var l_n = s.P1.Y - l_m * s.P1.X;
            var r_m = (r.P2.Y - r.P1.Y) / (r.P2.X - r.P1.X);
            var r_n = r.P1.Y - r_m * r.P1.X;

            //hallar la acotacion del segmento en x
            var x_min = Math.Min(s.P1.X, s.P2.X);
            var x_max = Math.Max(s.P1.X, s.P2.X);
            //hallar la acotacion del segmento en y
            var y_min = Math.Min(s.P1.Y, s.P2.Y);
            var y_max = Math.Max(s.P1.Y, s.P2.Y);

            //son paralelas
            if (l_m == r_m)
            {
                //son exactamente iguales
                if (l_n == r_n) return new PointSequence(true);
                //no se cortan
                else return new PointSequence(new List<Point>());
            }
            //se cortan
            else
            {
                var xr = (r_n - l_n) / (l_m - r_m);
                var yr = l_m * xr + l_n;

                //es intercepto
                if (Ray_Acot(r, xr, yr) && Is_Acot(xr, x_min, x_max) && Is_Acot(yr, y_min, y_max))
                {
                    return new PointSequence(new List<Point>() { new Point("intersect", color, xr, yr) });
                }
                //si no lo esta, no lo es
                else return new PointSequence(new List<Point>());
            }
        }

        //con arco
        public static PointSequence Intersect(Segment s, Arc a , string color)
        {
            var radius = a.Distance.Execute();
            List<Point> Result = new List<Point>();

            //hallar la acotacion del arco en x
            var x_min_1 = Math.Min(a.P3.X, a.P2.X);
            var x_max_1 = Math.Max(a.P3.X, a.P2.X);
            //hallar la acotacion del arco en y
            var y_min_1 = Math.Min(a.P3.Y, a.P2.Y);
            var y_max_1 = Math.Max(a.P3.Y, a.P2.Y);

            //hallar la acotacion del segmento en x
            var x_min_2 = Math.Min(s.P1.X, s.P2.X);
            var x_max_2 = Math.Max(s.P1.X, s.P2.X);
            //hallar la acotacion del segmento en y
            var y_min_2 = Math.Min(s.P1.Y, s.P2.Y);
            var y_max_2 = Math.Max(s.P1.Y, s.P2.Y);

            //Si la distancia del centro a la recta es mayor que el radio, no hay intersección
            if (Distancia_Punto_Recta(a.P1, s.P1, s.P2) > radius)
            {
                return new PointSequence(new List<Point>());
            }

            //Si la distancia del punto a la recta es igual o menor al radio, se intersectan en un solo punto o en dos
            else
            {
                //Si no podemos calcular la pendiente por la via trivial, hay que hacerlo de otra forma
                if (s.P2.X - s.P1.X == 0)
                {
                    double X = s.P1.X;
                    if (Is_Acot(X, x_min_1, x_max_1) && Is_Acot(X, x_min_2, x_max_2))
                    {
                        double Y = a.P1.Y + Math.Sqrt((radius * radius) - ((X - a.P1.X) * (X - a.P1.X)));

                        if (Is_Acot(Y, y_min_1, y_max_1) && Is_Acot(Y, y_min_2, y_max_2)) Result.Add(new Point("intersection", color, X, Y));

                        Y = a.P1.Y - Math.Sqrt((radius * radius) - ((X - a.P1.X) * (X - a.P1.X)));

                        if (Is_Acot(Y, y_min_1, y_max_1) && Is_Acot(Y, y_min_2, y_max_2)) Result.Add(new Point("intersection", color, X, Y));
                    }

                }
                else
                {
                    //Hallando m y n
                    double m = (s.P2.Y - s.P1.Y) / (s.P2.X - s.P1.X);
                    double n = s.P2.Y - (m * s.P2.X);
                    //Parametrizando
                    double A = 1 + (m * m);
                    double B = (2 * m * n) - (2 * a.P1.Y * m) - (2 * a.P1.X);
                    double C = (a.P1.X * a.P1.X) + (a.P1.Y * a.P1.Y) - (radius * radius) - (2 * n * a.P1.Y) + (n * n);
                    double Discriminante = (B * B) - (4 * A * C);
                    //Si el dicriminante es 0, tiene una sola intersección
                    if (Discriminante == 0)
                    {
                        double X = (-B) / (2 * A);
                        double Y = (m * X) + n;
                        if (Is_Acot(X, x_min_1, x_max_1) && Is_Acot(X, x_min_2, x_max_2) && Is_Acot(Y, y_min_1, y_max_1) && Is_Acot(Y, y_min_2, y_max_2))
                        {
                            Result.Add(new Point("intersection", color, X, Y));
                        }
                    }
                    //Si no es 0, tiene 2 intersecciones
                    else
                    {
                        double X = ((-B) + Math.Sqrt(Discriminante)) / (2 * A);
                        double Y = (m * X) + n;

                        if (Is_Acot(X, x_min_1, x_max_1) && Is_Acot(X, x_min_2, x_max_2) && Is_Acot(Y, y_min_1, y_max_1) && Is_Acot(Y, y_min_2, y_max_2)) Result.Add(new Point("intersection", color, X, Y));

                        X = ((-B) - Math.Sqrt(Discriminante)) / (2 * A);
                        Y = (m * X) + n;

                        if (Is_Acot(X, x_min_1, x_max_1) && Is_Acot(X, x_min_2, x_max_2) && Is_Acot(Y, y_min_1, y_max_1) && Is_Acot(Y, y_min_2, y_max_2)) Result.Add(new Point("intersection", color, X, Y));
                    }
                }
            }
            return new PointSequence(Result);
        }

        //con circunferencia
        public static PointSequence Intersect(Segment s, Circle c , string color)
        {
            var radius = c.Radius.Execute();
            List<Point> Result = new List<Point>();

            //hallar la acotacion del segmento1 en x
            var x_min = Math.Min(s.P1.X, s.P2.X);
            var x_max = Math.Max(s.P1.X, s.P2.X);
            //hallar la acotacion del segmento1 en y
            var y_min = Math.Min(s.P1.Y, s.P2.Y);
            var y_max = Math.Max(s.P1.Y, s.P2.Y);

            //Si la distancia del centro a la recta es mayor que el radio, no hay intersección
            if (Distancia_Punto_Recta(c.P1, s.P1, s.P2) > radius)
            {
                return new PointSequence(new List<Point>());
            }

            //Si la distancia del punto a la recta es igual o menor al radio, se intersectan en un solo punto o en dos
            else
            {
                //Si no podemos calcular la pendiente por la via trivial, hay que hacerlo de otra forma
                if (s.P2.X - s.P1.X == 0)
                {
                    double X = s.P1.X;
                    if (Is_Acot(X, x_min, x_max))
                    {
                        double Y = c.P1.Y + Math.Sqrt((radius * radius) - ((X - c.P1.X) * (X - c.P1.X)));

                        if (Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));

                        Y = c.P1.Y - Math.Sqrt((radius * radius) - ((X - c.P1.X) * (X - c.P1.X)));

                        if (Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));
                    }
                }
                else
                {
                    //Hallando m y n
                    double m = (s.P2.Y - s.P1.Y) / (s.P2.X - s.P1.X);
                    double n = s.P2.Y - (m * s.P2.X);
                    //Parametrizando
                    double A = 1 + (m * m);
                    double B = (2 * m * n) - (2 * c.P1.Y * m) - (2 * c.P1.X);
                    double C = (c.P1.X * c.P1.X) + (c.P1.Y * c.P1.Y) - (radius * radius) - (2 * n * c.P1.Y) + (n * n);
                    double Discriminante = (B * B) - (4 * A * C);
                    //Si el dicriminante es 0, tiene una sola intersección
                    if (Discriminante == 0)
                    {
                        double X = (-B) / (2 * A);
                        double Y = (m * X) + n;

                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));
                    }
                    //Si no es 0, tiene 2 intersecciones
                    else
                    {
                        double X = ((-B) + Math.Sqrt(Discriminante)) / (2 * A);
                        double Y = (m * X) + n;

                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));

                        X = ((-B) - Math.Sqrt(Discriminante)) / (2 * A);
                        Y = (m * X) + n;

                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max)) Result.Add(new Point("intersection", color, X, Y));
                    }
                }
            }
            return new PointSequence(Result);
        }

        //RAYOS
        //con rayo
        public static PointSequence Intersect(Ray r1, Ray r2 , string color)
        {
            //hallando la ecuacion de la recta
            var l_m = (r1.P2.Y - r1.P1.Y) / (r1.P2.X - r1.P1.X);
            var l_n = r1.P1.Y - l_m * r1.P1.X;
            var r_m = (r2.P2.Y - r2.P1.Y) / (r2.P2.X - r2.P1.X);
            var r_n = r2.P1.Y - r_m * r2.P1.X;

            //son paralelas
            if (l_m == r_m)
            {
                //son exactamente iguales
                if (l_n == r_n) return new PointSequence(true);
                //no se cortan
                else return new PointSequence(new List<Point>());
            }
            //se cortan
            else
            {
                var xr = (r_n - l_n) / (l_m - r_m);
                var yr = l_m * xr + l_n;

                //es intercepto
                if (Ray_Acot(r1, xr, yr) && Ray_Acot(r2, xr, yr))
                {
                    return new PointSequence(new List<Point>() { new Point("intersect", color, xr, yr) });
                }
                //si no lo esta, no lo es
                else return new PointSequence(new List<Point>());
            }
        }

        //con arco
        public static PointSequence Intersect(Ray p, Arc a , string color)
        {
            var radius = a.Distance.Execute();
            List<Point> Result = new List<Point>();

            //hallar la acotacion del arco en x
            var x_min = Math.Min(a.P3.X, a.P2.X);
            var x_max = Math.Max(a.P3.X, a.P2.X);
            //hallar la acotacion del arco en y
            var y_min = Math.Min(a.P3.Y, a.P2.Y);
            var y_max = Math.Max(a.P3.Y, a.P2.Y);

            //Si la distancia del centro a la recta es mayor que el radio, no hay intersección
            if (Distancia_Punto_Recta(a.P1, p.P1, p.P2) > radius)
            {
                return new PointSequence(new List<Point>());
            }

            //Si la distancia del punto a la recta es igual o menor al radio, se intersectan en un solo punto o en dos
            else
            {
                //Si no podemos calcular la pendiente por la via trivial, hay que hacerlo de otra forma
                if (p.P2.X - p.P1.X == 0)
                {
                    double X = p.P1.X;
                    if (Is_Acot(X, x_min, x_max))
                    {
                        double Y = a.P1.Y + Math.Sqrt((radius * radius) - ((X - a.P1.X) * (X - a.P1.X)));

                        if (Is_Acot(Y, y_min, y_max) && Ray_Acot(p, X, Y)) Result.Add(new Point("intersection", color, X, Y));

                        Y = a.P1.Y - Math.Sqrt((radius * radius) - ((X - a.P1.X) * (X - a.P1.X)));

                        if (Is_Acot(Y, y_min, y_max) && Ray_Acot(p, X, Y)) Result.Add(new Point("intersection", color, X, Y));
                    }

                }
                else
                {
                    //Hallando m y n
                    double m = (p.P2.Y - p.P1.Y) / (p.P2.X - p.P1.X);
                    double n = p.P2.Y - (m * p.P2.X);
                    //Parametrizando
                    double A = 1 + (m * m);
                    double B = (2 * m * n) - (2 * a.P1.Y * m) - (2 * a.P1.X);
                    double C = (a.P1.X * a.P1.X) + (a.P1.Y * a.P1.Y) - (radius * radius) - (2 * n * a.P1.Y) + (n * n);
                    double Discriminante = (B * B) - (4 * A * C);
                    //Si el dicriminante es 0, tiene una sola intersección
                    if (Discriminante == 0)
                    {
                        double X = (-B) / (2 * A);
                        double Y = (m * X) + n;
                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max) && Ray_Acot(p, X, Y))
                        {
                            Result.Add(new Point("intersection", color, X, Y));
                        }
                    }
                    //Si no es 0, tiene 2 intersecciones
                    else
                    {
                        double X = ((-B) + Math.Sqrt(Discriminante)) / (2 * A);
                        double Y = (m * X) + n;

                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max) && Ray_Acot(p, X, Y)) Result.Add(new Point("intersection", color, X, Y));

                        X = ((-B) - Math.Sqrt(Discriminante)) / (2 * A);
                        Y = (m * X) + n;

                        if (Is_Acot(X, x_min, x_max) && Is_Acot(Y, y_min, y_max) && Ray_Acot(p, X, Y)) Result.Add(new Point("intersection", color, X, Y));
                    }
                }
            }
            return new PointSequence(Result);
        }

        //con circunferencia
        public static PointSequence Intersect(Ray s, Circle c , string color)
        {
            var radius = c.Radius.Execute();
            List<Point> Result = new List<Point>();

            //Si la distancia del centro a la recta es mayor que el radio, no hay intersección
            if (Distancia_Punto_Recta(c.P1, s.P1, s.P2) > radius)
            {
                return new PointSequence(new List<Point>());
            }

            //Si la distancia del punto a la recta es igual o menor al radio, se intersectan en un solo punto o en dos
            else
            {
                //Si no podemos calcular la pendiente por la via trivial, hay que hacerlo de otra forma
                if (s.P2.X - s.P1.X == 0)
                {
                    double X = s.P1.X;

                    double Y = c.P1.Y + Math.Sqrt((radius * radius) - ((X - c.P1.X) * (X - c.P1.X)));

                    if (Ray_Acot(s, X, Y)) Result.Add(new Point("intersection", color, X, Y));

                    Y = c.P1.Y - Math.Sqrt((radius * radius) - ((X - c.P1.X) * (X - c.P1.X)));

                    if (Ray_Acot(s, X, Y)) Result.Add(new Point("intersection", color, X, Y));
                }
                else
                {
                    //Hallando m y n
                    double m = (s.P2.Y - s.P1.Y) / (s.P2.X - s.P1.X);
                    double n = s.P2.Y - (m * s.P2.X);
                    //Parametrizando
                    double A = 1 + (m * m);
                    double B = (2 * m * n) - (2 * c.P1.Y * m) - (2 * c.P1.X);
                    double C = (c.P1.X * c.P1.X) + (c.P1.Y * c.P1.Y) - (radius * radius) - (2 * n * c.P1.Y) + (n * n);
                    double Discriminante = (B * B) - (4 * A * C);
                    //Si el dicriminante es 0, tiene una sola intersección
                    if (Discriminante == 0)
                    {
                        double X = (-B) / (2 * A);
                        double Y = (m * X) + n;

                        if (Ray_Acot(s, X, Y)) Result.Add(new Point("intersection", color, X, Y));
                    }
                    //Si no es 0, tiene 2 intersecciones
                    else
                    {
                        double X = ((-B) + Math.Sqrt(Discriminante)) / (2 * A);
                        double Y = (m * X) + n;

                        if (Ray_Acot(s, X, Y)) Result.Add(new Point("intersection", color, X, Y));

                        X = ((-B) - Math.Sqrt(Discriminante)) / (2 * A);
                        Y = (m * X) + n;

                        if (Ray_Acot(s, X, Y)) Result.Add(new Point("intersection", color, X, Y));
                    }
                }
            }
            return new PointSequence(Result);
        }

        //ARCOS
        //con arco
        public static PointSequence Intersect(Arc c1, Arc c2 , string color)
        {
            //hallar la acotacion del arco1 en x
            var x_min_1 = Math.Min(c1.P3.X, c1.P2.X);
            var x_max_1 = Math.Max(c1.P3.X, c1.P2.X);
            //hallar la acotacion del arco1 en y
            var y_min_1 = Math.Min(c1.P3.Y, c1.P2.Y);
            var y_max_1 = Math.Max(c1.P3.Y, c1.P2.Y);

            //hallar la acotacion del arco2 en x
            var x_min_2 = Math.Min(c2.P3.X, c2.P2.X);
            var x_max_2 = Math.Max(c2.P3.X, c2.P2.X);
            //hallar la acotacion del arco2 en y
            var y_min_2 = Math.Min(c2.P3.Y, c2.P2.Y);
            var y_max_2 = Math.Max(c2.P3.Y, c2.P2.Y);

            var radius_1 = c1.Distance.Execute();
            var radius_2 = c2.Distance.Execute();

            var result = new List<Point>();

            //calculo la distancia entre los centros de las circunferencias
            var distance = Math.Sqrt(Math.Pow(c2.P1.X - c1.P1.X, 2) + Math.Pow(c2.P1.Y - c1.P1.Y, 2));

            //si la distancia es mayor o menor que las umas y restas de los radios de las circunferencias, no se cortan
            if (distance > radius_1 + radius_2 || distance < Math.Abs(radius_1 - radius_2))
            {
                return new PointSequence(result);
            }

            //si son tangentes, tienen un punto de interseccion
            else if (Math.Abs(distance - (radius_1 + radius_2)) < 1e-6)
            {
                var x = (c1.P1.X * radius_2 + c2.P1.X * radius_1) / (radius_1 + radius_2);
                var y = (c1.P1.Y * radius_2 + c2.P1.Y * radius_1) / (radius_1 + radius_2);

                if (Is_Acot(x, x_min_1, x_max_1) && Is_Acot(x, x_min_2, x_max_2) && Is_Acot(y, y_min_2, y_max_2) && Is_Acot(y, y_min_1, y_max_1))
                {
                    result.Add(new Point("intersect", color, x, y));
                }
                return new PointSequence(result);
            }

            //si son exactamente iguales
            else if (radius_1 == radius_2 && c1.P1.X == c2.P1.X && c1.P1.Y == c2.P1.Y)
            {
                return new PointSequence(true);
            }

            //se cortan en dos puntos
            else
            {
                var a = (radius_1 * radius_1 - radius_2 * radius_2 + distance * distance) / (2 * distance);
                var h = Math.Sqrt(radius_1 * radius_1 - a * a);

                var x = c1.P1.X + a * (c2.P1.X - c1.P1.X) / distance;
                var y = c1.P1.Y + a * (c2.P1.Y - c1.P1.Y) / distance;

                var v = (c2.P1.Y - c1.P1.Y) / distance;
                var w = (c2.P1.X - c1.P1.X) / distance;

                var X = x - h * v;
                var Y = y + h * w;

                if (Is_Acot(X, x_min_1, x_max_1) && Is_Acot(X, x_min_2, x_max_2) && Is_Acot(Y, y_min_2, y_max_2) && Is_Acot(Y, y_min_1, y_max_1))
                {
                    result.Add(new Point("intersect", color, X, Y));
                }

                X = x + h * v;
                Y = y - h * w;

                if (Is_Acot(X, x_min_1, x_max_1) && Is_Acot(X, x_min_2, x_max_2) && Is_Acot(Y, y_min_2, y_max_2) && Is_Acot(Y, y_min_1, y_max_1))
                {
                    result.Add(new Point("intersect", color, X, Y));
                }
                return new PointSequence(result);
            }
        }


        //con circunferencia
        public static PointSequence Intersect(Arc p, Circle c , string color)
        {
            //hallar la acotacion del arco en x
            var x_min = Math.Min(p.P3.X, p.P2.X);
            var x_max = Math.Max(p.P3.X, p.P2.X);
            //hallar la acotacion del arco en y
            var y_min = Math.Min(p.P3.Y, p.P2.Y);
            var y_max = Math.Max(p.P3.Y, p.P2.Y);

            var radius_1 = p.Distance.Execute();
            var radius_2 = c.Radius.Execute();

            var result = new List<Point>();

            //calculo la distancia entre los centros de las circunferencias
            var distance = Math.Sqrt(Math.Pow(c.P1.X - p.P1.X, 2) + Math.Pow(c.P1.Y - p.P1.Y, 2));

            //si la distancia es mayor o menor que las umas y restas de los radios de las circunferencias, no se cortan
            if (distance > radius_1 + radius_2 || distance < Math.Abs(radius_1 - radius_2))
            {
                return new PointSequence(result);
            }

            //si son tangentes, tienen un punto de interseccion
            else if (Math.Abs(distance - (radius_1 + radius_2)) < 1e-6)
            {
                var x = (p.P1.X * radius_2 + c.P1.X * radius_1) / (radius_1 + radius_2);
                var y = (p.P1.Y * radius_2 + c.P1.Y * radius_1) / (radius_1 + radius_2);

                if (Is_Acot(x, x_min, x_max) && Is_Acot(y, y_min, y_max))
                {
                    result.Add(new Point("intersect", color, x, y));
                }
                return new PointSequence(result);
            }

            //si son exactamente iguales
            else if (radius_1 == radius_2 && p.P1.X == c.P1.X && p.P1.Y == c.P1.Y)
            {
                return new PointSequence(true);
            }

            //se cortan en dos puntos
            else
            {
                var a = (radius_1 * radius_1 - radius_2 * radius_2 + distance * distance) / (2 * distance);
                var h = Math.Sqrt(radius_1 * radius_1 - a * a);

                var x = p.P1.X + a * (c.P1.X - p.P1.X) / distance;
                var y = p.P1.Y + a * (c.P1.Y - p.P1.Y) / distance;

                var v = (c.P1.Y - p.P1.Y) / distance;
                var w = (c.P1.X - p.P1.X) / distance;

                var X = x - h * v;
                var Y = y + h * w;

                if (Is_Acot(x, x_min, x_max) && Is_Acot(y, y_min, y_max))
                {
                    result.Add(new Point("intersect", color, X, Y));
                }

                X = x + h * v;
                Y = y - h * w;

                if (Is_Acot(x, x_min, x_max) && Is_Acot(y, y_min, y_max))
                {
                    result.Add(new Point("intersect", color, X, Y));
                }
                return new PointSequence(result);
            }
        }

        //CIRCUNFERENCIAS
        //con circunferencia
        public static PointSequence Intersect(Circle c1, Circle c2 , string color)
        {
            var radius_1 = c1.Radius.Execute();
            var radius_2 = c2.Radius.Execute();

            var result = new List<Point>();

            //calculo la distancia entre los centros de las circunferencias
            var distance = Math.Sqrt(Math.Pow(c2.P1.X - c1.P1.X, 2) + Math.Pow(c2.P1.Y - c1.P1.Y, 2));

            //si la distancia es mayor o menor que las umas y restas de los radios de las circunferencias, no se cortan
            if (distance > radius_1 + radius_2 || distance < Math.Abs(radius_1 - radius_2))
            {
                return new PointSequence(result);
            }

            //si son tangentes, tienen un punto de interseccion
            else if (Math.Abs(distance - (radius_1 + radius_2)) < 1e-6)
            {
                var x = (c1.P1.X * radius_2 + c2.P1.X * radius_1) / (radius_1 + radius_2);
                var y = (c1.P1.Y * radius_2 + c2.P1.Y * radius_1) / (radius_1 + radius_2);

                result.Add(new Point("intersect", color, x, y));
                return new PointSequence(result);
            }

            //si son exactamente iguales
            else if (radius_1 == radius_2 && c1.P1.X == c2.P1.X && c1.P1.Y == c2.P1.Y)
            {
                return new PointSequence(true);
            }

            //se cortan en dos puntos
            else
            {
                var a = (radius_1 * radius_1 - radius_2 * radius_2 + distance * distance) / (2 * distance);
                var h = Math.Sqrt(radius_1 * radius_1 - a * a);

                var x = c1.P1.X + a * (c2.P1.X - c1.P1.X) / distance;
                var y = c1.P1.Y + a * (c2.P1.Y - c1.P1.Y) / distance;

                var v = (c2.P1.Y - c1.P1.Y) / distance;
                var w = (c2.P1.X - c1.P1.X) / distance;

                result.Add(new Point("intersect", color, x - h * v, y + h * w));
                result.Add(new Point("intersect", color, x + h * v, y - h * w));

                return new PointSequence(result);
            }
        }


        //métodos auxiliares
        public static bool Point_Line(Point p, Point P1, Point P2) 
        {
            //hallando ecuación de la recta
            var m = (P2.Y - P1.Y) / (P2.X - P1.X);
            var n = P1.Y - m * P1.X;

            //si la igualdad coincide, devuelve p, sino retorna vacío
            if (p.Y == m * p.X + n) return true;
            return false;
        }
        public static bool Point_Circle(Point p, Point P1, double Radius)
        {
            //sustituyendo el punto en la ecuacion de la circunferencia
            if (Math.Pow(p.X - P1.X, 2) + Math.Pow(p.Y - P1.Y, 2) == Math.Pow(Radius, 2)) return true;
            //no es intercepto
            else return false;
        }
        public static bool Is_Acot(double a, double min, double max)
        {
            return a >= min && a <= max;
        }
        public static bool Ray_Acot(Ray r, double x, double y)
        {
            bool acot_x = false; //si esta en el rango de las x
            bool acot_y = false; //si esta en el rango de las y

            //si el rayo se desplaza hacia la izquierda 
            if (r.P1.X > r.P2.X && x <= r.P1.X) acot_x = true;
            //si el rayo se desplaza hacia la derecha
            else if (r.P1.X < r.P2.X && x >= r.P1.X) acot_x = true;
            //si el rayo se desplaza hacia abajo
            if (r.P1.Y > r.P2.Y && y <= r.P1.Y) acot_y = true;
            //si el rayo se desplaza hacia arriba
            else if (r.P1.Y < r.P2.Y && y >= r.P1.Y) acot_y = true;

            return acot_x && acot_y;
        }
        public static double Distancia_Punto_Recta(Point punto, Point recta_p1, Point recta_p2)
        {
            double distance;
            if (recta_p1.X == recta_p2.X)
            {
                distance = recta_p1.X - punto.X;
            }
            else if (recta_p1.Y == recta_p2.Y)
            {
                distance = recta_p1.Y - punto.Y;
            }
            else
            {
                //Calculando ecuación cartesiana
                double m = (recta_p2.Y - recta_p1.Y) / (recta_p2.X - recta_p1.X);
                double n = recta_p2.Y - (m * recta_p2.X);
                //Declarando parámetros
                double A = m;
                double B = -1;
                double C = n;
                //Calcuando distancia
                distance = ((A * punto.X) + (B * punto.Y) + C) / Math.Sqrt((A * A) + (B * B));
            }
            //Devolver el módulo de la distancia
            if (distance < 0) return -distance;
            return distance;
        }
    }
}