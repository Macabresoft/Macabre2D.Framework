﻿namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {

    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// Draws a grid for the editor.
    /// </summary>
    public sealed class EditorGridComponent : BaseDrawerComponent {
        private readonly ISceneService _sceneService;
        private CameraComponent _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGridComponent" /> class.
        /// </summary>
        /// <param name="sceneService">The scene service.</param>
        public EditorGridComponent(ISceneService sceneService) {
            this._sceneService = sceneService;
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea => _camera?.BoundingArea ?? BoundingArea.Empty;

        /// <summary>
        /// Gets or sets the size of the major grid.
        /// </summary>
        /// <value>The size of the major grid.</value>
        public byte MajorGridSize { get; set; } = 5;

        /// <summary>
        /// Gets or sets the number of divisions between major grid lines.
        /// </summary>
        /// <value>The number of divisions.</value>
        public byte NumberOfDivisions { get; set; } = 5;

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            this.UseDynamicLineThickness = true;
            if (!this.Entity.TryGetComponent(out this._camera)) {
                throw new ArgumentNullException(nameof(this._camera));
            }

            this.ResetColor();
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer == null) {
                return;
            }

            if (this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                if (this.MajorGridSize > 0) {
                    if (!GameScene.IsNullOrEmpty(this._sceneService.CurrentScene) && this.Color != this._sceneService.CurrentScene.BackgroundColor) {
                        this.ResetColor();
                    }

                    var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                    if (this.NumberOfDivisions > 0) {
                        var minorGridSize = this.MajorGridSize / this.NumberOfDivisions;
                        this.DrawGrid(spriteBatch, viewBoundingArea, minorGridSize, lineThickness, 0.2f);
                    }

                    this.DrawGrid(spriteBatch, viewBoundingArea, this.MajorGridSize, lineThickness, 0.5f);
                }
            }
        }

        private void DrawGrid(SpriteBatch spriteBatch, BoundingArea boundingArea, float gridSize, float lineThickness, float alpha) {
            var columns = GridDrawerComponent.GetGridPositions(boundingArea.Minimum.X, boundingArea.Maximum.X, gridSize, 0f);
            var color = this.Color * alpha;
            foreach (var column in columns) {
                this.PrimitiveDrawer.DrawLine(
                    spriteBatch,
                    new Vector2(column, boundingArea.Minimum.Y),
                    new Vector2(column, boundingArea.Maximum.Y),
                    color,
                    lineThickness);
            }

            var rows = GridDrawerComponent.GetGridPositions(boundingArea.Minimum.Y, boundingArea.Maximum.Y, gridSize, 0f);
            foreach (var row in rows) {
                this.PrimitiveDrawer.DrawLine(
                    spriteBatch,
                    new Vector2(boundingArea.Minimum.X, row),
                    new Vector2(boundingArea.Maximum.X, row),
                    color,
                    lineThickness);
            }
        }

        private void ResetColor() {
            if (!GameScene.IsNullOrEmpty(this._sceneService.CurrentScene)) {
                this.Color = this._sceneService.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite();
            }
            else {
                this.Color = this.Entity.Scene.BackgroundColor.GetContrastingBlackOrWhite();
            }
        }

        private void Scene_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameScene.BackgroundColor)) {
                this.ResetColor();
            }
        }
    }
}