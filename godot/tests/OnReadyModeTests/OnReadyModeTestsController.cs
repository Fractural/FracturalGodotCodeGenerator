using Fractural.GodotCodeGenerator.Attributes;
using Godot;

namespace Tests.OnReadyModeTests
{
    public partial class OnReadyModeTestsController : Control
    {
        [OnReadyGet(Mode = ExportMode.Resource)]
        public ICustomResourceOrNode resourceManual;
        [OnReadyGet("res://tests/OnReadyModeTests/ConcreteCustomResource.tres", Mode = ExportMode.Resource)]
        public ICustomResourceOrNode resourceGetPath;

        [OnReadyGet(Mode = ExportMode.Node)]
        public ICustomResourceOrNode nodeManual;
        [OnReadyGet("ConcreteCustomNode", Mode = ExportMode.Node)]
        public ICustomResourceOrNode nodeGetPath;
        [OnReadyFind("ConcreteCustomNode", Mode = ExportMode.Node)]
        public ICustomResourceOrNode nodeFind;
    }
}