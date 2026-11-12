open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.ViewEngine

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddGiraffe() |> ignore

let app = builder.Build()

let template (dt: DateTime) =
    html [] [
        head [] [ title [] [ str "Hello" ] ]
        body [] [
            h1 [] [ str "Hello World!" ]
            p [] [ str (sprintf "Current time is %s" (dt.ToString "R")) ]
        ]
    ]

let home: HttpHandler =
    fun next ctx -> htmlString (template DateTime.UtcNow |> RenderView.AsString.htmlNode) next ctx

let webApp =
    choose [
        route "/" >=> GET >=> home
        text "Not found"
    ]

app.UseGiraffe(webApp)
app.Run()
