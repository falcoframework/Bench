open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Frank.Builder
open Falco.Markup

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

let home =
    resource "/" {
        name "Home"
        get (fun (ctx: HttpContext) ->
            ctx.Response.ContentType <- "text/html; charset=utf-8"
            ctx.Response.WriteAsync(indexTemplate (message().msg) |> renderNode))
    }

let text =
    resource "/text" {
        name "Text"
        get (fun (ctx: HttpContext) ->
            ctx.Response.ContentType <- "text/html; charset=utf-8"
            ctx.Response.WriteAsync(message().msg))
    }

let notFound (app: IApplicationBuilder) =
    app.Use(fun (ctx: HttpContext) (next: Func<Task>) ->
        if isNull (ctx.GetEndpoint()) then
            ctx.Response.StatusCode <- 404
            ctx.Response.ContentType <- "text/html; charset=utf-8"
            ctx.Response.WriteAsync("Not found")
        else
            next.Invoke())

let args = Environment.GetCommandLineArgs()

webHost args {
    configure (fun builder ->
        builder
            .UseKestrel()
            .ConfigureLogging(fun logging -> logging.ClearProviders() |> ignore))
    plug notFound
    resource home
    resource text
}
