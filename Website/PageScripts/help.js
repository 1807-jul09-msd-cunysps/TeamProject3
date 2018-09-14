function getKBArticles() {
    let container = document.getElementById("kbResultsTaget");
    while (container.firstChild) {
        container.removeChild(container.firstChild);
    }

    let loadingImg = document.createElement("img");
    loadingImg.src = "../Pics/loading.gif";
    loadingImg.alt = "loaing";
    loadingImg.classList.add("kb_loading");
    container.appendChild(loadingImg);

    let search = document.getElementById("searchInput").value;
    let xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            let kbList;
            if (this.responseText === "No Record Found.") {
                kbList === this.responseText;

                let noRecords = document.createElement("h3");
                noRecords.innerHTML = kbList;
                container.appendChild(noRecords);
                
                container
            } else {
                container.removeChild(loadingImg);
                kbList = JSON.parse(this.responseText);
                for (var i = 0; i < kbList.length; i++) {
                    let element = document.createElement("DIV");
                    element.classList.add("profile");
                    element.classList.add("col-md-10");
                    element.classList.add("mb-4");

                    let title = document.createElement("div");
                    title.classList.add("row");
                    title.classList.add("mortgage_name");
                    title.classList.add("mortgage_panel_ele");
                    title.innerHTML = kbList[i]["Title"];
                    //=============================================================
                    let row1 = document.createElement("div");
                    row1.classList.add("row");
                    row1.classList.add("account_row");

                    let contentLabel = document.createElement("div");
                    contentLabel.classList.add("col-md-4");
                    contentLabel.innerHTML = "Content:";
                    
                    row1.appendChild(contentLabel);
                    //=============================================================
                    let row2 = document.createElement("div");
                    row2.classList.add("row");
                    row2.classList.add("account_row");
                    row2.classList.add("kb_content");

                    let contentValue = document.createElement("div");
                    contentValue.classList.add("col-md-4");
                    contentValue.innerHTML = kbList[i]["Content"];

                    row2.appendChild(contentValue);

                    element.appendChild(title);
                    element.appendChild(row1);
                    element.appendChild(row2);
                    //element.onclick = showMortgage.bind(this, true, i, termYears, balance);

                    container.appendChild(element);
                }
            }
        } else if (this.readyState === 4 && this.status === 400) {
            document.getElementById("error").innerHTML = "Case Request Failed.";
        }
    };
    xmlHttp.open("GET", "https://team3webapi.azurewebsites.net/api/knowledge/" + search, true);
    xmlHttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xmlHttp.send();
}