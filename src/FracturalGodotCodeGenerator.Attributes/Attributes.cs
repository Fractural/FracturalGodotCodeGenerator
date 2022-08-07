using System;

namespace Fractural.GodotCodeGenerator.Attributes
{
    /// <summary>
    /// Controls what parent type the exported property will accept.
    /// </summary>
    public enum ExportMode
    {
        /// <summary>
        /// Exported property will accept Nodes or Resources, with a preference for Nodes when given an interface.
        /// </summary>
        Auto,
        /// <summary>
        /// Exported property will only accept Nodes.
        /// </summary>
        Node,
        /// <summary>
        /// Exported property will only accept Resources.
        /// </summary>
        Resource,
    }

    /// <summary>
    /// Generates code to initialize this property or field when the node is ready, and make the
    /// initialization path configurable in the Godot editor. This attribute works on properties and
    /// fields of types that subclass either Node or Resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class OnReadyGetAttribute : Attribute
    {
        public string Path { get; }

        /// <summary>
        /// If true, always generates a property with [Export], even if Path isn't set. The exported
        /// property can be used in the editor to change the Path.
        /// </summary>
        public bool Export { get; set; }

        /// <summary>
        /// Allows nulls and unexpected node types in the ready method without throwing exceptions.
        /// Effectively makes the OnReadyGet optional.
        /// </summary>
        public bool OrNull { get; set; }

        /// <summary>
        /// If set, the given property of the node will be injected instead of the node itself.
        /// Path will also always be treated as a Node path, not a Resource path.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Controls what parent type the exported property will accept.
        /// </summary>
        public ExportMode Mode { get; set; } = ExportMode.Auto;

        /// <param name="path">
        /// The path that will be loaded when the node is ready. If not set, a property is generated
        /// with [Export], so the path can be set in the Godot editor.
        /// </param>
        public OnReadyGetAttribute(string path = "", ExportMode mode = ExportMode.Auto)
        {
            Path = path;
            Mode = mode;
        }
    }

    /// <summary>
    /// Generates code to initialize this property or field when the node is ready using FindNode,
    /// with a find mask configurable in the Godot editor. This attribute works only for fields
    /// of types that subclass Node.
    /// </summary>
    public sealed class OnReadyFindAttribute : OnReadyGetAttribute
    {
        /// <summary>
        /// If true, do not search recursively by passing false to FindNode's recursive parameter.
        /// </summary>
        public bool NonRecursive { get; set; }

        /// <summary>
        /// If true, allow FindNode to find unowned nodes by passing false to the "owned" parameter.
        /// </summary>
        public bool Unowned { get; set; }

        /// <param name="mask">
        /// The mask that will be found when the node is ready. If not set, a property is generated
        /// with [Export], so the mask can be set in the Godot editor.
        /// </param>
        public OnReadyFindAttribute(string mask = "") : base(mask) { }
    }

    /// <summary>
    /// Calls the applied 0-argument method during the generated '_Ready' method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class OnReadyAttribute : Attribute
    {
        /// <summary>
        /// Sets the relative order of this method vs. others with this attribute. When deciding
        /// which order to call OnReady methods, the generator first sorts by Order, then by
        /// declaration order. All OnReadyGet initialization happens after the last '-1' Order
        /// OnReady and before the first '0' Order OnReady.
        /// </summary>
        public int Order { get; set; }
    }
}
