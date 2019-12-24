///Summary              created by: Vishnu Kalihari     Date: 5/9/2015
///Validate various type of file when uploading and for image type it is preview the image  at same time
///Summary
///====================== Start Here ===================
function ValidatePreviewImage(maxFileSize, file, fileType) {
  
    if (!file.value == null || !file.value == '') {
        //converts the file size from bytes to MB
      
        var fileSize = file.files[0].size / 1024; // / 1024;

        //gets the full file name including the extension
      
        var fileName = file.files[0].name;

        //finds where the extension starts

        var dotPosition = fileName.lastIndexOf(".");
       
        //gets only the extension

        var fileExt = fileName.substring(dotPosition);
      
        //checks whether the file is valid format and less than 1 MB
        if (fileType == "Photo" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif")) {
            //successfully validated
           
            var oFReader = new FileReader();
            oFReader.readAsDataURL(document.getElementById("uploadImage").files[0]);

            oFReader.onload = function (oFREvent) {
                document.getElementById("uploadPreview").src = oFREvent.target.result;
            };
        }
        else if (fileType == "ID" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".doc" || fileExt.toLowerCase() == ".docx" || fileExt.toLowerCase() == ".pdf" || fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif")) {
            //successfully validated
        }
        else if (fileType == "CV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".doc" || fileExt.toLowerCase() == ".docx" || fileExt.toLowerCase() == ".pdf")) {
            //successfully validated
        }
        else if (fileType == "ATTACHMENT" && fileSize <= maxFileSize) {
            //successfully validated
        }
        else if (fileType == "CSV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".csv")) {
            //successfully validated
        }

        else {
            //alert("Invalid file type or file size");
            $.alert({
                title: 'Info',
                content: 'Invalid file type or file size',
                confirmButton: 'OK',
                confirmButtonClass: 'btn-primary',
                icon: 'fa fa-info',
                animation: 'zoom',
                confirm: function () {
                }
            });
            // alert(this.value);
            file.value = null;
        }
    }
};

///====================== End Here =====================

///=======================Start Here====================

function ValidatePreviewfile(maxFileSize, file, fileType) {

    if (!file.value == null || !file.value == '') {
        //converts the file size from bytes to MB

        var fileSize = file.files[0].size / 1024; // / 1024;

        //gets the full file name including the extension

        var fileName = file.files[0].name;

        //finds where the extension starts

        var dotPosition = fileName.lastIndexOf(".");

        //gets only the extension

        var fileExt = fileName.substring(dotPosition);
        if (fileType == "CSV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".csv")) {
            //successfully validated
        }

        else {
            //alert("Invalid file type or file size");
            $.alert({
                title: 'Info',
                content: 'Invalid file type or file size',
                confirmButton: 'OK',
                confirmButtonClass: 'btn-primary',
                icon: 'fa fa-info',
                animation: 'zoom',
                confirm: function () {
                }
            });
            // alert(this.value);
            file.value = null;
        }
    }
};

///=======================End Here=================


///Summary              created by: Vishnu Kalihari     Date: 5/9/2015
///Validate various type of file when uploading and for image type it is preview the image  at same time
///Summary
///====================== Start Here ===================
function ValidatePreview(maxFileSize, controllerId, file, fileType) {

    if (!file.value == null || !file.value == '') {
        //converts the file size from bytes to MB

        var fileSize = file.files[0].size / 1024; // / 1024;

        //gets the full file name including the extension

        var fileName = file.files[0].name;

        //finds where the extension starts

        var dotPosition = fileName.lastIndexOf(".");

        //gets only the extension

        var fileExt = fileName.substring(dotPosition);

        //checks whether the file is valid format and less than 1 MB
        if (fileType == "Photo" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif")) {
            //successfully validated

            var oFReader = new FileReader();
            oFReader.readAsDataURL(document.getElementById(controllerId).files[0]);

            oFReader.onload = function (oFREvent) {
                document.getElementById("previewLogo").src = oFREvent.target.result;
            };
        }
        else if (fileType == "ID" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".doc" || fileExt.toLowerCase() == ".docx" || fileExt.toLowerCase() == ".pdf" || fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif")) {
            //successfully validated
        }
        else if (fileType == "CV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".doc" || fileExt.toLowerCase() == ".docx" || fileExt.toLowerCase() == ".pdf")) {
            //successfully validated
        }
        else if (fileType == "ATTACHMENT" && fileSize <= 1) {
            //successfully validated
        }
        else if (fileType == "CSV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".csv")) {
            //successfully validated
        }

        else {
           // alert("Invalid file type or file size");
            // alert(this.value);
            $.alert({
                title: 'Info',
                content: 'Invalid file type or file size',
                confirmButton: 'OK',
                confirmButtonClass: 'btn-primary',
                icon: 'fa fa-info',
                animation: 'zoom',
                confirm: function () {
                }
            });
            file.value = null;
        }
    }
};
///====================== End Here =====================

