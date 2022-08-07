using Godot;
using System.Threading.Tasks;

namespace Tests.OnReadyDerivedTests
{
    [Start(nameof(Start))]
    [Pre(nameof(Pre))]
    [Post(nameof(Post))]
    public class OnReadyDerivedTests : WAT.Test
    {
        public PackedScene scene;
        public OnReadyDerivedTestsController controller;

        public void Start()
        {
            scene = ResourceLoader.Load<PackedScene>("res://tests/OnReadyDerivedTests/OnReadyDerivedTests.tscn");
        }

        public void Pre()
        {
            controller = scene.Instance<OnReadyDerivedTestsController>();
        }

        public async Task Post()
        {
            await UntilTimeout(1f);
            controller.QueueFree();
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
