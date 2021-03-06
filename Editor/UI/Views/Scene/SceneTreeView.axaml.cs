﻿namespace Macabresoft.Macabre2D.Editor.UI.Views.Scene {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.Library.ViewModels.Scene;

    public class SceneTreeView : UserControl {
        public SceneTreeView() {
            this.DataContext = Resolver.Resolve<SceneTreeViewModel>();
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}