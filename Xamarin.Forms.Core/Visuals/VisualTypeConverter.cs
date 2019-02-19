using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	[Xaml.TypeConversion(typeof(IVisual))]
	public class VisualTypeConverter : TypeConverter
	{
		static Dictionary<string, IVisual> _visualTypeMappings;
		void InitMappings()
		{
			var mappings = new Dictionary<string, IVisual>(StringComparer.OrdinalIgnoreCase);
			Assembly[] assemblies = Device.GetAssemblies();

			foreach (var assembly in assemblies)
				Register(assembly, mappings);

			if (Internals.Registrar.ExtraAssemblies != null)
				foreach (var assembly in Internals.Registrar.ExtraAssemblies)
					Register(assembly, mappings);

			_visualTypeMappings = mappings;
		}

		static void Register(Assembly assembly, Dictionary<string, IVisual> mappings)
		{
#if NETSTANDARD2_0
			foreach (var type in assembly.GetExportedTypes())
				if (typeof(IVisual).IsAssignableFrom(type) && type != typeof(IVisual))
					Register(type, mappings);
#else
			foreach (var type in assembly.ExportedTypes)
				if (typeof(IVisual).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && type != typeof(IVisual))
					Register(type, mappings);

#endif
		}

		static void Register(Type visual, Dictionary<string, IVisual> mappings)
		{
			if(!mappings.ContainsKey(visual.FullName))
			{
				try
				{
					IVisual registeredVisual = (IVisual)Activator.CreateInstance(visual);
					string name = visual.Name;
					string fullName = visual.FullName;

					if (name.EndsWith("Visual", StringComparison.OrdinalIgnoreCase))
					{
						name = name.Substring(0, name.Length - 6);
						fullName = fullName.Substring(0, fullName.Length - 6);
					}

					mappings[name] = registeredVisual;
					mappings[fullName] = registeredVisual;
					mappings[$"{name}Visual"] = registeredVisual;
					mappings[$"{fullName}Visual"] = registeredVisual;
				}
				catch
				{
					Internals.Log.Warning("Visual", $"Unable to register {visual} please add default constructor or call VisualTypeConverter.Register");
				}
			}
		}

		public override object ConvertFromInvariantString(string value)
		{
			if (_visualTypeMappings == null)
				InitMappings();

			if (value != null)
			{
				IVisual returnValue = null;
				if (_visualTypeMappings.TryGetValue(value, out returnValue))
					return returnValue;

				return VisualMarker.Default;
			}

			throw new XamlParseException($"Cannot convert \"{value}\" into {typeof(IVisual)}");
		}
	}
}