//init
var roverPhotos = null;

(function () {
    getPhotoDates();
    getRoverManifests();    
})();

//Begin Upload Date File
let formData = new FormData();
const inputFile = document.getElementById('file');
inputFile.addEventListener('change', e => formData.append('file', e.currentTarget.files[0]));
const uploadDateForm = document.getElementById('upload-date-file-form');
uploadDateForm.addEventListener('submit', e => {
    e.preventDefault();
    e.stopPropagation();
    postDateInputFile();
});

function postDateInputFile() {
    const request = new XMLHttpRequest();
    request.open('POST', 'https://localhost:5001/api/v1/dates');
    request.onload = () => {
        let uploadResults = JSON.parse(request.responseText);
        console.log(JSON.parse(request.responseText));
        getPhotoDates(); //update photo date dropdown
        showFileResults(uploadResults);
    };
    request.send(formData);
}

function showFileResults(uploadResults){
    var fileResultsTable = document.getElementById('file-results');
    
    var readListDiv = document.createElement('div');
    readListDiv.className = 'col-sm-3';

    let readH = document.createElement('h5');
    readH.innerHTML = 'Read';
    readListDiv.appendChild(readH);

    readListDiv.appendChild(readListUl(uploadResults.Read));

    fileResultsTable.appendChild(readListDiv);

    var invalidListDiv = document.createElement('div');
    invalidListDiv.className = 'col-sm-3';

    let invalidH = document.createElement('h5');
    invalidH.innerHTML ='Invalid'
    invalidListDiv.appendChild(invalidH);

    invalidListDiv.appendChild(invalidListUl(uploadResults.Invalid));
    fileResultsTable.appendChild(invalidListDiv);

    var existsListDiv = document.createElement('div');
    existsListDiv.className = 'col-sm-3';

    let existsH = document.createElement('h5');
    existsH.innerHTML ='Exists';
    existsListDiv.appendChild(existsH);

    existsListDiv.appendChild(existsListUl(uploadResults.Exists));
    fileResultsTable.appendChild(existsListDiv);

    var addedListDiv = document.createElement('div');
    addedListDiv.className = 'col-sm-3';

    let addedH = document.createElement('h5');
    addedH.innerHTML ='Added';
    addedListDiv.appendChild(addedH);

    addedListDiv.appendChild(addedListUl(uploadResults.Added));

    fileResultsTable.appendChild(addedListDiv);
}

function readListUl(readList) {
    var readListUL = document.createElement('ul');
    for(var i = 0; i < readList.length; i++){
        var readListLi = document.createElement('li');
        readListLi.innerHTML = readList[i];
        readListUL.appendChild(readListLi);
    }
    return readListUL;    
}

function invalidListUl(invalidList) {
    var invalidListUL = document.createElement('ul');
    for(var i = 0; i < invalidList.length; i++){
        var invalidListLi = document.createElement('li');
        invalidListLi.innerHTML = invalidList[i];
        invalidListUL.appendChild(invalidListLi);
    }
    return invalidListUL;    
}

function existsListUl(existsList) {
    var existsListUL = document.createElement('ul');
    for(var i = 0; i < existsList.length; i++){
        var existsListLi = document.createElement('li');
        existsListLi.innerHTML = new Date(existsList[i]).toLocaleDateString();
        existsListUL.appendChild(existsListLi);
    }
    return existsListUL;    
}

function addedListUl(addedList) {
    var addedListUL = document.createElement('ul');
    for(var i = 0; i < addedList.length; i++){
        var addedListLi = document.createElement('li');
        addedListLi.innerHTML = addedList[i];
        addedListUL.appendChild(addedListLi);
    }
    return addedListUL;    
}
//End Upload Date File

//Begin Get Rover Photos
const getRoverPhotoForm = document.getElementById('rover-photo-request-form');
getRoverPhotoForm.addEventListener('submit', e => {
    e.preventDefault();
    e.stopPropagation();

    let roverId = document.getElementById('rover-list').value;
    let photoDate = document.getElementById('photo-dates').value;

    getRoverPhotos(roverId, photoDate);
});
//End Get Rover Photos


function getRoverManifests() {
    const request = new XMLHttpRequest();
    request.open('GET', 'https://localhost:5001/api/v1/rovers');
    request.onload = () => {
        let roverManifests = JSON.parse(request.responseText);
        console.log(roverManifests);
        populateRoverList(roverManifests);
    };
    request.send(null);
}

function populateRoverList(rovers) {

    var roverDropDown = document.getElementById('rover-list');

    for (var i = 0; i < rovers.length; i++) {
        var option = document.createElement('option');
        option.innerHTML = rovers[i].name.uppercaseFirst();
        option.value = rovers[i].roverId;
        roverDropDown.appendChild(option);
    }
}

function getPhotoDates() {
    const request = new XMLHttpRequest();
    request.open('GET', 'https://localhost:5001/api/v1/dates');
    request.onload = () => {
        let photoDates = JSON.parse(request.responseText);
        console.log(photoDates);
        populatePhotoDates(photoDates);
    };
    request.send(null);
}

function populatePhotoDates(photoDates) {
    var photoDatesDropDown = document.getElementById('photo-dates');
    photoDatesDropDown.innerHTML = "";
    for (var i = 0; i < photoDates.length; i++) {
        var option = document.createElement('option');
        option.innerHTML = new Date(photoDates[i].earthDate).toLocaleDateString();
        option.value = photoDates[i].earthDate;
        photoDatesDropDown.appendChild(option);
    }
}

function getRoverPhotos(roverId, photoDate) {
    const request = new XMLHttpRequest();
    request.open('GET', 'https://localhost:5001/api/v1/rovers/' + roverId + '/photos?photoDate=' + photoDate);
    request.onload = () => { 
        let roverPhotosRes = JSON.parse(request.responseText);
        this.roverPhotos = roverPhotosRes;
        console.log(roverPhotosRes); 
        loadRoverPhotoGallery();
    }; 
    request.send(null);
}


function loadRoverPhotoGallery(){
    var gallery = document.getElementById('rover-image-gallery');
    gallery.innerHTML = '';

    var rowSize = 7;
    var numRows = Math.ceil(this.roverPhotos.length/rowSize);

    for(var i = 0; i < numRows; i++)
    {
        var imageRow = document.createElement('div');
        imageRow.className = 'row';

        var startRowIndex = rowSize * (i);
        var endRowIndex = startRowIndex + rowSize;
        
        if(endRowIndex > this.roverPhotos.length - 1)
        {
            endRowIndex = this.roverPhotos.length-1;
        }

        for(var j = startRowIndex; j < endRowIndex; j++){
            var alt = document.getElementById('rover-list').value + '_' + document.getElementById('photo-dates').value + '_' + j;
            
            var imageATag = document.createElement('a');
            imageATag.setAttribute('href', this.roverPhotos[j].photoUrl);
            imageATag.setAttribute('target', '_blank');

            var imageThumb = document.createElement('img');
            imageThumb.setAttribute('src', this.roverPhotos[j].photoUrl);
            imageThumb.setAttribute('alt', alt);
            imageThumb.setAttribute('style', 'height:200px;width:200px;');
            imageThumb.className = 'img-thumbnail';

            imageATag.appendChild(imageThumb);
            
            imageRow.appendChild(imageATag);
        }

        gallery.appendChild(imageRow);
    }
}

String.prototype.uppercaseFirst = function () {
    let first = this.substr(0, 1).toLocaleUpperCase();
    let rest = this.substr(1, this.length);
    return first.concat(rest);
}   