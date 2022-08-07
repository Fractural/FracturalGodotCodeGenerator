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
            Assert.IsNull(controller.GenericType);
            Assert.IsNull(controller.GenericTypeProperty);
            Assert.IsNull(controller.buttonManual);
            Assert.IsNull(controller.buttonGetPath);
            Assert.IsNull(controller.buttonFind);

            try
            {
                AddChild(controller);
            }
            catch { }

            Assert.IsNotNull(controller.GenericType);
            Assert.IsNotNull(controller.GenericTypeProperty);
            Assert.IsNotNull(controller.buttonManual);
            Assert.IsNotNull(controller.buttonGetPath);
            Assert.IsNotNull(controller.buttonFind);
        }
    }
}