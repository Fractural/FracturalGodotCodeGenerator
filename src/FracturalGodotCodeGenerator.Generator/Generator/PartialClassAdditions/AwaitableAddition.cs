using Fractural.GodotCodeGenerator.Attributes;
using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;

namespace Fractural.GodotCodeGenerator.Generator.PartialClassAdditions
{
    public class AwaitableAdditionStrategy : PartialClassAdditionStrategy
    {
        private INamedTypeSymbol awaitableAttributeSymbol;

        public override void Init(GeneratorExecutionContext context)
        {
            base.Init(context);
            awaitableAttributeSymbol = GetSymbolByName<AwaitableAttribute>();
        }

        public override PartialClassAddition? Use(EventAttributeSite site)
        {
            if (!SymbolEquals(site.Attribute.AttributeClass, awaitableAttributeSymbol))
                return null;

            return new AwaitableAddition(site);
        }
    }

    public class AwaitableAddition : PartialClassAddition
    {
        public override Action<SourceStringBuilder>? DeclarationWriter => g =>
        {
            g.Line("public Task ", eventSite.Event.Name, "_Raised()");
            g.BlockBrace(() =>
            {
                g.Line("var taskCompletionSource = new TaskCompletionSource<bool>();");
                g.Line(eventSite.Event.Type.ToDisplayString(), " handler = null;");
                g.IndentText("handler = (");
                int parameterCount = ((IMethodSymbol)eventSite.Event.Type.GetMembers()[1]).Parameters.Length;
                for (int i = 1; i <= parameterCount; i++)
                {
                    g.Text("arg", i.ToString());
                    if (i < parameterCount)
                        g.Text(", ");
                }
                g.Text(") => ");
                g.InlineBlockBrace(() =>
                {
                    g.Line("taskCompletionSource.SetResult(true);");
                    g.Line(eventSite.Event.Name, " -= handler;");
                }, ";", true);
                g.Line(eventSite.Event.Name, " += handler;");
                g.Line("return taskCompletionSource.Task;");
            });
        };

        private EventAttributeSite eventSite;

        public AwaitableAddition(EventAttributeSite eventSite)
            : base(eventSite.Class)
        {
            this.eventSite = eventSite;
        }
    }
}
