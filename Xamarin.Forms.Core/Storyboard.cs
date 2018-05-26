using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms
{
	[ContentProperty(nameof(Children))]
	public class Storyboard : Timeline
	{
		Dictionary<VisualElement, List<Animation>> _animationsPerElement = new Dictionary<VisualElement, List<Animation>>();
		List<Animation> _runningAnimations = new List<Animation>();	
		public TimelineCollection Children { get; }
		public static readonly BindableProperty TargetNameProperty = BindableProperty.CreateAttached("TargetName", typeof(VisualElement), typeof(Timeline), null);

		public static readonly BindableProperty TargetPropertyProperty = BindableProperty.CreateAttached("TargetProperty", typeof(BindableProperty), typeof(Timeline), null);

		public static void SetTarget(BindableObject bindable, VisualElement value)
		{
			bindable.SetValue(TargetNameProperty, value);
		}

		public static VisualElement GetTarget(BindableObject bindable)
		{
			return (VisualElement)bindable.GetValue(TargetNameProperty);
		}

		public static void SetTargetProperty(BindableObject bindable, BindableProperty value)
		{
			bindable.SetValue(TargetPropertyProperty, value);
		}

		public static BindableProperty GetTargetProperty(BindableObject bindable)
		{
			return (BindableProperty)bindable.GetValue(TargetPropertyProperty);
		}

		public Storyboard()
		{
			Children = new TimelineCollection();
		}

		public void Add(Timeline animation)
		{
			Children.Add(animation);
		}

		public void Remove(Timeline animation)
		{
			Children.Remove(animation);
		}

		public void Begin()
		{
			_animationsPerElement.Clear();
			_runningAnimations.Clear();
			var maxDuration = Duration;
			var defaultTarget = Storyboard.GetTarget(this);
			var defaultTargetProperty = Storyboard.GetTargetProperty(this);
			foreach (var item in Children)
			{
				if (item is Animation<Color> timeline)
				{
					var animationTarget = Storyboard.GetTarget(timeline);
					if (animationTarget == null)
						animationTarget = defaultTarget;
					
					var animationTargetProperty = Storyboard.GetTargetProperty(timeline);
					if (animationTargetProperty == null)
						animationTargetProperty = defaultTargetProperty;
					
					var initialValue = animationTarget.GetValue(animationTargetProperty);

					var finalValue = timeline.To;
					var newAnimation = new Animation(v =>
					{
						object newValue = null;

						if (timeline.From is IInterpolatable interpolatable)
						{
							newValue = interpolatable.InterpolateTo(initialValue, finalValue, v);
						}

						animationTarget.SetValue(animationTargetProperty, newValue);
					}, 0, 1, timeline.Easing, () => timeline?.Completed?.Invoke(this, new EventArgs()));

					//mainAnimation.Add(animation.BeginTime, 1, newAnimation);
					maxDuration = Math.Max(maxDuration, timeline.Duration);

					if(!_animationsPerElement.ContainsKey(animationTarget))
					{
						var list = new List<Animation>();
						list.Add(newAnimation);
						_animationsPerElement.Add(animationTarget, list);
					}
					else
					{
						_animationsPerElement[animationTarget].Add(newAnimation);
					}
				}

			}

			foreach (var anim in _animationsPerElement)
			{
				var elementAnimation = new Animation();
				foreach (var animation in anim.Value)
				{
					elementAnimation.Add(0, 1, animation);
			
				}
				_runningAnimations.Add(elementAnimation);
				elementAnimation.Commit(anim.Key, "ChildAnimations", 16, maxDuration, finished: (v, c) => Completed?.Invoke(this, new EventArgs()));
			}
			//
		}
		public void End()
		{
			
		}

		//public void Pause();
		//public void Resume();
		//public static void SetTarget(this Storyboard s, string target);
		//public static void SetTargetProperty(this Storyboard s, string property);
	}

	public interface IInterpolatable
	{
		object InterpolateTo(object from, object target, double interpolation);
	}

	public interface IInterpolatable<T> : IInterpolatable
	{
		T InterpolateTo(T from, T target, double interpolation);
	}

	public class Animation<T> : Timeline where T : IInterpolatable<T>
	{
		public T From { get; set; }
		public T To { get; set; }
	}

	public class DoubleAnimation : Timeline
	{
		public double From { get; set; }
		public double To { get; set; }
	}

	public abstract class Timeline : VisualElement
	{
		const uint defaultDuration = 4000;

		protected Timeline()
		{
			Duration = defaultDuration;
			Easing = Easing.Linear;
		}

		public uint BeginTime { get; set; }

		public uint Duration { get; set; }

		public bool AutoReverse { get; set; }

		public Easing Easing { get; set; }
		public EventHandler Completed { get; set; }

	}

	public class TimelineCollection : List<Timeline>
	{

	}
}
