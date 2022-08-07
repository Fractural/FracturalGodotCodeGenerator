using Godot;
using System.Collections.Generic;

namespace Tests.OnReadyOrderTests
{
    [Start(nameof(Start))]
    [Pre(nameof(Pre))]
    [Post(nameof(Post))]
    public class OnReadyOrderTests : WAT.Test
    {
        public PackedScene scene;
        public OnReadyOrderTestsController controller;

        public void Start()
        {
            scene = ResourceLoader.Load<PackedScene>("res://tests/OnReadyOrderTests/OnReadyOrderTests.tscn");
        }

        public void Pre()
        {
            controller = scene.Instance<OnReadyOrderTestsController>();
        }

        public void Post()
        {
            controller.QueueFree();
        }

        private Queue<string> expectedNewReadyOrder;

        [Test]
        public void WhenReadied_ShouldCallNewOnReadyMethods()
        {
            expectedNewReadyOrder = new Queue<string>(new[] { nameof(controller.NewReadyN1), nameof(controller.NewReady0), nameof(controller.NewReady1), nameof(controller.NewReady2) });
            controller.Connect(nameof(OnReadyOrderTestsController.NewReadyFired), this, nameof(OnNewReadyFired));

            try
            {
                AddChild(controller);
            }
            catch { }

            Assert.IsEqual(expectedNewReadyOrder.Count, 0, "Expected ready queue count == 0");
        }

        private void OnNewReadyFired(string methodName)
        {
            Assert.IsGreaterThan(expectedNewReadyOrder.Count, 0, "Expected ready queue count > 0");
            var expectedMethodName = expectedNewReadyOrder.Dequeue();
            Assert.IsEqual(expectedMethodName, methodName, $"Expected ready methods to be called in correct order.");
        }
    }
}
