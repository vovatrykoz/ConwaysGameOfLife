# Conway's Game of Life (F# Toy Project)

A simple toy project written in F# that implements Conway's Game of Life.  
Created to explore the performance capabilities of F# with a large grid and basic save/load functionality.

---

## Features

- Random grid generation on start  
- Save and load game state  
- Default board size: **1000x1000** (configurable via command-line arguments)  
- Ability to run the simulation continuously or step-by-step  
- Builds available for Windows and Ubuntu  

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later installed  

### Build and Run

Clone the repository:

```bash
git clone https://github.com/vovatrykoz/ConwaysGameOfLife.git
cd ConwaysGameOfLife
```

To create a JiT build use:
```
dotnet build
```

To create an AoT build use:
```
dotnet publish -r <your_platform> -c Release --property:PublishDir=<output_directory>
```

### Usage
Run Game.exe to start the game. On launch, the program generates a random grid. Use save/load buttons to save or load game state. Run the simulation continuously or step-by-step based on your preference.

To change the size of the starting grid, run Game.exe from the command line with width and height arguments:
```
./Game.exe <width> <height>
```

# Controls

- `Click and drag` to move around the grid  
- `Shift + Left Mouse Button` — make a cell alive  
- `Shift + Right Mouse Button` — make a cell dead  
- `Space` — run the grid; press again to pause  
- `Right arrow` — move to the next generation
- `Mouse wheel` - zoom in/zoom out

# Buttons

- `Save` — save the current state of the game to a file  
- `Load` — load a previously saved state from a file  
- `Run` — run the grid; press again to pause  
- `Next` — advance to the next generation  
- `Clear` — set all cells to dead  
- `Reset` — restore the grid to its initial state.  
  - If loaded from a file, resets to that saved state  
  - Otherwise, resets to the state at game start  

<img width="1023" height="794" alt="image" src="https://github.com/user-attachments/assets/51f07896-6b65-4c30-a920-ba41c3978bb5" />



