var dtble;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $('#orderTable').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetData",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "autoWidth": true },
            { "data": "name", "autoWidth": true },
            { "data": "phone", "autoWidth": true },
            { "data": "applicationUser.email", "autoWidth": true },
            { "data": "orderStatus", "autoWidth": true },
            { "data": "totalPrice", "autoWidth": true },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a href="/Admin/Order/Details?orderid=${data}" class="btn btn-warning text-white" style="cursor:pointer; width:70px;">Details</a>
                    `;
                }
            }
        ]
    });
}

function DeleteItem(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: "Deleted!",
                            text: "Your file has been deleted.",
                            icon: "success"
                        });
                        dtble.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        Swal.fire({
                            title: "Error!",
                            text: "Something went wrong.",
                            icon: "error"
                        });
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
