using System;
using System.Globalization;
using Newtonsoft.Json;

namespace SC.DevChallenge.Api.Converters
{
	public class DecimalFormatConverter : JsonConverter
	{
		private readonly int _precision;

		public DecimalFormatConverter(int precision)
		{
			_precision = precision;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue(((decimal)value).ToString($"F{_precision}", CultureInfo.InvariantCulture));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(decimal);
		}
	}
}
