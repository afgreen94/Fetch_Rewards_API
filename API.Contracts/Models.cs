using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace API.Contracts
{
    [JsonSerializable(typeof(Receipt))]
    public class Receipt
    {

        [JsonPropertyName("retailer")]
        [Required(ErrorMessage = "Cannot be null")]
        public string Retailer { get; set; }

        [JsonPropertyName("purchaseDate")]
        [Required(ErrorMessage = "Cannot be null")]
        public DateTime PurchaseDate { get; set; }

        [JsonPropertyName("purchaseTime")]
        [Required(ErrorMessage = "Cannot be null")]
        [JsonConverter(typeof(ClockTimeJsonConverter))]
        public ClockTime PurchaseTime { get; set; }

        [JsonPropertyName("items")]
        [Required(ErrorMessage = "Cannot be null")]
        [RequiredNonEmpty]
        public Item[] Items { get; set; }


        [JsonSerializable(typeof(Item))]
        public class Item
        {
            [JsonPropertyName("shortDescription")]
            [Required(ErrorMessage = "Cannot be null")]
            public string ShortDescription { get; set; }


            private double price;

            [JsonPropertyName("price")]
            [Required(ErrorMessage = "Cannot be null")]
            public double Price { get { return this.price; } set { this.price = Math.Round(value, 2); } }
        }
    }

    public class ProcessCallResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
    }

    public class PointsCallResponse
    {
        [JsonPropertyName("points")]
        public int Points { get; set; }
    }


    //Enforces array-type json property non-emptiness 
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredNonEmptyAttribute : ValidationAttribute
    {
        public RequiredNonEmptyAttribute()
            : base("The {0} property must not be null or empty.") // Default error message
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is Array arr && arr.Length > 0) return ValidationResult.Success;

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
        }
    }

    public class ReceiptPointValueWrapper
    {
        public Receipt Receipt { get; set; }
        public int? Points { get; set; }
    }

    public class IdReceiptPointsWrapper : ReceiptPointValueWrapper
    {
        public Guid Id { get; set; }
    }

    public struct ClockTime
    {
        private readonly int hour;
        private readonly int minute;

        public readonly int Hour => this.hour;
        public readonly int Minute => this.minute;

        public ClockTime(int hour, int minute) { this.hour = hour; this.minute = minute; }


        public static bool operator ==(ClockTime c0, ClockTime c1) => c0.Hour == c1.Hour && c0.Minute == c1.Minute;
        public static bool operator !=(ClockTime c0, ClockTime c1) => c0.Hour != c1.Hour || c0.Minute != c1.Minute;
        public static bool operator >(ClockTime c0, ClockTime c1) => c0.Hour > c1.Hour || (c0.Hour == c1.Hour && c0.Minute > c1.Minute);
        public static bool operator <(ClockTime c0, ClockTime c1) => c0.Hour < c1.Hour || (c0.Hour == c1.Hour && c0.Minute < c1.Minute);
        public static bool operator >=(ClockTime c0, ClockTime c1) => c0.Hour > c1.Hour || (c0.Hour == c1.Hour && c0.Minute >= c1.Minute);
        public static bool operator <=(ClockTime c0, ClockTime c1) => c0.Hour < c1.Hour || (c0.Hour == c1.Hour && c0.Minute <= c1.Minute);

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(this.hour);
            sb.Append(':');
            sb.Append(this.minute);

            return sb.ToString();
        }

    }

    public class ClockTimeJsonConverter : JsonConverter<ClockTime>
    {
        public override ClockTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();

            var parts = s.Split(':');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int hour) ||
                !int.TryParse(parts[1], out int minute))
                throw new JsonException("Invalid time format.");

            if (hour < 0 || hour > 23)
                throw new JsonException("Invalid hour value. (0 - 23)");

            if (minute < 0 || minute > 59)
                throw new JsonException("Invalid minute value. (0 - 59)");

            return new ClockTime(hour, minute);
        }

        public override void Write(Utf8JsonWriter writer, ClockTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
    }


    public abstract class MicrosoftConfigurationWrapper(IConfiguration innerConfiguration)
    {
        protected IConfiguration innerConfiguration = innerConfiguration;
    }
}
