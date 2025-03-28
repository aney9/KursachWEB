function removeFromFavorites(productId) {
    const card = document.querySelector(`.product-card[data-id="${productId}"]`);

    fetch('/User/RemoveFromFavorites', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ productId: productId })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                card.remove();
                if (!document.querySelector('.product-card')) {
                    document.getElementById('favoritesList').innerHTML = '<p class="text-center" style="color: #FFFFFF;">Ваш список избранного пуст.</p>';
                }
            } else {
                alert(data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
}