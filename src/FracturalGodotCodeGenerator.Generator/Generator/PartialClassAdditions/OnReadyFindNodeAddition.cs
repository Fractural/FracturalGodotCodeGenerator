using Fractural.GodotCodeGenerator.Generator.Util;
using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace Fractural.GodotCodeGenerator.Generator.PartialClassAdditions
{
    public class OnReadyFindAdditionStrategy : PartialClassAdditionStrategy
    {
        INamedTypeSymbol onReadyFindAttributeSymbol;

        public override void Init(GeneratorExecutionContext context)
        {
            base.Init(context);
            onReadyFindAttributeSymbol = GetAttributeByName("OnReadyFind"); //GetSymbolByName(typeof(OnReadyAttribute).FullName);
        }

        public override PartialClassAddition? Use(MemberAttributeSite site)
        {
            if (SymbolEquals(site.Attribute.AttributeClass, onReadyFindAttributeSymbol))
                return new OnReadyFindNodeAddition(site);
            return null;
        }
    }

    public class OnReadyFindNodeAddition : OnReadyGetAddition
    {
        public OnReadyFindNodeAddition(MemberAttributeSite site) : base(site) { }

        public override Action<SourceStringBuilder>? DeclarationWriter => g =>
        {
            string export = Path is not { Length: > 0 } || Export
                ? "[Export] "
                : "";

            g.Line();
            g.Line(export, "public string ", ExportPropertyName, " { get; set; }");

            g.BlockTab(() =>
            {
                if (Path is { Length: > 0 })
                {
                    g.Line("= ", SyntaxFactory.Literal(Path).ToString(), ";");
                }
                else
                {
                    g.Line("= \"\";");
                }
            });
        };

        public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
        {
            g.Line();

            g.Line("if (", ExportPropertyName, " != null)");
            g.BlockBrace(() =>
            {
                g.Line(Member.Name, " = ",
                    "(", Member.Type.ToFullDisplayString(), ")",
                    "FindNode", "(", ExportPropertyName);

                g.BlockTab(() =>
                {
                    if (NonRecursive)
                    {
                        g.Line(", recursive: false");
                    }
                    if (Unowned)
                    {
                        g.Line(", owned: false");
                    }

                    g.Line(")");

                    if (Property is { Length: > 0 } property)
                    {
                        g.Line("?.Get(", SyntaxFactory.Literal(property).ToString(), ")");
                    }

                    g.Line(";");
                });
            });

            if (!OrNull)
            {
                WriteMemberNullCheck(g, ExportPropertyName);
            }
        };

        protected virtual string ExportPropertyName => SuffixlessExportPropertyName + "Mask";
    }
}
