getDeleteButtons()

window.addEventListener("beforeunload", function(event) {
    // Cancel the event as we'll show our own confirmation message
    event.preventDefault();
    // Chrome requires returnValue to be set
    event.returnValue = '';

    // Show the warning message
    var confirmationMessage = 'Are you sure you want to leave this page? Your changes may not be saved.';
    (event || window.event).returnValue = confirmationMessage;
    return confirmationMessage;
});

function createParagraph(id, textArea) {
    var textAreaValue = document.getElementById(textArea).value.trim();
    if (textAreaValue !== '') {
        var convertedValue = textAreaValue.replace(/\n/g, "<br>"); // Convert line breaks to <br> tags
        var newParagraph = document.createElement('p');
        newParagraph.innerHTML = convertedValue; // Use innerHTML to insert HTML content
        var deleteButton = document.createElement('button');
        deleteButton.className = "deleteButton";
        deleteButton.textContent = 'Delete';
        var containerDiv = document.createElement('div');
        containerDiv.appendChild(newParagraph);
        containerDiv.appendChild(deleteButton);
        document.getElementById(id).appendChild(containerDiv);
        document.getElementById(textArea).value = "";
        getDeleteButtons();
    }
}

function getDeleteButtons(){
    const deleteButtons = document.querySelectorAll(".deleteButton")
    deleteButtons.forEach(element => {
        element.onclick = function (){
            this.parentElement.remove()
        }
    });
}


async function readCurrentHTMLFileAndSave() {
    currentPageHTML = document.documentElement.outerHTML;
    var currentFile = window.location.href.match(/\/versions\/(.*?)\.html/)[1];
    console.log(currentFile)
    try {
        const fileOptions = {
            suggestedName: currentFile,
            types: [{
                description: 'HTML Files',
                accept: {
                    'text/html': ['.html']
                }
            }]
        };
        
        // Use the File System Access API to save the file
        const fileHandle = await window.showSaveFilePicker(fileOptions);
        const writableStream = await fileHandle.createWritable();
        await writableStream.write(currentPageHTML);
        await writableStream.close();
    } catch (error) {
        console.error('Error reading or saving HTML file:', error);
    }
}

function toggleAccordion(btn, index) {
    var content = document.getElementsByClassName("accordion-content")[index - 1];
    content.classList.toggle("active");
    btn.classList.toggle('underline');
}

function shrunk(){
    var accordions = document.getElementsByClassName("accordion-content");
    for (var i = 0; i < accordions.length; i++) {
        accordions[i].classList.remove("active");
    }
    var headers = document.getElementsByClassName("content-header");
    for (var i = 0; i < headers.length; i++) {
        headers[i].classList.remove("underline");
    }
}

function scrollToTop() {
    // Check if the browser supports the behavior option
    if ('scrollBehavior' in document.documentElement.style) {
      // If supported, use the smooth scrolling behavior
      window.scrollTo({
        top: (window.screenY - window.scrollY),
        behavior: "smooth"
      });
    } else {
      // If not supported, use a polyfill or fallback to instant scrolling
      window.scrollTo(0, 0);
    }
}