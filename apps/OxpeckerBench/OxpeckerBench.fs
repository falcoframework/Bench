open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Oxpecker
open Oxpecker.ViewEngine

type Message = { msg: string }

let message () =
    { msg = sprintf "Current time is %s" (DateTime.UtcNow.ToString "R") }

let indexTemplate (message : string) =
    html() {
        head() { title() { "Hello" } }
        body() {
            h1() { "Hello World!" }
            p() { message }
        }
    }

let home: EndpointHandler = fun ctx ->
    ctx.WriteHtmlView (indexTemplate (message().msg))

let text message : EndpointHandler =  fun ctx ->
    ctx.Request.ContentType <- "text/plain; charset=utf-8"
    ctx.WriteText message.msg

let json message : EndpointHandler = fun ctx ->
    ctx.Request.ContentType <- "application/json"
    ctx.WriteJsonChunked message

let endpoints = [
    GET [ route "/" home ]
    GET [ route "/text" (text (message())) ]
    GET [ route "/json" (json (message())) ] ]

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddRouting().AddOxpecker() |> ignore
let app = builder.Build()

app.UseRouting().UseOxpecker(endpoints) |> ignore
app.Run(fun ctx ->
    ctx.Response.StatusCode <- 404
    ctx.Response.ContentType <- "text/plain"
    ctx.WriteText("Not found"))

app.Run()
