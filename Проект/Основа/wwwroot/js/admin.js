function openAdminTab(btnId, tabId) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(tabId).style.display = "block";
    document.getElementById(btnId).className += " active";
}

function editCityDistrict(id, city, district) {
    const targetForm = document.forms["editCityDistrictForm"];

    targetForm.elements['id'].value = id;
    targetForm.elements['city'].value = city;
    targetForm.elements['district'].value = district;

    $('#addCityDistrictModal').modal('show');
}

$(document).ready(function () {
    openAdminTab('btnCityDistrict', 'cityDistrict');

    $('#addRecordNavLink').click(function () {
        $('#addCityDistrictModal').modal('show');
    });
});