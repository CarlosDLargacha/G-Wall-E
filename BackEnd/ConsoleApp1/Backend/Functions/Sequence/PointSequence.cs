using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System;
using System.Collections.Generic;
using System.Linq;

namespace G_Wall_E
{
    public class PointSequence : ISequence<Point> //secuencia de puntos
    {
        public List<List<Point>> values { get; set; }
        public string name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count
        {
            get
            {
                int count = 0;
                foreach (var x in values) foreach (var y in x) count++;
                return count;
            }
        }
        public List<int> is_undefined_concat { get; set; }

        public PointSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<Point>>();
            is_undefined_concat = new List<int>();
        }
        public PointSequence(List<Point> points) //constructor para una secuencia
        {
            values = new List<List<Point>>() { points };
            is_undefined_concat = new List<int>();
        }
        public PointSequence(List<List<Point>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            is_undefined_concat = new List<int>();
        }
        public PointSequence(string name, string color, bool is_finite) //cobstructor para una secuencia aleatoria
        {
            this.name = name;
            values = new List<List<Point>>();
            if (is_finite)
            {
                var l = new List<Point>();
                int count = 0;
                while (count != 10)
                {
                    l.Add(new Point("V" + count.ToString(), color));
                    count++;
                }
                values.Add(l);
            }
            else is_infinite = true;
            is_undefined_concat = new List<int>();
        }
        public PointSequence Concat(PointSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<Point>> concat_values = new List<List<Point>>();
            PointSequence concat_sequence = new PointSequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<Point>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                //concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<Point>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                //concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }
        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;

                //si es una secuencia undefined, asignar valor undefined
                if (is_undefined)
                {
                    var_value = "undefined";
                    var_fam.Children.Add(new Node { Type = name, Value = var_value });
                    continue;
                }

                //si la secuencia esta vacia, retornar undefined a todas las variables, menos la ultima que sera secuencia vacia
                if (values.Count() == 1 && values[0].Count() == 0)
                {
                    //si es la ultima varible, se le asigna secuencia vacia
                    if (node.Children.Last() == child)
                    {
                        var_value = new PointSequence(new List<Point>());
                        //lo anado al nodo de familia de variables
                        var_fam.Children.Add(new Node { Type = name, Value = var_value });
                        continue;
                    }
                    //al resto de variables se les asigna valor undefined
                    var_value = "undefined";
                    var_fam.Children.Add(new Node { Type = name, Value = var_value });
                    continue;
                }

                //si no alcanzo secuencia para el valor de la variable y es la ultima de los hijos asignarle una secuencia vacua
                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new PointSequence(false);
                    //lo anado al nodo de familia de variables
                    var_fam.Children.Add(new Node { Type = name, Value = var_value });
                    continue;
                }

                //si no alcanzo secuencia para el valor de la variable y no es la ultima de los hijos asignarle undefined
                else if (i > values.Count() - 1)
                {
                    var_value = "undefined";
                    //lo anado al nodo de familia de variables
                    var_fam.Children.Add(new Node { Type = name, Value = var_value });
                    continue;
                }

                else if (child.Value.ToString() == "underscore")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<Point>>();
                    PointSequence s = new PointSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<Point>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                    //lo anado al nodo de familia de variables
                    var_fam.Children.Add(new Node { Type = name, Value = var_value });
                    break;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });

                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                else if (j < values[i].Count - 1 || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }
}
