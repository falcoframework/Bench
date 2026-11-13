open System
open Falco
open Falco.Markup
open Falco.Routing
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

type Message = { msg: string }

let message () =
    { msg = sprintf "Current time is %s" (DateTime.UtcNow.ToString "R") }

let template (body : XmlNode list) =
    _html [] [
        _head [] [
            _title [] [ _text "Hello" ]
        ]
        _body [] body
    ]

let indexTemplate message =
    template  [
        _h1' "Hello World!"
        _p' message
    ]

let endpoints = [
    get "/" (fun ctx -> Response.ofHtml (indexTemplate (message().msg)) ctx)
    get "/text" (fun ctx -> Response.ofPlainText (message().msg) ctx)
    get "/json" (fun ctx -> Response.ofJsonChunked (message()) ctx)
]

let bldr = WebApplication.CreateSlimBuilder()
bldr.Logging.ClearProviders() |> ignore

let wapp = bldr.Build()

wapp.UseRouting()
    .UseFalco(endpoints)
    .Run(Response.ofPlainText "Not found")
