using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.AwaitableEventTests
{
    [Start(nameof(Start))]
    [Pre(nameof(Pre))]
    [Post(nameof(Post))]
    public class AwaitableEventTests : WAT.Test
    {
        public PackedScene scene;
        public AwaitableEventTestsController controller;

        public void Start()
        {
            scene = ResourceLoader.Load<PackedScene>("res://tests/AwaitableEventTests/AwaitableEventTests.tscn");
        }

        public void Pre()
        {
            controller = scene.Instance<AwaitableEventTestsController>();
        }

        public void Post()
        {
            controller.QueueFree();
        }

        [Test]
        public async Task WhenAwaitActionEvent_ShouldWork()
        {
            bool running = false;
            Task asyncTask = Task.Run(async () =>
            {
                running = true;
                await controller.StringEvent_Raised();
                running = false;
            });
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block.");
            Assert.IsTrue(running, "Expected task to be running.");
            await UntilTimeout(0.5f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block after 0.5s.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeStringEvent("heyo");
            Assert.IsTrue(asyncTask.IsCompleted, "Expected task to finish after event runs.");
            Assert.IsFalse(running, "Expected task to not be running.");
        }

        [Test]
        public async Task WhenAwaitTripleActionEvent_ShouldWork()
        {
            bool running = false;
            Task asyncTask = Task.Run(async () =>
            {
                running = true;
                await controller.StringIntDoubleEvent_Raised();
                running = false;
            });
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block.");
            Assert.IsTrue(running, "Expected task to be running.");
            await UntilTimeout(0.5f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block after 0.5s.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeStringIntDoubleEvent("heyo", 10, 30.0);
            Assert.IsTrue(asyncTask.IsCompleted, "Expected task to finish after event runs.");
            Assert.IsFalse(running, "Expected task to not be running.");
        }


        [Test]
        public async Task WhenAwaitCustomDelegateEvent_ShouldWork()
        {
            bool running = false;
            Task asyncTask = Task.Run(async () =>
            {
                running = true;
                await controller.CustomDelegateEvent_Raised();
                running = false;
            });
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block.");
            Assert.IsTrue(running, "Expected task to be running.");
            await UntilTimeout(0.5f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block after 0.5s.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeCustomDelegateEvent(false, "asdf");
            Assert.IsTrue(asyncTask.IsCompleted, "Expected task to finish after event runs.");
            Assert.IsFalse(running, "Expected task to not be running.");
        }
    }
}
