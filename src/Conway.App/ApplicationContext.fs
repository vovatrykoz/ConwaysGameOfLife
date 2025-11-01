namespace Conway.App

open Conway.Core
open System.Threading
open Conway.App.Controls
open Raylib_cs

type ApplicationContext(gameMode: GameState, canvas: Canvas, texture: RenderTexture2D) =
    let _lock = new ReaderWriterLockSlim()

    let mutable _gameMode = gameMode

    member _.GameMode
        with get () =
            try
                _lock.EnterReadLock()
                _gameMode
            finally
                _lock.ExitReadLock()
        and set value =
            try
                _lock.EnterWriteLock()
                _gameMode <- value
            finally
                _lock.ExitWriteLock()

    member val Canvas = canvas with get

    member val Texture = texture with get
