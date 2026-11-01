using System.Globalization;

namespace Project6_HotelApi.Models
{
	public class HotelSearchPageViewModel
	{
		public string? CityName { get; set; }
		public DateTime CheckIn { get; set; }
		public DateTime CheckOut { get; set; }
		public int Adults { get; set; } = 2;
		public int Children { get; set; }
		public int Rooms { get; set; } = 1;
		public int ChildAge { get; set; } = 2;
		public string Currency { get; set; } = "EUR";
		public string? LocationLabel { get; set; }
		public string? ErrorMessage { get; set; }
		public List<HotelCardViewModel> Hotels { get; set; } = new();
	}

	public class HotelCardViewModel
	{
		public long HotelId { get; set; }
		public string Name { get; set; } = "";
		public string? PhotoUrl { get; set; }
		public string? City { get; set; }
		public double? Price { get; set; }
		public string? Currency { get; set; }
		public double? ReviewScore { get; set; }
		public string? ReviewScoreWord { get; set; }
		public int? ReviewCount { get; set; }
		public int? Stars { get; set; }
	}

	public class HotelDetailViewModel
	{
		public long HotelId { get; set; }
		public string Name { get; set; } = "";
		public string Address { get; set; } = "";
		public string? AccommodationType { get; set; }
		public int? ReviewCount { get; set; }
		public double? ReviewScore { get; set; }
		public string? ReviewScoreWord { get; set; }
		public double? Price { get; set; }
		public string? Currency { get; set; }
		public string? BookingUrl { get; set; }
		public string? CityName { get; set; }
		public string? City { get; set; }
		public string? Country { get; set; }
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public DateTime CheckIn { get; set; }
		public DateTime CheckOut { get; set; }
		public int Adults { get; set; }
		public int Children { get; set; }
		public int Rooms { get; set; } = 1;
		public int ChildAge { get; set; } = 2;
		public List<HotelRoomViewModel> RoomsList { get; set; } = new();
		public List<string> Photos { get; set; } = new();
		public List<string> Facilities { get; set; } = new();
		public List<string> Descriptions { get; set; } = new();
		public List<string> Notes { get; set; } = new();
		public string? ErrorMessage { get; set; }
	}
	public class HotelRoomViewModel
	{
		public string Name { get; set; } = "";
		public string? PhotoUrl { get; set; }
		public double? Size { get; set; }
		public int? Adults { get; set; }
		public int? Children { get; set; }
		public bool BreakfastIncluded { get; set; }
		public bool Refundable { get; set; }
		public string? Policy { get; set; }
	}

	public static class HotelFormat
	{
		private static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

		public static string Money(double? value, string? currency)
		{
			if (!value.HasValue)
				return "Fiyat yok";

			var symbol = currency switch
			{
				"EUR" => "€",
				"USD" => "$",
				"TRY" => "₺",
				"GBP" => "£",
				_ => (currency ?? "") + " "
			};
			return symbol + value.Value.ToString("N0", Tr);
		}

		public static string Score(double? score)
		{
			return score?.ToString("0.0", Tr) ?? "";
		}

		public static string ScoreWord(double? score, string? fallback)
		{
			if (score >= 9) return "Olağanüstü";
			if (score >= 8) return "Çok İyi";
			if (score >= 7) return "İyi";
			if (score >= 6) return "Keyifli";
			return string.IsNullOrWhiteSpace(fallback) ? "Puan" : fallback;
		}

		public static int Nights(DateTime checkIn, DateTime checkOut)
		{
			var nights = (checkOut.Date - checkIn.Date).Days;
			return nights > 0 ? nights : 1;
		}

		public static string Guests(int adults, int children)
		{
			return children > 0 ? $"{adults} Yetişkin, {children} Çocuk" : $"{adults} Yetişkin";
		}

		public static string Stars(int count)
		{
			return new string('★', count);
		}

		public static string Date(DateTime date)
		{
			return date.ToString("dd MMMM yyyy", Tr);
		}
	}
}
