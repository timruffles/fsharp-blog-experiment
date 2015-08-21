open System.IO;
open FParsec.CharParsers;
open FParsec.Primitives;
open System.Text.RegularExpressions;
open FSharp.Formatting;

type Post = { title: string }

let tapp x =
    (Regex.Escape >> printf "'%s'\n") x
    x

let metadataPair = 
  (regex "[a-z]+") .>>. ((regex ":\s*") >>. regex "[^\n]+") .>> skipNewline


let postParser =
    many metadataPair .>>. (manyCharsTill anyChar eof)

 

let parsePost s =
    match run postParser s with
        | Success(result, _, _) -> printfn "%A" result
        | Failure(errorMsg, _, _) -> printfn "%s" errorMsg


let getPosts directory = 
    Directory.GetFiles(directory, "*.txt")
    |> Seq.map (File.ReadAllText >> tapp >> parsePost)



try
    let posts = getPosts(__SOURCE_DIRECTORY__ + "/posts")
    printf "%A" posts
with | _ as ex ->
    printf "failed to load posts"



