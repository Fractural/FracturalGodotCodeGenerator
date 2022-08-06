using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FracturalGodotCodeGenerator.Generator.Data
{
    public interface IMemberSymbol
    {
        ITypeSymbol Type { get; }
        string Name { get; }
        ISymbol UnderlyingSymbol { get; }
    }

    public record MemberSymbol(ITypeSymbol Type, string Name, ISymbol UnderlyingSymbol) : IMemberSymbol
    {
        public static MemberSymbol Create(IPropertySymbol member)
        {
            if (member == null) throw new ArgumentNullException(paramName: nameof(member));
            return new(member.Type, member.Name, member);
        }

        public static MemberSymbol Create(IFieldSymbol member)
        {
            if (member == null) throw new ArgumentNullException(paramName: nameof(member));
            return new(member.Type, member.Name, member);
        }

        public static IEnumerable<MemberSymbol> CreateAll(ITypeSymbol type)
        {
            if (type == null) throw new ArgumentNullException(paramName: nameof(type));
            return type.GetMembers().OfType<IPropertySymbol>().Select(Create).Concat(
                type.GetMembers().OfType<IFieldSymbol>().Select(Create));
        }
    }
}
