namespace Conway.App.Desktop

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.Layout
open Conway.Core

module Model =
    let gridButton row col (content: string) =
        Button.create [
            Grid.row row
            Grid.column col
            Button.content content
            Button.horizontalAlignment HorizontalAlignment.Stretch
            Button.verticalAlignment VerticalAlignment.Stretch
            Button.verticalContentAlignment VerticalAlignment.Center
            Button.horizontalContentAlignment HorizontalAlignment.Center
        ]

module Main =

    let view () =
        Component(fun ctx ->

            let state = ctx.useState (ConwayGrid.createDead 3 3)

            DockPanel.create [
                DockPanel.children [
                    Grid.create [
                        Grid.dock Dock.Bottom
                        Grid.columnDefinitions "*,*,*"
                        Grid.rowDefinitions "*,*,*"
                        Grid.showGridLines true
                        Grid.children [
                            Model.gridButton 0 0 "1"
                            Model.gridButton 0 1 "2"
                            Model.gridButton 0 2 "3"
                            Model.gridButton 1 0 "4"
                            Model.gridButton 1 1 "5"
                            Model.gridButton 1 2 "6"
                            Model.gridButton 2 0 "7"
                            Model.gridButton 2 1 "8"
                            Model.gridButton 2 2 "9"
                        ]
                    ]
                ]
            ])

type MainWindow() =
    inherit HostWindow()

    do
        base.Title <- "Conway's Game of Life"
        base.Content <- Main.view ()

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Load "avares://Conway.App.Desktop/Assets/Styles/Night.axaml"
        this.RequestedThemeVariant <- Styling.ThemeVariant.Light

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder.Configure<App>().UsePlatformDetect().UseSkia().StartWithClassicDesktopLifetime(args)
