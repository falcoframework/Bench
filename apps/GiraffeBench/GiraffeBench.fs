open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Giraffe

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddGiraffe() |> ignore

let app = builder.Build()

let template (dt: DateTime) =
    sprintf
        "<html><head><title>Hello</title></head><body><h1>Hello World!</h1><p>Current time is %s</p></body></html>"
        (dt.ToString "R")

let home: HttpHandler =
    fun next ctx -> htmlString (template DateTime.UtcNow) next ctx

let webApp =
    choose [
        route "/" >=> GET >=> home
        text "Not found"
    ]

app.UseGiraffe(webApp)
app.Run()
