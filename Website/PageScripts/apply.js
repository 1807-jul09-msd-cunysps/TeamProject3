




//Global Variables
//Variable to hole all field values on page
var formFields = {
    FirstName: false,
    LastName: false,
    Email: false,
    SSN: false,
    BusinessPhone: false,
    MobilePhone: false,
    AddressLine1: false,
    AddressLine2: false,
    City: false,
    State: false,
    Country: false,
    ZipCode: false,
    MortgageName: false,
    MortgageAmount: false,
    Option: true
};
//variables to store returned ContactGUID and Mortgage Account Number
var contactID, mortgageResult, applyPage, files = [], files64 = [], filesUploaded = 0, state;



function applyCheckSession() {
    let value = applyGetSession("user");
    if (value !== false) {
        let userObj = JSON.parse(value);
        applyPage = "mortgage";
        contactID = userObj["GUID"];
    } else {
        applyPage = "apply";
    }
    applyPageHandler();
}

function applyGetSession(cName) {
    cName = cName + "=";
    let cookieList = document.cookie.split(";");
    for (var i = 0; i < cookieList.length; i++) {
        let cookie = cookieList[i];
        cookie = cookie.trim();
        if (cookie === "") { continue; }
        while (cookie.charAt(0) === "") {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(cName) === 0) {
            let result = cookie.substring(cName.length, cookie.length);
            return result;
        }
    }
    return false;
}
function applyPageHandler() {
    if (applyPage !== "apply") {
        document.getElementById("formMortgage").classList.remove("hiddenForm");
        document.getElementById("formMortgage").classList.remove("no_display");
        document.getElementById("formContact").classList.add("fadeOutNext");

        document.getElementById("sign_in").classList.add("no_display");
        document.getElementById("account").classList.remove("no_display");
    } else {
        document.getElementById("sign_in").classList.remove("no_display");
        document.getElementById("account").classList.add("no_display");
    }
}


//Validation====================================================
function validatePasswordLength(string) {
    return (string.length >= 8) ? true : false;
}
function validatePasswordChars(element) {
    var val = /[a-zA-Z\s]/;
    let results = val.test(element.value) ? true : false;
    return results;
}

//Validate All Charcters
function validateCharsCheck(element) {
    var val = /^[a-zA-Z\s]+$/;
    let results = val.test(element.value) ? true : false;
    return results;
}

