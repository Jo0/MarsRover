CREATE TABLE Rover(
    RoverId INTEGER  PRIMARY KEY,
    Name TEXT,
    LandingDate TEXT,
    RecentPhotoDate TEXT,
    LastUpdated TEXT
);

CREATE TABLE PhotoDate(
    PhotoDateId INTEGER PRIMARY KEY,
    EarthDate TEXT
);

CREATE TABLE RoverPhoto(
    RoverPhotoId INTEGER PRIMARY KEY,
    RoverId INTEGER,
    RoverCameraInfo TEXT, 
    PhotoUrl TEXT,
    PhotoDateId INTEGER,
    FOREIGN KEY(RoverId) REFERENCES Rover(RoverId),
    FOREIGN KEY(PhotoDateId) REFERENCES PhotoDate(PhotoDateId)
 );