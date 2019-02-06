## Mars Rover

This .NET Core console application pulls images taken by all rovers on a given date by calling the NASA Mars Rover Photo API.

This application is not limited to a Windows environment. You will be able to build this for OSX, Linux, and ARM envrionments.

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
1. Reads `input.txt` from the same directory the executable is in

   * `input.txt` is expected to have line separated date strings
   * To change the input file name, modify `inputFileName` in `appsettings.json` to the desired file name
      * The file must still reside in the same directory as the executable.

2. Retrieve rover manifest for `curiosity`, `opportunity`, `spirit`

    * Rover manifest is used primarily for the `Landing Date` and `Recent Photo Date` properties

3. For each rover and each date that was parsed from the input file, retrieve photo pages corresponding to the rover and date (if input date )
   
   * By default, application will only retrieve the first photo page
   * To retrieve more than one photo page, modify `numOfPagesToRequest` in `appsettings.json` to value >= 0

4. For each rover, each date, and each page the application will download the first 10 images to the `{executable_dir}/Photos/{rover_name}/{date}/` directory

5. Serialize the photo pages by rover and write to `{executable_dir}/output.json` 
   


