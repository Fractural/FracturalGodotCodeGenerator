using Fractural.GodotCodeGenerator.Attributes;
using Godot;

namespace Tests.OnReadyOrderTests
{
    public partial class OnReadyOrderTestsController : Control
    {
        [Signal] public delegate void NewReadyFired(string methodName);

        [OnReady(Order = 2)]
        public void NewReady2()
        {
            EmitSignal(nameof(NewReadyFired), nameof(NewReady2));
        }

        [OnReady(Order = 0)]
        public void NewReady0()
        {
            EmitSignal(nameof(NewReadyFired), nameof(NewReady0));
        }


        [OnReady(Order = 1)]
        public void NewReady1()
        {
            EmitSignal(nameof(NewReadyFired), nameof(NewReady1));
        }

        [OnReady(Order = -1)]
        public void NewReadyN1()
        {
            EmitSignal(nameof(NewReadyFired), nameof(NewReadyN1));
        }
    }
}