using Antlr4.Runtime;
using FlightQuery.Parser.AntlrParser;
using FlightQuery.Sdk.SqlAst;
using System.IO;

namespace FlightQuery.Parser
{
    public class LangParser
    {
        public static Element Parse(string s)
        {
            var inputStream = new AntlrInputStream(new StringReader(s));
            var lexer = new SqlLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new SqlParser(tokenStream);
            parser.AddErrorListener(new ErrorListener());

            var cst = parser.program();
            return new AstBuilder().VisitProgram(cst);
        }

        private class ErrorListener : BaseErrorListener
        {
            public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
            }
        }
        
    }

}
