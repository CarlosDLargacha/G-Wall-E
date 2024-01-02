using System.Xml;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using G_Wall_E;

namespace INTERPRETE_C__to_HULK
{
	public class Parser
	{
		// Lista de tokens
		List<Token> TS;

		// Posicion actual en la lista de tokens
		int position;

		// Diccionario de las variables
		public Dictionary<string, object> Variables;

		//booleano que me dice si una variable esta dentro de una sequencia o no
		bool is_in_seq;

		//booleano que me dice si la variable esta en foma de parametro en una funcion
		bool is_param;

		//booleano que dice si la variable esta dentro de una declaracion de variable en el let
		bool is_let;

		
		bool NotSaveVariable;

		// Constructor de la clase Parser
		public Parser(List<Token> Tokens_Sequency)
		{
			position = 0; //inicializa la posicion a 0
			is_in_seq = false; //incializa en falso
			TS = Tokens_Sequency; // Almacena la secuencia de Tokens
			Variables = new Dictionary<string, object>(); // Inicializa el diccionario de variables
		}

		// Método Parse que genera el árbol de análisis sintáctico, retorna el árbol de sintaxis AST
		public Node Parse()
		{
			List<Node> Children = new List<Node>();

			while (TS[position].Type != TokenType.EOF)
			{
				Children.Add(Global_Layer());
				Expect(TokenType.D_COMMA, ";");
			}

			return new Node { Type = "Root_of_the_tree", Children = Children };
		}

		// Método Global_Layer que decide qué acción tomar en función del token actual
		public Node Global_Layer()
		{
			if (position < TS.Count && Convert.ToString(TS[position].Value) == "draw")
			{
				return Drawable();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "measure")
			{
				return Measure();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "intersect")
			{
				return Intersect();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "points")
			{
				return Points();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "samples")
			{
				return Samples();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "randoms")
			{
				return Randoms();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "count")
			{
				return Count();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "let")
			{
				return Assigment();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "if")
			{
				return Conditional();
			}
			if (position < TS.Count && Convert.ToString(TS[position].Value) == "import")
			{
				return Import_Code();
			}

			return Layer_6();
		}

		
		public Node Import_Code()
		{
			position++;
			if (TS[position].Type == TokenType.STRING)
			{
				string dir = TS[position++].Value.ToString();

				Import imported = new Import(dir);
				string code = imported.Code();

				Lexer T = new Lexer(code);
				List<Token> new_TS = T.Tokens_sequency;

				Parser P = new Parser(new_TS);
				Node N = P.Parse();

				return new Node { Type = "import", Children = new List<Node> { N } };
			}
			Input_Error("Se espera un string con la direccion del archivo a importar");
			return null;
		}

		
		public Node Assigment()
		{
			position++;
			Node assigments = new Node { Type = "assigment_list" };
			bool d_comma = false;

			do
			{
				

				if (TS[position].Type == TokenType.D_COMMA)
				{
					position++;
				}

				
				
				if (TS[position].Type == TokenType.IN)
				{
					break;
				}

				Node expression = Global_Layer();
				assigments.Children.Add(expression);
				

			} while (TS[position].Type == TokenType.D_COMMA);
			is_let = false;

			Expect(TokenType.IN, "in");
			Node operations = Global_Layer();
			Exceptions_Missing(operations, "let-in");

			Node variable = new Node { Type = "Let", Children = new List<Node> { assigments, operations } };
			return variable;
		}

		// Este método se encarga de procesar los objetos a dibujar (DRAW)
		public Node Drawable()
		{
			position++;
			Node expression; 
			Node str = new Node { Type = "empty" };
			Node save_variable = new Node { Type = "save" }; 

			if (IsGeometricToken()) 
			{
				NotSaveVariable = true;
				save_variable = new Node { Type = "not_save" };
				expression = Global_Layer();
			}
			else 
			{
				expression = Global_Layer();
			}

			if (TS[position].Type == TokenType.STRING)
			{
				str = Factor();
			}

			return new Node { Type = "draw", Children = new List<Node> { expression, str, save_variable } }; 
		}

