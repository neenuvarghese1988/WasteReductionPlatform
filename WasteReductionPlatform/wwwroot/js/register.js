$(document).ready(function () {
    $("#Province").change(function () {
        var provinceCode = $(this).val();
        updateAutocompleteOptions(provinceCode);
    });

    $("#City").on("input", function () {
        validateCityPostalCode();
    });

    $("#PostalCode").on("input", function () {
        validateCityPostalCode();
    });

    function updateAutocompleteOptions(provinceCode) {
        if (provinceCode && locationData[provinceCode]) {
            var cities = locationData[provinceCode].cities.map(city => `<option value="${city}">`).join('');
            var postalCodes = locationData[provinceCode].postalCodes.map(code => `<option value="${code}">`).join('');
            $("#CityList").html(cities);
            $("#PostalCodeList").html(postalCodes);
        } else {
            $("#CityList").html('');
            $("#PostalCodeList").html('');
        }
    }

    function validateCityPostalCode() {
        var provinceCode = $("#Province").val();
        var city = $("#City").val();
        var postalCode = $("#PostalCode").val();

        var validCity = locationData[provinceCode]?.cities.includes(city);
        var validPostalCode = locationData[provinceCode]?.postalCodes.includes(postalCode);

        if (!validCity) {
            $("#City").addClass("is-invalid");
        } else {
            $("#City").removeClass("is-invalid");
        }

        if (!validPostalCode) {
            $("#PostalCode").addClass("is-invalid");
        } else {
            $("#PostalCode").removeClass("is-invalid");
        }
    }
});
