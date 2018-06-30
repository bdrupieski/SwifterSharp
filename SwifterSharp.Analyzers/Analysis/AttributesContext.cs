using System;
using Microsoft.CodeAnalysis;

namespace SwifterSharp.Analyzers.Analysis
{
    internal class AttributesContext
    {
        readonly Lazy<INamedTypeSymbol> _lazyForceNamedArgumentsAttribute;
        public INamedTypeSymbol ForceNamedArgumentsAttribute => _lazyForceNamedArgumentsAttribute?.Value;

        public AttributesContext(Compilation compilation)
        {
            _lazyForceNamedArgumentsAttribute = new Lazy<INamedTypeSymbol>(() => compilation.GetTypeByMetadataName(typeof(ForceNamedArgumentsAttribute).FullName));
        }
    }
}
