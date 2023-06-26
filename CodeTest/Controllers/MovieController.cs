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

            //string apiKey = ConfigurationManager.AppSettings["OMDbApiKey"];
            string apiKey = "9f901c25";

            //string url = $"http://www.omdbapi.com/?t={title}&apikey=9f901c25";
            string url = $"http://www.omdbapi.com/?t={title}&apikey={apiKey}";
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


                Movie? result = await response.Content.ReadFromJsonAsync<Movie>();
                return View(result);
            }
        }
    }
}
