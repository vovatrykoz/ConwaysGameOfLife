namespace Conway.App

open Conway.Core
open System.Threading
open Conway.App.Controls
open Raylib_cs

type ApplicationContext(gameState: GameState, canvas: Canvas, texture: RenderTexture2D) =

    let mutable _gameMode = gameState

    member val GameMode = gameState with get, set

    member val Canvas = canvas with get

    member val Texture = texture with get
