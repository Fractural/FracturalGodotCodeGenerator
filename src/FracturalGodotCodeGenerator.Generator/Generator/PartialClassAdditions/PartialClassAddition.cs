#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable CS8601 // Possible null reference assignment.
using FracturalGodotCodeGenerator.Generator.Data;
using FracturalGodotCodeGenerator.Generator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Fractural.GodotCodeGenerator.Generator.PartialClassAdditions
{
    public abstract class PartialClassAdditionStrategy
    {
        public static readonly string AttributesNamespace = $"{nameof(Fractural)}.{nameof(GodotCodeGenerator)}.Attributes";
        public static readonly string GodotNamespace = $"Godot";

        protected GeneratorExecutionContext context;

        protected PartialClassAdditionStrategy(GeneratorExecutionContext context)
        {
            this.context = context;
        }

        protected static bool SymbolEquals(ISymbol? a, ISymbol? b)
        {
            return SymbolEqualityComparer.Default.Equals(a, b);
        }

        protected INamedTypeSymbol GetSymbolByName(string fullName)
        {
            return context.Compilation.GetTypeByMetadataName(fullName)
                ?? throw new Exception($"Can't find {fullName}");
        }

        protected INamedTypeSymbol GetAttributeByName(string attributeName)
        {
            return GetSymbolByName($"{AttributesNamespace}.{attributeName}Attribute");
        }

        public bool TryUse(MemberAttributeSite site, out PartialClassAddition addition)
        {
            addition = Use(site);
            return addition != null;
        }

        public bool TryUse(PropertyAttributeSite site, out PartialClassAddition addition)
        {
            addition = Use(site);
            return addition != null;
        }

        public bool TryUse(FieldAttributeSite site, out PartialClassAddition addition)
        {
            addition = Use(site);
            return addition != null;
        }

        public bool TryUse(ClassAttributeSite site, out PartialClassAddition addition)
        {
            addition = Use(site);
            return addition != null;
        }

        public bool TryUse(MethodAttributeSite site, out PartialClassAddition addition)
        {
            addition = Use(site);
            return addition != null;
        }

        public virtual PartialClassAddition? Use(MemberAttributeSite site)
        {
            return null;
        }

        public virtual PartialClassAddition? Use(PropertyAttributeSite site)
        {
            return null;
        }

        public virtual PartialClassAddition? Use(FieldAttributeSite site)
        {
            return null;
        }

        public virtual PartialClassAddition? Use(ClassAttributeSite site)
        {
            return null;
        }

        public virtual PartialClassAddition? Use(MethodAttributeSite site)
        {
            return null;
        }
    }

    public abstract class PartialClassAddition
    {
        protected PartialClassAddition(INamedTypeSymbol @class)
        {
            Class = @class;
        }

        public INamedTypeSymbol Class { get; }

        public int Order { get; set; }

        public virtual Action<SourceStringBuilder>? DeclarationWriter => null;
        public virtual Action<SourceStringBuilder>? ConstructorStatementWriter => null;
        public virtual Action<SourceStringBuilder>? OnReadyStatementWriter => null;

        public virtual Action<SourceStringBuilder>? OutsideClassStatementWriter => null;

        public ICollection<Diagnostic> Diagnostics { get; private set; } = new List<Diagnostic>();
    }
}