///Summary              created by: Vishnu Kalihari     Date:5/9/2015
///compare 2 field values
///Summary
///====================== Start Here ===================
function CompareValue(object1, object2, message,alterMessage) {
   
    var val1 = parseFloat(object1.value);
    var val2 = parseFloat(object2.value);
   
    if (message == 'GreaterThen') {
        if (val1 < val2) {
           // alert(alterMessage + " " + val2);
            $.alert({
                title: 'Info',
                content: alterMessage + ' ' + val2,
                confirmButton: 'OK',
                confirmButtonClass: 'btn-primary',
                icon: 'fa fa-info',
                animation: 'zoom',
                confirm: function () {
                }
            });
            object1.value = null;
            object1.focus();
        }
    }
   if (message == 'LessThen') {
        if (val1 > val2) {
            // alert(alterMessage + " " + val2);
            $.alert({
                title: 'Info',
                content: alterMessage + ' ' + val2,
                confirmButton: 'OK',
                confirmButtonClass: 'btn-primary',
                icon: 'fa fa-info',
                animation: 'zoom',
                confirm: function () {
                }
            });

            object1.value = null;
            object1.focus();
        }
   }

   if (message == 'GreaterThenEqual') {
       if (val1 <= val2) {
           //   alert(alterMessage + " " + val2);
           $.alert({
               title: 'Info',
               content: alterMessage + ' ' + val2,
               confirmButton: 'OK',
               confirmButtonClass: 'btn-primary',
               icon: 'fa fa-info',
               animation: 'zoom',
               confirm: function () {
               }
           });
           object1.value = null;
           object1.focus();
       }
   }
   if (message == 'LessThenEqual') {
       if (val1 >= val2) {
           //  alert(alterMessage + " " + val2);
           $.alert({
               title: 'Info',
               content: alterMessage + ' ' + val2,
               confirmButton: 'OK',
               confirmButtonClass: 'btn-primary',
               icon: 'fa fa-info',
               animation: 'zoom',
               confirm: function () {
               }
           });
           object1.value = null;
           object1.focus();
       }
   }

   if (message == 'Equal') {
       if (val1 != val2) {
           // alert(alterMessage + " " + val2);
           $.alert({
               title: 'Info',
               content: alterMessage + ' ' + val2,
               confirmButton: 'OK',
               confirmButtonClass: 'btn-primary',
               icon: 'fa fa-info',
               animation: 'zoom',
               confirm: function () {
               }
           });
           object1.value = null;
           object1.focus();
       }
   }
};
///====================== End Here =====================






//----------------------For Ads Logo Image---------------------//
function ValidatePreviewLogo(maxFileSize, file, fileType) {
    if (!file.value == null || !file.value == '') {
        //converts the file size from bytes to MB

        var fileSize = file.files[0].size / 1024; // / 1024;

        //gets the full file name including the extension

        var fileName = file.files[0].name;

        //finds where the extension starts

        var dotPosition = fileName.lastIndexOf(".");

        //gets only the extension

        var fileExt = fileName.substring(dotPosition);

        //checks whether the file is valid format and less than 1 MB
        if (fileType == "Photo" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif")) {
            //successfully validated
            var oFReader = new FileReader();
            oFReader.readAsDataURL(document.getElementById("upLogo").files[0]);

            oFReader.onload = function (oFREvent) {
                document.getElementById("uploadLogo").src = oFREvent.target.result;
            };
        }
        else if (fileType == "ID" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".doc" || fileExt.toLowerCase() == ".docx" || fileExt.toLowerCase() == ".pdf" || fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif")) {
            //successfully validated
        }
        else if (fileType == "CV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".doc" || fileExt.toLowerCase() == ".docx" || fileExt.toLowerCase() == ".pdf")) {
            //successfully validated
        }
        else if (fileType == "ATTACHMENT" && fileSize <= 1) {
            //successfully validated
        }
        else if (fileType == "CSV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".csv")) {
            //successfully validated
        }

        else {
            //alert("Invalid file type or file size");
            $.alert({
                title: 'Info',
                content: 'Invalid file type or file size',
                confirmButton: 'OK',
                confirmButtonClass: 'btn-primary',
                icon: 'fa fa-info',
                animation: 'zoom',
                confirm: function () {
                }
            });
            // alert(this.value);
            file.value = null;
        }
    }
};

