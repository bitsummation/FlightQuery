namespace FlightQuery.Sdk.Semantic
{
    public class RequiredMissingParameter : SemanticError
    {
        public RequiredMissingParameter(string parameter)
        {
            Variable = parameter;
        }

        public string Variable { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("{0} is required", Variable);
            }
        }

    }
}
