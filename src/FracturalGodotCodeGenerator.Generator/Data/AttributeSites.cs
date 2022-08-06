using Microsoft.CodeAnalysis;

namespace FracturalGodotCodeGenerator.Generator.Data
{
    public record ClassAttributeSite(
        INamedTypeSymbol Class,
        AttributeData Attribute);

    public record FieldAttributeSite(
        INamedTypeSymbol Class,
        IFieldSymbol Field,
        AttributeData Attribute);

    public record PropertyAttributeSite(
        INamedTypeSymbol Class,
        IPropertySymbol Property,
        AttributeData Attribute);

    public record MemberAttributeSite(
        INamedTypeSymbol Class,
        IMemberSymbol Member,
        AttributeData Attribute);

    public record MethodAttributeSite(
        INamedTypeSymbol Class,
        IMethodSymbol Method,
        AttributeData Attribute);
}
