﻿namespace Macabre2D.UI.CommonLibrary.Dialogs {

    using Macabre2D.UI.CommonLibrary.Dialogs;

    public partial class SelectSpriteDialog {

        public SelectSpriteDialog(SelectSpriteViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public SelectSpriteViewModel ViewModel {
            get {
                return this.DataContext as SelectSpriteViewModel;
            }

            set {
                if (this.DataContext == null) {
                    this.DataContext = value;
                }
            }
        }

        private void ViewModel_Finished(object sender, bool e) {
            this.DialogResult = e;
            this.Close();
        }
    }
}