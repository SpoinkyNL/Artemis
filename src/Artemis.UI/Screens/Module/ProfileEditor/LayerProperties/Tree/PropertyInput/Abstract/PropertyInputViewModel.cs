﻿using System;
using Stylet;

namespace Artemis.UI.Screens.Module.ProfileEditor.LayerProperties.Tree.PropertyInput.Abstract
{
    public abstract class PropertyInputViewModel<T> : ValidatingModelBase, IDisposable
    {
        private T _inputValue;

        protected PropertyInputViewModel(LayerPropertyViewModel<T> layerPropertyViewModel)
        {
            LayerPropertyViewModel = layerPropertyViewModel;
            LayerPropertyViewModel.LayerProperty.Updated += LayerPropertyOnUpdated;
        }

        protected PropertyInputViewModel(LayerPropertyViewModel<T> layerPropertyViewModel, IModelValidator validator) : base(validator)
        {
            LayerPropertyViewModel = layerPropertyViewModel;
            LayerPropertyViewModel.LayerProperty.Updated += LayerPropertyOnUpdated;
        }

        public LayerPropertyViewModel<T> LayerPropertyViewModel { get; }

        public bool InputDragging { get; private set; }

        public T InputValue
        {
            get => _inputValue;
            set
            {
                _inputValue = value;
                ApplyInputValue();
            }
        }

        public virtual void Dispose()
        {
            LayerPropertyViewModel.LayerProperty.Updated -= LayerPropertyOnUpdated;
        }

        protected virtual void OnInputValueChanged()
        {
        }

        private void LayerPropertyOnUpdated(object sender, EventArgs e)
        {
            UpdateInputValue();
        }

        private void UpdateInputValue()
        {
            // Avoid unnecessary UI updates and validator cycles
            if (_inputValue.Equals(LayerPropertyViewModel.LayerProperty.CurrentValue))
                return;

            // Override the input value
            _inputValue = LayerPropertyViewModel.LayerProperty.CurrentValue;

            // Notify a change in the input value
            OnInputValueChanged();
            NotifyOfPropertyChange(nameof(InputValue));

            // Force the validator to run with the overridden value
            Validate();
        }

        private void ApplyInputValue()
        {
            // Force the validator to run
            Validate();
            // Only apply the input value to the layer property if the validator found no errors
            if (!HasErrors)
                LayerPropertyViewModel.SetCurrentValue(_inputValue, !InputDragging);

            OnInputValueChanged();
        }

        #region Event handlers

        public void InputDragStarted(object sender, EventArgs e)
        {
            InputDragging = true;
        }

        public void InputDragEnded(object sender, EventArgs e)
        {
            InputDragging = false;
            LayerPropertyViewModel.ProfileEditorService.UpdateSelectedProfileElement();
        }

        #endregion
    }
}