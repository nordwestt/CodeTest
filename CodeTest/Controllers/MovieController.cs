using CodeTest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace CodeTest.Controllers
{
    public class MovieController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Search(string title, string? year=null, string? plot=null)
        {

            string apiKey = "9f901c25";

            // Construct base URL with the API key included
            string url = $"http://www.omdbapi.com/?t={title}&apikey={apiKey}";

            // Only include filters if user has provided them 
            if (!string.IsNullOrEmpty(year))
            {
                url += $"&y={year}";
            }
            if (!string.IsNullOrEmpty(plot))
            {
                url += $"&plot={plot}";
            }


            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                // Handle HTTP errors
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError("", "Not Found. Please check your spelling of the movie title.");
                    return View("Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "Unauthorized. Please check your API key.");
                    return View("Index");
                }

                try
                {
                    Movie? result = await response.Content.ReadFromJsonAsync<Movie>();
                    
                    // OMDB does not seem to make use of HTTP 401, so the Response field must be checked
                    if(result.Response=="False") return View("NotFound");

                    // Return successful result
                    return View(result);
                }
                catch(Exception ex)
                {
                    MovieError? result = await response.Content.ReadFromJsonAsync<MovieError>();
                    return View("NotFound");
                }

            }
        }
    }
}
