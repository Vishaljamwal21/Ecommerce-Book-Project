﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/OrderManager/GetAll"
        },
        "columns": [
            { "data": "id", "width": "15%" },
            { "data": "orderDate", "width": "15%" },
            { "data": "name", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div> 
                     <a href="/Admin/OrderManager/Viewdata/${data}" class="btn btn-info">
                     <i class="fas fa-edit"></i>
                     </a>
                     </div>
                    `;

                }
            }
        ]
    })
}

function Delete(url) {
    swal({
        title: "Want to delete data ?",
        text: "Delete Information",
        buttons: true,
        icon: "warning",
        dangermodel: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}


