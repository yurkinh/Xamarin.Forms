using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	[ContentProperty(nameof(Children))]
	public class Storyboard : Timeline, IList<Timeline>
	{
		Dictionary<VisualElement, List<Animation>> _animationsPerElement = new Dictionary<VisualElement, List<Animation>>();
		List<Animation> _runningAnimations = new List<Animation>();


		List<Timeline> _timelines;

		public List<Timeline> Children => _timelines;

		public int Count => Children.Count;

		public bool IsReadOnly => false;

		public Timeline this[int index] { get => Children[index]; set => Children[index] = value; }

		public static readonly BindableProperty TargetNameProperty = BindableProperty.CreateAttached("TargetName", typeof(VisualElement), typeof(Timeline), null,
			propertyChanged: (bindable, oldValue, newValue) =>
			{

				foreach (var item in (bindable as Storyboard)?.Children)
				{
					item.SetValue(BindingContextProperty, (newValue as VisualElement).BindingContext);
				}

			});

		public static readonly BindableProperty TargetPropertyProperty = BindableProperty.CreateAttached("TargetProperty", typeof(BindableProperty), typeof(Timeline), null
			);

		public static void SetTarget(BindableObject bindable, VisualElement value)
		{
			bindable.SetValue(TargetNameProperty, value);
			SetInheritedBindingContext(bindable, value?.BindingContext);
		}

		public static VisualElement GetTarget(BindableObject bindable)
		{
			return (VisualElement)bindable.GetValue(TargetNameProperty);
		}

		public static void SetTargetProperty(BindableObject bindable, BindableProperty value)
		{
			bindable.SetValue(TargetPropertyProperty, value);
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			foreach (var item in Children)
			{
				item.SetValue(BindingContextProperty, BindingContext);
				//SetInheritedBindingContext(item, BindingContext);

			}
		}

		public static BindableProperty GetTargetProperty(BindableObject bindable)
		{
			return (BindableProperty)bindable.GetValue(TargetPropertyProperty);
		}

		public Storyboard()
		{
			_timelines = new List<Timeline>();
		}

		public void Begin(Timeline item)
		{
			_animationsPerElement.Clear();
			_runningAnimations.Clear();
			var maxDuration = Duration;
			var defaultTarget = Storyboard.GetTarget(this);
			var defaultTargetProperty = Storyboard.GetTargetProperty(this);


			if (item is Animation<Color> timeline)
			{
				maxDuration = AddAnimationColor(maxDuration, defaultTarget, defaultTargetProperty, timeline);
			}

			if (item is DoubleAnimation doubleAnimation)
			{
				AddDoubleAnimation(defaultTarget, defaultTargetProperty, doubleAnimation);
			}

			if (item is PositionAnimation positionAnimation)
			{
				AddPositionAnimation(defaultTarget, defaultTargetProperty, positionAnimation);
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
		}

		public new void Begin()
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
					maxDuration = AddAnimationColor(maxDuration, defaultTarget, defaultTargetProperty, timeline);
				}

				if (item is DoubleAnimation doubleAnimation)
				{
					AddDoubleAnimation(defaultTarget, defaultTargetProperty, doubleAnimation);
				}

				if (item is PositionAnimation positionAnimation)
				{
					AddPositionAnimation(defaultTarget, defaultTargetProperty, positionAnimation);
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

		void AddPositionAnimation(VisualElement defaultTarget, BindableProperty defaultTargetProperty, PositionAnimation positionAnimation)
		{
			var animationTarget = GetTarget(positionAnimation);
			if (animationTarget == null)
				animationTarget = defaultTarget;

			var animationTargetProperty = GetTargetProperty(positionAnimation);
			if (animationTargetProperty == null)
				animationTargetProperty = defaultTargetProperty;


			//maxDuration = Math.Max(maxDuration, doubleAnimation.Duration);
			AddToInternalAnimationList(positionAnimation, animationTarget, animationTargetProperty);
		}

		void AddDoubleAnimation(VisualElement defaultTarget, BindableProperty defaultTargetProperty, DoubleAnimation doubleAnimation)
		{
			var animationTarget = Storyboard.GetTarget(doubleAnimation);
			if (animationTarget == null)
				animationTarget = defaultTarget;

			var animationTargetProperty = Storyboard.GetTargetProperty(doubleAnimation);
			if (animationTargetProperty == null)
				animationTargetProperty = defaultTargetProperty;


			//maxDuration = Math.Max(maxDuration, doubleAnimation.Duration);
			AddToInternalAnimationList(doubleAnimation, animationTarget, animationTargetProperty);
		}

		uint AddAnimationColor(uint maxDuration, VisualElement defaultTarget, BindableProperty defaultTargetProperty, Animation<Color> timeline)
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

			if (!_animationsPerElement.ContainsKey(animationTarget))
			{
				var list = new List<Animation>();
				list.Add(newAnimation);
				_animationsPerElement.Add(animationTarget, list);
			}
			else
			{
				_animationsPerElement[animationTarget].Add(newAnimation);
			}

			return maxDuration;
		}

		void AddToInternalAnimationList<T>(Animation<T> animation, VisualElement animationTarget, BindableProperty animationTargetProperty, Animation newAnimation = null)
		{
			if (animationTarget == null)
				throw new InvalidOperationException("You didn't specify a Target");


			if (animationTargetProperty == null)
				throw new InvalidOperationException("You didn't specify a TargetProperty");

			//not sure 
			SetInheritedBindingContext(animation, animationTarget.BindingContext);

			var initialValue = (double)animationTarget.GetValue(animationTargetProperty);

			double finalValue = initialValue;

			if (animation is DoubleAnimation)
				finalValue = (double)animation.GetValue(DoubleAnimation.ToProperty);

			if (animation is PositionAnimation)
				finalValue = (int)animation.GetValue(PositionAnimation.ToProperty);

			if (newAnimation == null)
			{
				if (animation is PositionAnimation)
				{
					Rectangle start = animationTarget.Bounds;
					Func<double, Rectangle> computeBounds = null;
					bool isX = animationTargetProperty.PropertyName.ToLower() == "x";
					Point point = isX ? new Point(finalValue, start.Y) : new Point(start.X, finalValue);
					var bounds = new Rectangle(point, start.Size);

					if (isX)
					{
						computeBounds = progress =>
						{
							double x = start.X + (bounds.X - start.X) * progress;
							double y = animationTarget.Bounds.Y;
							double w = bounds.Width;
							double h = bounds.Height;
							return new Rectangle(x, y, w, h);
						};
					}
					else
					{
						computeBounds = progress =>
						{
							double x = animationTarget.Bounds.X;
							double y = start.Y + (bounds.Y - start.Y) * progress;
							double w = bounds.Width;
							double h = bounds.Height;

							return new Rectangle(x, y, w, h);
						};
					}
					newAnimation = GetAnimateTo(animationTarget, 0, 1, (v, value) => v.Layout(computeBounds(value)), animation.Easing ?? Easing);

				}
				else
				{
					newAnimation = GetAnimateTo(animationTarget, initialValue, finalValue, (v, value) => animationTarget.SetValue(animationTargetProperty, value), animation.Easing ?? Easing);
				}

			}

			//mainAnimation.Add(animation.BeginTime, 1, newAnimation);

			if (!_animationsPerElement.ContainsKey(animationTarget))
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

		static Animation GetAnimateTo(VisualElement view, double start, double end,
			Action<VisualElement, double> updateAction, Easing easing = null)
		{
			if (easing == null)
				easing = Easing.Linear;

			var tcs = new TaskCompletionSource<bool>();

			var weakView = new WeakReference<VisualElement>(view);

			void UpdateProperty(double f)
			{
				if (weakView.TryGetTarget(out VisualElement v))
				{
					updateAction(v, f);
				}
			}

			return new Animation(UpdateProperty, start, end, easing);
		}

		public void End()
		{

		}


		public static string GetTargetName(object t)
		{
			throw new NotImplementedException();
		}

		void AnimationPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == DoubleAnimation.ToProperty.PropertyName ||
				e.PropertyName == PositionAnimation.ToProperty.PropertyName)
			{
				Begin(sender as Timeline);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _timelines.GetEnumerator();
		}

		public int IndexOf(Timeline item)
		{
			return Children.IndexOf(item);
		}

		public void Insert(int index, Timeline item)
		{
			Children.Insert(index, item);
		}

		public void Add(Timeline item)
		{
			Children.Add(item);
			item.PropertyChanged += AnimationPropertyChanged;

			//((IList<IAnimation>)Children).Add(item);
		}

		public bool Contains(Timeline item)
		{
			return Children.Contains(item);
		}

		public void CopyTo(Timeline[] array, int arrayIndex)
		{
			Children.CopyTo(array, arrayIndex);
		}

		public bool Remove(Timeline item)
		{
			item.PropertyChanged -= AnimationPropertyChanged;

			return Children.Remove(item);
		}

		IEnumerator<Timeline> IEnumerable<Timeline>.GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		public void RemoveAt(int index)
		{
			Children.RemoveAt(index);
		}

		public void Clear()
		{
			Children.Clear();
		}

		//Storyboard.SetTarget(animOpacity, Control_);
		//  Storyboard.SetTarget(animTransform, Control_);
		//  Storyboard.SetTargetProperty(animOpacity, new PropertyPath("Opacity"));
		//  Storyboard.SetTargetProperty(animTransform, new PropertyPath(

		//public void Pause();
		//public void Resume();
		//public static void SetTarget(this Storyboard s, string target);
		//public static void SetTargetProperty(this Storyboard s, string property);
	}

	public interface IInterpolatable
	{
		object InterpolateTo(object from, object target, double interpolation);
	}

	public interface IAnimation
	{
		void Begin();
	}

	public interface IAnimation<T> : IAnimation
	{
		T From { get; set; }
		T To { get; set; }
	}

	public interface IInterpolatable<T> : IInterpolatable
	{
		T InterpolateTo(T from, T target, double interpolation);
	}

	public abstract class Animation<T> : Timeline
	{
		public abstract T From { get; set; }
		public abstract T To { get; set; }
	}


	public class AnimateExtension : BindableObject, IMarkupExtension<Timeline>
	{
	
		public BindableProperty Property { set; get; }

		public string Binding { set; get; }

		public Timeline ProvideValue(IServiceProvider serviceProvider)
		{
			var anim = new PositionAnimation();

			BindingBase GetBinding()
			{
				return new Binding(Binding);
			}
			
			anim.SetBinding(PositionAnimation.ToProperty, GetBinding());
			Storyboard.SetTargetProperty(anim, Property);
			return anim;
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<Timeline>).ProvideValue(serviceProvider);
		}
	}


	public abstract class InterpolatableAnimation<T> : Animation<T> where T : IInterpolatable<T>
	{
	}


	public class PositionAnimation : Animation<int>
	{

		public static readonly BindableProperty FromProperty = BindableProperty.Create(nameof(From), typeof(int), typeof(PositionAnimation), 0);

		public static readonly BindableProperty ToProperty = BindableProperty.Create(nameof(To), typeof(int), typeof(PositionAnimation), 0);

		public override int From
		{
			get { return (int)GetValue(FromProperty); }
			set { SetValue(FromProperty, value); }
		}

		public override int To
		{
			get { return (int)GetValue(ToProperty); }
			set { SetValue(ToProperty, value); }
		}

	}

	public class DoubleAnimation : Animation<double>
	{
		public static readonly BindableProperty FromProperty = BindableProperty.Create(nameof(From), typeof(double), typeof(DoubleAnimation), .0);

		public static readonly BindableProperty ToProperty = BindableProperty.Create(nameof(To), typeof(double), typeof(DoubleAnimation), .0);

		public override double From
		{
			get { return (double)GetValue(FromProperty); }
			set { SetValue(FromProperty, value); }
		}

		public override double To
		{
			get { return (double)GetValue(ToProperty); }
			set { SetValue(ToProperty, value); }
		}
	}

	public abstract class Timeline : BindableObject, IAnimation
	{
		const uint defaultDuration = 300;

		protected Timeline()
		{
			Duration = defaultDuration;
			Easing = Easing.SpringOut;
		}

		public uint BeginTime { get; set; }

		public uint Duration { get; set; }

		public bool AutoReverse { get; set; }

		public Easing Easing { get; set; }

		public EventHandler Completed { get; set; }

		public void Begin()
		{
			throw new NotImplementedException();
		}
	}
}
