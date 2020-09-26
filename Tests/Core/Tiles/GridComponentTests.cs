﻿using Macabresoft.MonoGame.Core;
using Macabresoft.MonoGame.Core.Tiles;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Macabresoft.MonoGame.Tests.Core.Tiles {

    [TestFixture]
    public static class GridComponentTests {

        [Test]
        [Category("Unit Test")]
        public static void GridComponent_ScaleTest() {
            var entity = new GameEntity();
            using (var gridComponent = entity.AddComponent<GridComponent>()) {
                gridComponent.Initialize(entity);
                var localPosition = gridComponent.Grid.GetTilePosition(new Point(1, 1));
                var worldPosition = gridComponent.WorldGrid.GetTilePosition(new Point(1, 1));

                Assert.AreEqual(localPosition, worldPosition);

                entity.LocalScale = new Vector2(2f, 1f);
                localPosition = gridComponent.Grid.GetTilePosition(new Point(1, 1));
                worldPosition = gridComponent.WorldGrid.GetTilePosition(new Point(1, 1));

                Assert.AreNotEqual(localPosition, worldPosition);
                Assert.AreEqual(localPosition.X * 2f, worldPosition.X);
            }
        }
    }
}