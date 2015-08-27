open System.IO;
open FParsec.CharParsers;
open FParsec.Primitives;
open System.Text.RegularExpressions;


type Post = { 
    title: string
    body: string
}


let tapp x =
    (Regex.Escape >> printf "'%s'\n") x
    x

let metadataPair = 
  (regex "[a-z]+") .>>. ((regex ":\s*") >>. regex "[^\n]+") .>> skipNewline


let postParser =
    many metadataPair .>>. (manyCharsTill anyChar eof)

let markdown2html s =
    let md = new MarkdownSharp.Markdown()
    md.Transform(s)

let createPostWithMeta (xs) =
    let map = Map.ofSeq xs
    let title = Map.find "title" map
    { 
        title = title
        body = "" 
    }


let parsePost s =
    match run postParser s with
        | Success((meta, markdownBody), _,  _) ->
            let post = createPostWithMeta meta
            Some { post with body = markdown2html(markdownBody) }
        | Failure(errorMsg, _, _) ->
            printfn "%s" errorMsg
            None

let file2post = File.ReadAllText >> tapp >> parsePost

let getPosts directory = 
    Directory.GetFiles(directory, "*.txt")
    |> Seq.map file2post
    // remove invalid posts
    |> Seq.choose id
 

type Blog() as this =
    inherit Nancy.NancyModule()
    do
        this.Get.["/"] <- fun _ -> "Hi there" :> obj



[<EntryPoint>]
let main args =

    try
        let posts = getPosts(__SOURCE_DIRECTORY__ + "/posts")
        printf "%A" posts
    with | _ as ex ->
        printf "failed to load posts"

 
    let nancy = new Nancy.Hosting.Self.NancyHost(new System.Uri("http://localhost:" + "8100"))
    nancy.Start()
    while true do System.Console.ReadLine() |> ignore
    0




