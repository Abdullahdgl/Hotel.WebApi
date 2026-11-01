using Microsoft.AspNetCore.Mvc;
using Project6_HotelApi.Models;
using System.Text.Json;

namespace Project6_HotelApi.Controllers
{
	public class ImdbController : Controller
	{
		public async Task<IActionResult> Index()
		{

			var client = new HttpClient();
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("https://imdb-top-100-movies.p.rapidapi.com/"),
				Headers =
	{
		{ "x-rapidapi-key", "466ec838c9msh7cdfbe275792f6ep149304jsnd4107fdb4495" },
		{ "x-rapidapi-host", "imdb-top-100-movies.p.rapidapi.com" },
	},
			};
			using (var response = await client.SendAsync(request))
			{
				response.EnsureSuccessStatusCode();
				var body = await response.Content.ReadAsStringAsync();
				var result = JsonSerializer.Deserialize<List<ImdbMoviesViewModel>>(body);
				return View(result);
			}



		}
	}
}
