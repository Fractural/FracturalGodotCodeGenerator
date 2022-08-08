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
        }

        public void Pre()
        {
            controller = scene.Instance<OnReadyTestsController>();
        }

        public void Post()
        {
            controller.QueueFree();
        }

        [Test]
        public void WhenReadied_ButtonFindShouldBeSet()
        {
            Assert.IsNull(controller.button1Find);

            try
            {
                AddChild(controller);
            }
            catch { }

            Assert.IsNotNull(controller.button1Find, "Expected OnReadyFind to find button.");
            Assert.IsEqual(controller.button1Find.Name, "Button1", $"Expected {nameof(controller.button1Find)} to be 'Button1'.");
        }

        [Test]
        public void WhenReadied_ButtonGetShouldBeSet()
        {
            Assert.IsNull(controller.button2ManualGet);

            try
            {
                AddChild(controller);
            }
            catch { }

            Assert.IsNotNull(controller.button2ManualGet, "Expected OnReadyGet to use manually assigned button.");
            Assert.IsEqual(controller.button2ManualGet.Name, "Button2", $"Expected {nameof(controller.button2ManualGet)} to be 'Button2'.");
        }

        [Test]
        public void WhenReadied_ButtonGetPathShouldBeSet()
        {
            Assert.IsNull(controller.button3GetPath);

            try
            {
                AddChild(controller);
            }
            catch { }

            Assert.IsNotNull(controller.button3GetPath, "Expected OnReadyGet with path to find button at path.");
            Assert.IsEqual(controller.button3GetPath.Name, "Button3", $"Expected {nameof(controller.button3GetPath)} to have 'Button3'.");
        }
    }
}
