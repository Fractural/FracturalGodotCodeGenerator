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
            Assert.IsNull(controller.nodeGetPath);
            Assert.IsNull(controller.nodeManual);
            Assert.IsNull(controller.resourceGetPath);
            Assert.IsNull(controller.resourceManual);

            try
            {
                AddChild(controller);
            }
            catch { }

            Assert.IsNotNull(controller.nodeGetPath);
            Assert.IsNotNull(controller.nodeManual);
            Assert.IsNotNull(controller.resourceGetPath);
            Assert.IsNotNull(controller.resourceManual);
        }
    }
}