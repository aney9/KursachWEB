document.addEventListener('DOMContentLoaded', function () {
    // Показать модальное окно авторизации
    function showLoginModal() {
        closeAllModals();
        document.getElementById('loginModal').style.display = 'block';
        document.body.style.overflow = 'hidden';
    }

    // Закрыть модальное окно авторизации
    function closeLoginModal() {
        document.getElementById('loginModal').style.display = 'none';
        document.body.style.overflow = 'auto';
    }

    // Показать модальное окно регистрации
    function showRegisterModal() {
        closeAllModals();
        document.getElementById('registerModal').style.display = 'block';
        document.body.style.overflow = 'hidden';
    }

    // Закрыть модальное окно регистрации
    function closeRegisterModal() {
        document.getElementById('registerModal').style.display = 'none';
        document.body.style.overflow = 'auto';
    }

    // Показать модальное окно восстановления пароля
    function showForgotPasswordModal() {
        closeAllModals();
        document.getElementById('forgotPasswordModal').style.display = 'block';
        document.body.style.overflow = 'hidden';
    }

    // Закрыть модальное окно восстановления пароля
    function closeForgotPasswordModal() {
        document.getElementById('forgotPasswordModal').style.display = 'none';
        document.body.style.overflow = 'auto';
    }

    // Показать модальное окно ввода кода
    function showVerifyCodeModal() {
        closeAllModals();
        document.getElementById('verifyCodeModal').style.display = 'block';
        document.body.style.overflow = 'hidden';
    }

    // Закрыть модальное окно ввода кода
    function closeVerifyCodeModal() {
        document.getElementById('verifyCodeModal').style.display = 'none';
        document.body.style.overflow = 'auto';
    }

    // Показать модальное окно смены пароля
    function showResetPasswordModal() {
        closeAllModals();
        document.getElementById('resetPasswordModal').style.display = 'block';
        document.body.style.overflow = 'hidden';
    }

    // Закрыть модальное окно смены пароля
    function closeResetPasswordModal() {
        document.getElementById('resetPasswordModal').style.display = 'none';
        document.body.style.overflow = 'auto';
    }

    // Закрыть все модальные окна
    function closeAllModals() {
        closeLoginModal();
        closeRegisterModal();
        closeForgotPasswordModal();
        closeVerifyCodeModal();
        closeResetPasswordModal();
    }

    // Отправка кода на email
    function sendResetCode(event) {
        event.preventDefault();
        const email = document.getElementById('ForgotEmail').value;
        const errorElement = document.getElementById('forgotPasswordError');

        fetch('/Profile/ForgotPassword', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email: email }),
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    document.getElementById('VerifyEmail').value = email;
                    showVerifyCodeModal();
                } else {
                    errorElement.textContent = data.message;
                }
            })
            .catch(error => {
                errorElement.textContent = 'Произошла ошибка. Попробуйте снова.';
                console.error('Error:', error);
            });
    }

    // Проверка кода
    function verifyResetCode(event) {
        event.preventDefault();
        const code = document.getElementById('ResetCode').value;
        const email = document.getElementById('VerifyEmail').value;
        const errorElement = document.getElementById('verifyCodeError');

        fetch('/Profile/VerifyCode', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email: email, code: code }),
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    document.getElementById('ResetEmail').value = email;
                    showResetPasswordModal();
                } else {
                    errorElement.textContent = data.message;
                }
            })
            .catch(error => {
                errorElement.textContent = 'Произошла ошибка. Попробуйте снова.';
                console.error('Error:', error);
            });
    }

    // Смена пароля
    function resetPassword(event) {
        event.preventDefault();
        const email = document.getElementById('ResetEmail').value;
        const newPassword = document.getElementById('NewPassword').value;
        const confirmPassword = document.getElementById('ConfirmPassword').value;
        const errorElement = document.getElementById('resetPasswordError');

        if (newPassword !== confirmPassword) {
            errorElement.textContent = 'Пароли не совпадают';
            return;
        }

        fetch('/Profile/ResetPassword', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email: email, newPassword: newPassword }),
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    closeResetPasswordModal();
                    showLoginModal();
                } else {
                    errorElement.textContent = data.message;
                }
            })
            .catch(error => {
                errorElement.textContent = 'Произошла ошибка. Попробуйте снова.';
                console.error('Error:', error);
            });
    }

    // Закрытие модального окна при клике на фон
    window.onclick = function (event) {
        const loginModal = document.getElementById('loginModal');
        const registerModal = document.getElementById('registerModal');
        const forgotPasswordModal = document.getElementById('forgotPasswordModal');
        const verifyCodeModal = document.getElementById('verifyCodeModal');
        const resetPasswordModal = document.getElementById('resetPasswordModal');

        if (event.target == loginModal) closeLoginModal();
        if (event.target == registerModal) closeRegisterModal();
        if (event.target == forgotPasswordModal) closeForgotPasswordModal();
        if (event.target == verifyCodeModal) closeVerifyCodeModal();
        if (event.target == resetPasswordModal) closeResetPasswordModal();
    };

    // Экспорт функций в глобальную область видимости
    window.showLoginModal = showLoginModal;
    window.closeLoginModal = closeLoginModal;
    window.showRegisterModal = showRegisterModal;
    window.closeRegisterModal = closeRegisterModal;
    window.showForgotPasswordModal = showForgotPasswordModal;
    window.closeForgotPasswordModal = closeForgotPasswordModal;
    window.showVerifyCodeModal = showVerifyCodeModal;
    window.closeVerifyCodeModal = closeVerifyCodeModal;
    window.showResetPasswordModal = showResetPasswordModal;
    window.closeResetPasswordModal = closeResetPasswordModal;
    window.sendResetCode = sendResetCode;
    window.verifyResetCode = verifyResetCode;
    window.resetPassword = resetPassword;
});