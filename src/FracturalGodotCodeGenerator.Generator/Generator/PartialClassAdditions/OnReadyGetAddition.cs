using Fractural.GodotCodeGenerator.Generator.Util;
using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Fractural.GodotCodeGenerator.Generator.PartialClassAdditions
{
    public class OnReadyGetAdditionStrategy : PartialClassAdditionStrategy
    {
        private INamedTypeSymbol nodeSymbol;
        private INamedTypeSymbol resourceSymbol;

        private INamedTypeSymbol onReadyGetAttributeSymbol;

        public override void Init(GeneratorExecutionContext context)
        {
            base.Init(context);
            nodeSymbol = GetSymbolByName($"{GodotNamespace}.Node");
            resourceSymbol = GetSymbolByName($"{GodotNamespace}.Resource");
            onReadyGetAttributeSymbol = GetAttributeByName("OnReadyGet"); //GetSymbolByName(typeof(OnReadyGetAttribute).FullName);
        }

        public override PartialClassAddition? Use(MemberAttributeSite site)
        {
            if (!SymbolEquals(site.Attribute.AttributeClass, onReadyGetAttributeSymbol))
                return null;

            //OnReadyGetAttribute castedAttr = site.Attribute.MapToType<OnReadyGetAttribute>();
            var member = site.Member;
            if (member.Type.IsOfBaseType(nodeSymbol))
            {
                return new OnReadyGetNodeAddition(site);
            }
            else if (site.Attribute.NamedArguments
                .Any(a => a.Key == "Property" && a.Value.Value is string { Length: > 0 }))
            {
                // If a Property path is given, always treat it as a node. The
                // primary use of Property is to get a Resource from a Node that
                // doesn't have a statically-typed .NET API.
                return new OnReadyGetNodeAddition(site);
            }
            else if (member.Type.IsOfBaseType(resourceSymbol))
            {
                return new OnReadyGetResourceAddition(site);
            }
            else if (member.Type.IsInterface())
            {
                // Assume an interface means the intent is to get a node. This is
                // ambiguous: it could be a resource! But this is unlikely.
                // See https://github.com/31/GodotOnReady/issues/30
                return new OnReadyGetNodeAddition(site);
            }
            else if (member.Type.TypeKind == TypeKind.TypeParameter)
            {
                if (member.Type.IsReferenceType)
                {
                    // Assume any T is a node. This works with GetNode because it's
                    // only constrained to "class", not "Node". This assumption means
                    // that a "Fetcher<T> where ... { [OnReadyGet] T Foo; }" can be used
                    // for both interface and node values of T.
                    return new OnReadyGetNodeAddition(site);
                }
                else
                {
                    context.ReportDiagnostic(Rules.FGCG0003(member));
                }
            }
            else
            {
                context.ReportDiagnostic(Rules.FGCG0006(member, "Node", "Resource", "interface"));
            }
            return null;
        }
    }

    public abstract class OnReadyGetAddition : PartialClassAddition
    {
        public IMemberSymbol Member { get; }

        /// <summary>
        /// The name of the export property, without any suffix that differentiates it from the main
        /// member (such as Resource or Path).
        /// </summary>
        public string SuffixlessExportPropertyName { get; }

        public string? Path { get; }
        public bool Export { get; }
        public bool OrNull { get; }
        public string? Property { get; }
        public bool NonRecursive { get; }
        public bool Unowned { get; }

        public OnReadyGetAddition(MemberAttributeSite memberSite)
            : base(memberSite.Class)
        {
            Member = memberSite.Member;

            // Handle field name convention: _ prefix with lowercase name.
            string pathName = Member.Name.TrimStart('_');
            pathName =
                pathName[0].ToString().ToUpperInvariant() +
                pathName.Substring(1);

            SuffixlessExportPropertyName = pathName;

            foreach (var constructorArg in memberSite.Attribute.ConstructorArguments)
            {
                if (constructorArg.Value is string path)
                {
                    Path = path;
                }
            }

            foreach (var namedArg in memberSite.Attribute.NamedArguments)
            {
                switch (namedArg.Key)
                {
                    case "Path" when namedArg.Value.Value is string s:
                        Path = s;
                        break;
                    case "Export" when namedArg.Value.Value is bool b:
                        Export = b;
                        break;
                    case "OrNull" when namedArg.Value.Value is bool b:
                        OrNull = b;
                        break;
                    case "Property" when namedArg.Value.Value is string s:
                        Property = s;
                        break;
                    case "NonRecursive" when namedArg.Value.Value is bool b:
                        NonRecursive = b;
                        break;
                    case "Unowned" when namedArg.Value.Value is bool b:
                        Unowned = b;
                        break;
                }
            }
        }

        protected void WriteMemberNullCheck(SourceStringBuilder g, string exportMemberName)
        {
            g.Line("if (", Member.Name, " == null)");
            g.BlockBrace(() =>
            {
                g.Line(
                    "throw new NullReferenceException($\"",
                    "'{((Resource)GetScript()).ResourcePath}' member '",
                    Member.Name,
                    "' is unexpectedly null in '{GetPath()}', '{this}'. Ensure '",
                    exportMemberName,
                    "' is set correctly, or set OrNull = true in the attribute to allow null.\");");
            });
        }
    }
}
