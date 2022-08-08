using Godot;

namespace Tests.OnReadyModeTests
{
    [Start(nameof(Start))]
    [Pre(nameof(Pre))]
    [Post(nameof(Post))]
    public class OnReadyModeTests : WAT.Test
    {
        public PackedScene scene;
        public OnReadyModeTestsController controller;

        public void Start()
        {
            scene = ResourceLoader.Load<PackedScene>("res://tests/OnReadyModeTests/OnReadyModeTests.tscn");
        }

        public void Pre()
        {
            controller = scene.Instance<OnReadyModeTestsController>();
        }

        public void Post()
        {
            controller.QueueFree();
        }

        [Test]
        public void WhenReadiedAndExportModeSet_ShouldSetResourceAndNodesCorrectly()
        {
            Assert.IsNull(controller.nodeGetPath, $"Expected {nameof(controller.nodeGetPath)} to be null.");
            Assert.IsNull(controller.nodeManual, $"Expected {nameof(controller.nodeManual)} to be null.");
            Assert.IsNull(controller.resourceGetPath, $"Expected {nameof(controller.resourceGetPath)} to be null.");
            Assert.IsNull(controller.nodeFind, $"Expected {nameof(controller.nodeFind)} to be null.");

            try
            {
                AddChild(controller);
            }
            catch { }

            Assert.IsNotNull(controller.nodeGetPath, $"Expected {nameof(controller.nodeGetPath)} to be set.");
            Assert.IsNotNull(controller.nodeManual, $"Expected {nameof(controller.nodeManual)} to be set.");
            Assert.IsNotNull(controller.resourceGetPath, $"Expected {nameof(controller.resourceGetPath)} to be set.");
            Assert.IsNotNull(controller.nodeFind, $"Expected {nameof(controller.nodeFind)} to be set.");

            Assert.IsNotNull(controller.resourceManual, $"Expected {nameof(controller.resourceManual)} to be set.");
        }
    }
}