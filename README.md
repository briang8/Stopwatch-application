# Stopwatch Application

A Windows Forms stopwatch built in C# (.NET 8) with millisecond precision and a comprehensive MSTest unit test suite.

---

## Features

| Button | What it does |
|--------|--------------|
| **Start**  | Begins the timer from 00:00:00.000 |
| **Pause**  | Freezes the timer and shows the current time in the status bar |
| **Resume** | Continues from the paused time |
| **Reset**  | Returns everything to 00:00:00.000 |
| **Stop**   | Ends the session and displays the final recorded time |

**Enhanced Features:**
- **Millisecond Precision**: Displays time in format `00:00:00.000` (hours:minutes:seconds.milliseconds)
- **High-Frequency Updates**: Timer updates every 10 milliseconds for smooth, accurate timing
- **Professional UI**: Modern dark theme with intuitive button layout

---

## Project structure

```
StopwatchApp/
├── Program.cs               # Entry point
├── MainForm.cs              # Windows Forms UI with millisecond precision
├── StopwatchLogic.cs        # Core timer logic (supports millisecond precision)
├── StopwatchApp.csproj      # Main app project
├── StopwatchLogicTests.cs   # MSTest unit tests (32 comprehensive tests)
├── StopwatchApp.Tests.csproj
└── README.md
```

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (Windows)
- Visual Studio 2022 **or** the `dotnet` CLI

---

## How to run

### Using Visual Studio
1. Open the folder in Visual Studio 2022.
2. Set **StopwatchApp** as the startup project.
3. Press **F5**.

### Using the CLI
```bash
# From the project root directory
dotnet run --project StopwatchApp\StopwatchApp.csproj

# Alternative: Navigate to the project folder and run
cd StopwatchApp
dotnet run
```

---

## How to run the tests

```bash
# Run all 32 tests
dotnet test

```

All 32 tests cover `StopwatchLogic` in isolation — no UI dependency needed to run them. Tests include comprehensive coverage of millisecond precision functionality.

---

## Build Commands

```bash
# Build entire solution
dotnet build

# Clean and rebuild
dotnet clean
dotnet build
```

---

## Design notes

- `StopwatchLogic` holds all state and is completely independent of Windows Forms. This makes it straightforward to test and easy to swap the UI layer later.
- **Enhanced Precision**: The UI timer (`System.Windows.Forms.Timer`) now fires every 10 milliseconds (was 1000ms) and passes a millisecond-accurate tick count into the logic layer.
- **Backward Compatibility**: The logic class maintains both millisecond and second APIs for compatibility while providing enhanced precision.
- **Deterministic Testing**: The logic class never touches `DateTime` directly, which keeps it deterministic in tests.
- Buttons are enabled/disabled in response to state changes so the user can only take actions that make sense at any given moment.

---

## Technical Implementation

- **Timer Frequency**: 10ms intervals (100Hz) for smooth millisecond display updates
- **Display Format**: `HH:MM:SS.fff` format for hours, minutes, seconds, and milliseconds
- **Memory Efficient**: Uses long integers for millisecond storage instead of DateTime objects
- **Test Coverage**: 32 comprehensive unit tests covering all functionality including edge cases

---

## XML Documentation

All public methods carry `/// <summary>` XML doc comments. To generate the HTML docs:

```bash
dotnet build /p:GenerateDocumentationFile=true
```

The resulting `StopwatchApp.xml` file will appear in the build output folder.
