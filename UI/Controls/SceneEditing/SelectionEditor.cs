﻿namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Extensions;
    using Macabre2D.Framework.Physics;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class SelectionEditor {
        private readonly IComponentService _componentService;
        private readonly ScaleGizmo _scaleGizmo;
        private readonly TranslationGizmo _translationGizmo;

        private BoundingAreaDrawer _boundingAreaDrawer = new BoundingAreaDrawer() {
            Color = new Color(255, 255, 255, 150),
            LineThickness = 1f,
            UseDynamicLineThickness = true
        };

        private ColliderDrawer _colliderDrawer = new ColliderDrawer() {
            Color = new Color(255, 255, 255, 150),
            LineThickness = 1f,
            UseDynamicLineThickness = true
        };

        private EditorGame _game;
        private ButtonState _previousLeftMouseButtonState = ButtonState.Released;

        public SelectionEditor(
            IComponentService componentService,
            ScaleGizmo scaleGizmo,
            TranslationGizmo translationGizmo) {
            this._componentService = componentService;
            this._componentService.SelectionChanged += this.ComponentService_SelectionChanged;
            this._scaleGizmo = scaleGizmo;
            this._translationGizmo = translationGizmo;
        }

        public void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            if (this._game.CurrentScene != null) {
                var contrastingColor = this._game.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite();
                this._boundingAreaDrawer.Color = contrastingColor;
                this._colliderDrawer.Color = contrastingColor;
            }

            if (this._game.ShowSelection) {
                this._boundingAreaDrawer.Draw(gameTime, viewBoundingArea);
                this._colliderDrawer.Draw(gameTime, viewBoundingArea);
            }

            if (this._game.SelectedGizmo == GizmoType.Scale) {
                this._scaleGizmo.Draw(gameTime, viewBoundingArea, this._componentService.SelectedItem?.Component);
            }
            else if (this._game.SelectedGizmo == GizmoType.Translation) {
                this._translationGizmo.Draw(gameTime, viewBoundingArea, this._componentService.SelectedItem?.Component);
            }
        }

        public void Initialize(EditorGame game) {
            this._game = game;

            this._boundingAreaDrawer = new BoundingAreaDrawer() {
                Color = new Color(255, 255, 255, 150),
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this._colliderDrawer = new ColliderDrawer() {
                Color = new Color(255, 255, 255, 150),
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this.ResetDependencies(this._componentService.SelectedItem);
            this._boundingAreaDrawer.Initialize(this._game.CurrentScene);
            this._colliderDrawer.Initialize(this._game.CurrentScene);
            this._scaleGizmo.Initialize(this._game);
            this._translationGizmo.Initialize(this._game);
        }

        public void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState) {
            if (this._game.CurrentScene != null && this._game.CurrentCamera != null) {
                var hadInteractions = false;
                var mousePosition = this._game.CurrentCamera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position);

                if (this._componentService.SelectedItem?.Component != null) {
                    if (this._game.SelectedGizmo == GizmoType.Scale) {
                        if (this._scaleGizmo.Update(gameTime, mouseState, keyboardState, mousePosition, this._componentService.SelectedItem)) {
                            this._componentService.SelectedItem.RaisePropertyChanged(nameof(BaseComponent.LocalScale));
                            hadInteractions = true;
                        }
                    }
                    else if (this._game.SelectedGizmo == GizmoType.Translation) {
                        if (this._translationGizmo.Update(gameTime, mouseState, keyboardState, mousePosition, this._componentService.SelectedItem)) {
                            this._componentService.SelectedItem.RaisePropertyChanged(nameof(BaseComponent.LocalPosition));
                            hadInteractions = true;
                        }
                    }
                }

                if (!hadInteractions && mouseState.LeftButton == ButtonState.Pressed && this._previousLeftMouseButtonState == ButtonState.Released) {
                    this._componentService.SelectComponent(null);
                    foreach (var drawable in this._game.CurrentScene.GetVisibleDrawableComponents()) {
                        if (drawable.BoundingArea.Contains(mousePosition) && drawable is BaseComponent drawableComponent) {
                            this._componentService.SelectComponent(drawableComponent);
                        }
                    }
                }
            }

            this._previousLeftMouseButtonState = mouseState.LeftButton;
        }

        private void ComponentService_SelectionChanged(object sender, ValueChangedEventArgs<ComponentWrapper> e) {
            if (e.NewValue is ComponentWrapper wrapper) {
                this.ResetDependencies(wrapper.Component);
            }
            else {
                this.ResetDependencies(null);
            }
        }

        private void ResetDependencies(object newValue) {
            if (newValue is Body body) {
                this._boundingAreaDrawer.Boundable = body;
                this._colliderDrawer.Body = body;
            }
            else if (newValue is IBoundable boundable) {
                this._boundingAreaDrawer.Boundable = boundable;
                this._colliderDrawer.Body = null;
            }
            else {
                this._boundingAreaDrawer.Boundable = null;
                this._colliderDrawer.Body = null;
            }
        }
    }
}