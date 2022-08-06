using Fractural.GodotCodeGenerator.Attributes;
using Godot;

public partial class FetchByGenericButton : FetchByGeneric<Button>
{
    [OnReady] private void TalkAboutTheButton() => GD.Print("Button text is:", F.Text);
}
