using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using INTERPRETE_C_to_HULK;
using G_Wall_E;
using System.Numerics;
using System.Linq.Expressions;

namespace INTERPRETE_C__to_HULK
{
	public class Semantic_Analyzer
	{
		Node AST; // Árbol de Análisis Sintáctico Abstracto (AST)

		string FigureColor; // Color actual para las figuras

		Dictionary<string, object> variables_globales; // Diccionario para almacenar las variables globales

		public List<Function_B> functions_declared = new List<Function_B>(); // Lista para almacenar las funciones declaradas

		public List<Dictionary<string, dynamic>> Scopes; // Lista de diccionarios para almacenar los ámbitos (scopes)

		public List<IDrawable> Drawables;

		// Constructor de la clase Semantic_Analyzer
		public Semantic_Analyzer()
		{
			// Inicializa el diccionario de variables globales con algunas variables predefinidas
			variables_globales = new Dictionary<string, object>
			{
				{"PI",Math.PI},
				{"TAU",Math.Pow(Math.PI,2)},
				{"true",true},
				{"false",false},
			};

			FigureColor = "white";
			Drawables = new List<IDrawable>();
		}

		// Método para leer el AST generado por el Parser
		public void Read_Parser(Node n)
		{
			AST = n;
			Scopes = new List<Dictionary<string, object>> { variables_globales };
		}

		//  Método para decidir qué acción tomar en función del tipo de nodo
		public List<IDrawable> Choice(Node node)
		{
			switch (node.Type)
			{
				// Si el nodo es una función declarada, evalúa el nodo y muestra el resultado
				case "print":
					Console.WriteLine(Evaluate(node.Children[0]));
					break;
				// Para cualquier otro tipo de nodo, simplemente evalúa el nodo 
				default:
					Evaluate(node);
					break;
			}

			return Drawables;
		}

