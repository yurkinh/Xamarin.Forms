using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms
{
	[Xaml.TypeConversion(typeof(IVisual))]
	public class VisualTypeConverter : TypeConverter
	{
		static Dictionary<string, IVisual> _visualTypeMappings;
		static Dictionary<string, IVisual> InitMappings()
		{
			var mappings = new Dictionary<string, IVisual>(StringComparer.OrdinalIgnoreCase);
			_visualTypeMappings = mappings;
			return mappings;
		}

		public static void Register(string visualKey, IVisual visual)
		{
			if (_visualTypeMappings == null)
				InitMappings();

			_visualTypeMappings[visualKey] = visual;
		}

		internal static void Register(Type visual)
		{
			if (_visualTypeMappings == null)
				InitMappings();

			if(!_visualTypeMappings.ContainsKey(visual.FullName))
			{
				try
				{
					IVisual registeredVisual = (IVisual)Activator.CreateInstance(visual);
					_visualTypeMappings[visual.Name] = registeredVisual;
					_visualTypeMappings[visual.FullName] = registeredVisual;
					_visualTypeMappings[$"{visual.Name}Visual"] = registeredVisual;
					_visualTypeMappings[$"{visual.FullName}Visual"] = registeredVisual;
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

			throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(IVisual)}");
		}
	}
}