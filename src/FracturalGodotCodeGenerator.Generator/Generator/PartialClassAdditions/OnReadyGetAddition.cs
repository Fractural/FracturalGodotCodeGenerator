using Fractural.GodotCodeGenerator.Attributes;
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
            onReadyGetAttributeSymbol = GetSymbolByName<OnReadyGetAttribute>();
        }

        public override PartialClassAddition? Use(MemberAttributeSite site)
        {
            if (!SymbolEquals(site.Attribute.AttributeClass, onReadyGetAttributeSymbol))
                return null;

            OnReadyGetAttribute castedAttr = site.Attribute.MapToType<OnReadyGetAttribute>();
            if (castedAttr.Property?.Length > 0)
                castedAttr.Mode = ExportMode.Node;

            /// <summary>
            /// Fetches the correct OnReadyGet...Addition class based on the export mode of the attribute.
            /// </summary>
            PartialClassAddition GetCorrectOnReadyGetAddition()
            {
                switch (castedAttr.Mode)
                {
                    case ExportMode.Auto:
                    case ExportMode.Node:
                    default:
                        return new OnReadyGetNodeAddition(site);
                    case ExportMode.Resource:
                        return new OnReadyGetResourceAddition(site);
                }
            }

            bool modeAcceptsNode = castedAttr.Mode == ExportMode.Resource || castedAttr.Mode == ExportMode.Auto;
            bool modeAcceptsResource = castedAttr.Mode == ExportMode.Node || castedAttr.Mode == ExportMode.Auto;

            var member = site.Member;
            if (member.Type.IsOfBaseType(nodeSymbol))
            {
                if (!modeAcceptsNode)
                    context.ReportDiagnostic(
                        Rules.FGCG0007(
                            castedAttr.Mode,
                            nameof(ExportMode.Node),
                            site.Member.UnderlyingSymbol.Locations.FirstOrDefault()
                        )
                    );
                return new OnReadyGetNodeAddition(site);
            }
            else if (member.Type.IsOfBaseType(resourceSymbol))
            {
                if (!modeAcceptsResource)
                    context.ReportDiagnostic(
                        Rules.FGCG0007(
                            castedAttr.Mode,
                            nameof(ExportMode.Resource),
                            site.Member.UnderlyingSymbol.Locations.FirstOrDefault()
                        )
                    );
                return new OnReadyGetResourceAddition(site);
            }
            else if (member.Type.IsInterface())
            {
                return GetCorrectOnReadyGetAddition();
            }
            else if (member.Type.TypeKind == TypeKind.TypeParameter)
            {
                if (member.Type.IsReferenceType)
                {
                    return GetCorrectOnReadyGetAddition();
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
        public ExportMode Mode { get; }

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
                    case "Mode" when namedArg.Value.Value is int i:
                        Mode = (ExportMode)i;
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
