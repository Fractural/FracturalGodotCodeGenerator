using Fractural.GodotCodeGenerator.Attributes;
using Godot;
using SomeNamespace;

public partial class RandomNumberLabel : Label
{
    [InjectAncestorValue(typeof(MyGui), "Gui")] private NumberService _ns;

    [OnReady]
    private void SetText()
    {
        Text = $"Random number: {_ns.Random()}";
    }
}
