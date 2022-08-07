using Fractural.GodotCodeGenerator.Attributes;
using Godot;

namespace Tests.OnReadyGenericTests
{
    public abstract partial class OnReadyGenericAncestorTestsController<T> : Control where T : class
    {
        [OnReadyGet] public T GenericType;
        [OnReadyGet] public T GenericTypeProperty { get; private set; }


        [OnReadyGet]
        public Button buttonManual;
        [OnReadyGet("HBoxContainer/VBoxContainer/Button1")]
        public Button buttonGetPath;
        [OnReadyFind("Button2")]
        public Button buttonFind;
    }

    public partial class OnReadyGenericTestsController : OnReadyGenericAncestorTestsController<Button>
    {

    }
}