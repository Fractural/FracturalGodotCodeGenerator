using Fractural.GodotCodeGenerator.Attributes;
using Godot;

public partial class FetchByGenericInterfaceConcrete : FetchByGenericInterface<NodeImplementingIShout>
{
    [OnReady]
    private void TalkAboutTheButton()
    {
        GD.Print("This shout implementer fetched by generic interface:");
        F.Shout();
    }
}
