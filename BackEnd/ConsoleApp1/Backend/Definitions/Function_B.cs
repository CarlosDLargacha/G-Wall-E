using System.Globalization;
using System.IO;
using INTERPRETE_C__to_HULK;
using System;
using System.Collections.Generic;

namespace INTERPRETE_C_to_HULK
{
    public class Function_B
    {
        public Dictionary<string , object> variable_param;
        public string Name_function;
        public Node Operation_Node;

        public Function_B(string name, Node node,Dictionary<string , object> param )
        {
            Name_function = name;
            Operation_Node = node;
            variable_param = param;
        }
    }
}