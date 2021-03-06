﻿namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// Plays a <see cref="AudioClip" />.
    /// </summary>
    [Display(Name = "Audio Player")]
    public sealed class AudioPlayerComponent : GameComponent {
        private SoundEffectInstance? _currentSoundEffectInstance;
        private float _pan;
        private float _pitch;
        private bool _shouldLoop;
        private float _volume = 1f;

        /// <summary>
        /// Gets the audio clip reference.
        /// </summary>
        [DataMember(Order = 0, Name = "Audio Clip")]
        public AssetReference<AudioClip> AudioClipReference { get; } = new();

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public SoundState State => this._currentSoundEffectInstance?.State ?? SoundState.Stopped;

        /// <summary>
        /// Gets or sets the pan.
        /// </summary>
        /// <value>The pan.</value>
        [DataMember(Order = 3, Name = "Pan")]
        public float Pan {
            get => this._pan;

            set {
                if (this.Set(ref this._pan, MathHelper.Clamp(value, -1f, 1f)) &&
                    this._currentSoundEffectInstance is SoundEffectInstance soundEffectInstance) {
                    soundEffectInstance.Pan = this._pan;
                }
            }
        }

        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        /// <value>The pitch.</value>
        [DataMember(Order = 4, Name = "Pitch")]
        public float Pitch {
            get => this._pitch;

            set {
                if (this.Set(ref this._pitch, MathHelper.Clamp(value, -1f, 1f)) &&
                    this._currentSoundEffectInstance is SoundEffectInstance soundEffectInstance) {
                    soundEffectInstance.Pitch = this._pitch;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AudioPlayerComponent" /> should loop.
        /// </summary>
        /// <value><c>true</c> if should loop; otherwise, <c>false</c>.</value>
        [DataMember(Order = 1, Name = "Should Loop")]
        public bool ShouldLoop {
            get => this._shouldLoop;

            set {
                if (this.Set(ref this._shouldLoop, value) &&
                    this._currentSoundEffectInstance is SoundEffectInstance soundEffectInstance) {
                    soundEffectInstance.IsLooped = this._shouldLoop;
                }
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>The volume.</value>
        [DataMember(Order = 2, Name = "Volume")]
        public float Volume {
            get => this._volume;

            set {
                if (this.Set(ref this._volume, MathHelper.Clamp(value, 0f, 1f)) &&
                    this._currentSoundEffectInstance is SoundEffectInstance soundEffectInstance) {
                    soundEffectInstance.Volume = this._volume;
                }
            }
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Entity.Scene.Game.Project.Assets.ResolveAsset<AudioClip, SoundEffect>(this.AudioClipReference);

            if (this._shouldLoop && this.Entity.IsEnabled) {
                this.Play();
            }
        }

        /// <summary>
        /// Play this instance.
        /// </summary>
        public void Play() {
            if (this._currentSoundEffectInstance is SoundEffectInstance currentInstance) {
                currentInstance.Stop(true);
                currentInstance.Dispose();
            }

            if (this.AudioClipReference.Asset is AudioClip audioClip) {
                this._currentSoundEffectInstance = audioClip.GetSoundEffectInstance(this.Volume, this.Pan, this.Pitch);

                if (this._currentSoundEffectInstance != null) {
                    this._currentSoundEffectInstance.IsLooped = this._shouldLoop;
                    this._currentSoundEffectInstance.Play();
                }
            }
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop() {
            this.Stop(true);
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        /// <param name="isImmediate">If set to <c>true</c> is immediate.</param>
        public void Stop(bool isImmediate) {
            if (this._currentSoundEffectInstance is SoundEffectInstance soundEffectInstance) {
                soundEffectInstance.Stop(isImmediate);
                soundEffectInstance.Dispose();
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.IsEnabled)) {
                if (this.ShouldLoop && this.Entity.IsEnabled) {
                    this.Play();
                }
                else if (!this.Entity.IsEnabled) {
                    this.Stop();
                }
            }
        }
    }
}