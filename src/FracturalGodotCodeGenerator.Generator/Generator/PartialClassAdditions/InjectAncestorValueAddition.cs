using Fractural.GodotCodeGenerator.Generator.Util;
using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;

namespace Fractural.GodotCodeGenerator.Generator.PartialClassAdditions
{
    public class InjectAncestorValueAdditionStrategy : PartialClassAdditionStrategy
    {
        private INamedTypeSymbol injectAncestorValueAttribute;

        public InjectAncestorValueAdditionStrategy(GeneratorExecutionContext context) : base(context)
        {
            injectAncestorValueAttribute = GetAttributeByName("InjectAncestorValue");
        }

        public override PartialClassAddition? Use(MemberAttributeSite site)
        {
            if (SymbolEquals(site.Attribute.AttributeClass, injectAncestorValueAttribute))
            {
                return new InjectAncestorValueAddition(site);
            }
            return null;
        }
    }

    public class InjectAncestorValueAddition : PartialClassAddition
    {
        public IMemberSymbol Member { get; }

        private readonly string nodeName;
        private readonly INamedTypeSymbol nodeType;
        private readonly MemberSymbol? sourceMember;

        public InjectAncestorValueAddition(MemberAttributeSite memberSite)
            : base(memberSite.Class)
        {
            Member = memberSite.Member;

            nodeType = (INamedTypeSymbol)Member.Type;
            nodeName = nodeType.Name;

            var args = memberSite.Attribute.ConstructorArguments;
            if (args.Length > 0)
            {
                if (args[0].Value is INamedTypeSymbol s)
                {
                    nodeType = s;
                    nodeName = nodeType.Name;
                }
            }
            if (args.Length > 1)
            {
                if (args[1].Value is string s)
                {
                    nodeName = s;
                }
            }

            // We don't want the node, we want a property or field of the node with a specific type.
            if (!Member.Type.Equals(nodeType, SymbolEqualityComparer.Default))
            {
                var matches = MemberSymbol.CreateAll(nodeType)
                    .Where(s =>
                        // Ignore backing fields.
                        !s.UnderlyingSymbol.IsImplicitlyDeclared &&
                        s.Type.IsOfBaseType(Member.Type))
                    .ToArray();

                if (matches.Length == 0)
                {
                    Diagnostics.Add(Rules.FGCG0004(Member, nodeType));
                }
                else if (matches.Length > 1)
                {
                    Diagnostics.Add(Rules.FGCG0005(Member, nodeType, matches));
                }
                else
                {
                    sourceMember = matches[0];
                }
            }
        }

        public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
        {
            if (Diagnostics.Any())
                return;

            string noParent = $"FindParent(\"{nodeName}\") found no parent.";
            string wrongType = $"FindParent(\"{nodeName}\") is not of type \"{nodeType}\".";

            g.Line();
            g.BlockBrace(() =>
            {
                g.Line(
                    "var ancestor = (FindParent(",
                    SyntaxFactory.Literal(nodeName).ToString(),
                    ")",
                    " ?? throw new Exception(",
                    SyntaxFactory.Literal(noParent).ToString(),
                    "))");
                g.BlockTab(() =>
                {
                    g.Line(
                        "as ",
                        nodeType.ToFullDisplayString(),
                        " ?? throw new Exception(",
                        SyntaxFactory.Literal(wrongType).ToString(),
                        ")",
                        ";");
                });
                if (sourceMember is null)
                {
                    g.Line(Member.Name, " = ancestor;");
                }
                else
                {
                    g.Line(Member.Name, " = ancestor.", sourceMember.Name, ";");
                }
            });
        };
    }
}
