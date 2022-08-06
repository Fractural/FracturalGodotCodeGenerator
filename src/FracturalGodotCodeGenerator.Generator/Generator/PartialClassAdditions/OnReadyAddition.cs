using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using System;

namespace Fractural.GodotCodeGenerator.Generator.PartialClassAdditions
{
    public class OnReadyAdditionStrategy : PartialClassAdditionStrategy
    {
        INamedTypeSymbol onReadyAttribute;

        public OnReadyAdditionStrategy(GeneratorExecutionContext context) : base(context)
        {
            onReadyAttribute = GetAttributeByName("OnReadyAttribute");
        }

        public override PartialClassAddition? Use(MethodAttributeSite site)
        {
            if (SymbolEquals(site.Attribute.AttributeClass, )
        }
    }

    public class OnReadyAddition : PartialClassAddition
    {
        public IMethodSymbol Method { get; }

        public OnReadyAddition(
            IMethodSymbol method,
            AttributeData attribute,
            INamedTypeSymbol @class)
            : base(@class)
        {
            Method = method;

            foreach (var namedArg in attribute.NamedArguments)
            {
                if (namedArg.Key == "Order" && namedArg.Value.Value is int i)
                {
                    Order = i;
                }
            }
        }

        public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
        {
            g.Line();
            g.Line(Method.Name, "();");
        };
    }
}
