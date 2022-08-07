using Godot;
using Fractural.CSharpResourceRegistry;

namespace Tests.OnReadyModeTests
{
    [RegisteredType(nameof(ConcreteCustomResource))]
    public class ConcreteCustomResource : Resource, ICustomResourceOrNode
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