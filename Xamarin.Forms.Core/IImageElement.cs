﻿using System;
using System.ComponentModel;
using Xamarin.Forms.Core;

namespace Xamarin.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IImageElement : ITintColor
	{
		//note to implementor: implement this property publicly
		Aspect Aspect { get; }
		ImageSource Source { get; }
		bool IsOpaque { get; }


		//note to implementor: but implement these methods explicitly
		void RaiseImageSourcePropertyChanged();
		void OnImageSourceSourceChanged(object sender, EventArgs e);
		bool IsLoading { get; }
		bool IsAnimationPlaying { get; }
	}
}
