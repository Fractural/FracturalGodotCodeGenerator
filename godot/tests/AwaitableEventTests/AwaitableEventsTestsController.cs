using Fractural.GodotCodeGenerator.Attributes;
using Godot;
using System;

namespace Tests.OnReadyTests
{
    public partial class AwaitableEventsTestsController : Node
    {
        [Awaitable]
        public event Action<string> StringEvent;
    }
}