open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Giraffe
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

let webApp =
    choose [
        route "/" >=> GET >=> home
        route "/text" >=> GET >=> setHttpHeader "Content-Type" "text/plain; charset=utf-8" >=> text (message().msg)
        route "/json" >=> GET >=> setHttpHeader "Content-Type" "application/json" >=> jsonChunked (message())
        setStatusCode 404 >=> text "Not found"
    ]

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddGiraffe() |> ignore

let app = builder.Build()

app.UseGiraffe(webApp)
app.Run()
