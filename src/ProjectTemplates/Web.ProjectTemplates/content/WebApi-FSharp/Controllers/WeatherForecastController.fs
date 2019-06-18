namespace WebApplication1.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

[<ApiController>]
[<Route("[controller]")>]
type WeatherForecastController (logger : ILogger<WeatherForecastController>) =
    inherit ControllerBase()
    static let mutable Summaries = [| "Freezing"; "Bracing"; "Chilly"; "Cool"; "Mild"; "Warm"; "Balmy"; "Hot"; "Sweltering"; "Scorching" |]
    let mutable logger = logger

    [<HttpGet>]
    member this.Get() =
        let rng = System.Random()
        let result = List.init 5 (fun index -> {
            Date = DateTime.Now.AddDays(float index);
            TemperatureC = rng.Next(-20,55);
            Summary = Summaries.[rng.Next(Summaries.Length)];
        })
        result
