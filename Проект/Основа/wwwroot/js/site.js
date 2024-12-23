$(document).ready(function () {
    $('#loginIcon').click(function () {
        $('#logoutModal').modal('show');
    });

    //Скрипт для открытия/закрытия модального окна при клике на иконку
    $('#loginIcon').click(function () {
        $('#loginModal').modal('show');
    });
    $('#registerLordLinkId').click(function () {
        $('#loginModal').modal('hide');
    });
    $('#registerBuyerLinkId').click(function () {
        $('#loginModal').modal('hide');
    });
});