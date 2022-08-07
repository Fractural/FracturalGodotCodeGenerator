using Godot;

namespace Tests.OnReadyTests
{
    [Start(nameof(Start))]
    public class OnReadyTests : WAT.Test
    {
        public PackedScene scene;

        public void Start()
        {
            scene = ResourceLoader.Load<PackedScene>("res://tests/OnReadyTests/OnReadyTests.tscn");
        }

        [Test]
        public void WhenReadied_ButtonFindShouldBeSet()
        {
            var controller = scene.Instance<OnReadyTestsController>();
            Assert.IsNull(controller.button1Find);
            AddChild(controller);
            Assert.IsNotNull(controller.button1Find, "Expected OnReadyFind to find button.");
            Assert.IsEqual(controller.button1Find.Name, "Button1");
            controller.QueueFree();
        }

        [Test]
        public void WhenReadied_ButtonGetShouldBeSet()
        {
            var controller = scene.Instance<OnReadyTestsController>();
            Assert.IsNull(controller.button2ManualGet);
            AddChild(controller);
            Assert.IsNotNull(controller.button2ManualGet, "Expected OnReadyGet to use manually assigned button.");
            Assert.IsEqual(controller.button2ManualGet.Name, "Button2");
            controller.QueueFree();
        }

        [Test]
        public void WhenReadied_ButtonGetPathShouldBeSet()
        {
            var controller = scene.Instance<OnReadyTestsController>();
            Assert.IsNull(controller.button3GetPath);
            AddChild(controller);
            Assert.IsNotNull(controller.button3GetPath, "Expected OnReadyGet to use manual path.");
            Assert.IsEqual(controller.button3GetPath.Name, "Button3");
            controller.QueueFree();
        }
    }
}
