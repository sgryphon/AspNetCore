namespace Company.WebApplication1

type WeatherForecast =
    { Date: DateTime; TemperatureC: int; Summary: string;}
    member x.TemperatureF = 32 + (int (float x.TemperatureC / 0.5556));
