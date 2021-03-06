﻿namespace Macabresoft.Macabre2D.Framework {
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Extension methods for dealing with pixel snapping.
    /// </summary>
    public static class PixelSnapExtensions {
        /// <summary>
        /// Converts a <see cref="float" /> to a pixel snapped value according to the <see cref="IGameSettings" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>A pixel snapped value</returns>
        public static float ToPixelSnappedValue(this float value, IGameSettings settings) {
            return (int)Math.Round(value * settings.PixelsPerUnit, 0, MidpointRounding.AwayFromZero) * settings.InversePixelsPerUnit;
        }

        /// <summary>
        /// Converts a <see cref="Vector2" /> to a pixel snapped value according to the <see cref="IGameSettings" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>A pixel snapped value</returns>
        public static Vector2 ToPixelSnappedValue(this Vector2 value, IGameSettings settings) {
            return new(
                (int)Math.Round(value.X * settings.PixelsPerUnit, 0, MidpointRounding.AwayFromZero) * settings.InversePixelsPerUnit,
                (int)Math.Round(value.Y * settings.PixelsPerUnit, 0, MidpointRounding.AwayFromZero) * settings.InversePixelsPerUnit);
        }

        /// <summary>
        /// Converts a <see cref="Transform" /> to a pixel snapped value.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>A pixel snapped value.</returns>
        public static Transform ToPixelSnappedValue(this Transform transform, IGameSettings settings) {
            var position = transform.Position.ToPixelSnappedValue(settings);
            var scale = new Vector2((int)Math.Round(transform.Scale.X, 0, MidpointRounding.AwayFromZero), (int)Math.Round(transform.Scale.Y, 0, MidpointRounding.AwayFromZero));
            return new Transform(position, scale);
        }
    }
}