using Antlr4.Runtime;
using FlightQuery.Parser.AntlrParser;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;
using System.IO;

namespace FlightQuery.Parser
{
    public class LangParser
    {
        private string _source;
        private bool _intellisense;

        public LangParser(string source, ExecuteFlags flags)
        {
            Errors = new ErrorsCollection();
            _source = source;
            _intellisense = ((flags & ExecuteFlags.Intellisense) == ExecuteFlags.Intellisense);
        }

        public ErrorsCollection Errors { get; private set; }

        public Element Parse()
        {
            var inputStream = new AntlrInputStream(new StringReader(_source));
            var lexer = new SqlLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new SqlParser(tokenStream);
            parser.AddErrorListener(new ErrorListener(Errors));

            var cst = parser.program();
            if (Errors.Count == 0 || _intellisense)
            {
                if(_intellisense)
                    Errors = new ErrorsCollection();

                return new AstBuilder().VisitProgram(cst);
            }
            
            return null;
        }

        private class ErrorListener : BaseErrorListener
        {
            private ErrorsCollection _errors;
            public ErrorListener(ErrorsCollection errors)
            {
                _errors = errors;
            }

            public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                _errors.Add(new ParseError(msg, new ParseInfo(line, charPositionInLine)));
                
                base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
            }
        }
        
    }

}
