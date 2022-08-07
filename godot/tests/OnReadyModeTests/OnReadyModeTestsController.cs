using Fractural.GodotCodeGenerator.Attributes;
using Godot;

namespace Tests.OnReadyModeTests
{
    public partial class OnReadyModeTestsController : Control
    {
        [OnReadyGet(Mode = ExportMode.Resource)]
        public ICustomResourceOrNode resourceManual;
        [OnReadyGet("res://tests/OnReadyModeTests/ConcreteCustomResource.tres", ExportMode.Resource)]
        public ICustomResourceOrNode resourceGetPath;

        [OnReadyGet(Mode = ExportMode.Node)]
        public ICustomResourceOrNode nodeManual;
        [OnReadyGet("HBoxContainer/VBoxContainer/Button1", ExportMode.Node)]
        public ICustomResourceOrNode nodeGetPath;
    }
}