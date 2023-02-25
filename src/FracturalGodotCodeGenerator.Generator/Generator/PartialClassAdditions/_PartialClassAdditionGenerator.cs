using Fractural.GodotCodeGenerator.Generator.PartialClassAdditions;
using Fractural.GodotCodeGenerator.Generator.Util;
using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FracturalGodotCodeGenerator.Generator
{
    [Generator]
    public class PartialClassAdditionGenerator : ISourceGenerator
    {
        private GeneratorInitializationContext initializationContext;
        private GeneratorExecutionContext executionContext;

        public void Initialize(GeneratorInitializationContext context)
        {
            initializationContext = context;
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //#if DEBUG
            //            if (!Debugger.IsAttached)
            //            {
            //                Debugger.Launch();
            //            }
            //#endif
            executionContext = context;
            var receiver = context.SyntaxReceiver as SyntaxReceiver ?? throw new Exception();
            var processedSyntaxData = receiver.GenerateProcessedSyntaxData(context);

            List<PartialClassAdditionStrategy> strategies = ReflectionUtils.GetInstances<PartialClassAdditionStrategy>().ToList();
            strategies.ForEach(x => x.Init(context));
            List<PartialClassAddition> additions = new();

            foreach (var classData in processedSyntaxData.ClassData)
            {
                var classSymbol = classData.Symbol;
                var usesPartialClassAddition = false;

                foreach (var attribute in classSymbol.GetAttributes())
                {
                    var site = new ClassAttributeSite(classSymbol, attribute);
                    foreach (var strategy in strategies)
                        if (strategy.TryUse(site, out PartialClassAddition addition)) { additions.Add(addition); usesPartialClassAddition = true; }
                }

                foreach (var memberSymbol in MemberSymbol.CreateAll(classSymbol).ToArray())
                {
                    foreach (var attribute in memberSymbol.UnderlyingSymbol.GetAttributes())
                    {
                        var site = new MemberAttributeSite(classSymbol, memberSymbol, attribute);
                        foreach (var strategy in strategies)
                            if (strategy.TryUse(site, out PartialClassAddition addition)) { additions.Add(addition); usesPartialClassAddition = true; }
                    }
                }

                foreach (var methodSymbol in classSymbol.GetMembers().OfType<IMethodSymbol>())
                {
                    foreach (var attribute in methodSymbol.GetAttributes())
                    {
                        var site = new MethodAttributeSite(classSymbol, methodSymbol, attribute);
                        foreach (var strategy in strategies)
                            if (strategy.TryUse(site, out PartialClassAddition addition)) { additions.Add(addition); usesPartialClassAddition = true; }
                    }
                }

                foreach (var eventSymbol in classSymbol.GetMembers().OfType<IEventSymbol>())
                {
                    foreach (var attribute in eventSymbol.GetAttributes())
                    {
                        var site = new EventAttributeSite(classSymbol, eventSymbol, attribute);
                        foreach (var strategy in strategies)
                            if (strategy.TryUse(site, out PartialClassAddition addition)) { additions.Add(addition); usesPartialClassAddition = true; }
                    }
                }

                // Verify that all uses of partial class additions are for partial classes.
                if (usesPartialClassAddition)
                {
                    var nonPartialDeclarations = classData.NonPartialDeclarations();
                    if (nonPartialDeclarations.Any())
                    {
                        context.ReportDiagnostic(Rules.FGCG0002(classData.Symbol, nonPartialDeclarations.FirstOrDefault().GetLocation()));
                    }
                }
            }

            bool nullable = context.Compilation is CSharpCompilation csc && csc.Options.NullableContextOptions != NullableContextOptions.Disable;

            foreach (var classAdditionGroup in additions.GroupBy(a => a.Class))
            {
                SourceStringBuilder source = CreateInitializedSourceBuilder();

                // If the project has NRT enabled, disable it for our generated code. We can't
                // simply always disable because this is not valid syntax in old versions of C#.
                if (nullable)
                {
                    source.Line("#nullable disable");
                }

                if (classAdditionGroup.Key is not { } classSymbol) continue;

                source.NamespaceBlockBraceIfExists(classSymbol.GetSymbolNamespaceName(), () =>
                {
                    source.Line("public partial class ", classAdditionGroup.Key.Name);
                    if (classAdditionGroup.Key.IsGenericType)
                    {
                        source.BlockTab(() =>
                        {
                            source.Line(
                                "<",
                                string.Join(
                                    ", ",
                                    classAdditionGroup.Key.TypeParameters
                                        .Select(p => p.ToFullDisplayString())),
                                ">");
                        });
                    }

                    source.BlockBrace(() =>
                    {
                        foreach (var addition in classAdditionGroup)
                        {
                            addition.DeclarationWriter?.Invoke(source);
                        }

                        if (classAdditionGroup.Any(a => a.ConstructorStatementWriter is not null))
                        {
                            source.Line();
                            source.Line("public ", classAdditionGroup.Key.Name, "()");
                            source.BlockBrace(() =>
                            {
                                foreach (var addition in classAdditionGroup.OrderBy(a => a.Order))
                                {
                                    addition.ConstructorStatementWriter?.Invoke(source);
                                }

                                source.Line("Constructor();");
                            });

                            source.Line("partial void Constructor();");
                        }

                        if (classAdditionGroup.Any(a => a.OnReadyStatementWriter is not null))
                        {
                            source.Line();
                            source.Line("public override void _Ready()");
                            source.BlockBrace(() =>
                            {
                                source.Line("base._Ready();");

                                // OrderBy is a stable sort.
                                // Sort by Order, then by discovery order (implicitly).
                                foreach (var addition in classAdditionGroup.OrderBy(a => a.Order))
                                {
                                    addition.OnReadyStatementWriter?.Invoke(source);
                                }
                            });
                        }
                    });

                    foreach (var addition in classAdditionGroup)
                    {
                        addition.OutsideClassStatementWriter?.Invoke(source);
                    }
                });

                foreach (var d in additions.SelectMany(a => a.Diagnostics))
                {
                    context.ReportDiagnostic(d);
                }

                string escapedNamespace =
                    classAdditionGroup.Key.GetSymbolNamespaceName()?.Replace(".", "_") ?? "";

                context.AddSource(
                    $"Partial_{escapedNamespace}_{classAdditionGroup.Key.Name}",
                    source.ToString());
            }
        }

        private static SourceStringBuilder CreateInitializedSourceBuilder()
        {
            var builder = new SourceStringBuilder();
            builder.Line("using Godot;");
            builder.Line("using System;");
            builder.Line("using System.Threading.Tasks;");
            builder.Line();
            return builder;
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> AllClasses { get; } = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax cds)
                {
                    AllClasses.Add(cds);
                }
            }

            public ProcessedSyntaxData GenerateProcessedSyntaxData(GeneratorExecutionContext context)
            {
                Dictionary<INamedTypeSymbol, ClassData> processedClassesDict = new();

                foreach (var classDecl in AllClasses)
                {
                    INamedTypeSymbol? classSymbol = context.Compilation
                        .GetSemanticModel(classDecl.SyntaxTree)
                        .GetDeclaredSymbol(classDecl) as INamedTypeSymbol;

                    if (classSymbol is null)
                    {
                        context.ReportDiagnostic(Rules.FGCG0001(classDecl));
                    }
                    else
                    {
                        if (processedClassesDict.TryGetValue(classSymbol, out ClassData data))
                            data.Declarations.Add(classDecl);
                        else
                            processedClassesDict.Add(
                                classSymbol,
                                new ClassData(classSymbol,
                                new List<ClassDeclarationSyntax>(new[] { classDecl }))
                            );
                    }
                };

                return new ProcessedSyntaxData(processedClassesDict.Values);
            }

            public record ClassData(INamedTypeSymbol Symbol, ICollection<ClassDeclarationSyntax> Declarations)
            {
                public IEnumerable<ClassDeclarationSyntax> NonPartialDeclarations()
                {
                    return Declarations.Where(x => !x.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)));
                }
            }
            public record ProcessedSyntaxData(IEnumerable<ClassData> ClassData);
        }
    }
}