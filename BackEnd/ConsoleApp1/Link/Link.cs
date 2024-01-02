
using G_Wall_E;
using INTERPRETE_C__to_HULK;
using System.Runtime.CompilerServices;

namespace GeoWall_E
{

    public class Link
    {
        
        public static List<IDrawable> Start(string code)
        {
            try
            {
                Semantic_Analyzer analyzer = new Semantic_Analyzer();
                Lexer T = new Lexer(code);
                List<Token> tokens = T.Tokens_sequency;
                Parser parser = new Parser(tokens);
                Node node = parser.Parse();
                analyzer.Read_Parser(node);
                return analyzer.Choice(node);
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
                return new List<IDrawable>();
            }
        }
    }
}