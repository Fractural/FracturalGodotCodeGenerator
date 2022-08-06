using Fractural.GodotCodeGenerator.Attributes;
using Godot;

public partial class FetchByGenericInterface<T> : Node where T : class, IShout
{
    [OnReadyGet] public T F { get; set; }
}
