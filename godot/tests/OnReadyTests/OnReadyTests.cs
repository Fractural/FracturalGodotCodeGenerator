using Godot;

namespace Tests.OnReadyTests
{
    [Start(nameof(Start))]
    [Pre(nameof(Pre))]
    [Post(nameof(Post))]
    public class OnReadyTests : WAT.Test
    {
        public PackedScene scene;
        public OnReadyTestsController controller;

        public void Start()
        {
            scene = ResourceLoader.Load<PackedScene>("res://tests/OnReadyTests/OnReadyTests.tscn");
            GD.Print("OnReadyTests started " + scene.ResourceName);
        }

        public void Pre()
        {
            controller = scene.Instance<OnReadyTestsController>();
            GD.Print("Pre c:" + controller.Name);
        }

        public void Post()
        {
            controller.QueueFree();
            GD.Print("Post c:" + controller);
        }

        [Test]
        public void WhenReadied_ButtonFindShouldBeSet()
        {
            Assert.IsNull(controller.button1Find);
            AddChild(controller);
            Assert.IsNotNull(controller.button1Find, "Expected OnReadyFind to find button.");
            Assert.IsEqual(controller.button1Find.Name, "Button1");
        }

        [Test]
        public void WhenReadied_ButtonGetShouldBeSet()
        {
            Assert.IsNull(controller.button2ManualGet);
            AddChild(controller);
            Assert.IsNotNull(controller.button2ManualGet, "Expected OnReadyGet to use manually assigned button.");
            Assert.IsEqual(controller.button2ManualGet.Name, "Button2");
        }

        [Test]
        public void WhenReadied_ButtonGetPathShouldBeSet()
        {
            Assert.IsNull(controller.button3GetPath);
            AddChild(controller);
            Assert.IsNotNull(controller.button3GetPath, "Expected OnReadyGet to use manual path.");
            Assert.IsEqual(controller.button3GetPath.Name, "Button3");
        }
    }
}
