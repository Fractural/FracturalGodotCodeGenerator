using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using System;

namespace Fractural.GodotCodeGenerator.Generator.PartialClassAdditions
{
    public class OnReadyAdditionStrategy : PartialClassAdditionStrategy
    {
        INamedTypeSymbol onReadyAttributeSymbol;

        public override void Init(GeneratorExecutionContext context)
        {
            base.Init(context);
            onReadyAttributeSymbol = GetAttributeByName("OnReady"); //GetSymbolByName(typeof(OnReadyAttribute).FullName);
        }

        public override PartialClassAddition? Use(MethodAttributeSite site)
        {
            if (SymbolEquals(site.Attribute.AttributeClass, onReadyAttributeSymbol))
                return new OnReadyAddition(site);
            return null;
        }
    }

    public class OnReadyAddition : PartialClassAddition
    {
        public IMethodSymbol Method { get; }

        public OnReadyAddition(MethodAttributeSite site)
            : base(site.Class)
        {
            Method = site.Method;

            foreach (var namedArg in site.Attribute.NamedArguments)
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
