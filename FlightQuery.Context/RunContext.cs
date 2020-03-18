using FlightQuery.Parser;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;

namespace FlightQuery.Context
{ 
    public class RunContext
    {
        private string _source;
        private IHttpExecutor _semanticHttpExecutor;
        private IHttpExecutor _httpExecutor;
        private ExecuteFlags _flags;
        public ErrorsCollection Errors { get; private set; }

        public RunContext(string source)
            : this(source, ExecuteFlags.Run)
        {
        }

        public RunContext(string source, ExecuteFlags flags)
            : this(source, flags, new EmptyHttpExecutor())
        {
        }

        public RunContext(string source, ExecuteFlags flags, IHttpExecutor semanticHttpExecutor)
            : this(source, flags, semanticHttpExecutor, null)
        {
        }

        public RunContext(string source, ExecuteFlags flags, IHttpExecutor semanticHttpExecutor, IHttpExecutor httpExecutor)
        {
            _source = source;
            _flags = flags;
            _semanticHttpExecutor = semanticHttpExecutor;
            _httpExecutor = httpExecutor;
            Errors = new ErrorsCollection();
        }

        public SelectTable Run()
        {
            SelectTable table = null;
            //semantic check
            var ast = LangParser.Parse(_source);

            if ((_flags & ExecuteFlags.Semantic) == ExecuteFlags.Semantic)
            {
                var inter = new Interpreter.Execution.Interpreter(ast, _semanticHttpExecutor);
                inter.Execute();
                if (inter.Errors.Count > 0)
                    Errors = inter.Errors;
            }
            
            if(Errors.Count == 0 && ( (_flags & ExecuteFlags.Execute) == ExecuteFlags.Execute) ) // we run
            {
                var inter = new Interpreter.Execution.Interpreter(ast, _httpExecutor);
                table = inter.Execute();
            }

            return table;
        }
    }
}
