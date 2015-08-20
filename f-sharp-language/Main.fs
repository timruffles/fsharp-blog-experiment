open System.IO;
open Yaml;
open System.Text.RegularExpressions;

type Post = { title: string }

let unix2WindowsNl s = Regex.Replace(s, "\n", "\r\n")

let tapp x =
    (Regex.Escape >> printf "%s") x
    x

let getPosts directory = 
    Directory.GetFiles(directory, "*.txt")
    |> Seq.map (File.ReadAllText >> unix2WindowsNl >> tapp >> Yaml.load<Post>)



try
    let posts = getPosts(__SOURCE_DIRECTORY__ + "/posts")
    printf "%A" posts
with _ as ex ->
    printf "failed to load posts"

