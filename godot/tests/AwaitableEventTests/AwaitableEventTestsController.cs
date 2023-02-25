using Fractural.GodotCodeGenerator.Attributes;
using Godot;
using System;
using System.Threading.Tasks;

namespace Tests.AwaitableEventTests
{
    public partial class AwaitableEventTestsController : Node
    {
        [Awaitable]
        public event Action ParameterlessEvent;

        [Awaitable]
        public event Action<string> StringEvent;

        [Awaitable]
        public event Action<string, int, double> StringIntDoubleEvent;

        public delegate void CustomDelegate(bool someBool, string coolString);
        [Awaitable]
        public event CustomDelegate CustomDelegateEvent;

        public void InvokeParameterlessEvent() => ParameterlessEvent?.Invoke();
        public void InvokeStringEvent(string arg) => StringEvent?.Invoke(arg);
        public void InvokeStringIntDoubleEvent(string arg1, int arg2, double arg3) => StringIntDoubleEvent?.Invoke(arg1, arg2, arg3);
        public void InvokeCustomDelegateEvent(bool someBool, string coolString) => CustomDelegateEvent?.Invoke(someBool, coolString);
    }
}