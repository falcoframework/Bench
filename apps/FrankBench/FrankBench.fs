open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Frank.Builder
open Falco.Markup

let template (dt: DateTime) =
    _html [] [
        _head [] [ _title [] [ _text "Hello" ] ]
        _body [] [
            _h1' "Hello World!"
            _p' (sprintf "Current time is %s" (dt.ToString "R"))
        ]
    ]

let home =
    resource "/" {
        name "Home"
        get (fun (ctx: HttpContext) ->
            ctx.Response.ContentType <- "text/html"
            ctx.Response.WriteAsync(template DateTime.UtcNow |> renderNode))
    }

let notFound (app: IApplicationBuilder) =
    app.Use(fun (ctx: HttpContext) (next: Func<Task>) ->
        if isNull (ctx.GetEndpoint()) then
            ctx.Response.ContentType <- "text/plain"
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
}
