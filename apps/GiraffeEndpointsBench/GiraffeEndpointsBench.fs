open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.EndpointRouting

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddRouting() |> ignore

let template (dt: DateTime) =
    sprintf
        "<html><head><title>Hello</title></head><body><h1>Hello World!</h1><p>Current time is %s</p></body></html>"
        (dt.ToString "R")

let home: HttpHandler =
    fun next ctx -> htmlString (template DateTime.UtcNow) next ctx

let endpoints = [ GET [ route "/" home ] ]

let app = builder.Build()

app.UseRouting() |> ignore
app.UseEndpoints(fun endpointBuilder -> endpointBuilder.MapGiraffeEndpoints(endpoints))
|> ignore
app.Run(fun ctx ->
    ctx.Response.ContentType <- "text/plain"
    ctx.Response.WriteAsync("Not found"))
