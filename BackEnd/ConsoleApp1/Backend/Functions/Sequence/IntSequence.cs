using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System;
using System.Collections.Generic;
using System.Linq;

namespace G_Wall_E
{
    public class IntSequence : ISequence<double> //secuencia de enteros
    {
        public List<List<double>> values { get; set; }
        public string name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count   //define la cantidad de elementos que tiene la secuencia 
        {
            get
            {
                int count = 0;
                foreach (var x in values) foreach (var y in x) count++;
                return count;
            }
        }
        public List<int> is_undefined_concat { get; set; }

        public IntSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<double>>();
            is_undefined_concat = new List<int>();
        }
        public IntSequence(List<double> values) //
        {
            this.values = new List<List<double>>() { values };
            is_undefined_concat = new List<int>();
        }
        public IntSequence(List<List<double>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            is_undefined_concat = new List<int>();
        }
        public IntSequence(double inf, double sup)
        {
            var l = new List<double>();
            while (inf <= sup)
            {
                l.Add(inf);
                inf++;
            }
            values = new List<List<double>>() { l };
            is_undefined_concat = new List<int>();
        }
        public IntSequence(double inf)
        {
            //ir anadiendo a medida que se vayan pidiendo valores ya que es infinito*/
            this.is_infinite = true;
            values = new List<List<double>>() { new List<double>() { inf } };
            is_undefined_concat = new List<int>();
        }
        public IntSequence Concat(IntSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<double>> concat_values = new List<List<double>>();
            IntSequence concat_sequence = new IntSequence(concat_values);
            //concat_sequence.Count = 0;

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<double>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                //concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }
            //si la parte de la secuencia que estamos analizando es infinita, detiene la concatenacion, ya que al ser infinita, nunca llega a anadir a la otra
            if (is_infinite)
            {
                concat_sequence.is_infinite = true;
                return concat_sequence;
            }


            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<double>();
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
            //si la parte de la secuencia que estamos analizando es infinita, incluir al resultado como secuencia infinita
            if (s.is_infinite)
            {
                concat_sequence.is_infinite = true;
            }

            return concat_sequence;
        }
        public void GrowSequence() //metodo para anadir uno mas a la secuencia infinita
        {
            int index = values.Count - 1;
            double i = values[index][values[index].Count - 1];
            values[index].Add(i + 1);
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
                dynamic var_value = null;

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
                        var_value = new IntSequence(new List<double>());
                    }
                    //al resto de variables se les asigna valor undefined
                    var_value = "undefined";
                    var_fam.Children.Add(new Node { Type = name, Value = var_value });
                    continue;
                }

                //si no alcanzo secuencia para el valor de la variable asignarle una secuencia vacua
                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new IntSequence(false);
                    continue;
                }

                else if (i > values.Count() - 1)
                {
                    var_value = "undefined";
                    var_fam.Children.Add(new Node { Type = name, Value = var_value });
                    continue;
                }

                if (child.Value.ToString() == "underscore")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<double>>();
                    IntSequence s = new IntSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<double>();

                        //si la scuencia actual es infinita, guarda esa secuencia y para
                        if (is_infinite && i == values.Count() - 1)
                        {
                            index = values.Count() - 1;
                            golbal_sequence.Add(values[index]);
                            s.is_infinite = true;
                            break;
                        }

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
                    var_fam.Children.Add(new Node { Type = name, Value = var_value });
                    break;
                }

                //si la secuencia es infinita y es el ultimo hijo
                else if (node.Children.Last() == child && is_infinite && var_value is null)
                {
                    int index = values.Count - 1;
                    var s = new IntSequence(values[index][0]); //la secuencia infinita siempre la va a tener en el ultimo lugar, con el valor en el primero
                    s.is_infinite = true;
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });

                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }

                //si ya llegue al final de la secuencia y la secuencia debe ser infinita, agrandar la secuencia y seguir
                if (i == values.Count - 1 && j == values[i].Count - 1 && is_infinite)
                {
                    GrowSequence();
                    j++;
                }

                else if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;

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