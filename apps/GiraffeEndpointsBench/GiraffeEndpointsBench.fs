open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.EndpointRouting
open Giraffe.ViewEngine

let builder = WebApplication.CreateSlimBuilder()
builder.Logging.ClearProviders() |> ignore
builder.Services.AddRouting() |> ignore

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

let endpoints = [ GET [ route "/" home ] ]

let app = builder.Build()

app.UseRouting() |> ignore
app.UseEndpoints(fun endpointBuilder -> endpointBuilder.MapGiraffeEndpoints(endpoints))
|> ignore
app.Run(fun ctx ->
    ctx.Response.ContentType <- "text/plain"
    ctx.Response.WriteAsync("Not found"))

app.Run()
