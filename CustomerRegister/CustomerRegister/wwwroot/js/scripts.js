$(document).ready(function () {

    var msg = $('#msg');
    var formHeader = $('#formHeader');

    var addCustomer = $('.addCustomer');
    var addCustomerForm = $('.addCustomerForm');
    var seedFunction = $('.seedFunction');

    var tableBody = $('#tableBody');

    var firstNameInput = $('#firstNameInput');
    var lastNameInput = $('#lastNameInput');
    var emailInput = $('#emailInput');
    var genderSelect = $('#genderSelect');
    var ageInput = $('#ageInput');

    getCustomers();

    function getCustomers() {

        $.ajax({
            url: '/api/customer',
            type: 'GET'
        }).done(function (result) {

            tableBody.empty();
            $.each(result, function (index, row) {
                var newRow = $('<tr></tr>');
                newRow.append(constructTableRow(row));
                newRow.appendTo(tableBody);
            });

        }).fail(function (xhr, status, error) {
            console.log('fail at get customers');
        });

    }

    function constructTableRow(row) {
        return `<td>${row.id}</td>` +
            `<td>${row.firstName}</td>` +
            `<td>${row.lastName}</td>` +
            `<td>${row.email}</td>` +
            `<td>${row.gender}</td>` +
            `<td>${row.age}</td>` +
            `<td>${row.customerCreated}</td>` +
            `<td>${row.customerEdited}</td>` +
            `<td><button id=${row.id} class="btn btn-warning editCustomer">Redigera</button></td>` +
            `<td><button id=${row.id} class="btn btn-danger removeCustomer">Ta bort</button></td>`;
    }

    addCustomer.on('click', function () {

        var data = {};
        data.firstName = firstNameInput.val();
        data.lastName = lastNameInput.val();
        data.email = emailInput.val();
        data.gender = genderSelect.val();
        data.age = ageInput.val();

        $.ajax({
            url: '/api/customer',
            type: 'POST',
            data: { 'Customer': data }
        }).done(function () {
            getCustomers();
            resetValues();
        }).fail(function (xhr, status, error) {
            msg
                .html(xhr.responseText)
                .slideDown().delay(2000).slideUp();
        });

    });

    $(document).on('click', '.editCustomer', function () {

        $(document).find('.saveEdit').remove();
        $(document).find('.cancelEdit').remove();
        var id = $(this).attr('id');
        getCustomer(id);
        addCustomer.css('display', 'none');
        addCustomerForm.append($(`<button id=${id} class="btn btn-primary btn-block saveEdit">Spara</button>`));
        addCustomerForm.append($(`<button class="btn btn-danger btn-block cancelEdit">Avbryt</button>`));
        formHeader.html('Redigera Kund');

    });

    $(document).on('click', '.saveEdit', function () {

        var data = {};
        data.id = $(this).attr('id');
        data.firstName = firstNameInput.val();
        data.lastName = lastNameInput.val();
        data.email = emailInput.val();
        data.gender = genderSelect.val();
        data.age = ageInput.val();

        $.ajax({
            url: '/api/customer',
            type: 'PATCH',
            data: { 'Customer': data }
        }).done(function () {
            resetValues();
            getCustomers();
        }).fail(function (xhr, status, error) {
            console.log('fail at save edit');
        });

    });

    $(document).on('click', '.cancelEdit', function () {

        resetValues();

    });

    function resetValues() {

        addCustomerForm.find(':input').val('');
        genderSelect.val('Man');
        $(document).find('.saveEdit').remove();
        $(document).find('.cancelEdit').remove();
        addCustomer.css('display', 'block');
        formHeader.html('Ny Kund');

    }

    function getCustomer(id) {

        $.ajax({
            url: '/api/customer/' + id,
            type: 'GET'
        }).done(function (customer) {
            firstNameInput.val(customer.firstName);
            lastNameInput.val(customer.lastName);
            emailInput.val(customer.email);
            genderSelect.val(customer.gender);
            ageInput.val(customer.age);
        });

    }

    $(document).on('click', '.removeCustomer', function () {

        var id = $(this).attr('id');

        $.ajax({
            url: '/api/customer',
            type: 'DELETE',
            data: { 'id': id }
        }).done(function (result) {
            getCustomers();
        }).fail(function (xhr, status, error) {
            console.log('fail at remove customer');
        });

    });

    seedFunction.on('click', function () {

        console.log('inside seed function');

        $.ajax({
            url: '/api/customer/seed',
            type: 'GET'
        }).done(function (result) {
            console.log('success seed ajax');
            getCustomers();
        }).fail(function (xhr, status, error) {
            console.log('Fail seed ajax');
        });

    });

});