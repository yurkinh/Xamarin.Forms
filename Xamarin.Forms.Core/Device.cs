using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial struct Profile : IDisposable
	{
		const int Capacity = 1000;
		const string PROFILE = "PROFILE";
		public struct Datum
		{
			public string Name;
			public string Id;
			public long? Ticks;
			public int Depth;
			public string Path;
			public int Line;
		}
		public static List<Datum> Data = new List<Datum>(Capacity);

		static Stack<Profile> s_stack = new Stack<Profile>(Capacity);
		static int s_depth = 0;
		static bool s_stopped = false;

		readonly long start;
		readonly string name;
		readonly int slot;
		readonly string id;
		readonly string path;
		readonly int line;

		public static void Stop()
		{
			// unwind stack
			s_stopped = true;
			while (s_stack.Count > 0)
				s_stack.Pop();
		}
		[Conditional(PROFILE)]
		public static void Push(
			[CallerMemberName] string name = "",
			string id = null,
			[CallerFilePath] string path = "",
			[CallerLineNumber] int line = 0)
		{
			if (s_stopped)
				return;

			s_stack.Push(new Profile(name, id, path, line));
		}
		[Conditional(PROFILE)]
		public static void Pop()
		{
			if (s_stopped)
				return;

			var profile = s_stack.Pop();
			profile.Dispose();
		}
		[Conditional(PROFILE)]
		public static void PopPush(
			string id,
			[CallerFilePath] string path = "",
			[CallerLineNumber] int line = 0)
		{
			if (s_stopped)
				return;

			var profile = s_stack.Pop();
			var name = profile.name;
			profile.Dispose();

			Push(name, id, path, line);
		}

		private Profile(
			string name,
			string id = null,
			string path = null,
			int line = 0)
		{
			this = default(Profile);
			start = DateTime.Now.Ticks;

			this.name = name;
			this.id = id;
			this.path = path;
			this.line = line;

			slot = Data.Count;
			Data.Add(new Datum()
			{
				Depth = s_depth,
				Name = name,
				Id = id,
				Ticks = null,
				Path = path,
				Line = line
			});

			s_depth++;
		}
		public void Dispose()
		{
			if (s_stopped && start == 0)
				return;

			var ticks = DateTime.Now.Ticks - start;
			var depth = --s_depth;

			var datum = Data[slot];
			datum.Ticks = ticks;
			Data[slot] = datum;
		}
	}

	public static class Device
    {
        public const string iOS = "iOS";
        public const string Android = "Android";
        public const string UWP = "UWP";
        public const string macOS = "macOS";
        public const string GTK = "GTK";
        public const string Tizen = "Tizen";
		public const string WPF = "WPF";

		[EditorBrowsable(EditorBrowsableState.Never)]
        public static DeviceInfo info;

        static IPlatformServices s_platformServices;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetIdiom(TargetIdiom value) => Idiom = value;
        public static TargetIdiom Idiom { get; internal set; }

		//TODO: Why are there two of these? This is never used...?
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetTargetIdiom(TargetIdiom value) => Idiom = value;

        [Obsolete("TargetPlatform is obsolete as of version 2.3.4. Please use RuntimePlatform instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable 0618
		public static TargetPlatform OS
        {
            get
            {
                TargetPlatform platform;
                if (Enum.TryParse(RuntimePlatform, out platform))
                    return platform;

                // In the old TargetPlatform, there was no distinction between WinRT/UWP
                if (RuntimePlatform == UWP)
                {
                    return TargetPlatform.Windows;
                }

                return TargetPlatform.Other;
            }
        }
#pragma warning restore 0618

		public static string RuntimePlatform => PlatformServices.RuntimePlatform;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static DeviceInfo Info
		{
			get
			{
				if (info == null)
					throw new InvalidOperationException("You MUST call Xamarin.Forms.Init(); prior to using it.");
				return info;
			}
			set { info = value; }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetFlowDirection(FlowDirection value) => FlowDirection = value;
		public static FlowDirection FlowDirection { get; internal set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsInvokeRequired
		{
			get { return PlatformServices.IsInvokeRequired; }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IPlatformServices PlatformServices
		{
			get
			{
				if (s_platformServices == null)
					throw new InvalidOperationException("You MUST call Xamarin.Forms.Init(); prior to using it.");
				return s_platformServices;
			}
			set { s_platformServices = value; }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IReadOnlyList<string> Flags { get; private set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetFlags(IReadOnlyList<string> flags)
		{
			Flags = flags;
		}

		public static void BeginInvokeOnMainThread(Action action)
		{
			PlatformServices.BeginInvokeOnMainThread(action);
		}

		public static double GetNamedSize(NamedSize size, Element targetElement)
		{
			return GetNamedSize(size, targetElement.GetType());
		}

        public static double GetNamedSize(NamedSize size, Type targetElementType)
        {
            return GetNamedSize(size, targetElementType, false);
        }

        [Obsolete("OnPlatform is obsolete as of version 2.3.4. Please use 'switch (Device.RuntimePlatform)' instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void OnPlatform(Action iOS = null, Action Android = null, Action WinPhone = null, Action Default = null)
        {
            switch (OS)
            {
                case TargetPlatform.iOS:
                    if (iOS != null)
                        iOS();
                    else if (Default != null)
                        Default();
                    break;
                case TargetPlatform.Android:
                    if (Android != null)
                        Android();
                    else if (Default != null)
                        Default();
                    break;
                case TargetPlatform.Windows:
                case TargetPlatform.WinPhone:
                    if (WinPhone != null)
                        WinPhone();
                    else if (Default != null)
                        Default();
                    break;
                case TargetPlatform.Other:
                    if (Default != null)
                        Default();
                    break;
            }
        }

        [Obsolete("OnPlatform<> (generic) is obsolete as of version 2.3.4. Please use 'switch (Device.RuntimePlatform)' instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static T OnPlatform<T>(T iOS, T Android, T WinPhone)
        {
            switch (OS)
            {
                case TargetPlatform.iOS:
                    return iOS;
                case TargetPlatform.Android:
                    return Android;
                case TargetPlatform.Windows:
                case TargetPlatform.WinPhone:
                    return WinPhone;
            }

            return iOS;
        }

        public static void OpenUri(Uri uri)
        {
            PlatformServices.OpenUriAction(uri);
        }

        public static void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            PlatformServices.StartTimer(interval, callback);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Assembly[] GetAssemblies()
        {
            return PlatformServices.GetAssemblies();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
        {
            return PlatformServices.GetNamedSize(size, targetElementType, useOldSizes);
        }

        internal static Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            return PlatformServices.GetStreamAsync(uri, cancellationToken);
        }

        public static class Styles
        {
            public static readonly string TitleStyleKey = "TitleStyle";

            public static readonly string SubtitleStyleKey = "SubtitleStyle";

            public static readonly string BodyStyleKey = "BodyStyle";

            public static readonly string ListItemTextStyleKey = "ListItemTextStyle";

            public static readonly string ListItemDetailTextStyleKey = "ListItemDetailTextStyle";

            public static readonly string CaptionStyleKey = "CaptionStyle";

            public static readonly Style TitleStyle = new Style(typeof(Label)) { BaseResourceKey = TitleStyleKey };

            public static readonly Style SubtitleStyle = new Style(typeof(Label)) { BaseResourceKey = SubtitleStyleKey };

            public static readonly Style BodyStyle = new Style(typeof(Label)) { BaseResourceKey = BodyStyleKey };

            public static readonly Style ListItemTextStyle = new Style(typeof(Label)) { BaseResourceKey = ListItemTextStyleKey };

            public static readonly Style ListItemDetailTextStyle = new Style(typeof(Label)) { BaseResourceKey = ListItemDetailTextStyleKey };

            public static readonly Style CaptionStyle = new Style(typeof(Label)) { BaseResourceKey = CaptionStyleKey };
        }
    }
}
