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
        public async Task WhenAwaitParameterlessEvent_ShouldWork()
        {
            bool running = false;
            Task asyncTask = Task.Run(async () =>
            {
                running = true;
                await controller.ParameterlessEventRaised();
                running = false;
            });
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block.");
            Assert.IsTrue(running, "Expected task to be running.");
            await UntilTimeout(0.5f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block after 0.5s.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeParameterlessEvent();
            await UntilTimeout(0.1f);
            Assert.IsTrue(asyncTask.IsCompleted, "Expected task to finish after event runs.");
            Assert.IsFalse(running, "Expected task to not be running.");
        }

        [Test]
        public async Task WhenAwaitStringActionEvent_ShouldWork()
        {
            bool running = false;
            string result = "";
            Task asyncTask = Task.Run(async () =>
            {
                running = true;
                result = await controller.StringEventRaised();
                running = false;
            });
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block.");
            Assert.IsTrue(running, "Expected task to be running.");
            await UntilTimeout(0.5f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block after 0.5s.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeStringEvent("heyo");
            await UntilTimeout(0.1f);
            Assert.IsTrue(asyncTask.IsCompleted, "Expected task to finish after event runs.");
            Assert.IsFalse(running, "Expected task to not be running.");
            Assert.IsEqual(result, "heyo", "Expected await result to equal input.");
        }

        [Test]
        public async Task WhenAwaitTripleActionEvent_ShouldWork()
        {
            bool running = false;
            (string, int, double) result = ("", 0, 0);
            Task asyncTask = Task.Run(async () =>
            {
                running = true;
                result = await controller.StringIntDoubleEventRaised();
                running = false;
            });
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block.");
            Assert.IsTrue(running, "Expected task to be running.");
            await UntilTimeout(0.5f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block after 0.5s.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeStringIntDoubleEvent("heyo", 10, 30.0);
            await UntilTimeout(0.1f);
            Assert.IsTrue(asyncTask.IsCompleted, "Expected task to finish after event runs.");
            Assert.IsFalse(running, "Expected task to not be running.");
            Assert.IsEqual(result, ("heyo", 10, 30.0), "Expected await result to equal input.");
        }

        [Test]
        public async Task WhenAwaitCustomDelegateEvent_ShouldWork()
        {
            bool running = false;
            (bool, string) result = (false, "");
            Task asyncTask = Task.Run(async () =>
            {
                running = true;
                result = await controller.CustomDelegateEventRaised();
                running = false;
            });
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block.");
            Assert.IsTrue(running, "Expected task to be running.");
            await UntilTimeout(0.5f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block after 0.5s.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeCustomDelegateEvent(false, "asdf");
            await UntilTimeout(0.1f);
            Assert.IsTrue(asyncTask.IsCompleted, "Expected task to finish after event runs.");
            Assert.IsFalse(running, "Expected task to not be running.");
            Assert.IsEqual(result, (false, "asdf"), "Expected await result to equal input.");
        }

        [Test]
        public async Task WhenAwaitStringEvent_WithCondition_OnlyReturnWhenConditionMatched()
        {
            bool running = false;
            string result = "";
            Task asyncTask = Task.Run(async () =>
            {
                running = true;
                result = await controller.StringEventRaised((str) =>
                {
                    return str == "the_key";
                });
                running = false;
            });
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeStringEvent("heyo");
            await UntilTimeout(0.1f);
            Assert.IsFalse(asyncTask.IsCompleted, "Expected task to block after \"heyo\" input.");
            Assert.IsTrue(running, "Expected task to be running.");
            controller.InvokeStringEvent("the_key");
            await UntilTimeout(0.1f);
            Assert.IsTrue(asyncTask.IsCompleted, "Expected task to finish after \"the_key\" input.");
            Assert.IsFalse(running, "Expected task to not be running.");
            Assert.IsEqual(result, "the_key", "Expected await result to equal \"the_key\".");
        }
    }
}
