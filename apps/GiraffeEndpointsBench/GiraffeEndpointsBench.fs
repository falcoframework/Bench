open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.EndpointRouting
open Giraffe.ViewEngine

type Message = { msg: string }

let message () =
    { msg = sprintf "Current time is %s" (DateTime.UtcNow.ToString "R") }

let template body' =
    html [] [
        head [] [ title [] [ str "Hello" ] ]
        body [] body'
    ]


let indexTemplate message =
    template [
        h1 [] [ str "Hello World!" ]
        p [] [ str message ]
    ]

let home: HttpHandler =
    fun next ctx -> htmlString (indexTemplate (message().msg) |> RenderView.AsString.htmlNode) next ctx

let endpoints = [
    GET [ route "/" home ]
    GET [ route "/text" (setHttpHeader "Content-Type" "text/plain; charset=utf-8" >=> text (message().msg)) ]
    GET [ route "/json" (setHttpHeader "Content-Type" "application/json" >=> jsonChunked (message())) ]
]

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddRouting() |> ignore

let app = builder.Build()

app.UseRouting() |> ignore
app.UseEndpoints(fun endpointBuilder -> endpointBuilder.MapGiraffeEndpoints(endpoints))
|> ignore
app.Run(fun ctx ->
    ctx.Response.StatusCode <- 404
    ctx.Response.ContentType <- "text/plain"
    ctx.Response.WriteAsync("Not found"))

app.Run()
