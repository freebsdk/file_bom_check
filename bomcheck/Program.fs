open System
open System.IO






let HasBom(filename : string) =
    try
        let fileinfo = FileInfo(filename)
        if fileinfo.Length <= 3L then failwith "Error : too short file."
        else if fileinfo.Length > 2147483648L then failwith "Error : too long file."
        else
            use reader = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            let a = reader.ReadBytes(3)
            match a.[0],a.[1],a.[2]  with
            | 0xEFuy,0xBBuy,0xBFuy -> Some(true)
            | _ -> Some(false)
    with 
    | _ as ex ->
        printfn "Exception : %s" ex.Message 
        None
   



let RemoveBom(filename : string) =
    try
        match HasBom(filename) with
        | Some(true) -> 
            let d_filename = filename + ".tmp"
            let s_fileinfo = FileInfo(filename)
            
            use h_src = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read))
            use h_dst = new BinaryWriter(File.Open(d_filename, FileMode.Create, FileAccess.Write))
            
            //ignore bom bytes
            h_src.ReadBytes(3) |> ignore

            let ctx = h_src.ReadBytes(int32(s_fileinfo.Length - 3L))
            h_dst.Write(ctx, 0, ctx.Length)

        | _ ->
            ()
    with
    | _ as ex -> 
        printfn "Exception : %s." ex.Message
        ()


[<EntryPoint>]
let main argv = 
    
    if argv.Length <> 2 then
        printfn "Usage : $bomcheck [check|remove] [filename]"
    else
        match argv.[0] with
        | "check" -> 
            let res = HasBom(argv.[1])
            match res with
            | Some(true) -> printfn "true"
            | Some(false) -> printfn "false"
            | _ -> ()  
        | "remove" -> RemoveBom(argv.[1])
        | _ -> printfn "Error : Invalid mode."    
    0 // return an integer exit code
