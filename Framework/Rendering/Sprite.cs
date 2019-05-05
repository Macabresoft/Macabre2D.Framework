﻿namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a sprite with the content path and the Texture2D.
    /// </summary>
    [DataContract]
    public sealed class Sprite : IDisposable, IIdentifiable {
        private bool _disposedValue = false;

        [DataMember]
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        public Sprite() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        public Sprite(Guid id) {
            this.ContentId = id;
            this.LoadTexture();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Macabre2D.Sprite"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="location">The location of this specific sprite on the <see cref="Texture2D"/>.</param>
        /// <param name="size">The size of this specific sprite on the <see cref="Texture2D"/>.</param>
        public Sprite(Guid id, Point location, Point size) {
            this.ContentId = id;
            this.LoadTexture(location, size);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <remarks>
        /// This constructor should be used for dynamic sprite creation and will not be properly
        /// saved to a scene as there will be no content path.
        /// </remarks>
        public Sprite(Texture2D texture) : this(texture, Point.Zero, new Point(texture.Width, texture.Height)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="location">The location.</param>
        /// <param name="size">The size.</param>
        public Sprite(Texture2D texture, Point location, Point size) {
            this.LoadTexture(texture, location, size);
        }

        /// <summary>
        /// Gets or sets the content identifier.
        /// </summary>
        /// <value>The content identifier.</value>
        [DataMember]
        public Guid ContentId { get; internal set; }

        /// <inheritdoc/>
        public Guid Id {
            get {
                return this._id;
            }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        [DataMember]
        public Point Location { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        [DataMember]
        public Point Size { get; set; }

        internal Texture2D Texture { get; set; }

        /// <inheritdoc/>
        public void Dispose() {
            this.Dispose(true);
        }

        internal void SetErrorTexture(SpriteBatch spriteBatch) {
            if (this.Size.X != 0 && this.Size.Y != 0 && spriteBatch != null) {
                var errorTexture = new Texture2D(spriteBatch.GraphicsDevice, this.Size.X, this.Size.Y, false, SurfaceFormat.Color);
                var pixels = new Color[this.Size.X * this.Size.Y];

                for (var i = 0; i < pixels.Length; i++) {
                    pixels[i] = GameSettings.Instance.ErrorSpritesColor;
                }

                errorTexture.SetData(pixels);
                this.Texture = errorTexture;
            }
        }

        private void Dispose(bool disposing) {
            if (!this._disposedValue) {
                if (disposing) {
                    this.Texture.Dispose();
                }

                this._disposedValue = true;
            }
        }

        private void LoadTexture() {
            var texture = AssetManager.Instance.Load<Texture2D>(this.ContentId);
            this.LoadTexture(texture, Point.Zero, new Point(texture.Width, texture.Height));
        }

        private void LoadTexture(Point location, Point size) {
            var texture = AssetManager.Instance.Load<Texture2D>(this.ContentId);
            this.LoadTexture(texture, location, size);
        }

        private void LoadTexture(Texture2D texture, Point location, Point size) {
            this.Texture = texture;
            this.Location = location;
            this.Size = size;
        }
    }
}