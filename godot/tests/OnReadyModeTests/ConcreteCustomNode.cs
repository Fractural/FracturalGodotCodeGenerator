using Fractural.CSharpResourceRegistry;
using Godot;

namespace Tests.OnReadyModeTests
{
    [RegisteredType(nameof(ConcreteCustomNode), baseType = nameof(Control))]
    public class ConcreteCustomNode : Control, ICustomResourceOrNode
    {
        [Export] public string hello;
        [Export] public bool on;
        [Export] public float decimalNumber;

        public string Hello()
        {
            return hello;
        }
    }
}