		// Este método se encarga de procesar las medidas entre 2 puntos (MEASURE)
		public Node Measure()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			is_param = true;
			Node p1 = Global_Layer();
			Expect(TokenType.COMMA, ",");
			Node p2 = Global_Layer();
			is_param = false;
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "measure", Children = new List<Node> { p1, p2 } };
		}

		//Este metodo se encarga de hallar el intercepto entre dos figuras
		public Node Intersect()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			is_param = true;
			Node f1 = Global_Layer();
			Expect(TokenType.COMMA, ",");
			Node f2 = Global_Layer();
			is_param = false;
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "intersect", Children = new List<Node> { f1, f2 } };
		}

		//Este metodo se encarga de hallar una secuencia finita de puntos aleatorios en una figura
		public Node Points()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			is_param = true;
			Node f = Factor();
			is_param = false;
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "points", Children = new List<Node>() { f } };
		}

		//Este metodo se necarga de hallar la cantidad de elementos que tiene una sequencia
		public Node Count()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			is_param = true;
			Node sec = Factor();
			is_param = false;
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "count", Children = new List<Node> { sec } };
		}

		//este metodo se encarga de hallar la secuencia de flotantes
		public Node Randoms()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "randoms", Children = new List<Node>() };
		}

		//este metodo se encarga de halalr la secuencia de puntos en el plano
		public Node Samples()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "samples", Children = new List<Node>() };
		}

		// Este método se encarga de procesar las estructuras condicionales del lenguaje (IF-ELSE)
		public Node Conditional()
		{
			position++;
			Node condition = Layer_6();
			Expect(TokenType.THEN, "then");
			Node operations_if = Global_Layer();
			Expect(TokenType.ELSE, "else");
			Node operations_else = Global_Layer();
			Node conditional_if_else = new Node { Type = "Conditional", Children = new List<Node> { condition, operations_if, operations_else } };
			return conditional_if_else;
		}

		// Este método se encarga de procesar la declaracion de funciones del lenguaje
		public Node Function()
		{
			position++;

			Node operation = Global_Layer();
			Exceptions_Missing(operation, "function");
			return operation;
		}

		#region CAPAS // Estos métodos implementan la precedencia de operadores del lenguaje

		// CAPA 6 (Operador '@' de concatenacion)
		public Node Layer_6()
		{
			Node node = Layer_5();
			if (position < TS.Count && Convert.ToString(TS[position].Value) == "@")
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_5();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		// CAPA 5 Operadores ('&' '|')
		public Node Layer_5()
		{
			Node node = Layer_4();
			while (position < TS.Count && (Convert.ToString(TS[position].Value) == "&" || Convert.ToString(TS[position].Value) == "|"))
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_4();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		// CAPA 4 (Operadores '>' '<' '==' '!=' '>=' '<=' de comparacion)
		public Node Layer_4()
		{
			Node node = Layer_3();
			while (position < TS.Count && (Convert.ToString(TS[position].Value) == "==" || Convert.ToString(TS[position].Value) == "!=" || Convert.ToString(TS[position].Value) == "<=" || Convert.ToString(TS[position].Value) == ">=" || Convert.ToString(TS[position].Value) == "<" || Convert.ToString(TS[position].Value) == ">"))
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_3();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		// CAPA 3 (Operadores '+' suma y  '-' resta)
		public Node Layer_3()
		{
			Node node = Layer_2();
			while (position < TS.Count && (Convert.ToString(TS[position].Value) == "+" || Convert.ToString(TS[position].Value) == "-"))
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_2();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;

		}

		// CAPA 2 (Operadores de '*' multiplicacion y '/' division)
		public Node Layer_2()
		{
			Node node = Layer_1();
			string? a = Convert.ToString(TS[position].Value);
			while (position < TS.Count && (Convert.ToString(TS[position].Value) == "*" || Convert.ToString(TS[position].Value) == "/" || Convert.ToString(TS[position].Value) == "%"))
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_1();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		// CAPA 1 (Operador '^' Potencia)
		public Node Layer_1()
		{
			Node node = Factor();
			while (position < TS.Count && Convert.ToString(TS[position].Value) == "^")
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Factor();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		public Node Layer_0()
		{
			if (TS[position].Type == TokenType.LINE) //recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) 
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Factor();
				Expect(TokenType.COMMA, ",");
				Node var2 = Factor();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "line", Children = new List<Node> { name, var1, var2 } };

			}

			else if (TS[position].Type == TokenType.POINT)//recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };
				if (NotSaveVariable) 
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Global_Layer();
				Expect(TokenType.COMMA, ",");
				Node var2 = Global_Layer();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "point_def", Children = new List<Node> { name, var1, var2 } };
			}

			else if (TS[position].Type == TokenType.SEGMENT) //recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) 
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Global_Layer();
				Expect(TokenType.COMMA, ",");
				Node var2 = Global_Layer();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "segment", Children = new List<Node> { name, var1, var2 } };

			}

			else if (TS[position].Type == TokenType.RAY)//recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) 
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Global_Layer();
				Expect(TokenType.COMMA, ",");
				Node var2 = Global_Layer();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "ray", Children = new List<Node> { name, var1, var2 } };

			}

			else if (TS[position].Type == TokenType.CIRCLE)//recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) 
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node point = Global_Layer();
				Expect(TokenType.COMMA, ",");
				Node measure = Global_Layer();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "circle", Children = new List<Node> { name, point, measure } };

			}

			else if (TS[position].Type == TokenType.ARC)//recibe tres variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) 
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Global_Layer();
				Expect(TokenType.COMMA, ",");
				Node var2 = Global_Layer();
				Expect(TokenType.COMMA, ",");
				Node var3 = Global_Layer();
				Expect(TokenType.COMMA, ",");
				Node var4 = Global_Layer();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "arc", Children = new List<Node> { name, var1, var2, var3, var4 } };
			}

			else if (TS[position].Type == TokenType.MEASURE)
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };
				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node point_1 = Global_Layer();
				Expect(TokenType.COMMA, ",");
				Node point_2 = Global_Layer();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");

				return new Node { Type = "measure_d", Children = new List<Node> { name, point_1, point_2 } };

			}

			else return null;
		}

		// CAPA 0 o CAPA FACTOR 
		public Node Factor()
		{
			Token current_token = TS[position]; // Obtiene el token actual
			if (position >= TS.Count)
				throw new Exception("Unexpected end of input");

			// Si el token actual es un paréntesis izquierdo, procesa una expresión entre paréntesis
			if (current_token.Type == TokenType.L_PHARENTESYS)
			{
				position++;
				Node node = Global_Layer();
				if (position >= TS.Count && TS[position].Type != TokenType.R_PHARENTESYS)
				{
					Input_Error(" ')' Expected!");
				}
				position++;
				return node;
			}

			//Si el token actual es una llave izquierda, procesa expresiones divididas por comas y terminadas por una llave cerrada
			if (current_token.Type == TokenType.L_KEY)
			{
				position++;
				int first_pun = -1;
				bool comma = false;
				bool infinite = false;
				int inf_count = 0;
				Node node = new Node { Type = "sequence" };
				while (TS[position].Type != TokenType.R_KEY)
				{
					//if (comma || infinite) position++;
					if (TS[position].Type == TokenType.COMMA)
					{
						if (first_pun == -1) first_pun = position;
						if (position == first_pun) comma = true;
						else if (infinite) Input_Error(" Invalid sequence declaration");
						position++;
					}
					if (TS[position].Type == TokenType.INFINITE)
					{
						inf_count++;
						if (inf_count > 1) Input_Error("Invalid sequence declaration");
						if (position == first_pun) infinite = true;
						if (first_pun == -1)
						{
							first_pun = position;
							infinite = true;
						}
						else if (comma) Input_Error(" Invalid sequence declaration");
						position++;
					}
					is_in_seq = true;
					Node children = Global_Layer();
					if (!infinite) Exceptions_Missing(children, "sequence");
					if (infinite && children.Type == "error" && TS[position].Type == TokenType.R_KEY) break;
					node.Children.Add(children);
				}
				if (infinite) node.Type = "inf_sequence";
				position++;
				is_in_seq = false;
				return node;
			}

			// si el token es !
			if (current_token.Value.ToString() == "!")
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node n = Global_Layer();
				return new Node { Type = op, Children = new List<Node> { n } };

			}

			//Si el token actual es un número, retorna un nodo de tipo "number" con el valor del número
			else if (TS[position].Type == TokenType.OPERATOR && (char)TS[position].Value == '-')
			{
				position++;
				dynamic value = Convert.ToDouble(TS[position++].Value);
				return new Node { Type = "number", Value = -value };
			}

			//Si el token actual es un número, retorna un nodo de tipo "number" con el valor del número
			else if (TS[position].Type == TokenType.NUMBER)
			{
				dynamic value = Convert.ToDouble(TS[position++].Value);
				return new Node { Type = "number", Value = value };
			}

			//Si el token actual es "true", retorna un nodo de tipo "true" con el valor true
			else if (TS[position].Type == TokenType.TRUE)
			{
				dynamic value = TS[position++].Value;
				return new Node { Type = "true", Value = value };
			}

			// Si el token actual es "false", retorna un nodo de tipo "false" con el valor false
			else if (TS[position].Type == TokenType.FALSE)
			{
				dynamic value = TS[position++].Value;
				return new Node { Type = "false", Value = value };
			}

			// Si el token actual es una cadena, retorna un nodo de tipo "string" con el valor de la cadena
			else if (TS[position].Type == TokenType.STRING)
			{
				dynamic? value = Convert.ToString(TS[position++].Value);
				return new Node { Type = "string", Value = value };
			}

			//si el token actual es undefined, retorna el nodo de tipo undefined 
			else if (TS[position].Type == TokenType.UNDEFINED)
			{
				position++;
				return new Node { Type = "undefined" };
			}

			//si el token es una variable o un underscore
			else if (TS[position].Type == TokenType.VARIABLE || TS[position].Type == TokenType.UNDERSCORE)
			{
				//si a la variable le precede una coma, procesar como asignacion de valores de una secuencia
				if (TS[position + 1].Type == TokenType.COMMA && !is_in_seq && !is_param)
				{
					//crear nodo hijo con la lista de variables a asignar
					Node vars = new Node { Type = "asign_seq_var_name" };
					bool comma = true;
					do
					{
						//si encuentro un underscore, anadir al arbol
						if (TS[position].Type == TokenType.UNDERSCORE)
						{
							position++;
							Node var = new Node { Type = "variable", Value = "underscore" };
							vars.Children.Add(var);
							comma = false;
							continue;
						}
						//si encuentro una variable, anadir
						else if (TS[position].Type == TokenType.VARIABLE)
						{
							dynamic? values = Convert.ToString(TS[position++].Value);
							Node var = new Node { Type = "variable", Value = values };
							comma = false;
							vars.Children.Add(var);
						}
						//si encuetro una coma y es valido, operar, sino, lanzar error
						if (TS[position].Type == TokenType.COMMA)
						{
							if (comma) Input_Error("Invalid operation in constant asignation");
							position++;
							comma = true;
						}
						//si encontre un igual y una coma le precede, error
						else if (TS[position].Type == TokenType.EQUAL)
						{
							if (comma) Input_Error("Invalid operation in constant asignation");
						}
						//si no encontro ninguna de las anteriores, retornar error

						else Input_Error("Invalid constant asignation");
					}
					while (TS[position].Type != TokenType.EQUAL);
					position++;

					//crear hijo con lista de valores a asiganr en las variables(secuencias)
					Node sequence = new Node { Type = "asign_values_seq" };
					Node node = Global_Layer();
					sequence.Children.Add(node);
					//devolver el nodo con el arbol de las asignaciones para el analisis semantico
					return new Node { Type = "sequence_asigment", Children = new List<Node>() { vars, sequence } };
				}

				//acciones que solo son validas si el token es una variable
				if (TS[position].Type == TokenType.VARIABLE)
				{
					//si a la variable le precede un igual, es de asignacion global
					if (TS[position + 1].Type == TokenType.EQUAL)
					{
						//este sistema vendria siendo parecido al del let in, con la unica diferencia de que cuardo exclusivamente una variable
						Node name = new Node { Type = "name", Value = TS[position].Value };
						position += 2;
						switch (TS[position].Type) //si lo que viene despues del igual son los metodos de Layer_0, llamar
						{
							case TokenType.LINE:
							case TokenType.SEGMENT:
							case TokenType.RAY:
							case TokenType.ARC:
							case TokenType.CIRCLE:
							case TokenType.MEASURE:
							case TokenType.POINT:
								return Layer_0();
							default: //si no, es un valor de variable cualquiera y lo guarda en un nodo
								Node var_value = Global_Layer();
								return new Node { Type = "global_var_asigment", Children = new List<Node> { name, var_value } };
						}
					}
					// si el token siguiente es parentesis procesar como funcion
					if (TS[position + 1].Type == TokenType.L_PHARENTESYS)
					{
						dynamic? f_name = Convert.ToString(TS[position++].Value);
						position++;
						Node name = new Node { Type = "d_function_name", Value = f_name };
						Node param = new Node { Type = "parameters" };
						if (TS[position].Type != TokenType.R_PHARENTESYS)
						{
							is_param = true;
							do
							{
								Node parammeter_name = new Node { Type = "p_name", Value = Layer_6() };
								param.Children.Add(parammeter_name);

								if (TS[position].Type == TokenType.COMMA)
								{
									position++;
								}
							} while (TS[position - 1].Type == TokenType.COMMA);
							is_param = false;
						}

						Expect(TokenType.R_PHARENTESYS, ")");
						if (TS[position].Type == TokenType.EQUAL)
						{
							Node action = Function();
							return new Node { Type = "function", Children = new List<Node> { name, param, action } };
						}
						return new Node { Type = "declared_function", Children = new List<Node> { name, param } };

					}
					// procesar como variable
					dynamic? value = Convert.ToString(TS[position++].Value);
					return new Node { Type = "variable", Value = value };
				}
				//si es un underscore, no es valido el resto de operaciones
				else
				{
					Input_Error("You're not using the underscore properly");
					return null; //no pasa nada porque siempre va a lanzar excepcion antes
				}

			}

			// Si el token actual es "cos", procesa una función coseno
			else if (TS[position].Type == TokenType.COS)
			{
				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				Node valor = Layer_6();
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "cos", Children = new List<Node> { valor } };
			}

			// Si el token actual es "sen", procesa una función seno
			else if (TS[position].Type == TokenType.SEN)
			{
				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				Node valor = Layer_6();
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "sin", Children = new List<Node> { valor } };
			}

			// Si el token actual es "log", procesa una función logaritmo
			else if (TS[position].Type == TokenType.LOG)
			{
				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				Node valor = Layer_6();
				Expect(TokenType.COMMA, ",");
				Node valor2 = Layer_6();
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "log", Children = new List<Node> { valor, valor2 } };
			}

			// Si el token actual es "let", procesa una asignacion
			else if (position < TS.Count && Convert.ToString(TS[position].Value) == "let")
			{
				return Assigment();
			}

			//* GEO_WALL_E 

			else if (TS[position].Type == TokenType.POINT) //caso: point p; ó point sequence ps;
			{
				if (is_let) return Layer_0();

				position++;
				Node name;
				int safe1; //anadido por incongruhencia con position++
				int safe2;
				if (TS[position].Type == TokenType.SEQUENCE)  //secuencia de puntos aleatorios
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					// safe2 = position;
					// position = safe1;
					// Expect(TokenType.VARIABLE, "reference");
					// position = safe2;

					return new Node { Type = "point_sequence", Children = new List<Node> { name } };
				}

				else //punto aleatorio
				{
					safe1 = position;

					if (TS[position].Type == TokenType.L_PHARENTESYS)
					{
						position--;
						return Layer_0();
					}

					name = Factor();
					Node x = null;
					Node y = null;
					Exceptions_Missing(name, "");

					// safe2 = position;
					// position = safe1;
					// Expect(TokenType.VARIABLE, "reference");
					// position = safe2;

					return new Node { Type = "point", Children = new List<Node> { name, x, y } };
				}
			}

			else if (TS[position].Type == TokenType.LINE)
			{
				NotSaveVariable = true;
				if (is_let) return Layer_0();

				position++;
				Node name;
				int safe1; //anadido por incongruencia con position++
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de lineas aleatorias
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					// safe2 = position;
					// position = safe1;
					// Expect(TokenType.VARIABLE, "reference");
					// position = safe2;

					return new Node { Type = "line_sequence", Children = new List<Node> { name } };
				}

				else //linea aleatoria
				{
					if (TS[position].Type == TokenType.L_PHARENTESYS)
					{
						position--;
						return Layer_0();
					}

					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					

					return new Node { Type = "line_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.SEGMENT)
			{
				NotSaveVariable = true;
				if (is_let) return Layer_0();

				position++;
				Node name;
				int safe1; 
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de segmentos aleatorios
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					

					return new Node { Type = "segment_sequence", Children = new List<Node> { name } };
				}

				else //segmento aleatorio
				{
					if (TS[position].Type == TokenType.L_PHARENTESYS)
					{
						position--;
						return Layer_0();
					}

					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					
					// position = safe2;

					return new Node { Type = "segment_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.RAY)
			{
				NotSaveVariable = true;
				if (is_let) return Layer_0();

				position++;
				Node name;
				int safe1; //anadido por incongruencia con position++
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de rayos aleatorios
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					

					return new Node { Type = "ray_sequence", Children = new List<Node> { name } };
				}

				else //rayo aleatorio
				{
					if (TS[position].Type == TokenType.L_PHARENTESYS)
					{
						position--;
						return Layer_0();
					}

					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					

					return new Node { Type = "ray_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.ARC)
			{
				NotSaveVariable = true;
				if (is_let) return Layer_0();

				position++;
				Node name;
				int safe1; 
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de arcos aleatorios
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					

					return new Node { Type = "arc_sequence", Children = new List<Node> { name } };
				}

				else //arco aleatorio
				{
					if (TS[position].Type == TokenType.L_PHARENTESYS)
					{
						position--;
						return Layer_0();
					}

					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					

					return new Node { Type = "arc_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.CIRCLE)
			{
				NotSaveVariable = true;
				if (is_let) return Layer_0();

				position++;
				Node name;
				int safe1; 
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de circunferencias aleatorias
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					// safe2 = position;
					// position = safe1;
					// Expect(TokenType.VARIABLE, "reference");
					// position = safe2;

					return new Node { Type = "circle_sequence", Children = new List<Node> { name } };
				}

				else //circunferencia aleatoria
				{
					if (TS[position].Type == TokenType.L_PHARENTESYS)
					{
						position--;
						return Layer_0();
					}

					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					

					return new Node { Type = "circle_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.COLOR) //si encuentro la oren de asignar color, rectificar que se le haya asigando uno y retornar nodo
			{
				int safe1;
				int safe2;
				position++;
				safe1 = position;
				Node name = Factor();
				Exceptions_Missing(name, "");
				safe2 = position;
				position = safe1;
				Expect(TokenType.COLOR_RANGE, "valid color");
				position = safe2;

				return new Node { Type = "color_asign", Children = new List<Node> { name } };
			}

			else if (TS[position].Type == TokenType.COLOR_RANGE)
			{
				if (position == 0 || TS[position - 1].Type != TokenType.COLOR)
				{
					Input_Error("SYNTAX ERROR: 'color' was expected before 'blue' call");
				}
				dynamic? value = Convert.ToString(TS[position++].Value);
				return new Node { Type = "color", Value = value };
			}

			else if (TS[position].Type == TokenType.RESTORE) 
			{
				dynamic? value = Convert.ToString(TS[position++].Value);
				return new Node { Type = "restore", Value = value };
			}

			// Si el token actual es nulo, retorna un nodo vacío
			else if (TS[position] == null)
			{
				return new Node { };
			}

			// Si el token actual no coincide con ninguno de los anteriores, retorna un nodo de error
			else
			{
				return new Node { Type = "error", Value = 0 };
			}
		}
		#endregion

		#region Auxiliar

		// Método que lanza una excepción con un mensaje de error de sintaxis
		private void Input_Error(string error)
		{
			throw new Exception("SYNTAX ERROR: " + error);
		}

		//  Método que verifica si un nodo es de tipo "error" y lanza una excepción en ese caso
		private void Exceptions_Missing(Node node, string op)
		{
			if (node.Type == "error")
			{
				if (op == "")
				{
					Input_Error($"Missing expression after variable `{TS[position - 1].Value}`");
				}
				else
				{
					string msg = $"Missing expression in `{op}` after variable `{TS[position - 1].Value}`";
					Input_Error(msg);
				}
			}
		}

		// Método que verifica si el token actual es del tipo esperado y avanza a la siguiente posición en ese caso, o lanza una excepción si no lo es
		public void Expect(TokenType tokenType, object? value)
		{
			if (TS[position].Type == tokenType)
			{
				position++;
			}
			else
			{
				Input_Error($"[{position}] `{value}` Expected! after `{TS[position - 1].Value}`,`{TS[position].Value}` was received");
			}
		}

		//nuevo de camila
		public bool IsGeometricToken()
		{
			switch (TS[position].Type)
			{
				case TokenType.LINE:
				case TokenType.SEGMENT:
				case TokenType.RAY:
				case TokenType.CIRCLE:
				case TokenType.ARC:
					return true;

				default:
					return false;
			}
		}

		#endregion
	}
}
