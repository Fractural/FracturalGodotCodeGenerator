using Fractural.GodotCodeGenerator.Attributes;
using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        private string GetSingleTypeOrTuple(IEnumerable<string> types)
        {
            int count = types.Count();
            if (count == 1) return types.First();
            else return "(" + string.Join(", ", types) + ")";
        }

        public override Action<SourceStringBuilder>? DeclarationWriter => g =>
        {
            IMethodSymbol eventInvokeMethod = ((IMethodSymbol)eventSite.Event.Type.GetMembers()[1]);
            //#if DEBUG
            //            if (!Debugger.IsAttached)
            //            {
            //                Debugger.Launch();
            //            }
            //#endif
            if (eventInvokeMethod.Parameters.Length == 0)
            {
                // Event with no parameters, therefore simple.
                g.Line("public Task ", eventSite.Event.Name, "Raised()");
                g.BlockBrace(() =>
                {
                    g.Line("var taskCompletionSource = new TaskCompletionSource<bool>();");
                    g.Line(eventSite.Event.Type.ToDisplayString(), " handler = null;");

                    g.IndentText("handler = () => ");
                    g.InlineBlockBrace(() =>
                    {
                        g.Line("taskCompletionSource.SetResult(true);");
                        g.Line(eventSite.Event.Name, " -= handler;");
                    }, ";", true);
                    g.Line(eventSite.Event.Name, " += handler;");
                    g.Line("return taskCompletionSource.Task;");
                });
            }
            else
            {
                // Event with parameters
                IEnumerable<string> eventInvokeMethodParamTypes = eventInvokeMethod.Parameters.Select(x => x.Type.ToDisplayString());
                string returnTupleType = GetSingleTypeOrTuple(eventInvokeMethodParamTypes);

                g.Line("public Task<", returnTupleType, "> ", eventSite.Event.Name, "Raised(Func<", string.Join(", ", eventInvokeMethodParamTypes), ", bool> verify = null)");
                g.BlockBrace(() =>
                {
                    g.Line("var taskCompletionSource = new TaskCompletionSource<", returnTupleType, ">();");
                    g.Line(eventSite.Event.Type.ToDisplayString(), " handler = null;");

                    string argsText = "";
                    int parameterCount = eventInvokeMethod.Parameters.Length;
                    string[] argsList = new string[parameterCount];
                    for (int i = 0; i < parameterCount; i++)
                    {
                        argsList[i] = "arg" + (i + 1).ToString();
                        argsText += argsList[i];
                        if (i < parameterCount - 1)
                            argsText += ", ";
                    }
                    string returnTuple = GetSingleTypeOrTuple(argsList);

                    g.IndentText("handler = (", argsText, ") => ");
                    g.InlineBlockBrace(() =>
                    {
                        g.Line("if (verify != null && !verify(", string.Join(", ", argsList), "))");
                        g.BlockTab(() =>
                        {
                            g.Line("return;");
                        });
                        g.Line("taskCompletionSource.SetResult(", returnTuple, ");");
                        g.Line(eventSite.Event.Name, " -= handler;");
                    }, ";", true);
                    g.Line(eventSite.Event.Name, " += handler;");
                    g.Line("return taskCompletionSource.Task;");
                });
            }
        };

        private EventAttributeSite eventSite;

        public AwaitableAddition(EventAttributeSite eventSite)
            : base(eventSite.Class)
        {
            this.eventSite = eventSite;
        }
    }
}
