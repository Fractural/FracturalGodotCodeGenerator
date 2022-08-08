using Godot;

namespace Tests.OnReadyGenericTests
{
    [Start(nameof(Start))]
    [Pre(nameof(Pre))]
    [Post(nameof(Post))]
    public class OnReadyGenericTests : WAT.Test
    {
        public PackedScene scene;
        public OnReadyGenericTestsController controller;

        public void Start()
        {
            scene = ResourceLoader.Load<PackedScene>("res://tests/OnReadyGenericTests/OnReadyGenericTests.tscn");
        }

        public void Pre()
        {
            controller = scene.Instance<OnReadyGenericTestsController>();
        }

        public void Post()
        {
            controller.QueueFree();
        }

        [Test]
        public void WhenReadied_ShouldCallNewOnReadyMethods()
        {
            Assert.IsNull(controller.GenericType, $"Expected {nameof(controller.GenericType)} to be null.");
            Assert.IsNull(controller.GenericTypeProperty, $"Expected {nameof(controller.GenericTypeProperty)} to be null.");
            Assert.IsNull(controller.buttonManual, $"Expected {nameof(controller.buttonManual)} to be null.");
            Assert.IsNull(controller.buttonGetPath, $"Expected {nameof(controller.buttonGetPath)} to be null.");
            Assert.IsNull(controller.buttonFind, $"Expected {nameof(controller.buttonFind)} to be null.");

            try
            {
                AddChild(controller);
            }
            catch { }

            Assert.IsNotNull(controller.GenericType, $"Expected {nameof(controller.GenericType)} to be set.");
            Assert.IsNotNull(controller.GenericTypeProperty, $"Expected {nameof(controller.GenericTypeProperty)} to be set.");
            Assert.IsNotNull(controller.buttonManual, $"Expected {nameof(controller.buttonManual)} to be set.");
            Assert.IsNotNull(controller.buttonGetPath, $"Expected {nameof(controller.buttonGetPath)} to be set.");
            Assert.IsNotNull(controller.buttonFind, $"Expected {nameof(controller.buttonFind)} to be set.");
        }
    }
}