using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.WebUI.Controllers
{
    public class WeatherForecastController : ApiController
    {
        [HttpGet]
        public Task<IEnumerable<WeatherForecast>> Get() => Mediator.Send(new GetWeatherForecastsQuery());
    }
}
