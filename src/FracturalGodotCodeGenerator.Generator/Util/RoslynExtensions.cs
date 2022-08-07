using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fractural.GodotCodeGenerator.Generator.Util
{
    internal static class RoslynExtensions
    {
        public static string ToFullDisplayString(this ISymbol s)
        {
            return s.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public static string? GetSymbolNamespaceName(this INamedTypeSymbol iTypeNode)
        {
            string? namespaceName = null;
            INamespaceSymbol? ns = iTypeNode.ContainingNamespace;

            while (ns?.ContainingNamespace is { } containing)
            {
                if (namespaceName == null)
                {
                    namespaceName = ns.Name;
                }
                else
                {
                    namespaceName = ns.Name + "." + namespaceName;
                }

                ns = containing;
            }

            return namespaceName;
        }

        public static bool IsOfBaseType(this ITypeSymbol? type, ITypeSymbol baseType)
        {
            if (type is ITypeParameterSymbol p)
            {
                return p.ConstraintTypes.Any(ct => ct.IsOfBaseType(baseType));
            }

            while (type != null)
            {
                if (SymbolEqualityComparer.Default.Equals(type, baseType))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        public static bool IsInterface(this ITypeSymbol? type)
        {
            return type?.TypeKind == TypeKind.Interface;
        }

        public static T MapToType<T>(this AttributeData attributeData) where T : Attribute
        {
            T attribute;
            if (attributeData.AttributeConstructor != null && attributeData.ConstructorArguments.Length > 0)
            {
                attribute = (T)Activator.CreateInstance(typeof(T), attributeData.GetActualConstuctorParams().ToArray());
            }
            else
            {
                attribute = (T)Activator.CreateInstance(typeof(T));
            }
            foreach (var p in attributeData.NamedArguments)
            {
                var field = typeof(T).GetField(p.Key);
                if (field != null)
                    field.SetValue(attribute, p.Value.Value);
                else
                    typeof(T).GetProperty(p.Key).SetValue(attribute, p.Value.Value);
            }
            return attribute;
        }

        public static IEnumerable<object> GetActualConstuctorParams(this AttributeData attributeData)
        {
            foreach (var arg in attributeData.ConstructorArguments)
            {
                if (arg.Kind == TypedConstantKind.Array)
                {
                    // Assume they are strings, but the array that we get from this
                    // should actually be of type of the objects within it, be it strings or ints
                    // This is definitely possible with reflection, I just don't know how exactly. 
                    yield return arg.Values.Select(a => a.Value).OfType<object>().ToArray();
                }
                else
                {
                    yield return arg.Value;
                }
            }
        }
    }
}
