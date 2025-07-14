document.querySelector("form").addEventListener("submit", function (e) {
   
    const sizeAR = document.getElementById("sizeAR");
    const sizeEN = document.getElementById("sizeEN");
    const sizePrice = document.getElementById("sizePrice");
    const sizeAddBtn = document.getElementById("addSizeBtn");

    if (sizeAR && sizeEN && sizePrice) {
        if (sizeAR.value.trim() !== "" || sizeEN.value.trim() !== "" || sizePrice.value.trim() !== "") {
            sizeAddBtn.click(); // trigger the add
        }
    }

    const flavourAR = document.getElementById("flavourAR");
    const flavourEN = document.getElementById("flavourEN");
    const flavourAddBtn = document.getElementById("addFlavourBtn");

    if (flavourAR && flavourEN) {
        if (flavourAR.value.trim() !== "" || flavourEN.value.trim() !== "") {
            flavourAddBtn.click();
        }
    }

  
    const colorName = document.getElementById("colorName");
    const colorPicker = document.getElementById("colorPicker");
    const colorAddBtn = document.getElementById("addColorBtn");

    if (colorName && colorPicker) {
        if (colorName.value.trim() !== "") {
            colorAddBtn.click();
        }
    }


    const sizeBlocks = document.querySelectorAll("#dynamicSizeInputs input[name^='Sizes']");
    const uploadedImages = document.querySelectorAll("#imagePreview img, .image-container img");

    if (sizeBlocks.length === 0) {
        alert("يجب إضافة حجم واحد على الأقل.");
        e.preventDefault(); // prevent form submission
        return;
    }

    if (uploadedImages.length === 0) {
        alert("يجب إضافة صورة واحدة على الأقل.");
        e.preventDefault(); // prevent form submission
        return;
    }
});