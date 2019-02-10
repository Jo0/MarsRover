## Mars Rover


## MarsRover.Console

This .NET Core console application pulls images taken by all rovers on a given date by calling the NASA Mars Rover Photo API.

### Dependencies

* .Net Core SDK 2.2 ([Download Here](https://dotnet.microsoft.com/download))

### Build/Run

* From Visual Studio
  * Open `MarsRover.sln`
  * Set `MarsRover.Console` as default startup project
  * Build
  * Debug/Run
* From CLI
  * `cd` to `MarsRover\MarsRover.Console` directory
  * `dotnet build`
  * `dotnet run`

### How It Works

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

### Dependencies

* .Net Core SDK 2.2 ([Download Here](https://dotnet.microsoft.com/download))

### Build/Run

* From Visual Studio
  * Open `MarsRover.sln`
  * Set `MarsRover.WebApi` as default startup project
  * Build
  * Debug/Run
  * `index.html` will launch in new browser
* From CLI
  * `cd` to `MarsRover\MarsRover.WebApi` directory
  * `dotnet build`
  * `dotnet run`
  * Navigate to `https://localhost:5001` or `http://localhost:5000` in browser

### How It Works

1. Check if `mars-rover.db` exists
    * If the sqlite database doesn't exists, the WebApi will generate a new `mars-rover.db` file with the expected schema for the WebApi.
2. Upload a date input file
   * Dates must be input into the WebApi by means of a file consisting of line separated date strings. Which is prompted at the `Upload Date File` section of the web page.
   * Once a file is uploaded to the WebApi, each line is read and the WebApi will persist each line that was read and will track what lines were invalid date string.
   * For valid lines the WebApi will check if the date exists in the `mars-rover.db`
     * If the date does not exist the line will be inserted into the `PhotoDate` table in ISO8601 date string format. (SQLite expects dates stored in a `TEXT` field to be ISO8601 strings `"YYYY-MM-DD HH:MM:SS.SSS"`)
       * The WebApi will track that this line was added into `PhotoDate`
     * If the date exists, the WebApi will skip inserting it into `PhotoDate`
       * The WebApi will track that this line already exists in the database.
   * The web page will display lists for lines that were `Read`, `Invalid`, `Exists`, and `Added`
3. Rover Manifsts
   * On page load, an api call is made to `/api/v1/rovers` to retrieve rover options for the dropdown list
     * The WebApi checks if any records exists for the rovers currently available from the NASA Mars Rover API (`curiosity`, `spirit`, `opportunity`)
       * If the rover doesn't exist, the rover's `Name`, `LandingDate`, and `RecentPhotoDate` is inserted into the `Rover` table with `LastUpdated` of a timestamp of insertion.
       * If the rover exists, a check is made on the `LastUpdated` field to see if the rover's manifest has been updated within 24 hours.
         * This is to reduce api calls being made to the NASA API
         * It's redundant to call the `https://api.nasa.gov/mars-photos/api/v1/manifests/{rover_name}` endpoint multiple times a day to update `RecentPhotoDate`
4. Rover Photos
   * In the `Rover Photos` section of the web page, select the desired rover and date from the respective dropdown lists.
   * When `Get Photos` is clicked, and api call is made to `/api/v1/rovers/{rover_id}/photos?photoDate={ISO8601_photo_date_string}`
     * Checks are made to validate the `rover_id` and `ISO8601_photo_date_string`
       * If either of these values don't exist in the database, `404 Not Found` will be returned from the API
    * The WebApi will then check if any photo records are currently in the `RoverPhoto` table for the requested `rover_id`
      * If there are photo records, the api will return the entire collection in the databse.
      * If there are no photo records, an api call is made to `https://api.nasa.gov/mars-photos/api/v1/rovers/{rover}/photos?earth_date={yyyy-M-d}` to retreive *ALL* photos for the rover on the requested date.
        * Each photo is then stored into the `RoverPhoto` table
          * The field `RoverCameraInfo` is a JSON string of the NASA API response `photo[n]:camera` property. 
          * The field `PhotoUrl` is the direct image url for that camera.
          * FK References
            * `RoverId`
            * `PhotoDateId`
      * The web page will then display all images in `200x200px` thumbnails in rows of 7.
        * Clicking on a thumbnail will open a new tab of the full image for better viewing.
  
