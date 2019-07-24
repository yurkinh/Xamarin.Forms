using System;

namespace Xamarin.Forms
{

	[ContentProperty(nameof(Storyboard))]
	public class AnimateTrigger : EventTrigger
	{
		public AnimateTrigger()
		{

		}

		internal override void OnAttachedTo(BindableObject bindable)
		{
			bindable.BindingContextChanged += BindableBindingContextChanged;
			base.OnAttachedTo(bindable);
			var existingTarget = Storyboard.GetTargetName(Storyboard);
			if (existingTarget == null)
			{
				Storyboard.SetTarget(Storyboard, bindable as VisualElement);

			}
			Actions.Add(new StoryboardTriggerAction(Storyboard));

		}

		internal override void OnDetachingFrom(BindableObject bindable)
		{
			bindable.BindingContextChanged -= BindableBindingContextChanged;
			base.OnDetachingFrom(bindable);
		}

		void BindableBindingContextChanged(object sender, EventArgs e)
		{
			//SetInheritedBindingContext(Storyboard, (sender as BindableObject).BindingContext);
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

	internal class StoryboardTriggerAction : TriggerAction<VisualElement>
	{
		Storyboard _storyboard;
		public StoryboardTriggerAction(Storyboard storyboard)
		{
			_storyboard = storyboard;
		}

		protected override void Invoke(VisualElement sender)
		{
			_storyboard?.Begin();
		}
	}
}