namespace PingPong

open System
open System.Windows.Forms
open PingPong.Lib
module App =
    [<EntryPoint>]
    [<STAThread>]
    let main argv =
        Application.Run(gameForm ())
        0