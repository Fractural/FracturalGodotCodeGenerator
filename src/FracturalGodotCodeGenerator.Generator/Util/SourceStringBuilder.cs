using System;
using System.Text;

namespace FracturalGodotCodeGenerator.Generator.Util
{
    public class SourceStringBuilder
    {
        private readonly StringBuilder sourceBuilder = new();
        private readonly StringBuilder indentPrefix = new();

        public void Line(params string[] parts)
        {
            if (parts.Length != 0)
            {
                sourceBuilder.Append(indentPrefix);

                foreach (var s in parts)
                {
                    sourceBuilder.Append(s);
                }
            }

            sourceBuilder.AppendLine();
        }

        public void BlockTab(Action writeInner)
        {
            BlockPrefix("\t", writeInner);
        }

        public void BlockPrefix(string delimiter, Action writeInner)
        {
            indentPrefix.Append(delimiter);
            writeInner();
            indentPrefix.Remove(indentPrefix.Length - delimiter.Length, delimiter.Length);
        }

        public void BlockBrace(Action writeInner)
        {
            Line("{");
            BlockTab(writeInner);
            Line("}");
        }

        public void BlockDecl(Action writeInner)
        {
            Line("{");
            BlockTab(writeInner);
            Line("};");
        }

        public void NamespaceBlockBraceIfExists(string? ns, Action writeInner)
        {
            if (ns is { Length: > 0 })
            {
                Line("namespace ", ns);
                BlockBrace(writeInner);
            }
            else
            {
                BlockPrefix("", writeInner);
            }
        }

        public override string ToString()
        {
            return sourceBuilder + Environment.NewLine +
                "/*" + Environment.NewLine +
                "---- END OF GENERATED SOURCE TEXT ----" + Environment.NewLine +
                "Roslyn doesn't clear the file when writing debug output for" + Environment.NewLine +
                "EmitCompilerGeneratedFiles, so I'm writing this message to" + Environment.NewLine +
                "make it more obvious what's going on when that happens." + Environment.NewLine +
                "*/" + Environment.NewLine;
        }
    }
}
