using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Project6_HotelApi.Controllers
{
	public class HotelController : Controller
	{
		private static readonly string[] AllowedCurrencies = { "TRY", "EUR", "USD", "GBP" };
		private readonly string _apiKey;
		private readonly string _apiHost;

		public HotelController(IConfiguration configuration)
		{
			_apiKey = configuration["RapidApi:Key"] ?? "";
			_apiHost = configuration["RapidApi:BookingHost"] ?? "booking-com15.p.rapidapi.com";
		}

		public async Task<IActionResult> Index(HotelSearchPageViewModel model)
		{
			model ??= new HotelSearchPageViewModel();
			PrepareForm(model);

			if (string.IsNullOrWhiteSpace(model.CityName))
				return View(model);

			if (string.IsNullOrWhiteSpace(_apiKey))
			{
				model.ErrorMessage = "RapidAPI anahtarı bulunamadı. appsettings.Local.json dosyasına anahtarını yaz.";
				return View(model);
			}

			if (model.CheckOut <= model.CheckIn)
			{
				model.ErrorMessage = "Çıkış tarihi, giriş tarihinden sonra olmalı.";
				return View(model);
			}

			if (model.CheckIn.Date < DateTime.Today)
			{
				model.ErrorMessage = "Giriş tarihi bugünden önce olamaz.";
				return View(model);
			}

			try
			{
				// 1) Şehir adından dest_id alınır.
				var destination = await FindDestinationAsync(model.CityName.Trim());
				if (destination == null || string.IsNullOrWhiteSpace(destination.dest_id))
				{
					model.ErrorMessage = "Bu isimde bir şehir bulunamadı. Yazımı kontrol edip tekrar dene.";
					return View(model);
				}

				model.LocationLabel = string.IsNullOrWhiteSpace(destination.label)
					? destination.name
					: destination.label;

				// 2) Şehir kimliği ve form bilgileriyle otel listesi çekilir.
				var hotels = await SearchHotelsAsync(destination, model);
				model.Hotels = hotels;

				if (model.Hotels.Count == 0)
					model.ErrorMessage = "Bu tarihlerde uygun otel bulunamadı. Tarihi veya şehir adını değiştirip tekrar dene.";
			}
			catch (Exception)
			{
				model.ErrorMessage = "Otel servisine şu an ulaşılamadı. Biraz sonra tekrar dene.";
			}

			return View(model);
		}

		public async Task<IActionResult> Details(long hotelId, DateTime checkIn, DateTime checkOut, int adults = 2, int children = 0, int rooms = 1, int childAge = 2, string currency = "EUR", string? cityName = null, double? reviewScore = null, string? reviewWord = null)
		{
			var page = new HotelDetailViewModel
			{
				HotelId = hotelId,
				CityName = cityName,
				CheckIn = checkIn == default ? DateTime.Today.AddDays(14) : checkIn,
				CheckOut = checkOut == default ? DateTime.Today.AddDays(16) : checkOut,
				Adults = adults < 1 ? 1 : adults,
				Children = children < 0 ? 0 : children,
				Rooms = rooms < 1 ? 1 : rooms,
				ChildAge = childAge < 0 ? 0 : childAge,
				Currency = AllowedCurrencies.Contains(currency) ? currency : "EUR",
				ReviewScore = reviewScore,
				ReviewScoreWord = reviewWord
			};

			if (hotelId <= 0)
			{
				page.ErrorMessage = "Otel seçilemedi. Listeden bir otele tekrar tıkla.";
				return View(page);
			}

			if (string.IsNullOrWhiteSpace(_apiKey))
			{
				page.ErrorMessage = "RapidAPI anahtarı bulunamadı. appsettings.Local.json dosyasına anahtarını yaz.";
				return View(page);
			}

			try
			{
				// 3) Seçilen otelin hotel_id değeriyle detay çekilir.
				var detail = await GetHotelDetailAsync(page);
				if (detail == null)
				{
					page.ErrorMessage = "Otel detayı bulunamadı.";
					return View(page);
				}

				page.Name = detail.hotel_name ?? "Otel";
				page.Address = BuildAddress(detail);
				page.City = detail.city_trans ?? detail.city;
				page.Country = detail.country_trans;
				page.Latitude = detail.latitude;
				page.Longitude = detail.longitude;
				page.AccommodationType = detail.accommodation_type_name;
				page.ReviewCount = detail.review_nr;
				page.BookingUrl = detail.url;
				page.Price = detail.composite_price_breakdown?.gross_amount?.value;
				page.Currency = detail.composite_price_breakdown?.gross_amount?.currency ?? page.Currency;
				page.Photos = CollectPhotos(detail);
				page.Facilities = CollectFacilities(detail);
				page.Descriptions = CollectDescriptions(detail);
				page.RoomsList = CollectRooms(detail);
				page.Notes = detail.hotel_important_information_with_codes?
					.Select(item => item.phrase)
					.Where(phrase => !string.IsNullOrWhiteSpace(phrase))
					.Select(phrase => phrase!)
					.Distinct()
					.Take(3)
					.ToList() ?? new List<string>();
			}
			catch (Exception)
			{
				page.ErrorMessage = "Otel detayı şu an alınamadı. Biraz sonra tekrar dene.";
			}

			return View(page);
		}

		private static void PrepareForm(HotelSearchPageViewModel model)
		{
			if (model.CheckIn == default)
				model.CheckIn = DateTime.Today.AddDays(14);
			if (model.CheckOut == default)
				model.CheckOut = model.CheckIn.AddDays(2);
			if (model.Adults < 1)
				model.Adults = 1;
			if (model.Adults > 10)
				model.Adults = 10;
			if (model.Children < 0)
				model.Children = 0;
			if (model.Children > 6)
				model.Children = 6;
			if (model.Rooms < 1)
				model.Rooms = 1;
			if (model.Rooms > 5)
				model.Rooms = 5;
			if (model.ChildAge < 0)
				model.ChildAge = 0;
			if (model.ChildAge > 17)
				model.ChildAge = 17;
			if (!AllowedCurrencies.Contains(model.Currency))
				model.Currency = "EUR";
		}

		private async Task<BookingDestination?> FindDestinationAsync(string cityName)
		{
			var url = BuildUrl("/api/v1/hotels/searchDestination", new Dictionary<string, string>
			{
				["query"] = cityName
			});

			var body = await GetJsonAsync(url);
			var result = JsonConvert.DeserializeObject<BookingDestinationResponse>(body);
			if (result == null || !result.status || result.data == null || result.data.Count == 0)
				return null;

			return result.data.FirstOrDefault(item =>
				string.Equals(item.search_type, "city", StringComparison.OrdinalIgnoreCase))
				?? result.data[0];
		}

		private async Task<List<HotelCardViewModel>> SearchHotelsAsync(BookingDestination destination, HotelSearchPageViewModel model)
		{
			var query = new Dictionary<string, string>
			{
				["dest_id"] = destination.dest_id ?? "",
				["search_type"] = string.IsNullOrWhiteSpace(destination.search_type)
					? "CITY"
					: destination.search_type.ToUpperInvariant(),
				["arrival_date"] = model.CheckIn.ToString("yyyy-MM-dd"),
				["departure_date"] = model.CheckOut.ToString("yyyy-MM-dd"),
				["adults"] = model.Adults.ToString(),
				["room_qty"] = model.Rooms.ToString(),
				["page_number"] = "1",
				["units"] = "metric",
				["temperature_unit"] = "c",
				["languagecode"] = "en-us",
				["currency_code"] = model.Currency
			};

			if (model.Children > 0)
				query["children_age"] = string.Join(",", Enumerable.Repeat(model.ChildAge.ToString(), model.Children));

			var body = await GetJsonAsync(BuildUrl("/api/v1/hotels/searchHotels", query));
			var result = JsonConvert.DeserializeObject<BookingHotelSearchResponse>(body);
			if (result == null || !result.status || result.data?.hotels == null)
				return new List<HotelCardViewModel>();

			var cards = new List<HotelCardViewModel>();
			foreach (var hotel in result.data.hotels)
			{
				if (hotel.property == null || hotel.hotel_id <= 0)
					continue;

				cards.Add(new HotelCardViewModel
				{
					HotelId = hotel.hotel_id,
					Name = hotel.property.name ?? "İsimsiz otel",
					City = hotel.property.wishlistName,
					PhotoUrl = hotel.property.photoUrls?.FirstOrDefault(),
					Price = hotel.property.priceBreakdown?.grossPrice?.value,
					Currency = hotel.property.priceBreakdown?.grossPrice?.currency ?? model.Currency,
					ReviewScore = hotel.property.reviewScore,
					ReviewScoreWord = hotel.property.reviewScoreWord,
					ReviewCount = hotel.property.reviewCount,
					Stars = hotel.property.propertyClass > 0 ? hotel.property.propertyClass : null
				});
			}

			return cards;
		}

		private async Task<BookingHotelDetail?> GetHotelDetailAsync(HotelDetailViewModel page)
		{
			var query = new Dictionary<string, string>
			{
				["hotel_id"] = page.HotelId.ToString(),
				["arrival_date"] = page.CheckIn.ToString("yyyy-MM-dd"),
				["departure_date"] = page.CheckOut.ToString("yyyy-MM-dd"),
				["adults"] = page.Adults.ToString(),
				["room_qty"] = page.Rooms.ToString(),
				["units"] = "metric",
				["temperature_unit"] = "c",
				["languagecode"] = "en-us",
				["currency_code"] = page.Currency ?? "EUR"
			};

			if (page.Children > 0)
				query["children_age"] = string.Join(",", Enumerable.Repeat(page.ChildAge.ToString(), page.Children));

			var body = await GetJsonAsync(BuildUrl("/api/v1/hotels/getHotelDetails", query));
			var result = JsonConvert.DeserializeObject<BookingHotelDetailResponse>(body);
			if (result == null || !result.status)
				return null;

			return result.data;
		}

		private static string BuildAddress(BookingHotelDetail detail)
		{
			var parts = new List<string>();
			if (!string.IsNullOrWhiteSpace(detail.address))
				parts.Add(detail.address);
			var cityLine = $"{detail.zip} {detail.city_trans}".Trim();
			if (!string.IsNullOrWhiteSpace(cityLine))
				parts.Add(cityLine);
			if (!string.IsNullOrWhiteSpace(detail.country_trans))
				parts.Add(detail.country_trans);
			return string.Join(", ", parts);
		}

		private static List<string> CollectPhotos(BookingHotelDetail detail)
		{
			var photos = new List<string>();
			if (detail.rooms == null)
				return photos;

			foreach (var room in detail.rooms.Values)
			{
				if (room.photos == null)
					continue;

				foreach (var photo in room.photos)
				{
					var url = photo.url_max750 ?? photo.url_max300;
					if (string.IsNullOrWhiteSpace(url) || photos.Contains(url))
						continue;

					photos.Add(url);
					if (photos.Count == 12)
						return photos;
				}
			}

			return photos;
		}

		private static List<string> CollectFacilities(BookingHotelDetail detail)
		{
			var facilities = new List<string>();
			if (detail.facilities_block?.facilities != null)
			{
				foreach (var facility in detail.facilities_block.facilities)
				{
					if (!string.IsNullOrWhiteSpace(facility.name))
						facilities.Add(facility.name);
				}
			}

			if (detail.property_highlight_strip != null)
			{
				foreach (var highlight in detail.property_highlight_strip)
				{
					if (!string.IsNullOrWhiteSpace(highlight.name) && !facilities.Contains(highlight.name))
						facilities.Add(highlight.name);
				}
			}

			return facilities;
		}

		private static List<HotelRoomViewModel> CollectRooms(BookingHotelDetail detail)
		{
			var rooms = new List<HotelRoomViewModel>();
			if (detail.block == null)
				return rooms;

			foreach (var block in detail.block)
			{
				if (string.IsNullOrWhiteSpace(block.room_name) && string.IsNullOrWhiteSpace(block.name))
					continue;

				string? photo = null;
				if (detail.rooms != null && detail.rooms.TryGetValue(block.room_id.ToString(), out var roomInfo))
					photo = roomInfo.photos?.FirstOrDefault()?.url_max300 ?? roomInfo.photos?.FirstOrDefault()?.url_max750;

				var policy = block.block_text?.policies?
					.FirstOrDefault(item => item.kind == "POLICY_CANCELLATION")?.content
					?? block.block_text?.policies?.FirstOrDefault()?.content;

				rooms.Add(new HotelRoomViewModel
				{
					Name = block.room_name ?? block.name ?? "Oda",
					PhotoUrl = photo,
					Size = block.room_surface_in_m2,
					Adults = block.nr_adults,
					Children = block.nr_children,
					BreakfastIncluded = block.breakfast_included == 1,
					Refundable = block.refundable == 1,
					Policy = policy
				});

				if (rooms.Count == 4)
					break;
			}

			return rooms;
		}

		private static List<string> CollectDescriptions(BookingHotelDetail detail)
		{
			var descriptions = new List<string>();
			if (!string.IsNullOrWhiteSpace(detail.hotel_text?.description))
				descriptions.Add(detail.hotel_text.description);

			if (detail.rooms == null)
				return descriptions;

			foreach (var room in detail.rooms.Values)
			{
				if (string.IsNullOrWhiteSpace(room.description) || descriptions.Contains(room.description))
					continue;

				descriptions.Add(room.description);
				if (descriptions.Count == 4)
					break;
			}

			return descriptions;
		}

		private string BuildUrl(string path, Dictionary<string, string> query)
		{
			var queryString = string.Join("&", query.Select(item =>
				$"{item.Key}={Uri.EscapeDataString(item.Value)}"));
			return $"https://{_apiHost}{path}?{queryString}";
		}

		private async Task<string> GetJsonAsync(string url)
		{
			using var client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(40);
			using var request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.Add("x-rapidapi-key", _apiKey);
			request.Headers.Add("x-rapidapi-host", _apiHost);

			using var response = await client.SendAsync(request);
			var body = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
				throw new InvalidOperationException("Otel servisi cevap vermedi.");

			return body;
		}



	}
}
