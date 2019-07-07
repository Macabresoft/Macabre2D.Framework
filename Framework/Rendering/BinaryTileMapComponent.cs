﻿namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A tile map component that is either "on" or "off". The on tiles will show the selected sprite.
    /// </summary>
    public sealed class BinaryTileMapComponent : TileableComponent, IAssetComponent<Sprite>, IDrawableComponent, ITileable<Sprite> {
        private Sprite _sprite;
        private Vector2 _spriteScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTileMapComponent"/> class.
        /// </summary>
        public BinaryTileMapComponent() : base() {
        }

        /// <inheritdoc/>
        public IEnumerable<Sprite> AvailableTiles {
            get {
                return new[] { this.Sprite };
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember]
        public Color Color { get; set; } = Color.White;

        /// <inheritdoc/>
        public Sprite DefaultTile {
            get {
                return this.Sprite;
            }
        }

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        [DataMember]
        public Sprite Sprite {
            get {
                return this._sprite;
            }
            set {
                if (this._sprite != value) {
                    this._sprite = value;
                    this.LoadContent();
                    this._spriteScale = this.GetSpriteScale();
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            if (this.Sprite?.Texture == null) {
                return;
            }

            // TODO: pass in the current camera bounding area to the Draw method and don't render a tile if it isn't within it.
            foreach (var tile in this.ActiveTiles) {
                var offset = new Vector2((tile.X * this.Grid.TileSize.X) + this.Grid.Offset.X, (tile.Y * this.Grid.TileSize.Y) + this.Grid.Offset.Y);
                var transform = this.GetWorldTransform(offset, this.LocalScale * this._spriteScale, this.Rotation.Angle);
                MacabreGame.Instance.SpriteBatch.Draw(this.Sprite, transform, this.Color);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Sprite != null ? new[] { this.Sprite.Id } : new Guid[0];
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this._sprite?.Id == id;
        }

        /// <inheritdoc/>
        public void RefreshAsset(Sprite newInstance) {
            if (newInstance != null && this.Sprite?.Id == newInstance.Id) {
                this.Sprite = newInstance;
            }
        }

        /// <inheritdoc/>
        public bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this.Sprite = null;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool SetDefaultTile(Sprite newDefault) {
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetAsset(Guid id, out Sprite asset) {
            var result = this.Sprite != null && this.Sprite.Id == id;
            asset = result ? this.Sprite : null;
            return result;
        }

        /// <inheritdoc/>
        protected override void OnGridChanged() {
            base.OnGridChanged();
            this._spriteScale = this.GetSpriteScale();
        }

        private Vector2 GetSpriteScale() {
            var result = Vector2.One;
            if (this.Sprite != null && this.Sprite.Size.X != 0 && this.Sprite.Size.Y != 0) {
                var spriteWidth = this.Sprite.Size.X * GameSettings.Instance.InversePixelsPerUnit;
                var spriteHeight = this.Sprite.Size.Y * GameSettings.Instance.InversePixelsPerUnit;

                result = new Vector2(this.Grid.TileSize.X / spriteWidth, this.Grid.TileSize.Y / spriteHeight);
            }

            return result;
        }
    }
}