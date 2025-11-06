namespace Conway.App

open Conway.Core
open Raylib_cs

type ApplicationContext(gameMode: GameState, canvas: Canvas, texture: RenderTexture2D) =

    member val GameMode = gameMode with get, set

    member val Canvas = canvas with get

    member val Texture = texture with get
