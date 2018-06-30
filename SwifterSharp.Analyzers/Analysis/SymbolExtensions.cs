using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SwifterSharp.Analyzers.Analysis
{
    internal static class SymbolExtensions
    {
        internal static bool ContainsAttributeType(this ImmutableArray<AttributeData> attributes, INamedTypeSymbol attributeType, bool exactMatch = false)
        {
            return attributes.Any(a => attributeType.IsAssignableFrom(a.AttributeClass, exactMatch));
        }

        internal static bool IsAssignableFrom(this ITypeSymbol targetType, ITypeSymbol sourceType, bool exactMatch = false)
        {
            if (targetType != null)
            {
                while (sourceType != null)
                {
                    if (sourceType.Equals(targetType))
                        return true;

                    if (exactMatch)
                        return false;

                    if (targetType.TypeKind == TypeKind.Interface)
                        return sourceType.AllInterfaces.Any(i => i.Equals(targetType));

                    sourceType = sourceType.BaseType;
                }
            }

            return false;
        }
    }
}
