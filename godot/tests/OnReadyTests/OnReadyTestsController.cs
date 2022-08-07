using Fractural.GodotCodeGenerator.Attributes;
using Godot;

namespace Tests.OnReadyTests
{
    public partial class OnReadyTestsController : Control
    {
        [Signal] public delegate void NewReadyFired(string methodName);

        [OnReadyFind("Button1")]
        public Button button1Find;
        [OnReadyGet]
        public Button button2ManualGet;
        [OnReadyGet("HBoxContainer/VBoxContainer/Button3")]
        public Button button3GetPath;

        [OnReadyFind("Button1", Export = true)]
        public Button button1FindPublic;
        [OnReadyFind("Button2", Export = true)]
        protected Button button2FindProtected;
        [OnReadyFind("Button3", Export = true)]
        private Button button3FindPrivate;

        [OnReadyGet("HBoxContainer/VBoxContainer/Button1")]
        public Button buttonGetPathProperty0 { get; private set; }
        [OnReadyGet("HBoxContainer/VBoxContainer/Button1")]
        public Button buttonGetPathProperty1 { get; set; }
        [OnReadyGet("HBoxContainer/VBoxContainer/Button1")]
        private Button buttonGetPathProperty2 { get; set; }
        [OnReadyGet("HBoxContainer/VBoxContainer/Button1")]
        protected Button buttonGetPathProperty3 { get; set; }

        [OnReady]
        public void PublicNewReady()
        {
            EmitSignal(nameof(NewReadyFired), nameof(PublicNewReady));
        }

        [OnReady]
        public void PrivateNewReady()
        {
            EmitSignal(nameof(NewReadyFired), nameof(PrivateNewReady));
        }

        [OnReady]
        protected void ProtectedNewReady()
        {
            EmitSignal(nameof(NewReadyFired), nameof(ProtectedNewReady));
        }
    }
}