using Fractural.GodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace FracturalGodotCodeGenerator.Generator.Data
{
    public static class Rules
    {
        protected static class Title
        {
            public const string Inspection = "Inspection";
            public const string Usage = "Usage";
        }

        protected enum Category
        {
            Parsing
        }

        public enum Code
        {
            FGCG0001,
            FGCG0002,
            FGCG0003,
            FGCG0004,
            FGCG0005,
            FGCG0006,
            FGCG0007,
            FGCG0008,
            FGCG0009,
            FGCG0010,
        }

        private static string CategoryString(Category category)
        {
            return $"FGCG.{Enum.GetName(typeof(Category), category)}";
        }

        private static Diagnostic CreateDiagnostic(
            Code id,
            string title,
            string messageFormat,
            Category category,
            DiagnosticSeverity defaultSeverity,
            Location location,
            bool isEnabledByDefault = true)
        {
            return Diagnostic.Create(
                new DiagnosticDescriptor(
                    Enum.GetName(typeof(Code), id),
                    title,
                    messageFormat,
                    category,
                    defaultSeverity,
                    isEnabledByDefault
                ),
                location
            );
        }

        /// <summary>
        /// Unable to find declared symbol for class
        /// </summary>
        /// <param name="classDecl"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Diagnostic FGCG0001(ClassDeclarationSyntax classDecl)
        {
            if (classDecl == null) throw new ArgumentNullException(nameof(classDecl));
            return CreateDiagnostic(
                Code.FGCG0001,
                Title.Inspection,
                $"Unable to find declared symbol for {classDecl}. Skipping.",
                Category.Parsing,
                DiagnosticSeverity.Warning,
                classDecl.GetLocation()
            );
        }

        /// <summary>
        /// Missing 'partial' keyword for class that uses partial class additions attributes.
        /// </summary>
        /// <param name="classSymbol"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Diagnostic FGCG0002(INamedTypeSymbol classSymbol, Location location)
        {
            if (classSymbol == null) throw new ArgumentNullException(nameof(classSymbol));
            if (location == null) throw new ArgumentNullException(nameof(location));
            string issue =
                $"Missing 'partial' keyword for class '{classSymbol.Name}', which uses " +
                "partial class additions attributes.";
            return CreateDiagnostic(
                Code.FGCG0002,
                Title.Usage,
                issue,
                Category.Parsing,
                DiagnosticSeverity.Error,
                location
            );
        }

        /// <summary>
        /// The type member is not supported. Expected a class extending one of the following.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="acceptedClasses"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Diagnostic FGCG0006(IMemberSymbol member, params string[] acceptedClasses)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            string issue =
                    $"The type '{member.Type}' of '{member.UnderlyingSymbol}' is not supported." +
                    $" Expected a class extending one of the following: {String.Join(", ", acceptedClasses)}";
            return CreateDiagnostic(
                Code.FGCG0006,
                Title.Inspection,
                issue,
                Category.Parsing,
                DiagnosticSeverity.Error,
                member.UnderlyingSymbol.Locations.FirstOrDefault()
            );
        }

        /// <summary>
        /// The type is a not constrained to reference types
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Diagnostic FGCG0003(IMemberSymbol member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            string issue =
                $"The type '{member.Type}' of '{member.UnderlyingSymbol}' is a " +
                $"type parameter, but not constrained to reference types. " +
                $"Ensure it has the 'where {member.Type} : class' constraint.";
            return CreateDiagnostic(
                Code.FGCG0003,
                Title.Inspection,
                issue,
                Category.Parsing,
                DiagnosticSeverity.Error,
                member.UnderlyingSymbol.Locations.FirstOrDefault()
            );
        }

        /// <summary>
        /// No field or property of type found in injection source
        /// </summary>
        /// <param name="member"></param>
        /// <param name="sourceSymbol"></param>
        /// <returns></returns>
        public static Diagnostic FGCG0004(IMemberSymbol member, INamedTypeSymbol sourceSymbol)
        {
            string issue =
                $"No field or property of type \"{member.Type.ToFullDisplayString()}\" " +
                $"found in injection source \"{sourceSymbol.ToFullDisplayString()}\". " +
                "Expected exactly one";
            return CreateDiagnostic(
                Code.FGCG0004,
                Title.Inspection,
                issue,
                Category.Parsing,
                DiagnosticSeverity.Error,
                member.UnderlyingSymbol.Locations.FirstOrDefault()
            );
        }

        /// <summary>
        /// Mutliple members of type found in injection source
        /// </summary>
        /// <param name="member"></param>
        /// <param name="nodeType"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static Diagnostic FGCG0005(IMemberSymbol member, INamedTypeSymbol nodeType, MemberSymbol[] matches)
        {
            string issue =
                $"Multiple members of type \"{member.Type.ToFullDisplayString()}\" " +
                $"found in injection source {nodeType.ToFullDisplayString()}, " +
                "but expected exactly one. Found: " +
                string.Join(", ", matches.Select(m => m.Name));
            return CreateDiagnostic(
                Code.FGCG0005,
                Title.Inspection,
                issue,
                Category.Parsing,
                DiagnosticSeverity.Error,
                member.UnderlyingSymbol.Locations.FirstOrDefault()
            );
        }
    }
}
