$.ajaxSetup({
    beforeSend: function (xhr) {
        var token = localStorage.getItem('accessToken');
        if (token) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        }
    }
});


var token = localStorage.getItem('accessToken');

if (token == null) {
    window.location.href = '/account/login';
}
else {
    var dataToPost = {
        'AccessToken': token,
        'RefreshToken': localStorage.getItem('refreshToken')
    }

    $.ajax({
        url: '/api/v1/auth/validate/',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataToPost),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        success: function (response) {
            
        },
        error: function () {
            
                localStorage.removeItem('accessToken');
                localStorage.removeItem('refreshToken');
                location.reload();
            
        }
    });
}
$(document).ready(function () {
    $('#logoutBtn').on('click', function () {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        location.reload();
    });


    $('.edit-btn').on('click', function () {
        var productCode = $(this).data('productcode');
        var productName = $(this).data('productname');
        var quantity = $(this).data('quantity');
        var productId = $(this).data('productid');

        var unitPriceString = $(this).data('unitprice') ;
        unitPriceString = unitPriceString.replace(/\,/g, '.') ;
        var unitPrice = parseFloat(unitPriceString) ;
        

        initializeEditModal(productCode, productName, quantity, unitPrice, productId);
        
    });

    $('.delete-btn').on('click', function () {
        var productId = $(this).data('productid');

        var dataToPost = {
            deletedProductId: productId,
        }

        $.ajax({
            url: '/api/v1/product/delete/' + productId,
            type: 'DELETE',
            contentType: 'application/json',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Silindi!',
                    text: 'Ürün başarı ile silindi',
                    timer: 1000, 
                    timerProgressBar: true, 
                    showCloseButton: false, 
                    showConfirmButton: false 
                }).then(() => {
                    window.location.href = '/home/index';
                });
            },
            error: function (response) {
                Swal.fire({
                    icon: 'error',
                    title: 'Silinemedi!',
                    text: response.responseJSON,
                }).then(() => {
                });
            }
        });
    });


    $('#addProductBtn').on('click', function () {
        initializeCreateModal();
    });


    $('#saveChangesBtn').on('click', function () {
        var processEndpoint = '/api/v1/product/update';
        var swalSuccessText = 'Ürün başarıyla güncellendi.';
        var swalErrorText = 'Ürün güncellenirken bir hata oluştu.';

        if ($('#ProductId').val() == '') {
            var formData = {
                ProductCode: $('#ProductCode').val(),
                ProductName: $('#ProductName').val(),
                Quantity: $('#Quantity').val(),
                UnitPrice: $('#UnitPrice').val(),
            };

            processEndpoint = '/api/v1/product/create';
            swalSuccessText = 'Ürün başarıyla oluşturuldu.';
            swalErrorText = 'Ürün oluşturulurken bir hata oluştu.';
        }
        else
        {
            var formData = {
                ProductCode: $('#ProductCode').val(),
                ProductName: $('#ProductName').val(),
                Quantity: $('#Quantity').val(),
                UnitPrice: $('#UnitPrice').val(),
                Id: $('#ProductId').val()
            };
            
        }

        $.ajax({
            url: processEndpoint, 
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Kaydedildi!',
                    text: swalSuccessText,
                    timer: 1000,
                    timerProgressBar: true,
                    showCloseButton: false,
                    showConfirmButton: false 
                }).then(() => {
                    $('#editProductModal').modal('hide');
                    location.reload();
                });
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Kaydedilmedi!',
                    text: swalErrorText,
                }).then(() => {
                    $('#editProductModal').modal('hide');
                    location.reload();
                });
            }
        });
        
    });


    $('#createDataBtn').on('click', function (e) {
        e.preventDefault(); 

        var jsonText = $("#productsJson").val();

        try {
            JSON.parse(jsonText); 
        } catch (e) {
            Swal.fire({
                icon: 'error',
                title: 'Kaydedilmedi!',
                text: 'Verilen girdi json formatında değil',
            }).then(() => {
            }); 
        }

        var dataToPost = {
            updatedProductsJson: jsonText,
        }

        $.ajax({
            url: '/api/v1/product/save-products',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataToPost),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Kaydedildi!',
                    text: 'Ürün başarı ile kaydedildi',
                    timer: 1000,
                    timerProgressBar: true,
                    showCloseButton: false,
                    showConfirmButton: false 
                }).then(() => {
                    window.location.href = '/home/index';
                });
            },
            error: function (response) {
                Swal.fire({
                    icon: 'error',
                    title: 'Kaydedilmedi!',
                    text: response.responseJSON,
                }).then(() => {
                });
            }
        });

    });
    
});

function initializeEditModal(productCode, productName, quantity, unitPrice, productId) {
    $('#editProductModalLabel').text('Ürünü Düzenle');
    $('#ProductCode').val(productCode);
    $('#ProductName').val(productName);
    $('#Quantity').val(quantity);
    $('#UnitPrice').val(unitPrice);
    $('#ProductId').val(productId);
}

function initializeCreateModal() {
    $('#editProductModalLabel').text('Yeni Ürün Ekle');
    $('#editProductForm')[0].reset();
    $('#ProductCode').val('');
    $('#ProductId').val('');
}