//--------------------------End here-----------------------------------//



///Summary              created by: Vishnu Kalihari     Date: 5/9/2015
///Validate various type of file when uploading and for image type it is preview the image  at same time
///Summary
///====================== Start Here ===================
function ValidatePreviewBanner(maxFileSize, file, fileType) {

    if (!file.value == null || !file.value == '') {
        //converts the file size from bytes to MB

        var fileSize = file.files[0].size / 1024; // / 1024;

        //gets the full file name including the extension

        var fileName = file.files[0].name;

        //finds where the extension starts

        var dotPosition = fileName.lastIndexOf(".");

        //gets only the extension

        var fileExt = fileName.substring(dotPosition);

        //checks whether the file is valid format and less than 1 MB
        if (fileType == "Photo" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif")) {
            //successfully validated

            var oFReader = new FileReader();
            oFReader.readAsDataURL(document.getElementById("uploadSmallImage").files[0]);

            oFReader.onload = function (oFREvent) {
                document.getElementById("uploadSmallPreview").src = oFREvent.target.result;
            };
        }
        else if (fileType == "ID" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".doc" || fileExt.toLowerCase() == ".docx" || fileExt.toLowerCase() == ".pdf" || fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif")) {
            //successfully validated
        }
        else if (fileType == "CV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".doc" || fileExt.toLowerCase() == ".docx" || fileExt.toLowerCase() == ".pdf")) {
            //successfully validated
        }
        else if (fileType == "ATTACHMENT" && fileSize <= 1) {
            //successfully validated
        }
        else if (fileType == "CSV" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".csv")) {
            //successfully validated
        }

        else {
            //alert("Invalid file type or file size");
            $.alert({
                title: 'Info',
                content: 'Invalid file type or file size',
                confirmButton: 'OK',
                confirmButtonClass: 'btn-primary',
                icon: 'fa fa-info',
                animation: 'zoom',
                confirm: function () {
                }
            });
            // alert(this.value);
            file.value = null;
        }
    }
};
///====================== End Here =====================


///Summary              created by: MALTI BAJAJ     Date: 02/07/2016
///Validate various type of file when uploading and for image type it is preview the image  at same time
///Summary
///====================== Start Here ===================
function ValidatePreviewFavicon(maxFileSize, file, fileType) {

    if (!file.value == null || !file.value == '') {
        //converts the file size from bytes to MB

        var fileSize = file.files[0].size / 1024; // / 1024;

        //gets the full file name including the extension

        var fileName = file.files[0].name;

        //finds where the extension starts

        var dotPosition = fileName.lastIndexOf(".");

        //gets only the extension

        var fileExt = fileName.substring(dotPosition);

        //checks whether the file is valid format and less than 1 MB
        if (fileType == "Photo" && fileSize <= maxFileSize && (fileExt.toLowerCase() == ".jpg" || fileExt.toLowerCase() == ".jpeg" || fileExt.toLowerCase() == ".png" || fileExt.toLowerCase() == ".gif" || fileExt.toLowerCase() == ".ico" || fileExt.toLowerCase() == ".zip")) {
            //successfully validated

            var oFReader = new FileReader();
            oFReader.readAsDataURL(document.getElementById("uploadFaviconImage").files[0]);

            oFReader.onload = function (oFREvent) {
                document.getElementById("uploadFaviconPreview").src = oFREvent.target.result;
            };
        }      
        else {
            //alert("Invalid file type or file size");
            $.alert({
                title: 'Info',
                content: 'Invalid file type or file size',
                confirmButton: 'OK',
                confirmButtonClass: 'btn-primary',
                icon: 'fa fa-info',
                animation: 'zoom',
                confirm: function () {
                }
            });
            // alert(this.value);
            file.value = null;
        }
    }
};
///====================== End Here =====================