open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Oxpecker

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddRouting().AddOxpecker() |> ignore

let template (dt: DateTime) =
    sprintf
        "<html><head><title>Hello</title></head><body><h1>Hello World!</h1><p>Current time is %s</p></body></html>"
        (dt.ToString "R")

let home: EndpointHandler =
    fun ctx ->
        ctx.Response.ContentType <- "text/html"
        ctx.Response.WriteAsync(template DateTime.UtcNow)

let endpoints = [ GET [ route "/" home ] ]

let app = builder.Build()

app.UseRouting().UseOxpecker(endpoints) |> ignore
app.Run(fun ctx ->
    ctx.Response.ContentType <- "text/plain"
    ctx.Response.WriteAsync("Not found"))

app.Run()
