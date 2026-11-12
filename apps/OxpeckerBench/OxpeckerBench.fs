open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Oxpecker
open Oxpecker.ViewEngine

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddRouting().AddOxpecker() |> ignore

let template (dt: DateTime) =
    html() {
        head() { title() { "Hello" } }
        body() {
            h1() { "Hello World!" }
            p() { sprintf "Current time is %s" (dt.ToString "R") }
        }
    }

let home: EndpointHandler =
    fun ctx ->
        ctx.Response.ContentType <- "text/html"
        ctx.Response.WriteAsync(template DateTime.UtcNow |> Render.toString)

let endpoints = [ GET [ route "/" home ] ]

let app = builder.Build()

app.UseRouting().UseOxpecker(endpoints) |> ignore
app.Run(fun ctx ->
    ctx.Response.ContentType <- "text/plain"
    ctx.Response.WriteAsync("Not found"))

app.Run()
