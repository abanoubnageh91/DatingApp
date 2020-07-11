using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing1", "Bracing1", "Chilly1", "Cool1", "Mild1", "Warm1", "Balmy1", "Hot1", "Sweltering1", "Scorching1"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DataContext _dataContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DataContext dataContext)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var WeatherForecasts = await _dataContext.WeatherForecasts.ToListAsync();
            return Ok(WeatherForecasts);
            // var rng = new Random();
            // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            // {
            //     Date = DateTime.Now.AddDays(index),
            //     TemperatureC = rng.Next(-20, 55),
            //     Summary = Summaries[rng.Next(Summaries.Length)]
            // })
            // .ToArray();
        }
    }
}