//Validate Symbols
function validateSymbolsCheck(element) {
    const val = /[~`!#$%\^&*+=\-\[\]\\';,/{}|\\":<>\?\(\)_]/;
    let results = val.test(element.value) ? true : false;
    return results;
}
//Validate Numbers
function validateNumbersCheck(element) {
    const val = /^\d*$/;
    let results = val.test(element.value) ? true : false;
    return results;
}

function validateContainsNumber(element) {
    const val = /\d/;
    let results = val.test(element.value) ? true : false;
    return results;
}
//Validate SSN format xxx-xx-xxxx | xxxxxxxxx
function validateSSNCheck(element) {
    const val = /^(?:\d{3}-\d{2}-\d{4}|\d{9})$/;
    let results = val.test(element.value) ? true : false;
    return results;
}
//Validate Phone Number (xxx) xxx-xxxx | xxx-xxx-xxxx | xxxxxxxxxx
function validatePhoneCheck(element) {
    const val = /^(?:\(\d{3}\)\d{3}-\d{4}|\d{3}-\d{3}-\d{4}|\d{10})$/;
    let results = val.test(element.value) ? true : false;
    return results;
}
//Validate Any input
function validate(element) {
    let text = "";
    if (element.value.length === 0) {
        element.classList.add("is-invalid");
        text = "This field is required.";
        formFields[element.id] = false;
    } else {
        element.classList.remove("is-invalid");
        formFields[element.id] = element.value;
    }
    document.getElementById(element.id + "Validate").innerHTML = text;
}
//Validate Alphanumeric Characters 
function validateChars(element) {
    let results;
    let text = "";
    if (element.disabled === true) {
        result = "N/A";
    } else {
        if (validateCharsCheck(element) && !validateSymbolsCheck(element)) {
            text = "";
            element.classList.remove("is-invalid");
            results = element.value;
        } else {
            element.classList.add("is-invalid");
            text = (element.value.length !== 0) ?
                "Numeric values and symbols are not allowed in this field." :
                "This field is required.";
            results = false;
        }
    }
    document.getElementById(element.id + "Validate").innerHTML = text;
    formFields[element.id] = results;
}
//Validate Numeric Characters
function validateNumbers(element) {
    let results;
    let text = "";
    if ((validateNumbersCheck(element) && !validateSymbolsCheck(element)) && element.value.length !== 0) {
        text = "";
        element.classList.remove("is-invalid");
        results = element.value;
    } else {
        element.classList.add("is-invalid");
        text = (element.value.length !== 0) ?
            "Only numeric values are allowed in this field." :
            "This field is required.";
        results = false;
    }
    document.getElementById(element.id + "Validate").innerHTML = text;
    formFields[element.id] = results;
}
//Validate SSN Format and Numeric Characters
function validateSSN(element) {
    let results;
    let text = "";
    if (validateSSNCheck(element) && !validateCharsCheck(element)) {
        text = "";
        element.classList.remove("is-invalid");
        results = element.value;
    } else {
        element.classList.add("is-invalid");
        text = (element.value.length !== 0) ?
            "Please input your 9 digit, '-' are optional." :
            "This field is required.";
        results = false;
    }
    document.getElementById(element.id + "Validate").innerHTML = text;
    formFields[element.id] = results;
}
//Validate Phone Number format and Numeric Characters
function validatePhone(element) {
    var results;
    let text = "";
    if (validatePhoneCheck(element) && !validateCharsCheck(element)) {
        text = "";
        element.classList.remove("is-invalid");
        results = element.value;
    } else {
        element.classList.add("is-invalid");
        text = (element.value.length !== 0) ?
            "Please input your phone number digit '()' and '-' are optional." :
            "This field is required.";
        results = false;
    }
    document.getElementById(element.id + "Validate").innerHTML = text;
    formFields[element.id] = results;
}
//Validation====================================================


//Submit Contact ================================================
function formContactSubmit() {
    /*let fields = [
        "FirstName", "LastName","State", "Country", "Email", "SSN"
    ];*/
    let fields = ["FirstName", "LastName", "City", "State", "Country", "Email", "AddressLine1", "AddressLine2", "MobilePhone", "BusinessPhone", "SSN", "ZipCode"];
    validateAllContactFields(fields);
    let clearedValidation = true;
    for (let i = 0; i < fields.length; i++) {
        if (formFields[fields[i]] === false) {
            clearedValidation = false;
        }
    }
    if (clearedValidation) {
        //Ajax call
        document.getElementById("contactSubmitValidate").innerHTML = "";
        let contactObj = {};
        for (let i = 0; i < fields.length; i++) {
            contactObj[fields[i]] = formFields[fields[i]];
        }
        contactObj["State"] = (contactObj["Country"] === "Canada") ? "" : contactObj["State"];

        document.getElementById("contactLoading").classList.remove("no_display");
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", "https://team3webapi.azurewebsites.net/api/user", true);
        xmlHttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        xmlHttp.send(JSON.stringify(contactObj));
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
                contactID = this.response;
                contactID = contactID.substring(1, contactID.length - 1);

                document.getElementById("formMortgage").classList.remove("hiddenForm");
                document.getElementById("formMortgage").classList.remove("no_display");
                document.getElementById("formContact").classList.add("fadeOutNext");
                document.getElementById("contactLoading").classList.add("no_display");
            } else if (xmlHttp.readyState === 4 && xmlHttp.status >= 400) {
                document.getElementById("contactSubmitValidate").innerHTML = "An Error has occured while executing request.";
                document.getElementById("contactLoading").classList.add("no_display");
            }
        };
    } else {
        document.getElementById("SubmitValidate").innerHTML = "There are errors on the form.";
    }
}

function validateAllContactFields(fields) {
    for (var i = 0; i < fields.length; i++) {
        let element = document.getElementById(fields[i]);
        if (i >= 0 && i < 5) {
            validateChars(element);
        } else if (i >= 5 && i < 8) {
            validate(element);
        } else if (i === 8 || i === 9) {
            validatePhone(element);
        } else if (i === 10) {
            validateSSN(element);
        } else {
            validateNumbers(element);
        }
    }
}

//Submit Mortgage===============================================
function formMortgageSubmit() {
    /*let fields = [
        "FirstName", "LastName","State", "Country", "Email", "SSN"
    ];*/
    let fields = ["MortgageName", "MortgageAmount", "Option"];
    validateChars(document.getElementById("MortgageName"));
    validateNumbers(document.getElementById("MortgageAmount"));
    validate(document.getElementById("Option"));
    let clearedValidation = true;
    for (var i = 0; i < fields.length; i++) {
        if (formFields[fields[i]] === false) {
            clearedValidation = false;
        }
    }
    if (clearedValidation) {
        //Ajax call
        document.getElementById("mortgageSubmitValidate").innerHTML = "";
        let mortgageObj = { ContactID: contactID };
        //let mortgageObj = { ContactID: "ddb4b385-3fb1-e811-a96b-000d3a1ca939" };
        for (let i = 0; i < fields.length; i++) {
            switch (fields[i]) {
                case "MortgageAmount":
                    mortgageObj[fields[i]] = parseFloat(formFields[fields[i]]);
                    break;
                case "Option":
                    mortgageObj[fields[i]] = parseInt(formFields[fields[i]]);
                    break;
                default:
                    mortgageObj[fields[i]] = formFields[fields[i]];
            }
        }

        document.getElementById("mortgageloading").classList.remove("no_display");
        document.querySelector("#Submit").disabled = true;
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", "https://team3webapi.azurewebsites.net/api/mortgage", true);
        xmlHttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        xmlHttp.send(JSON.stringify(mortgageObj));
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
                mortgageResult = this.responseText;
                for (var i = 0; i < files.length; i++) {
                    let fileObj = {
                        //"ContactID": "ddb4b385-3fb1-e811-a96b-000d3a1ca939",
                        "ContactID": contactID,
                        "Base64Data": files[i].file64,
                        //"Type": files[i].file.type,
                        "FileName": files[i].file.name
                    };
                    uploadFiles(fileObj);
                }
                if (files.length === 0) {
                    window.location.replace("/Pages/Account.html");
                }
                document.getElementById("mortgageloading").classList.add("no_display");
            } else if (xmlHttp.readyState === 4 && xmlHttp.status >= 400) {
                document.getElementById("mortgageSubmitValidate").innerHTML = "An Error has occured while executing request.";
                document.getElementById("mortgageloading").classList.add("no_display");
            }
        };
    } else {
        document.getElementById("SubmitValidate").innerHTML = "There are errors on the form.";
    }
}

function uploadFiles(fileObj) {
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("POST", "https://team3webapi.azurewebsites.net/api/note", true);
    xmlHttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xmlHttp.send(JSON.stringify(fileObj));
    xmlHttp.onreadystatechange = function () {
        if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
            checkUploadComplete();
        } else if (xmlHttp.readyState === 4 && xmlHttp.status >= 400) {
            document.getElementById("mortgageSubmitValidate").innerHTML = "An Error has occured while executing request.";
            document.getElementById("mortgageloading").classList.add("no_display");
        }
    };
}
function checkUploadComplete() {
    filesUploaded += 1;
    if (filesUploaded >= files.length) {
        document.querySelector("#Submit").disabled = false;
        document.getElementById("mortgageSubmitValidate").innerHTML = "";
        document.getElementById("mortgageloading").classList.add("no_display");
        window.location.replace("/Pages/Account.html");
    }
}

function handleFileInput(element) {
    let fileObj = { "file": element.files[0] };

    let reader = new FileReader();
    reader.readAsDataURL(element.files[0]);
    reader.onload = function () {
        let index = reader.result.indexOf("base64,");
        index += 7;
        let result = reader.result.substring(index, reader.result.length);

        fileObj["file64"] = result;
        files.push(fileObj);

        //let names = "";
        let fileLabel = document.createElement("span");
        fileLabel.id = element.files[0].name;
        fileLabel.classList.add("file_label");
        fileLabel.classList.add("label");
        fileLabel.classList.add("label-default");
        fileLabel.innerHTML = element.files[0].name;
        fileLabel.onclick = removeFile.bind(this, element.files[0].name)
        document.getElementById("files").appendChild(fileLabel);
    };
    reader.onerror = function () {
        document.getElementById("filesValidate").innerHTML = "Error Uplode: " + element.files[0].name;
        console.log('Error:', error);
    };
}
function removeFile(file) {
    for (var i = 0; i < files.length; i++) {
        if (files[i]["file"].name === file) {
            files.splice(i, 1);
        }
    }
    let element = document.getElementById(file);
    document.getElementById("files").removeChild(element);
}

function checkCanada() {
    let element = document.getElementById("Country");
    let result = (element.value === "Canada") ? true : false;
    document.getElementById("State").disabled = result;
}

//function addBase64(file) {
//    let reader = new FileReader();
//    reader.readAsDataURL(file);
//    reader.onload = function () {
//        file64.push(reader.result);
//    };
//    reader.onerror = function () {
//        console.log('Error:', error);
//    }
//}
//function deleteBase64(file) {
//    let reader = new FileReader();
//    reader.readAsDataURL(file);
//    reader.onload = function () {
//        for (var i = 0; i < files64.length; i++) {
//            if (files64[i] === reader.result) {
//                files64.splice(i, 1);
//            }
//        }
//    };
//    reader.onerror = function () {
//        console.log('Error:', error);
//    }
//}
