## Mars Rover

This .NET Core console application pulls images taken by all rovers on a given date by calling the NASA Mars Rover Photo API.

This application is not limited to a Windows environment. You will be able to build this for OSX, Linux in x86_x64 and ARM32 and ARM64.

### Dependencies

* .Net Core SDK 2.2 ([Download Here](https://dotnet.microsoft.com/download))

### Build

* From Visual Studio
  * Open `MarsRover.sln`
  * Build
  * Debug/Run
* From CLI
  * `cd` to `MarsRover` directory
  * `dotnet restore`
  * `dotnet build`
  * `dotnet run`

### How It Works
#### MarsRover.Console
1. Read in input file
    1. Read input file name from `appsettings.json`
        * Default to `input.txt`
        * Assumptions
            * file is in same directory as executable
            * file consists of line seperated date string
    2. Parse dates in each line and persist

2. Retrieve rover manifest for `curiosity`, `opportunity`, `spirit`

    * Rover manifest is used primarily for the `Landing Date` and `Recent Photo Date` properties

3. Retrieve photo pages
    * For each rover
        * For each date parsed from input file
            1. Check if `Landing Date` <= parsed input date >= `Recent Photo Date`
            2. Retrieve `numOfPagesToRequest` photo pages.
                * Configurable from `appsettings.json`
                * Default to 1

4. Download photos
    * For each rover
        * For each date
            * For each photo page
                * Download the first 10 images to the `{executable_dir}/Photos/{rover_name}/{date}/` directory

5. Write photo pages to json
    * Serialize the photo pages by rover and write to `{executable_dir}/output.json` 

#### MarsRover.WebApi


   


