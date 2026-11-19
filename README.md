# CareerTrack Desktop

CareerTrack Desktop is a cross-platform job application tracker built with C#, Avalonia UI, and .NET 10 for **CSCE 547 Windows Programming — Minor Project 7**.

The application was developed in Visual Studio on Windows and compiled and run on macOS on a MacBook Air. The instructor approved macOS as the target operating system for this assignment.

## Features

- Add, view, edit, and delete job applications.
- Store company, job title, location, status, date applied, and notes.
- Track Saved, Applied, Under Review, Interview, Offer, Rejected, and Withdrawn statuses.
- Search by company, job title, location, or status without case sensitivity.
- Display total application, interview, offer, and filtered result counts.
- Validate required company and job-title fields and supported statuses.
- Require a confirmation checkbox before deleting a selected application.
- Save records locally as JSON for use across application sessions.

## Architecture

The solution contains two projects:

| Project | Layer | Responsibility |
| --- | --- | --- |
| `CareerTrack.Desktop` | Client | Avalonia XAML interface, user input, and event handling |
| `CareerTrack.Core` | Service and data | Application model, validation, and JSON file storage |

`CareerTrack.Desktop` references `CareerTrack.Core`. The interface calls `JobApplicationService` to load, add, update, and delete records.

The window uses code-behind event handlers for its interactions. The generated MVVM template files are retained, but the window's application logic is implemented in `Views/MainWindow.axaml.cs`.

### Main files

- `CareerTrackDesktop.slnx` — solution file.
- `CareerTrack.Core/JobApplication.cs` — application record model.
- `CareerTrack.Core/JobApplicationService.cs` — validation and storage operations.
- `CareerTrack.Desktop/Views/MainWindow.axaml` — interface layout.
- `CareerTrack.Desktop/Views/MainWindow.axaml.cs` — interface event handlers.
- `CareerTrack.Desktop/App.axaml.cs` — application initialization.
- `.gitignore` — excludes generated output and local settings.

## Running the application

### Prerequisites

- .NET 10 SDK.
- Windows or macOS for the demonstrated desktop workflows.
- Internet access for the initial NuGet dependency restore.
- Optional: Visual Studio on Windows with .NET desktop development support and the Avalonia extension for editing and previewing XAML.

No database server, API server, connection string, or API key is required.

### Get the source

Clone this repository using its GitHub **Code** menu, or select **Code → Download ZIP** and extract it. Open a terminal in the solution folder containing `CareerTrack.Desktop`, `CareerTrack.Core`, and `CareerTrackDesktop.slnx`.

### Run from a terminal on Windows or macOS

```bash
dotnet restore CareerTrackDesktop.slnx
dotnet build CareerTrackDesktop.slnx
dotnet run --project CareerTrack.Desktop/CareerTrack.Desktop.csproj
```

The SDK restores the required NuGet packages and recreates the generated `bin` and `obj` folders. These folders are not required in the repository.

### Run using Visual Studio on Windows

1. Open `CareerTrackDesktop.slnx` in a Visual Studio version that supports .NET 10 and `.slnx` solutions.
2. Allow package restore to complete.
3. Set `CareerTrack.Desktop` as the startup project.
4. Select **Build → Build Solution**.
5. Press **Ctrl + F5** to start without debugging.

### Build on macOS

Use the same terminal commands above after copying or cloning the source to the Mac. The demonstrated workflow compiles the shared source on macOS; it does not require a macOS executable built on Windows.

## Using CareerTrack

1. Click **New / Clear** to prepare a new record.
2. Enter the company and job title, choose a status and date, and optionally add a location and notes.
3. Click **Save application**.
4. Select an existing record to edit it, then click **Save application** again.
5. Use the search box to filter the list. Clear the search to show all records.
6. To delete a record, select it, check the deletion confirmation box, and click **Delete selected**.

Scroll inside the form to reach controls that do not fit within the current window height.

## Data storage

Records are serialized with `System.Text.Json` to:

```text
<LocalApplicationData>/CareerTrackDesktop/applications.json
```

The application obtains `LocalApplicationData` from `Environment.GetFolderPath` and builds the path with `Path.Combine`, so it does not depend on a hard-coded Windows directory. It creates the storage folder when a record is first saved and writes a temporary file before replacing the JSON file.

Each computer and user account has its own records. Copying the source or cloning the repository does not transfer or synchronize application data. A fresh installation starts with an empty list; the sample applications shown in the submission screenshots were entered manually.

## Manual verification

Use the following checks after building a fresh copy:

| Check | Expected result |
| --- | --- |
| Save with an empty company or job title | Validation message appears |
| Add a valid application | Record appears and total count increases |
| Select, edit, and save a record | Existing record updates |
| Change a status to Interview or Offer | Corresponding summary count updates |
| Search for a company or job title | Matching records appear |
| Delete without checking confirmation | Deletion is blocked |
| Confirm and delete a temporary record | Record disappears and counts update |
| Close and reopen after saving | Saved records load again |

This is a manual verification checklist, not an automated test report.

## Current limitations and future improvements

- Records are local to each computer; there is no cloud synchronization or authentication.
- Storage is intended for a small, single-user desktop application.
- The form requires scrolling at smaller window sizes.
- Distribution currently uses source builds rather than signed installers.

Future work could include automated tests, improved accessibility and responsive layout, backup and recovery, shared API and database storage, signed Windows and macOS releases, and automatic updates.

## **Author:** 

Divyashanu Swain
