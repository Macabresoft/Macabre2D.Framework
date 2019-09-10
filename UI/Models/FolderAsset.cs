﻿namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using MahApps.Metro.IconPacks;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    public class FolderAsset : Asset, IParent<Asset> {

        [DataMember]
        private readonly ObservableCollection<Asset> _children = new ObservableCollection<Asset>();

        public FolderAsset(string name) : base(name) {
            this._children.CollectionChanged += this.Children_CollectionChanged;
        }

        internal FolderAsset() {
            this._children.CollectionChanged += this.Children_CollectionChanged;
        }

        public IReadOnlyCollection<Asset> Children {
            get {
                return this._children;
            }
        }

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.Folder;
            }
        }

        public bool AddChild(Asset child) {
            if (child != null && !this._children.Any(x => string.Equals(x.Name, child.Name, StringComparison.CurrentCultureIgnoreCase))) {
                this._children.Add(child);
                child.OnDeleted += this.Child_OnDeleted;
                this.RaisePropertyChanged(nameof(this.Children));
                return true;
            }

            return false;
        }

        public override void Delete() {
            var children = this._children.ToList();
            foreach (var child in children) {
                child.Delete();
            }

            Directory.Delete(this.GetPath(), true);
            this.RaiseOnDeleted();
        }

        public IEnumerable<Asset> GetAllContentAssets() {
            var children = new List<Asset>();

            foreach (var child in this.Children) {
                if (child is FolderAsset folderAsset) {
                    children.AddRange(folderAsset.GetAllContentAssets());
                }
                else {
                    children.Add(child);
                }
            }

            return children;
        }

        public IEnumerable<TAsset> GetAssetsOfType<TAsset>() where TAsset : Asset {
            var assets = new List<TAsset>();

            foreach (var child in this._children) {
                if (child is TAsset asset) {
                    assets.Add(asset);
                }
                else if (child is FolderAsset folder) {
                    assets.AddRange(folder.GetAssetsOfType<TAsset>());
                }
                else if (child is IParent<TAsset> parent) {
                    assets.AddRange(parent.Children);
                }
            }

            return assets;
        }

        public virtual void Refresh() {
            var serializer = new Serializer();
            var path = this.GetPath();
            var folders = Directory.EnumerateDirectories(path).OrderBy(x => x);
            var folderAssetsToAdd = new List<FolderAsset>();
            var files = Directory.EnumerateFiles(path).OrderBy(x => x);
            var assetsToAdd = new List<Asset>();

            this._children.Clear();

            foreach (var folder in folders) {
                var folderAsset = new FolderAsset(Path.GetFileName(folder));
                this.AddChild(folderAsset);
                folderAsset.Refresh();
            }

            foreach (var file in files) {
                var asset = this.GetAssetFromFilePath(file, serializer);
                this.AddChild(asset);
            }

            this.RaisePropertyChanged(nameof(this.Children));
        }

        public bool RemoveChild(Asset child) {
            if (this._children.Remove(child)) {
                this.RaisePropertyChanged(nameof(this.Children));
                return true;
            }

            return false;
        }

        private void Child_OnDeleted(object sender, EventArgs e) {
            if (sender is Asset asset) {
                this._children.Remove(asset);
            }
        }

        private void Child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(nameof(this.Children));
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var child in e.NewItems.Cast<Asset>().Where(x => x.Parent != this)) {
                    child.Parent = this;
                    child.PropertyChanged += this.Child_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var child in e.OldItems.Cast<Asset>().Where(x => x.Parent == this)) {
                    child.Parent = null;
                    child.PropertyChanged -= this.Child_PropertyChanged;
                }
            }
        }

        private T CreateAssetFromMetadata<T>(string filePath, string fileName, Serializer serializer) where T : MetadataAsset, new() {
            var metadataFilePath = $"{filePath}{FileHelper.MetaDataExtension}";
            T result;
            if (File.Exists(metadataFilePath)) {
                result = serializer.Deserialize<T>(metadataFilePath);
            }
            else {
                result = new T {
                    Name = fileName
                };

                serializer.Serialize(result, metadataFilePath);
            }

            return result;
        }

        private Asset GetAssetFromFilePath(string filePath, Serializer serializer) {
            Asset result = null;
            var fileName = Path.GetFileName(filePath);
            if (filePath.ToUpper().EndsWith(FileHelper.SceneExtension.ToUpper())) {
                result = this.CreateAssetFromMetadata<SceneAsset>(filePath, fileName, serializer);
            }
            else if (FileHelper.IsImageFile(fileName)) {
                result = this.CreateAssetFromMetadata<ImageAsset>(filePath, fileName, serializer);
            }
            else if (FileHelper.IsAudioFile(fileName)) {
                result = this.CreateAssetFromMetadata<AudioAsset>(filePath, fileName, serializer);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.SpriteAnimationExtension.ToUpper())) {
                result = this.CreateAssetFromMetadata<SpriteAnimationAsset>(filePath, fileName, serializer);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.SpriteFontExtension.ToUpper())) {
                result = this.CreateAssetFromMetadata<FontAsset>(filePath, fileName, serializer);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.AutoTileSetExtension.ToUpper())) {
                result = this.CreateAssetFromMetadata<AutoTileSetAsset>(filePath, fileName, serializer);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.PrefabExtension.ToUpper())) {
                result = this.CreateAssetFromMetadata<PrefabAsset>(filePath, fileName, serializer);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.ShaderExtension.ToUpper())) {
                result = this.CreateAssetFromMetadata<ShaderAsset>(filePath, fileName, serializer);
            }
            else if (!FileHelper.IsMetadataFile(fileName)) {
                result = new Asset(fileName);
            }

            return result;
        }
    }
}