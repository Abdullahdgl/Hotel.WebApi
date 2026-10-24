using Newtonsoft.Json;

namespace Project6_HotelApi.Models
{
	public class BookingDestinationResponse
	{
		public bool status { get; set; }
		public string? message { get; set; }
		public List<BookingDestination>? data { get; set; }
	}

	public class BookingDestination
	{
		public string? dest_id { get; set; }
		public string? search_type { get; set; }
		public string? name { get; set; }
		public string? label { get; set; }
	}

	public class BookingHotelSearchResponse
	{
		public bool status { get; set; }
		public string? message { get; set; }
		public BookingHotelSearchData? data { get; set; }
	}

	public class BookingHotelSearchData
	{
		public List<BookingHotelItem>? hotels { get; set; }
	}

	public class BookingHotelItem
	{
		public long hotel_id { get; set; }
		public BookingHotelProperty? property { get; set; }
	}

	public class BookingHotelProperty
	{
		public string? name { get; set; }
		public double? reviewScore { get; set; }
		public string? reviewScoreWord { get; set; }
		public int? reviewCount { get; set; }
		public int? propertyClass { get; set; }
		public string? wishlistName { get; set; }
		public List<string>? photoUrls { get; set; }
		public BookingPriceBreakdown? priceBreakdown { get; set; }
	}

	public class BookingPriceBreakdown
	{
		public BookingMoney? grossPrice { get; set; }
	}
	public class BookingMoney
	{
		public double value { get; set; }
		public string? currency { get; set; }
	}

	public class BookingHotelDetailResponse
	{
		public bool status { get; set; }
		public string? message { get; set; }
		public BookingHotelDetail? data { get; set; }
	}

	public class BookingHotelDetail
	{
		public long hotel_id { get; set; }
		public string? hotel_name { get; set; }
		public string? url { get; set; }
		public string? address { get; set; }
		public string? zip { get; set; }
		public string? city { get; set; }
		public string? city_trans { get; set; }
		public string? country_trans { get; set; }
		public double? latitude { get; set; }
		public double? longitude { get; set; }
		public int? review_nr { get; set; }
		public string? accommodation_type_name { get; set; }
		public BookingHotelText? hotel_text { get; set; }
		public BookingFacilitiesBlock? facilities_block { get; set; }
		public List<BookingHighlight>? property_highlight_strip { get; set; }
		public Dictionary<string, BookingRoom>? rooms { get; set; }
		public List<BookingImportantInfo>? hotel_important_information_with_codes { get; set; }
		public BookingCompositePrice? composite_price_breakdown { get; set; }
		public List<BookingRoomBlock>? block { get; set; }
	}

	public class BookingRoomBlock
	{
		public long room_id { get; set; }
		public string? room_name { get; set; }
		public string? name { get; set; }
		public double? room_surface_in_m2 { get; set; }
		public int? nr_adults { get; set; }
		public int? nr_children { get; set; }
		public int? refundable { get; set; }
		public int? breakfast_included { get; set; }
		public BookingBlockText? block_text { get; set; }
	}

	public class BookingBlockText
	{
		public List<BookingPolicy>? policies { get; set; }
	}
	public class BookingPolicy
	{
		[JsonProperty("class")]
		public string? kind { get; set; }
		public string? content { get; set; }
	}
	public class BookingHotelText
	{
		public string? description { get; set; }
	}

	public class BookingFacilitiesBlock
	{
		public List<BookingFacility>? facilities { get; set; }
	}

	public class BookingFacility
	{
		public string? name { get; set; }
	}

	public class BookingHighlight
	{
		public string? name { get; set; }
	}

	public class BookingRoom
	{
		public string? description { get; set; }
		public List<BookingRoomPhoto>? photos { get; set; }
	}

	public class BookingRoomPhoto
	{
		public string? url_max300 { get; set; }
		public string? url_max750 { get; set; }
	}

	public class BookingImportantInfo
	{
		public string? phrase { get; set; }
	}

	public class BookingCompositePrice
	{
		public BookingMoney? gross_amount { get; set; }
	}

}
