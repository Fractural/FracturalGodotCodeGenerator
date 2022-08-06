using Fractural.GodotCodeGenerator.Attributes;
using Godot;

public partial class SpawnButton : Button
{
    [OnReadyGet("res://Subgui.tscn")] public PackedScene _scene;

    public virtual void OnPress()
    {
        GetParent().AddChild(_scene.Instance());
    }
}
