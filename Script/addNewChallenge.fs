open System
open System.IO
open System.Text
open Microsoft.FSharp.Collections

let challengeTemplate challengeNo = $"""using Framework;

namespace Challenges2022;

public class Solution{challengeNo} : SolutionFramework
{{
    public Solution{challengeNo}() : base({challengeNo}) {{ }}
    
    public override string[] Solve()
    {{
        return Answers;
    }}
}}"""

let executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()

//Add new challenge solution file
let solutionPath =
    using(new StreamReader(executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + ".solutionPath.txt")))
        (fun sr -> sr.ReadLine())

let challengeDirectories = Directory.GetDirectories $"{solutionPath}Challenges 2022"

let lastChallengeNo = challengeDirectories |>
        Array.choose(fun dirName ->
            match System.Int32.TryParse (dirName.Split('\\') |> Array.last) with
            | true,int -> Some int
            | _ -> None) |> Array.max

let newChallengeNo = lastChallengeNo + 1

let newChallengeDirectory = Directory.CreateDirectory $"{solutionPath}Challenges 2022\\{newChallengeNo}"
File.Create $"{newChallengeDirectory.FullName}\\input.txt" |> ignore
let content = (challengeTemplate newChallengeNo) |> Encoding.UTF8.GetBytes

using(File.Create $"{newChallengeDirectory.FullName}\\Solution{newChallengeNo}.cs")
    (fun solutionFile ->
        solutionFile.Write (ReadOnlySpan<byte>(content))
    )
//
    
//Update cs proj to copy input file
let csprojFileFilepath = $"{solutionPath}Challenges 2022\\Challenges 2022.csproj"
let csprojFileContent = List.ofSeq (File.ReadLines csprojFileFilepath)

let updatedCsprojFileContent =
    let insertionIndex = (csprojFileContent |> List.findIndex (fun x -> x.Contains("INSERTION POINT MARKER")))
    csprojFileContent |> List.insertAt insertionIndex $"""        <None Update="{newChallengeNo}\input.txt">
          <CopyToOutputDirectory>Always</CopyToOutputDirectory>
        </None>"""
    |> List.reduce (fun acc line -> $"{acc}\n{line}") |> Encoding.UTF8.GetBytes
    
using (new FileStream(csprojFileFilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
    (fun fs -> fs.Write(ReadOnlySpan<byte>(updatedCsprojFileContent)))
//
    
// Update Executioner file
//
let executionerFileFilepath = $"{solutionPath}Challenges 2022\\Program.cs"
let executionerFileContent = List.ofSeq (File.ReadLines executionerFileFilepath)

let updatedExecutionerFileContent =
    let insertionIndex = (executionerFileContent |> List.findIndex (fun x -> x.Contains("INSERTION POINT MARKER")))
    executionerFileContent |> List.insertAt insertionIndex $"        new Challenges2022.Solution{newChallengeNo}().Solve,"
    |> List.reduce (fun acc line -> $"{acc}\n{line}") |> Encoding.UTF8.GetBytes
    
using (new FileStream(executionerFileFilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
    (fun fs -> fs.Write(ReadOnlySpan<byte>(updatedExecutionerFileContent)))
//


        
        
        
    
            

            
    

