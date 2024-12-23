function openBuyerTab(btnId, tabId) {
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

function viewFile(photoIds) {
    const newItems = [];
    const newIndicators = [];

    for (let i = 0; i < photoIds.length; i++) {
        const photoId = photoIds[i];

        if (i == 0) {
            newItems.push(htmlToNode(`<div class="carousel-item active"><img src = "file?id=${photoId}" class= "d-block w-100" alt = "Фото ${photoId}"></div>`));
            newIndicators.push(htmlToNode(`<button type="button" data-bs-target="#housingPhotoCarousel" data-bs-slide-to="${i}" class="active" aria-current="true" aria-label="Фото ${i}"></button>`));
        } else {
            newItems.push(htmlToNode(`<div class="carousel-item"><img src = "file?id=${photoId}" class= "d-block w-100" alt = "Фото ${photoId}"></div>`));
            newIndicators.push(htmlToNode(`<button type="button" data-bs-target="#housingPhotoCarousel" data-bs-slide-to="${i}" aria-label="Фото ${i}"></button>`));
        }
    }

    const items = $('#housingPhotoCarouselItems');
    const indicators = $('#housingPhotoCarouselIndicators');

    items[0].replaceChildren(...newItems);
    indicators[0].replaceChildren(...newIndicators);

    $('#viewFileModal').modal('show');
}

function htmlToNode(html) {
    const template = document.createElement('template');
    template.innerHTML = html;
    return template.content.firstChild;
}

$(document).ready(function () {

    openBuyerTab('btnApplicableHousing', 'applicableHousing');

    $('#addHousingRequirementsNavLink').click(function () {
        $.ajax({
            url: `${window.location.origin}/Dictionary/CityDistrict`,
            type: 'POST',
            success: function (results) {
                
                const target = $('#selectHousingCityDistrict');
                const resultHtml = results.map(x => `<option value="${x.id}">${x.name}</option>`).join('');

                target.empty();
                target.html(resultHtml);

                $.ajax({
                    url: `${window.location.origin}/Buyer/GetHousingRequirements`,
                    type: 'POST',
                    success: function (result) {

                        if (result) {
                            const targetForm = document.forms["editHousingRequirementsForm"];

                            for (const [key, value] of Object.entries(result)) {
                                const input = targetForm.elements[key];
                                if (input.type === 'checkbox') {
                                    input.checked = !!value;
                                } else if (input.type === 'select-multiple' && value instanceof Array && input.name === 'housingCityDistrict') {
                                    target.val(value);  
                                } else {
                                    input.value = value;
                                }
                            }
                        }                    

                        $('#addHousingRequirementsModal').modal('show');
                    }
                });
            }
        });
    });

    $('#accountNavLink').click(function () {
        $.ajax({
            url: `${window.location.origin}/Buyer/GetBuyer`,
            type: 'POST',
            success: function (result) {
                const targetForm = document.forms["editBuyerForm"];

                for (const [key, value] of Object.entries(result)) {
                    const input = targetForm.elements[key];
                    if (input.type === 'checkbox') {
                        input.checked = !!value;
                    } else {
                        input.value = value;
                    }
                }

                $('#editModalBuyer').modal('show');
            }
        });
    });
});