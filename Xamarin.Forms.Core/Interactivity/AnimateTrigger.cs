using System;

namespace Xamarin.Forms
{
    [ContentProperty(nameof(Storyboard))]
    public class AnimateTrigger : TriggerBase
    {
        public AnimateTrigger() : base(typeof(BindableObject))
        {

        }

        internal override void OnAttachedTo(BindableObject bindable)
        {
            bindable.BindingContextChanged += BindableBindingContextChanged;
            base.OnAttachedTo(bindable);
            Storyboard.SetTarget(Storyboard, bindable as VisualElement);
        }

        internal override void OnDetachingFrom(BindableObject bindable)
        {
            bindable.BindingContextChanged -= BindableBindingContextChanged;
            base.OnDetachingFrom(bindable);
        }

        void BindableBindingContextChanged(object sender, EventArgs e)
        {
            SetInheritedBindingContext(Storyboard, (sender as BindableObject).BindingContext);
        }

        Storyboard _storyboard;
        public Storyboard Storyboard
        {
            get
            {
                if (_storyboard != null)
                    return _storyboard;
                _storyboard = new Storyboard();
                return _storyboard;
            }
            set
            {
                if (_storyboard == value)
                    return;
                OnPropertyChanging();
                _storyboard = value;

                OnPropertyChanged();
            }
        }
    }
}