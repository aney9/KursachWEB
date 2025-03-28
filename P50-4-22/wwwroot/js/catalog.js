document.addEventListener('DOMContentLoaded', () => {
    // Привязываем события
    document.getElementById('sortSelect').addEventListener('change', sortProducts);
    document.getElementById('searchInput').addEventListener('keyup', filterProducts);
    document.querySelectorAll('.brand-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', filterProducts);
    });
    document.querySelectorAll('.category-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', filterProducts);
    });
    document.querySelector('.clear-filters-btn').addEventListener('click', clearFilters);

    // Инициализация при загрузке страницы
    filterProducts();
});

function clearFilters() {
    document.getElementById('searchInput').value = '';
    document.getElementById('sortSelect').value = 'priceAsc';
    document.querySelectorAll('.brand-checkbox').forEach(cb => cb.checked = false);
    document.querySelectorAll('.category-checkbox').forEach(cb => cb.checked = false);
    filterProducts();
}

function filterProducts() {
    const searchInput = document.getElementById('searchInput').value.toLowerCase();
    const selectedBrands = Array.from(document.querySelectorAll('.brand-checkbox:checked')).map(cb => cb.value);
    const selectedCategories = Array.from(document.querySelectorAll('.category-checkbox:checked')).map(cb => cb.value);
    const products = document.querySelectorAll('.product-card');

    products.forEach(product => {
        const name = product.getAttribute('data-name').toLowerCase();
        const brand = product.getAttribute('data-brand') || '';
        const category = product.getAttribute('data-category') || '';
        const matchesSearch = name.includes(searchInput);
        const matchesBrand = selectedBrands.length === 0 || selectedBrands.includes(brand);
        const matchesCategory = selectedCategories.length === 0 || selectedCategories.includes(category);

        if (matchesSearch && matchesBrand && matchesCategory) {
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

function toggleFavorite(productId) {
    const button = document.querySelector(`.favorite-btn[data-product-id="${productId}"]`);
    const isFavorited = button.classList.contains('favorited');

    fetch('/User/ToggleFavorite', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ productId: productId })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                if (data.isFavorite) {
                    button.classList.add('favorited');
                } else {
                    button.classList.remove('favorited');
                }
            } else {
                alert(data.message || 'Произошла ошибка при обновлении избранного.');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Произошла ошибка при выполнении запроса.');
        });
}