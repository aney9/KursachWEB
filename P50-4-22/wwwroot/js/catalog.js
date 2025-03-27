// catalog.js

document.addEventListener('DOMContentLoaded', () => {
    // Привязываем события
    document.getElementById('sortSelect').addEventListener('change', sortProducts);
    document.getElementById('searchInput').addEventListener('keyup', filterProducts);
    document.querySelectorAll('.brand-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', filterProducts);
    });
    document.querySelector('.clear-filters-btn').addEventListener('click', clearFilters);

    // Инициализация избранных товаров
    initializeFavorites();

    // Инициализация при загрузке страницы
    filterProducts();
});

function clearFilters() {
    document.getElementById('searchInput').value = '';
    document.getElementById('sortSelect').value = 'priceAsc';
    document.querySelectorAll('.brand-checkbox').forEach(cb => cb.checked = false);
    filterProducts();
}

function filterProducts() {
    const searchInput = document.getElementById('searchInput').value.toLowerCase();
    const selectedBrands = Array.from(document.querySelectorAll('.brand-checkbox:checked')).map(cb => cb.value);
    const products = document.querySelectorAll('.product-card');

    products.forEach(product => {
        const name = product.getAttribute('data-name').toLowerCase();
        const brand = product.getAttribute('data-brand') || '';
        const matchesSearch = name.includes(searchInput);
        const matchesBrand = selectedBrands.length === 0 || selectedBrands.includes(brand);

        if (matchesSearch && matchesBrand) {
            product.style.display = 'block';
            product.classList.add('fade-in');
        } else {
            product.style.display = 'none';
        }
    });

    sortProducts();
}

function sortProducts() {
    const sortValue = document.getElementById('sortSelect').value;
    const productList = document.getElementById('productList');
    const products = Array.from(productList.children);

    products.sort((a, b) => {
        const priceA = parseFloat(a.getAttribute('data-price'));
        const priceB = parseFloat(b.getAttribute('data-price'));
        return sortValue === 'priceAsc' ? priceA - priceB : priceB - priceA;
    });

    productList.innerHTML = '';
    products.forEach(product => {
        productList.appendChild(product);
        product.classList.add('fade-in');
    });
}

function initializeFavorites() {
    const favorites = JSON.parse(localStorage.getItem('favorites')) || [];
    document.querySelectorAll('.product-card').forEach(product => {
        const productId = product.getAttribute('data-id');
        const favoriteBtn = product.querySelector('.favorite-btn');
        if (favorites.includes(productId)) {
            favoriteBtn.classList.add('favorited');
        }
    });
}

function toggleFavorite(productId) {
    let favorites = JSON.parse(localStorage.getItem('favorites')) || [];
    const productCard = document.querySelector(`.product-card[data-id="${productId}"]`);
    const favoriteBtn = productCard.querySelector('.favorite-btn');

    if (favorites.includes(productId.toString())) {
        // Удаляем из избранного
        favorites = favorites.filter(id => id !== productId.toString());
        favoriteBtn.classList.remove('favorited');
    } else {
        // Добавляем в избранное
        favorites.push(productId.toString());
        favoriteBtn.classList.add('favorited');
    }

    localStorage.setItem('favorites', JSON.stringify(favorites));
}