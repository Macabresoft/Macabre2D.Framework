﻿namespace Macabresoft.Macabre2D.Framework {
    /// <summary>
    /// An asset which is packaged.
    /// </summary>
    public interface IPackagedAsset<in TPackage> : IAsset where TPackage : IAssetPackage  {
        /// <summary>
        /// Initializes this asset with its owning package.
        /// </summary>
        /// <param name="owningPackage">The package which owns this asset.</param>
        void Initialize(TPackage owningPackage);
    }
}