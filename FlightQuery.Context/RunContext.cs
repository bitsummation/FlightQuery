using FlightQuery.Parser;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;
using System.Linq;

namespace FlightQuery.Context
{ 
    public class RunContext
    {
        private string _source;
        private IHttpExecutor _semanticHttpExecutor;
        private IHttpExecutor _httpExecutor;
        private ExecuteFlags _flags;

        public ScopeModel ScopeModel { get; private set; }

        public ErrorsCollection Errors { get; private set; }
        public string Authorization { get; private set; }

        public RunContext(string source, string authorization)
            : this(source, authorization, ExecuteFlags.Run)
        {
        }

        public RunContext(string source, string authorization, ExecuteFlags flags)
            : this(source, authorization, flags, new EmptyHttpExecutor())
        {
        }

        public RunContext(string source, string authorization, ExecuteFlags flags, IHttpExecutor semanticHttpExecutor)
            : this(source, authorization, flags, semanticHttpExecutor, null)
        {
        }

        public RunContext(string source, string authorization, ExecuteFlags flags, IHttpExecutor semanticHttpExecutor, IHttpExecutor httpExecutor)
        {
            _source = source;
            _flags = flags;
            _semanticHttpExecutor = semanticHttpExecutor;
            _httpExecutor = httpExecutor;
            Authorization = authorization;

            Errors = new ErrorsCollection();
        }

        public SelectTable Run()
        {
            SelectTable table = null;
            
            var parser = new LangParser(_source, (_flags & ExecuteFlags.SkipParseErrors) == ExecuteFlags.SkipParseErrors);
            var ast = parser.Parse();
            parser.Errors.ToList().ForEach(x => Errors.Add(x));

            if (Errors.Count == 0)
            {
                //semantic check
                if ((_flags & ExecuteFlags.Semantic) == ExecuteFlags.Semantic)
                {
                    var inter = new Interpreter.Execution.Interpreter(ast, Authorization, _semanticHttpExecutor);
                    inter.Execute();
                    if (inter.Errors.Count > 0)
                        Errors = inter.Errors;

                    ScopeModel = inter.ScopeModel;
                }

                if (Errors.Count == 0 && ((_flags & ExecuteFlags.Execute) == ExecuteFlags.Execute)) // we run
                {
                    var inter = new Interpreter.Execution.Interpreter(ast, Authorization, _httpExecutor);
                    table = inter.Execute();
                    Errors = inter.Errors;
                }
            }

            return table;
        }
    }
}
