using Microsoft.CodeAnalysis;

namespace SwifterSharp.Analyzers.Analysis
{
    internal class SwifterSharpContext
    {
        public Compilation Compilation { get; }
        public AttributesContext AttributesContext { get; }

        internal SwifterSharpContext(Compilation compilation)
        {
            Compilation = compilation;
            AttributesContext = new AttributesContext(Compilation);
        }
    }
}
