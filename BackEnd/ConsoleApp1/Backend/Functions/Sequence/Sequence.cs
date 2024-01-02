using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System;
using System.Collections.Generic;
using System.Linq;

namespace G_Wall_E
{
    //pensar despues que hacer cuando me encuentre una secuencia de esta forma : {} ya que no es de un tipo ni de otro
    public interface ISequence<T> //interfaz sequence general para los tipos de secuencia
    {
        public List<List<T>> values { get; set; } //lista de secuencias que conforman a la secuencia concatenada
        public string name { get; set; } //nombre de la secuencia
        //las unicas secuecias que pueden ser infinitas son las de enteros
        public bool is_infinite { get; set; } //define si una secuencia es infinita o no, si es verdadero, la ultima de la lista de secuencias concatenadas es la que es infinita
        public bool is_undefined { get; set; } //define si la secuencia esta indefinida o no
        public int Count   //define la cantidad de elementos que tiene la secuencia 
        {
            get
            {
                int count = 0;
                foreach (var x in values) foreach (var y in x) count++;
                return count;
            }
        }
        public List<int> is_undefined_concat { get; set; } //define en que indices de la lista de secuencias, las secuencias estan concatenadas con undefined
    }
}