		// Metodo para Evaluar los Nodos
		public object? Evaluate(Node node)
		{
			// Dependiendo del tipo de nodo, realiza diferentes operaciones
			switch (node.Type)
			{
				case "Root_of_the_tree":
					for (int i = 0; i < node.Children.Count; i++)
					{
						Evaluate(node.Children[i]);
					}
					break;
				// Si el nodo es un número, retorna su valor
				case "number":
					return node.Value;
				// Si el nodo es un string, retorna su valor
				case "string":
					return node.Value;
				// Si el nodo es "true", retorna true
				case "true":
					return true;
				// Si el nodo es "false", retorna false
				case "false":
					return false;
				// Si el nodo es una variable, retorna su valor del ámbito actual
				case "variable":
					return Scopes[Scopes.Count - 1][node.Value.ToString()];
				// Si el nodo es el nombre de una función declarada, retorna su valor
				case "d_function_name":
					return node.Value;
				case "g_name":
					return node.Value;
				//Si el nodo es asignacion de color, igualar figurecolor al color asignado
				case "color_asign":
					FigureColor = node.Children[0].Value.ToString();
					break;
				//Si el nodo es restore, igualar figurecolor a negro;
				case "restore":
					FigureColor = "white";
					break;
				case "import":
					Evaluate(node.Children[0]);
					break;

				
				case "+":
					var left_unknown = Evaluate(node.Children[0]);
					var right_unknown = Evaluate(node.Children[1]);
					(bool u1_b, int u1_i) = IsSequence(left_unknown);
					(bool u2_b, int u2_i) = IsSequence(right_unknown);

					if (u1_b || u2_b)
					{   //si una secuencia esta vacia retorna la otra
						if (left_unknown == "empty_seq" && right_unknown == "empty_seq") return "empty_seq";
						else if (left_unknown == "empty_seq") return right_unknown;
						else if (right_unknown == "empty_seq") return left_unknown;

						//si se trata de dos secuencias, concatenarlas
						if (u1_b && u2_b && u1_i == u2_i)
						{
							if (u1_i == 1)
							{
								PointSequence l = (PointSequence)left_unknown;
								PointSequence r = (PointSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u1_i == 2)
							{
								LineSequence l = (LineSequence)left_unknown;
								LineSequence r = (LineSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u1_i == 3)
							{
								SegmentSequence l = (SegmentSequence)left_unknown;
								SegmentSequence r = (SegmentSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u1_i == 4)
							{
								RaySequence l = (RaySequence)left_unknown;
								RaySequence r = (RaySequence)right_unknown;
								return l.Concat(r);
							}
							else if (u1_i == 5)
							{

								ArcSequence l = (ArcSequence)left_unknown;
								ArcSequence r = (ArcSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u1_i == 6)
							{
								CircleSequence l = (CircleSequence)left_unknown;
								CircleSequence r = (CircleSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u1_i == 7)
							{
								StringSequence l = (StringSequence)left_unknown;
								StringSequence r = (StringSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u1_i == 8)
							{
								IntSequence l = (IntSequence)left_unknown;
								IntSequence r = (IntSequence)right_unknown;
								return l.Concat(r);
							}
							else
							{
								FloatSequence l = (FloatSequence)left_unknown;
								FloatSequence r = (FloatSequence)right_unknown;
								return l.Concat(r);
							}
						}

						//si se trata de dos secuencias y una de ellas undefined, concatenarlas
						else if (u1_b && right_unknown == "undefined")
						{
							if (u1_i == 1)
							{
								PointSequence l = (PointSequence)left_unknown;
								PointSequence r = new PointSequence(true);
								return l.Concat(r);
							}
							else if (u1_i == 2)
							{
								LineSequence l = (LineSequence)left_unknown;
								LineSequence r = new LineSequence(true);
								return l.Concat(r);
							}
							else if (u1_i == 3)
							{
								SegmentSequence l = (SegmentSequence)left_unknown;
								SegmentSequence r = new SegmentSequence(true);
								return l.Concat(r);
							}
							else if (u1_i == 4)
							{
								RaySequence l = (RaySequence)left_unknown;
								RaySequence r = new RaySequence(true);
								return l.Concat(r);
							}
							else if (u1_i == 5)
							{

								ArcSequence l = (ArcSequence)left_unknown;
								ArcSequence r = new ArcSequence(true);
								return l.Concat(r);
							}
							else if (u1_i == 6)
							{
								CircleSequence l = (CircleSequence)left_unknown;
								CircleSequence r = new CircleSequence(true);
								return l.Concat(r);
							}
							else if (u1_i == 7)
							{
								StringSequence l = (StringSequence)left_unknown;
								StringSequence r = new StringSequence(true);
								return l.Concat(r);
							}
							else if (u1_i == 8)
							{
								IntSequence l = (IntSequence)left_unknown;
								IntSequence r = new IntSequence(true);
								return l.Concat(r);
							}
							else
							{
								FloatSequence l = (FloatSequence)left_unknown;
								FloatSequence r = new FloatSequence(true);
								return l.Concat(r);
							}
						}

						//lo mismo para el otro lado
						else if (u2_b && left_unknown == "undefined")
						{
							if (u2_i == 1)
							{
								PointSequence l = new PointSequence(true);
								PointSequence r = (PointSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u2_i == 2)
							{
								LineSequence l = new LineSequence(true);
								LineSequence r = (LineSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u2_i == 3)
							{
								SegmentSequence l = new SegmentSequence(true);
								SegmentSequence r = (SegmentSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u2_i == 4)
							{
								RaySequence l = new RaySequence(true);
								RaySequence r = (RaySequence)right_unknown;
								return l.Concat(r);
							}
							else if (u2_i == 5)
							{

								ArcSequence l = new ArcSequence(true);
								ArcSequence r = (ArcSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u2_i == 6)
							{
								CircleSequence l = new CircleSequence(true);
								CircleSequence r = (CircleSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u2_i == 7)
							{
								StringSequence l = new StringSequence(true);
								StringSequence r = (StringSequence)right_unknown;
								return l.Concat(r);
							}
							else if (u2_i == 8)
							{
								IntSequence l = new IntSequence(true);
								IntSequence r = (IntSequence)right_unknown;
								return l.Concat(r);
							}
							else
							{
								FloatSequence l = new FloatSequence(true);
								FloatSequence r = (FloatSequence)right_unknown;
								return l.Concat(r);
							}
						}
						//si no es ninguno de los anteriores casos
						Input_Error("Failed sequence concatenation. Are you declaring the operation properly?");
						break;
					}
					//si ambas secuencias son undefined, retornar undefined
					else if (left_unknown == "undefined" && right_unknown == "undefined") return "undefined";
					//si se trata de otra cosa, realizar la accion normal de la suma

					else
					{
						if(right_unknown is Measure && left_unknown is Measure) return (Measure)left_unknown + (Measure)right_unknown;
						if(right_unknown is Measure && left_unknown is double) return (double)left_unknown + (Measure)right_unknown;
						if(right_unknown is double && left_unknown is Measure) return (double)right_unknown + (Measure)left_unknown;
						if(right_unknown is Point plus_p1 && left_unknown is Point plus_p2) 
						{
							return new Point("", FigureColor, plus_p1.X - plus_p2.X, plus_p1.Y - plus_p2.Y);
						} 
						Type_Expected(right_unknown, left_unknown, "number", "+");
						return (double)left_unknown + (double)right_unknown;
					}
					break;
					

				case "-":
				case "*":
				case "/":
				case "^":
				case "%":
					object? left_a = Evaluate(node.Children[0]);
					object? right_a = Evaluate(node.Children[1]);
					switch (node.Type)
					{
						case "-": 
						if(right_a is Measure && left_a is Measure) return (Measure)left_a - (Measure)right_a;
						if(right_a is Measure && left_a is double) return (double)left_a - (Measure)right_a;
						if(right_a is double && left_a is Measure) return (double)right_a - (Measure)left_a;
						if(right_a is Point _p1 && left_a is Point _p2) 
						{
							return new Point("", FigureColor, _p2.X - _p1.X, _p2.Y - _p1.Y);
						} 
						Type_Expected(right_a, left_a, "number", "-");
						return (double)left_a - (double)right_a;

						case "*": 
						if(right_a is Measure && left_a is Measure) return (Measure)left_a * (Measure)right_a;
						if(right_a is Measure && left_a is double) return (double)left_a * (Measure)right_a;
						if(right_a is double && left_a is Measure) return (double)right_a * (Measure)left_a;
						if(right_a is double mult_n && left_a is Point mult_p2) 
						{
							return new Point("", FigureColor, mult_n * mult_p2.X, mult_n * mult_p2.Y);
						}
						if(right_a is Point mult_p1 && left_a is double mult_n2) 
						{
							return new Point("", FigureColor, mult_n2 * mult_p1.X, mult_n2 * mult_p1.Y);
						}
						Type_Expected(right_a, left_a, "number", "*");
						return (double)left_a * (double)right_a;

						case "/": 
						if(right_a is Measure && left_a is Measure) return (Measure)left_a / (Measure)right_a;
						if(right_a is Measure && left_a is double) return (double)left_a / (Measure)right_a;
						if(right_a is double && left_a is Measure) return (double)right_a / (Measure)left_a;
						if(left_a is Point divm_p1 && right_a is Measure divm_n2) 
						{
							return new Point("", FigureColor, divm_p1.X / divm_n2.Execute(), divm_p1.Y / divm_n2.Execute());
						}
						if(left_a is Point div_p1 && right_a is double div_n2) 
						{
							return new Point("", FigureColor, div_p1.X / div_n2, div_p1.Y / div_n2);
						}
						Type_Expected(right_a, left_a, "number", "/");
						return (double)left_a / (double)right_a;

						case "^": 
						Type_Expected(right_a, left_a, "number", "^");
						return Math.Pow((double)left_a, (double)right_a); 

						default: 
						Type_Expected(right_a, left_a, "number", "nothing");
						return (double)left_a + (double)right_a;
					}


				//? Boolean operations
				// Si el nodo es una operación booleana (>, <, >=, <=, ==, !=, !), 
				//evalúa los nodos hijos y realiza la operación correspondiente
				case "!":
					object? not = Evaluate(node.Children[0]);
					Expected(not, "boolean", "! not");
					return !(bool)not;
				case ">":
				case "<":
				case ">=":
				case "<=":
				case "==":
				case "!=":
					object? left_b = Evaluate(node.Children[0]);
					object? right_b = Evaluate(node.Children[1]);

					if (left_b is Measure left_bm)
					{
						left_b = left_bm.Execute();
					}
					
					if (right_b is Measure right_bm)
					{
						right_b = right_bm.Execute();
					}

					switch (node.Type)
					{
						case ">": return (double)left_b > (double)right_b;
						case "<": return (double)left_b < (double)right_b;
						case ">=": return (double)left_b >= (double)right_b;
						case "<=": return (double)left_b <= (double)right_b;
						case "==":
							if (left_b is double && right_b is double)
							{
								return (double)left_b == (double)right_b;
							}
							else if (left_b is string && right_b is string)
							{
								return (string)left_b == (string)right_b;
							}
							else if (left_b is bool && right_b is bool)
							{
								return (bool)left_b == (bool)right_b;
							}
							else
							{
								Input_Error($"Operator '==' can not be used between values of diferent types");
							}
							break;
						default:
							if (left_b is double && right_b is double)
							{
								return (double)left_b != (double)right_b;
							}
							else if (left_b is string && right_b is string)
							{
								return (string)left_b != (string)right_b;
							}
							else if (left_b is bool && right_b is bool)
							{
								return (bool)left_b != (bool)right_b;
							}
							else
							{
								Input_Error($"Operator '==' can not be used between values of diferent types");
							}
							break;
					}
					break;
				case "@":
					object? left_st = Evaluate(node.Children[0]);
					object? right_st = Evaluate(node.Children[1]);
					string? type_right = Identify(right_st);
					string? type_left = Identify(left_st);
					if (type_right == "string" || type_left == "string")
					{
						return left_st.ToString() + right_st.ToString();
					}
					break;
				case "&":
					object? left_and = Evaluate(node.Children[0]);
					object? right_and = Evaluate(node.Children[1]);
					Type_Expected(right_and, left_and, "boolean", "&");
					return (bool)left_and && (bool)right_and;
				case "|":
					dynamic? left_or = Evaluate(node.Children[0]);
					dynamic? right_or = Evaluate(node.Children[1]);
					Type_Expected(right_or, left_or, "boolean", "|");
					return (bool)left_or || (bool)right_or;

				//? Expressions
				// Si el nodo es un "draw" lo agrega a la lista de objetos dibujables
				case "draw":
					var figures = Evaluate(node.Children[0]);

					(bool is_s, int kind_s) = IsSequence(figures);

					//si es una secuencia
					if (is_s)
					{
						if (kind_s == 1) Draw_Sequence((PointSequence)figures);
						else if (kind_s == 2) Draw_Sequence((LineSequence)figures);
						else if (kind_s == 3) Draw_Sequence((SegmentSequence)figures);
						else if (kind_s == 4) Draw_Sequence((RaySequence)figures);
						else if (kind_s == 5) Draw_Sequence((ArcSequence)figures);
						else if (kind_s == 6) Draw_Sequence((CircleSequence)figures);
					}
					else if (figures is string)
					{
						if (figures == "undefined") Input_Error("Infinie or undefined sequences can't be drawed");
						else if (figures == "empty_seq") break;
					}
					//si es solo una figura
					else
					{
						IDrawable fig = (IDrawable)figures;

						if (node.Children[1].Type != "empty")
						{
							fig.Msg = Evaluate(node.Children[1]).ToString();
						}

						
						if (node.Children[2].Type == "not_save")
						{
							string _key = string.Empty;

							foreach (var key in Scopes[Scopes.Count - 1].Keys)
							{
								if (key.StartsWith("_"))
								{
									_key = key;
									break;
								}
							}

							if (_key != string.Empty)
							{
								Scopes[Scopes.Count - 1].Remove(_key);
							}
						}

						Drawables.Add(fig);
						break;
					}
					break;

				case "measure":
					Point p1 = (Point)Evaluate(node.Children[0]);
					Point p2 = (Point)Evaluate(node.Children[1]);
					dynamic m = new Measure(FigureColor, "measure", p1, p2);

					if (Scopes[Scopes.Count - 1].ContainsKey("measure"))
					{
						Scopes[Scopes.Count - 1]["measure"] = m;
					}
					else
					{
						Scopes[Scopes.Count - 1].Add("measure", m);
					}

					return m;

				case "measure_d":
					string m_name = Evaluate(node.Children[0]).ToString();
					Point pd1 = (Point)Evaluate(node.Children[1]);
					Point pd2 = (Point)Evaluate(node.Children[2]);
					dynamic md = new Measure(FigureColor, m_name, pd1, pd2);

					if (Scopes[Scopes.Count - 1].ContainsKey(m_name))
					{
						Scopes[Scopes.Count - 1][m_name] = md;
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(m_name, md);
					}

					return md;

				//si el nodo es points, devuelve una secuencia aleatoria de puntos dentro de la figura
				case "points":
					var f = Evaluate(node.Children[0]);
					int f_f = IsKind_Seq(f);

					if (f_f == 1) return Points_Methods.Points((Point)f);

					else if (f_f == 2) return Points_Methods.Points((Line)f, FigureColor);

					else if (f_f == 3) return Points_Methods.Points((Segment)f, FigureColor);

					else if (f_f == 4) return Points_Methods.Points((Ray)f, FigureColor);

					else if (f_f == 5) return Points_Methods.Points((Arc)f, FigureColor);

					else if (f_f == 6) return Points_Methods.Points((Circle)f, FigureColor);

					else Input_Error("Invalid param in poins method call");
					break;
				//si el nodo es intersect, devuelve la secuencia de puntos de interseccion entre las figuras
				case "intersect":
					var f1 = Evaluate(node.Children[0]); //evaluar la primera figura
					var f2 = Evaluate(node.Children[1]); //evaluar la segunda figura
					(int f_f1, int f_f2) = (IsKind_Seq(f1), IsKind_Seq(f2)); //hallar que tipo de figuras son
																			 //si la primera figura es un punto
					if (f_f1 == 1)
					{
						if (f_f2 == 1) return Intersect_Methods.Intersect((Point)f1, (Point)f2);

						else if (f_f2 == 2) return Intersect_Methods.Intersect((Point)f1, (Line)f2);

						else if (f_f2 == 3) return Intersect_Methods.Intersect((Point)f1, (Segment)f2);

						else if (f_f2 == 4) return Intersect_Methods.Intersect((Point)f1, (Ray)f2);

						else if (f_f2 == 5) return Intersect_Methods.Intersect((Point)f1, (Arc)f2);

						else if (f_f2 == 6) return Intersect_Methods.Intersect((Point)f1, (Circle)f2);

						else Input_Error("Invalid parameters in intersect call");
						break;
					}

					else if (f_f1 == 2)
					{
						if (f_f2 == 2) return Intersect_Methods.Intersect((Line)f1, (Line)f2, FigureColor);

						else if (f_f2 == 3) return Intersect_Methods.Intersect((Line)f1, (Segment)f2, FigureColor);

						else if (f_f2 == 4) return Intersect_Methods.Intersect((Line)f1, (Ray)f2, FigureColor);

						else if (f_f2 == 5) return Intersect_Methods.Intersect((Line)f1, (Arc)f2, FigureColor);

						else if (f_f2 == 6) return Intersect_Methods.Intersect((Line)f1, (Circle)f2, FigureColor);

						else Input_Error("Invalid parameters in intersect call");
						break;
					}

					else if (f_f1 == 3)
					{
						if (f_f2 == 3) return Intersect_Methods.Intersect((Segment)f1, (Segment)f2, FigureColor);

						else if (f_f2 == 4) return Intersect_Methods.Intersect((Segment)f1, (Ray)f2, FigureColor);

						else if (f_f2 == 5) return Intersect_Methods.Intersect((Segment)f1, (Arc)f2, FigureColor);

						else if (f_f2 == 6) return Intersect_Methods.Intersect((Segment)f1, (Circle)f2, FigureColor);

						else Input_Error("Invalid parameters in intersect call");
						break;
					}

					else if (f_f1 == 4)
					{
						if (f_f2 == 4) return Intersect_Methods.Intersect((Ray)f1, (Ray)f2, FigureColor);

						else if (f_f2 == 5) return Intersect_Methods.Intersect((Ray)f1, (Arc)f2, FigureColor);

						else if (f_f2 == 6) return Intersect_Methods.Intersect((Ray)f1, (Circle)f2, FigureColor);

						else Input_Error("Invalid parameters in intersect call");
						break;
					}

					else if (f_f1 == 5)
					{
						if (f_f2 == 5) return Intersect_Methods.Intersect((Arc)f1, (Arc)f2, FigureColor);

						else if (f_f2 == 6) return Intersect_Methods.Intersect((Arc)f1, (Circle)f2, FigureColor);

						else Input_Error("Invalid parameters in intersect call");
						break;
					}

					else if (f_f1 == 6)
					{
						if (f_f2 == 6) return Intersect_Methods.Intersect((Circle)f1, (Circle)f2, FigureColor);

						else Input_Error("Invalid parameters in intersect call");
						break;
					}

					else Input_Error("Invalid parameters in intersect call");
					break;

				//si el nodo es randoms, devuelve una secuencia de valores float 
				case "randoms":
					if (Scopes[Scopes.Count - 1].ContainsKey("randoms")) return Scopes[Scopes.Count - 1]["randoms"];
					else
					{
						dynamic randoms = new FloatSequence();

						if (Scopes[Scopes.Count - 1].ContainsKey("randoms"))
						{
							Scopes[Scopes.Count - 1]["randoms"] = randoms;
						}
						else
						{
							Scopes[Scopes.Count - 1].Add("randoms", randoms);
						}
						return randoms;
					}
				//si el nodo es samples, devuelve una secuencia de puntos finita
				case "samples":
					if (Scopes[Scopes.Count - 1].ContainsKey("samples")) return Scopes[Scopes.Count - 1]["samples"];
					else
					{
						dynamic samples = new PointSequence("samples", FigureColor, true);

						if (Scopes[Scopes.Count - 1].ContainsKey("samples"))
						{
							Scopes[Scopes.Count - 1]["samples"] = samples;
						}
						else
						{
							Scopes[Scopes.Count - 1].Add("samples", samples);
						}
						return samples;
					}

				//si el nodo es de tipo count, analizar el nodo de la secuencia y hallar el largo
				case "count":
					var count_seq = Evaluate(node.Children[0]);
					(bool b, int j) = IsSequence(count_seq); //hago toda esta travesia horrible porque como es un var, no puedo llamar directamente a la lista de la secuencia
					if (b)
					{
						if (j == 1)
						{
							PointSequence s = (PointSequence)count_seq;
							return s.Count;
						}
						else if (j == 2)
						{
							LineSequence s = (LineSequence)count_seq;
							return s.Count;
						}
						else if (j == 3)
						{
							SegmentSequence s = (SegmentSequence)count_seq;
							return s.Count;
						}
						else if (j == 4)
						{
							RaySequence s = (RaySequence)count_seq;
							return s.Count;
						}
						else if (j == 5)
						{
							ArcSequence s = (ArcSequence)count_seq;
							return s.Count;
						}
						else if (j == 6)
						{
							CircleSequence s = (CircleSequence)count_seq;
							return s.Count;
						}
						else if (j == 7)
						{
							StringSequence s = (StringSequence)count_seq;
							return s.Count;
						}
						else if (j == 8)
						{
							IntSequence s = (IntSequence)count_seq;
							return s.Count;
						}
						else
						{
							FloatSequence s = (FloatSequence)count_seq;
							return s.Count;
						}
					}
					else
					{
						Input_Error("Invalid parameter in the call of function count");
						break;
					}
				// Si el nodo es el nombre de una función o un parámetro, retorna su valor
				case "f_name":
					return node.Value;
				case "p_name":
					return node.Value;
				case "name":
					return node.Value;
				case "print":
					// Si el nodo es una impresión (print), evalúa el nodo hijo y muestra el resultado
					object? value_print = Evaluate(node.Children[0]);
					Console.WriteLine(value_print);
					return value_print;
				// Si el nodo es una función coseno o seno, evalúa el nodo hijo y retorna el coseno o seno del resultado
				case "cos":
					object value_cos = Evaluate(node.Children[0]);
					return Math.Cos((double)value_cos * (Math.PI / 180));//convirtiendo 
				case "sin":
					object? value_sin = Evaluate(node.Children[0]);
					return Math.Sin((double)value_sin * (Math.PI / 180));//convirtiendo
																		 // Si el nodo es una función logaritmo, evalúa los nodos hijos y retorna el logaritmo del segundo resultado 
																		 //en base al primer resultado  
				case "log":
					object? value_agrument = Evaluate(node.Children[0]);
					object? value_base = Evaluate(node.Children[1]);
					return Math.Log((double)value_base, (double)value_agrument);
				// Si el nodo es un condicional, evalúa la condición y retorna la evaluación del primer o segundo nodo hijo 
				//dependiendo de si la condición es verdadera o falsa
				case "Conditional":
					object? condition = Evaluate(node.Children[0]);

					Expected(condition, "bool", "if");

					//si es una secuencia, si esta vacia o  es undefined, devuelve false, si tiene elementos devuelve true
					(bool is_sequence, int a) = IsSequence(condition);
					if (is_sequence) return Evaluate_Sequence_As_Boolean(a, condition, node);

					//si es undefined, devuelve la expresion del else
					else if (condition is string && (condition == "undefined" || condition == "empty_seq")) return Evaluate(node.Children[2]);

					//si es un numero o expresion booleana se evalua 
					if (Convert.ToBoolean(condition) && (condition is double || condition is bool))
					{
						return Evaluate(node.Children[1]);
					}

					//si no es ninguna de las anteriores, devuelve false
					return Evaluate(node.Children[2]);
				// Si el nodo es una función, crea una nueva función y la añade a la lista de funciones declaradas
				case "function":
					Dictionary<string, object> Var = Get_Var_Param(node.Children[1]);
					Function_B function = new Function_B(node.Children[0].Value.ToString(), node.Children[2], Var);
					if (Function_Exist(node.Children[0].Value.ToString()))
					{
						return functions_declared;
						// throw new Exception("The function " + "\' " + node.Children[0].Value + " \'" + "already exist in the current context");
					}
					functions_declared.Add(function);
					return functions_declared;
				// Si el nodo es una función declarada, llama a la función y retorna su valor
				case "declared_function":
					string name = node.Children[0].Value.ToString();
					Node param_f = node.Children[1];
					if (Function_Exist(name))
					{
						Dictionary<string, object> Scope_actual = new Dictionary<string, object>();
						Scopes.Add(Scope_actual);
						int f_position = Call_Function(functions_declared, name, param_f);
						object value = Evaluate(functions_declared[f_position].Operation_Node);
						Scopes.Remove(Scopes[Scopes.Count - 1]);
						return value;
					}
					else
					{
						Input_Error("The function " + name + " does not exist in the current context");
					}
					break;
				// Si el nodo es una lista de asignaciones, guarda las variables en el ámbito actual
				case "assigment_list":
					Save_Var(node);
					break;
				//si el nodo es una variable con un valor, guardarlo en el diccionario de variables globales
				case "global_var_asigment":
					Save_Global_Var(node);
					break;
				//si el nodo es una serie de variables a las que se les tiene que asignar un valor de una secuencia, guardar en diccionario de variables globales
				case "sequence_asigment":
					Save_Value_Of_Sequence(node);
					break;
				// Si el nodo es un bloque Let, evalúa las asignaciones y las operaciones y retorna el resultado de las operaciones
				case "Let":
					Evaluate(node.Children[0]);
					dynamic? result = Evaluate(node.Children[1]);
					Scopes.Remove(Scopes[Scopes.Count - 1]);
					return result;

				//* GEO_WALL_E

				// declaracion de punto aleatorio
				case "point":
					string name_p = node.Children[0].Value.ToString();
					dynamic point = new Point(name_p, FigureColor);
					
					if (Scopes[Scopes.Count - 1].ContainsKey(name_p))
					{
						Scopes[Scopes.Count - 1][name_p] = point;	
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_p, point);
					}
					return point;

				//declaracion de punto definido
				case "point_def":
					string name_pd = node.Children[0].Value.ToString();
					object p1_p = Evaluate(node.Children[1]);
					object p2_p = Evaluate(node.Children[2]);
					dynamic point_def = new Point(name_pd, FigureColor, (double)p1_p, (double)p2_p);
					
					if (Scopes[Scopes.Count - 1].ContainsKey(name_pd))
					{
						Scopes[Scopes.Count - 1][name_pd] = point_def;	
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_pd, point_def);
					}
					return point_def;

				//declaracion de linea aleatoria
				case "line_d":
					string name_ld = node.Children[0].Value.ToString();
					dynamic line = new Line(name_ld, FigureColor);

					if (Scopes[Scopes.Count - 1].ContainsKey(name_ld))
					{
						Scopes[Scopes.Count - 1][name_ld] = line;	
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_ld, line);
					}
					return line;

				//declaracion de segmento aleatorio
				case "segment_d":
					string name_sd = node.Children[0].Value.ToString();
					dynamic segment = new Segment(name_sd, FigureColor);

					if (Scopes[Scopes.Count - 1].ContainsKey(name_sd))
					{
						Scopes[Scopes.Count - 1][name_sd] = segment;	
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_sd, segment);
					}
					return segment;

				//declaracion de rayo aleatorio
				case "ray_d":
					string name_rd = node.Children[0].Value.ToString();
					dynamic ray = new Ray(name_rd, FigureColor);

					if (Scopes[Scopes.Count - 1].ContainsKey(name_rd))
					{
						Scopes[Scopes.Count - 1][name_rd] = ray;	
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_rd, ray);
					}
					return ray;

				//declaracion de arco aleatorio
				case "arc_d":
					string name_ad = node.Children[0].Value.ToString();
					dynamic arc = new Arc(name_ad, FigureColor);

					if (Scopes[Scopes.Count - 1].ContainsKey(name_ad))
					{
						Scopes[Scopes.Count - 1][name_ad] = arc;	
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_ad, arc);
					}
					return arc;

				//declaracion de circulo aleatorio
				case "circle_d":
					string name_cd = node.Children[0].Value.ToString();
					dynamic circle = new Circle(name_cd, FigureColor);

					if (Scopes[Scopes.Count - 1].ContainsKey(name_cd))
					{
						Scopes[Scopes.Count - 1][name_cd] = circle;	
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_cd, circle);
					}
					return circle;

				//declaracion de linea definida
				case "line":
					string name_l = node.Children[0].Value.ToString();
					object? p1_l = Evaluate(node.Children[1]);
					object? p2_l = Evaluate(node.Children[2]);
					dynamic line1 = new Line(name_l, FigureColor, (Point)p1_l, (Point)p2_l);
					
					if (Scopes[Scopes.Count - 1].ContainsKey(name_l))
					{
						Scopes[Scopes.Count - 1][name_l] = line1;
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_l, line1);
					}
					return line1;

				//declaracion de segmento definido
				case "segment":
					string name_s = node.Children[0].Value.ToString();
					object? p1_s = Evaluate(node.Children[1]);
					object? p2_s = Evaluate(node.Children[2]);
					dynamic segment1 = new Segment(name_s, FigureColor, (Point)p1_s, (Point)p2_s);

					if (Scopes[Scopes.Count - 1].ContainsKey(name_s))
					{
						Scopes[Scopes.Count - 1][name_s] = segment1;
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_s, segment1);
					}
					return segment1;

				//declaracion de rayo definido
				case "ray":
					string name_r = node.Children[0].Value.ToString();
					object? p1_r = Evaluate(node.Children[1]);
					object? p2_r = Evaluate(node.Children[2]);
					dynamic ray1 = new Ray(name_r, FigureColor, (Point)p1_r, (Point)p2_r);

					if (Scopes[Scopes.Count - 1].ContainsKey(name_r))
					{
						Scopes[Scopes.Count - 1][name_r] = ray1;
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_r, ray1);
					}
					return ray1;

				//declaracion de arco definido
				case "arc":
					string name_a = node.Children[0].Value.ToString();
					object? p1_a = Evaluate(node.Children[1]);
					object? p2_a = Evaluate(node.Children[2]);
					object? p3_a = Evaluate(node.Children[3]);
					object? m_a = Evaluate(node.Children[4]);
					dynamic arc1 = new Arc(name_a, FigureColor, (Point)p1_a, (Point)p2_a, (Point)p3_a, (Measure)m_a);
					
					if (Scopes[Scopes.Count - 1].ContainsKey(name_a))
					{
						Scopes[Scopes.Count - 1][name_a] = arc1;
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_a, arc1);
					}
					return arc1;

				//declaracion de circulo definido
				case "circle":
					string name_c = node.Children[0].Value.ToString();
					object? p_c = Evaluate(node.Children[1]);
					object? m_c = Evaluate(node.Children[2]);
					dynamic circle1 = new Circle(name_c, FigureColor, (Point)p_c, (Measure)m_c);
					
					if (Scopes[Scopes.Count - 1].ContainsKey(name_c))
					{
						Scopes[Scopes.Count - 1][name_c] = circle1;
					}
					else
					{
						Scopes[Scopes.Count - 1].Add(name_c, circle1);
					}
					return circle1;

				//SECUENCIAS
				//secuencia indefinida
				case "undefined":
					return "undefined";
				//secuencia de llaves con elemento divididos por coma
				case "sequence":
					var sec_l = new List<object>();

					foreach (Node child in node.Children) sec_l.Add(Evaluate(child));

					//si se declara una secuencia vacia explicita, crear una secuencias de puntos vacia, esto esta mal por mi parte
					if (node.Children.Count == 0) return "empty_seq";

					object sec = Evaluate(node.Children[0]);
					int sequence_type_1 = IsKind_Seq(sec);
					(bool o, int sequence_Type_2) = IsSequence(sec);

					//si es solo una secuencia de valores simples, proseguir
					if (!o) switch (sequence_type_1)
						{
							//si es una secuencia de puntos, generarla
							case 1:
								List<Point> p_l = new List<Point>();
								TryConvert(sec_l, p_l);
								PointSequence ps = new PointSequence(p_l);

								//Scopes[Scopes.Count - 1].Add("", ps);
								return ps;
							//si es una secuencia de lineas, generarla
							case 2:
								List<Line> l_l = new List<Line>();
								TryConvert(sec_l, l_l);
								LineSequence ls = new LineSequence(l_l);
								//Scopes[Scopes.Count - 1].Add("", ls);

								return ls;
							//si es una secuencia de segmentos, generarla
							case 3:
								List<Segment> segm_l = new List<Segment>();
								TryConvert(sec_l, segm_l);
								SegmentSequence seg_s = new SegmentSequence(segm_l);
								//Scopes[Scopes.Count - 1].Add("", seg_s);

								return seg_s;
							//si es una secuencia de rayos, generarla
							case 4:
								List<Ray> r_l = new List<Ray>();
								TryConvert(sec_l, r_l);
								RaySequence rs = new RaySequence(r_l);
								//Scopes[Scopes.Count - 1].Add("", rs);

								return rs;
							//si es una secuencia de arcos, generarla
							case 5:
								List<Arc> a_l = new List<Arc>();
								TryConvert(sec_l, a_l);
								ArcSequence arc_s = new ArcSequence(a_l);
								//Scopes[Scopes.Count - 1].Add("", arc_s);

								return arc_s;
							//si es una secuencia de circunferencias, generarla
							case 6:
								List<Circle> c_l = new List<Circle>();
								TryConvert(sec_l, c_l);
								CircleSequence cs = new CircleSequence(c_l);
								//Scopes[Scopes.Count - 1].Add("", cs);

								return cs;
							//si es una secuencia de enteros, generarla
							case 8:
								List<double> i_l = new List<double>();
								TryConvert(sec_l, i_l);
								IntSequence i_s = new IntSequence(i_l);
								//Scopes[Scopes.Count - 1].Add("", i_s);

								return i_s;
							//si es una secuencia de flotantes, generarla
							case 9:
								List<float> f_l = new List<float>();
								TryConvert(sec_l, f_l);
								FloatSequence f_s = new FloatSequence(f_l);
								//Scopes[Scopes.Count - 1].Add("", f_s);

								return f_s;
							//si es una secuencia de string, generarla
							case 7:
								List<string> string_l = new List<string>();
								TryConvert(sec_l, string_l);
								StringSequence string_s = new StringSequence(string_l);
								//Scopes[Scopes.Count - 1].Add("", string_s);

								return string_s;
							//si no es ninguna de las anteriores, devolver error
							default:
								Input_Error("This is not a valid sequence type");
								break;
						}
					//si es una secuencia de secuencias, evaluar el tipo de secuencia de cada una y devolver la concatenacion de todos ellos
					else
					{
						if (sequence_Type_2 == 1)
						{
							PointSequence s = new PointSequence(false);
							List<PointSequence> p_l = new List<PointSequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
						else if (sequence_Type_2 == 2)
						{
							LineSequence s = new LineSequence(false);
							List<LineSequence> p_l = new List<LineSequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
						else if (sequence_Type_2 == 3)
						{
							SegmentSequence s = new SegmentSequence(false);
							List<SegmentSequence> p_l = new List<SegmentSequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
						else if (sequence_Type_2 == 4)
						{
							RaySequence s = new RaySequence(false);
							List<RaySequence> p_l = new List<RaySequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
						else if (sequence_Type_2 == 5)
						{
							ArcSequence s = new ArcSequence(false);
							List<ArcSequence> p_l = new List<ArcSequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
						else if (sequence_Type_2 == 6)
						{
							CircleSequence s = new CircleSequence(false);
							List<CircleSequence> p_l = new List<CircleSequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
						else if (sequence_Type_2 == 7)
						{
							StringSequence s = new StringSequence(false);
							List<StringSequence> p_l = new List<StringSequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
						else if (sequence_Type_2 == 8)
						{
							IntSequence s = new IntSequence(false);
							List<IntSequence> p_l = new List<IntSequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
						else
						{
							FloatSequence s = new FloatSequence(false);
							List<FloatSequence> p_l = new List<FloatSequence>();
							TryConvert(sec_l, p_l);
							foreach (var x in p_l) s = s.Concat(x);
							return s;
						}
					}

					break;
				//secuencia de llaves con elementos divididos por puntos suspensitvos
				case "inf_sequence":

					var sec_inf = new List<object>();
					//evaluar cada nodo 
					foreach (Node child in node.Children) sec_inf.Add(Evaluate(child));
					//si no son valores enteros retornar error
					foreach (object x in sec_inf) if (!IsInt(sec_inf[0].ToString())) Input_Error("This is not a valid sequence type");
					IntSequence inf_seq;
					//si tiene cota superior e inferior
					if (sec_inf.Count() == 2) inf_seq = new IntSequence((double)sec_inf[0], (double)sec_inf[1]);
					//si no tiene cota superior es infinita
					else inf_seq = new IntSequence((double)sec_inf[0]);
					//Scopes[Scopes.Count - 1].Add("", inf_seq);

					return inf_seq;

				//declaracion de secuencia de puntos aleatorio
				case "point_sequence":
					string name_p_seq = node.Children[0].Value.ToString();
					PointSequence p_seq = new PointSequence(name_p_seq, FigureColor, true);
					Scopes[Scopes.Count - 1].Add(name_p_seq, p_seq);

					return p_seq;

				//declaracion de secuencia de lineas aleatoria
				case "line_sequence":
					string name_l_seq = node.Children[0].Value.ToString();
					LineSequence l_seq = new LineSequence(name_l_seq, FigureColor, true);
					Scopes[Scopes.Count - 1].Add(name_l_seq, l_seq);

					return l_seq;

				//declaracion de secuencia de segemntos aleatorios
				case "segment_sequence":
					string name_s_seq = node.Children[0].Value.ToString();
					SegmentSequence s_seq = new SegmentSequence(name_s_seq, FigureColor, true);
					Scopes[Scopes.Count - 1].Add(name_s_seq, s_seq);

					return s_seq;

				//declaracion de secuencia de rayos aleatorios
				case "ray_sequence":
					string name_r_seq = node.Children[0].Value.ToString();
					SegmentSequence r_seq = new SegmentSequence(name_r_seq, FigureColor, true);
					Scopes[Scopes.Count - 1].Add(name_r_seq, r_seq);

					return r_seq;

				//declaracion de secuencia de arcos aleatorios
				case "arc_sequence":
					string name_a_seq = node.Children[0].Value.ToString();
					SegmentSequence a_seq = new SegmentSequence(name_a_seq, FigureColor, true);
					Scopes[Scopes.Count - 1].Add(name_a_seq, a_seq);

					return a_seq;

				//declaracion de secuencia de circunferencias aleatoria
				case "circle_sequence":
					string name_c_seq = node.Children[0].Value.ToString();
					SegmentSequence c_seq = new SegmentSequence(name_c_seq, FigureColor, true);
					Scopes[Scopes.Count - 1].Add(name_c_seq, c_seq);

					return c_seq;

				// Si el nodo no coincide con ninguno de los anteriores lanza un error
				default:
					throw new Exception("SEMANTIC ERROR: Unknown operator: " + node.Type);
			
			}
			return 0;
		}

		#region Auxiliar

		// Metodo para obtener un diccionario con los parametros que se declaran en una funcion
		Dictionary<string, dynamic> Get_Var_Param(Node parameters)
		{
			// Crea un nuevo diccionario para almacenar los parámetros
			Dictionary<string, dynamic> Param = new Dictionary<string, dynamic>();

			// Para cada parámetro, añade su nombre al diccionario con un valor inicial de null
			for (int i = 0; i < parameters.Children.Count; i++)
			{
				Node node = (Node)parameters.Children[i].Value;
				string? name = node.Value.ToString();
				Param.Add(name, null);
			}
			return Param;
		}

		//Metodo para alamacenar y asignar la variable global declarada en codigo
		void Save_Global_Var(Node global_var)
		{

			string name = global_var.Children[0].Value.ToString();
			dynamic value = Evaluate(global_var.Children[1]);

			// Si el nombre de la variable coincide con el nombre de una función existente, lanza una excepción
			if (Function_Exist(name)) Input_Error("The variable " + name + " already has a definition as a function in the current context");

			//si la variable ya existe en el diccionario, lanzar excepcion
			//TODO if (Scopes[Scopes.Count - 1].ContainsKey(name)) Input_Error("The variable " + name + " already has a definition in the current context");
			
			
			if (Scopes[Scopes.Count - 1].ContainsKey(name))
				Scopes[Scopes.Count - 1][name] = value;

			else Scopes[Scopes.Count - 1].Add(name, value);
		}

		//metodo para almacenar y asignar las variables globales a las que se les asignan valores de una sequencia
		void Save_Value_Of_Sequence(Node seq_asign)
		{
			//evaluar la secuencia 
			var sequence = Evaluate(seq_asign.Children[1].Children[0]);
			(bool b, int i) = IsSequence(sequence);
			Node fam;

			//si es una secuencia, evaluar los valores y asignarlos
			if (b)
			{
				if (i == 1)
				{
					PointSequence s = (PointSequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}
				else if (i == 2)
				{
					LineSequence s = (LineSequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}
				else if (i == 3)
				{
					SegmentSequence s = (SegmentSequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}
				else if (i == 4)
				{
					RaySequence s = (RaySequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}
				else if (i == 5)
				{
					ArcSequence s = (ArcSequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}
				else if (i == 6)
				{
					CircleSequence s = (CircleSequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}
				else if (i == 7)
				{
					StringSequence s = (StringSequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}
				else if (i == 8)
				{
					IntSequence s = (IntSequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}
				else
				{
					FloatSequence s = (FloatSequence)sequence;
					fam = s.Return_Global_Var(seq_asign.Children[0]);
				}

				Sub_Save_Sequence_Value(fam);
			}
			//si es una secuencia undefined, asignar a los valores undefined
			else if (sequence == "undefined")
			{
				PointSequence s = new PointSequence(true);
				fam = s.Return_Global_Var(seq_asign.Children[0]);
				Sub_Save_Sequence_Value(fam);
			}
			//si es una secuencia vacia, asignarle valores
			else if (sequence == "empty")
			{
				PointSequence s = new PointSequence(new List<Point>());
				fam = s.Return_Global_Var(seq_asign.Children[0]);
				Sub_Save_Sequence_Value(fam);
			}
			//error
			else Input_Error("Invalid value asignation, it must be a sequence");
		}

		void Sub_Save_Sequence_Value(Node fam)
		{
			//itero por las variables con sus valores y las guardo en el diccionario
			foreach (Node child in fam.Children)
			{
				string name = child.Type;
				dynamic value = child.Value;

				// Si el nombre de la variable coincide con el nombre de una función existente, lanza una excepción
				if (Function_Exist(name)) Input_Error("The variable " + name + " already has a definition as a function in the current context");

				
				if (Scopes[Scopes.Count - 1].ContainsKey(name))
					Scopes[Scopes.Count - 1][name] = value;
				
				else Scopes[Scopes.Count - 1].Add(name, value);
			}
		}

		//metodo para intentar convertir los elementos de una lista de objects a un tipo T
		void TryConvert<T>(List<object> list1, List<T> list2)
		{
			foreach (object x in list1)
			{
				if (x is not T) Input_Error("Sorry, " + x + " is not a valid type");
				list2.Add((T)x);
			}
		}

		//saber si un object es int
		bool IsInt(string objectt)
		{
			foreach (char x in objectt) if (x == '.' || !char.IsDigit(x)) return false;
			return true;
		}

		//metodo para saber si un objeto contenido en una lista es valido para una secuencia
		int IsKind_Seq(object figure)
		{
			if (figure is Point) return 1;
			if (figure is Line) return 2;
			if (figure is Segment) return 3;
			if (figure is Ray) return 4;
			if (figure is Arc) return 5;
			if (figure is Circle) return 6;
			if (figure is string) return 7;
			if (figure is double) return 8;
			if (figure is float) return 9;
			else return -1;
		}

		//saber si un objecto es una secuencia
		(bool, int) IsSequence<T>(T objectt)
		{
			if (objectt is PointSequence) return (true, 1);
			if (objectt is LineSequence) return (true, 2);
			if (objectt is SegmentSequence) return (true, 3);
			if (objectt is RaySequence) return (true, 4);
			if (objectt is ArcSequence) return (true, 5);
			if (objectt is CircleSequence) return (true, 6);
			if (objectt is StringSequence) return (true, 7);
			if (objectt is IntSequence) return (true, 8);
			if (objectt is FloatSequence) return (true, 9);
			return (false, 0);
		}

		//metodo que evalua la secuencia en el if y determina si representa true o false en el llamado
		object Evaluate_Sequence_As_Boolean(int a, object sequence, Node node)
		{
			switch (a)
			{
				case 1:
					PointSequence s = (PointSequence)sequence;
					if ((s.values.Count == 1 && s.values[0].Count == 0) || s.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 2:
					LineSequence l = (LineSequence)sequence;
					if ((l.values.Count == 1 && l.values[0].Count == 0) || l.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 3:
					SegmentSequence ss = (SegmentSequence)sequence;
					if ((ss.values.Count == 1 && ss.values[0].Count == 0) || ss.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 4:
					RaySequence r = (RaySequence)sequence;
					if ((r.values.Count == 1 && r.values[0].Count == 0) || r.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 5:
					ArcSequence aa = (ArcSequence)sequence;
					if ((aa.values.Count == 1 && aa.values[0].Count == 0) || aa.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 6:
					CircleSequence c = (CircleSequence)sequence;
					if ((c.values.Count == 1 && c.values[0].Count == 0) || c.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 7:
					StringSequence sss = (StringSequence)sequence;
					if ((sss.values.Count == 1 && sss.values[0].Count == 0) || sss.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 8:
					IntSequence i = (IntSequence)sequence;
					if ((i.values.Count == 1 && i.values[0].Count == 0) || i.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				default:
					FloatSequence f = (FloatSequence)sequence;
					if ((f.values.Count == 1 && f.values[0].Count == 0) || f.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
			}
		}

		void Draw_Sequence<T>(ISequence<T> s)
		{
			//no admite secuencias infinitas o indefinidas
			if (s.is_infinite || s.is_undefined) Input_Error("Infinie or undefined sequences can't be drawed");

			else
			{
				foreach (var c in s.values)
				{
					foreach (var v in c)
					{
						//dibujar secuencias no admiten texto de entrada 
						IDrawable fig = (IDrawable)v;
						Drawables.Add(fig);
					}
				}
			}
		}

		// Metodo para almacenar y asignar las variables que se declaran en el LET
		void Save_Var(Node Children_assigment_list)
		{
			// Crea un nuevo diccionario para almacenar las variables del bloque Let
			Dictionary<string, dynamic> Var_let_in = new Dictionary<string, dynamic>();
			
			// Añade todas las variables del ámbito actual al nuevo diccionario
			foreach (string key in Scopes[Scopes.Count - 1].Keys)
			{
				Var_let_in.Add(key, Scopes[Scopes.Count - 1][key]);
			}

			// Añade el nuevo diccionario de variables al ámbito actual
			Scopes.Add(Var_let_in);

			// Para cada asignación en la lista de asignaciones, evalúa el valor y añade la variable al nuevo diccionario
			foreach (Node Child in Children_assigment_list.Children)
			{
				Evaluate(Child);

				
			}
		}

		// Metodo que llama a una funcion ya declarada
		int Call_Function(List<Function_B> f, string name, Node param)
		{
			bool is_found = false;
			// Recorre la lista de funciones declaradas
			for (int i = 0; i < f.Count; i++)
			{
				// Si encuentra una función con el mismo nombre
				if (f[i].Name_function == name)
				{
					is_found = true;
					// Si la función tiene el mismo número de parámetros
					if (f[i].variable_param.Count == param.Children.Count)
					{
						// Añade todas las variables del ámbito anterior al ámbito actual
						foreach (string key in Scopes[Scopes.Count - 2].Keys)
						{
							Scopes[Scopes.Count - 1].Add(key, Scopes[Scopes.Count - 2][key]);
						}

						int count = 0;
						// Para cada parámetro de la función, actualiza su valor en el ámbito actual
						foreach (string key in f[i].variable_param.Keys)
						{
							f[i].variable_param[key] = param.Children[count].Value;
							if (Scopes[Scopes.Count - 1].ContainsKey(key))
							{
								Scopes[Scopes.Count - 1][key] = Evaluate((Node)param.Children[count].Value);
								count++;
							}
							else
							{
								Scopes[Scopes.Count - 1].Add(key, Evaluate((Node)param.Children[count].Value));
								count++;
							}
						}

						return i;
					}
					// Si no coincide el numero de parametros de la funcion con los introducidos a la hora de llamarla
					//lanza un error
					else
					{
						Input_Error("Function " + name + " receives " + f[i].variable_param.Count + " argument(s), but " + param.Children.Count + " were given.");
					}
				}
			}
			// Si no se encuentra la funcion, no se ha declarado, lanza un error
			if (!is_found)
			{
				Input_Error("The function " + name + " has not been declared");
			}

			return -1;
		}

		// Metodo que verifica si la funcion existe declarada
		bool Function_Exist(string? name)
		{
			// Recorre la lista de funciones declaradas
			foreach (Function_B b in functions_declared)
			{
				// Si encuentra una función con el mismo nombre, retorna true
				if (b.Name_function == name)
				{
					return true;
				}
			}
			// Si no encuentra ninguna función con ese nombre, retorna false
			return false;
		}

		// Método que lanza una excepción con un mensaje de error semantico
		void Input_Error(string error)
		{
			throw new Exception("SEMANTIC ERROR: " + error);
		}

		// Metodo que verifica si dos valores son del mismo tipo (del tipo desperado)
		void Type_Expected(object value1, object value2, string type, string op)
		{
			// Si los valores son del tipo esperado, no hace nada
			if (value1 is double && value2 is double && type == "number")
			{
				return;
			}
			else if (value1 is string && value2 is string && type == "string")
			{
				return;
			}
			else if (value1 is bool && value2 is bool && type == "boolean")
			{
				return;
			}
			// Si los valores no son del tipo esperado, lanza una excepción
			else
			{
				Input_Error("Operator \'" + op + "\' cannot be used between \'" + Identify(value1) + "\' and \'" + Identify(value2) + "\'");
			}
		}

		// Metodo que dependiendo del tipo esperado, verifica si el valor es de ese tipo
		void Expected(object value1, string type, string express)
		{
			string v1_type = Identify(value1);

			if (v1_type == type) return;

			switch (type)
			{
				case "string":
					if (value1 is string)
						return;
					break;
				case "number":
					if (value1 is double)
						return;
					break;
				case "bool":
					(bool b, _) = IsSequence(value1);
					if (value1 is bool || value1 is double || b || value1 is string)  //anadido si la expresion booleana es un numero, secuencia o booleana
						return;
					break;
				default:
					Input_Error("The \'" + express + "\' expression must receive type \'" + value1 + "\'");
					break;
			}

			Input_Error("The \'" + express + "\' expression must receive type \'" + type + "\'");

		}


		string Identify(object value)
		{
			if (value is string) return "string";
			if (value is double) return "number";
			if (value is bool) return "bool";
			return "Unknown";
		}
	}
	#endregion
}
