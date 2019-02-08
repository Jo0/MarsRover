###Package Manager Console

    Scaffold-DbContext "Data Source=./Repository/Reference Docs/base-mars-rover.db" Microsoft.EntityFrameworkCore.Sqlite -OutputDir Repository/EntityModels -ContextDir Repository -Context MarsRoverContext

###DotNet CLI 
* From .../MarsRover.WebApi/

      dotnet ef dbcontext scaffold "Data Source=./Repository/Reference Docs/base-mars-rover.db" Microsoft.EntityFrameworkCore.Sqlite --output-dir Repository/EntityModels --context-dir Repository --context MarsRoverContext

###Connection String

* appsettings.json
  
       "ConnectionStrings": { "MarsRoverDb": "Data Source=mars-rover.db"}