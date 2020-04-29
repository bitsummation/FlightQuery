using FlightQuery.Parser;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Context
{ 
    public class RunContext
    {
        private string _source;
        private IHttpExecutor _semanticHttpExecutor;
        private IHttpExecutor _httpExecutor;
        private ExecuteFlags _flags;
        private Cursor _editorCursor;

        public ScopeModel ScopeModel { get; private set; }

        public ErrorsCollection Errors { get; private set; }
        public string Authorization { get; private set; }

        public static RunContext CreateRunContext(string code, IHttpExecutor httpExecutor)
        {
            return new RunContext(code, string.Empty, new Cursor(), ExecuteFlags.Run, new EmptyHttpExecutor(), httpExecutor);
        }

        public static RunContext CreateRunContext(string code, string authorization)
        {
            return new RunContext(code, authorization, new Cursor(), ExecuteFlags.Run);
        }

        public static RunContext CreateSemanticContext(string code)
        {
            return new RunContext(code, string.Empty, new Cursor(), ExecuteFlags.Semantic);
        }

        public static RunContext CreateSemanticContext(string code, IHttpExecutor semanticHttpExecutor)
        {
            return new RunContext(code, string.Empty, new Cursor(), ExecuteFlags.Semantic, semanticHttpExecutor);
        }

        public static RunContext CreateIntellisenseContext(string code, Cursor cursor)
        {
            return new RunContext(code, string.Empty, cursor, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
        }

        private RunContext(string source, string authorization, Cursor cursor, ExecuteFlags flags)
            : this(source, authorization, cursor, flags, new EmptyHttpExecutor())
        {
        }

        private RunContext(string source, string authorization, Cursor cursor, ExecuteFlags flags, IHttpExecutor semanticHttpExecutor)
            : this(source, authorization, cursor, flags, semanticHttpExecutor, null)
        {
        }

        private RunContext(string source, string authorization, Cursor cursor, ExecuteFlags flags, IHttpExecutor semanticHttpExecutor, IHttpExecutor httpExecutor)
        {
            _source = source;
            _flags = flags;
            _semanticHttpExecutor = semanticHttpExecutor;
            _httpExecutor = httpExecutor;
            Authorization = authorization;
            _editorCursor = cursor;

            Errors = new ErrorsCollection();
        }

        public SelectTable[] Run()
        {
            SelectTable[] table = null;
            
            var parser = new LangParser(_source, _flags);
            var ast = parser.Parse();
            parser.Errors.ToList().ForEach(x => Errors.Add(x));

            if (Errors.Count == 0)
            {
                //semantic check
                if ((_flags & ExecuteFlags.Semantic) == ExecuteFlags.Semantic)
                {
                    var inter = new Interpreter.Execution.Interpreter(ast, _editorCursor, Authorization, _semanticHttpExecutor);
                    inter.Execute();
                    if (inter.Errors.Count > 0)
                        Errors = inter.Errors;

                    ScopeModel = inter.ScopeModel;
                }

                if (Errors.Count == 0 && ((_flags & ExecuteFlags.Execute) == ExecuteFlags.Execute)) // we run
                {
                    var inter = new Interpreter.Execution.Interpreter(ast, _editorCursor, Authorization, _httpExecutor);
                    table = inter.Execute();
                    Errors = inter.Errors;
                }
            }

            return table;
        }
    }
}
