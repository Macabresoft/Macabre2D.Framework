﻿namespace Macabre2D.Tests.Rendering {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public static class SpriteAnimatorTests {

        [Test]
        [Category("Unit Tests")]
        public static void SpriteAnimator_LoopingTest() {
            var numberOfSteps = 3;
            var animation = CreateAnimation(numberOfSteps, true);
            var animator = CreateAnimator(animation, 1);
            var scene = Substitute.For<IScene>();
            animator.Initialize(Substitute.For<IScene>());

            var gameTime = new GameTime {
                ElapsedGameTime = TimeSpan.FromMilliseconds(100d)
            };

            var spriteRenderer = animator.GetChildOfType<SpriteRenderer>();
            Assert.AreEqual(animation.Steps.ElementAt(0).Sprite.Id, spriteRenderer.Sprite.Id);

            animator.UpdateAsync(gameTime).Wait();
            Assert.AreEqual(animation.Steps.ElementAt(0).Sprite.Id, spriteRenderer.Sprite.Id);

            for (var i = 1; i < numberOfSteps; i++) {
                gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
                animator.UpdateAsync(gameTime).Wait();
                Assert.AreEqual(animation.Steps.ElementAt(i).Sprite.Id, spriteRenderer.Sprite.Id);
            }

            gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);

            // Should loop here.
            animator.UpdateAsync(gameTime).Wait();
            Assert.AreEqual(animation.Steps.ElementAt(0).Sprite.Id, spriteRenderer.Sprite.Id);
        }

        private static SpriteAnimation CreateAnimation(int numberOfSteps, bool shouldLoop) {
            var steps = new List<SpriteAnimationStep>();
            for (var i = 0; i < numberOfSteps; i++) {
                steps.Add(new SpriteAnimationStep {
                    Frames = 1,
                    Sprite = new Sprite()
                });
            }

            return new SpriteAnimation(steps, shouldLoop);
        }

        private static SpriteAnimatorComponent CreateAnimator(SpriteAnimation animation, int frameRate) {
            var animator = new SpriteAnimatorComponent(animation) {
                FrameRate = frameRate
            };

            animator.AddChild(new SpriteRenderer());
            return animator;
        }
    }
}