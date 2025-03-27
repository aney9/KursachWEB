function showAddProductModal() {
    document.getElementById("addProductModal").style.display = "block";
    document.body.style.overflow = "hidden";
}

function closeAddProductModal() {
    document.getElementById("addProductModal").style.display = "none";
    document.body.style.overflow = "auto";
}

function showAddBrandModal() {
    document.getElementById("addBrandModal").style.display = "block";
    document.body.style.overflow = "hidden";
}

function closeAddBrandModal() {
    document.getElementById("addBrandModal").style.display = "none";
    document.body.style.overflow = "auto";
}

function showAddCategoryModal() {
    document.getElementById("addCategoryModal").style.display = "block";
    document.body.style.overflow = "hidden";
}

function closeAddCategoryModal() {
    document.getElementById("addCategoryModal").style.display = "none";
    document.body.style.overflow = "auto";
}

window.onclick = function (event) {
    const productModal = document.getElementById("addProductModal");
    const brandModal = document.getElementById("addBrandModal");
    const categoryModal = document.getElementById("addCategoryModal");
    if (event.target == productModal) {
        productModal.style.display = "none";
        document.body.style.overflow = "auto";
    }
    if (event.target == brandModal) {
        brandModal.style.display = "none";
        document.body.style.overflow = "auto";
    }
    if (event.target == categoryModal) {
        categoryModal.style.display = "none";
        document.body.style.overflow = "auto";
    }
}