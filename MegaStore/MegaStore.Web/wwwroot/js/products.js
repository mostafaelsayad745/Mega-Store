var dtble;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $('#productTable').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
           
            { "data": "name", "autoWidth": true },
            { "data": "description", "autoWidth": false },
            { "data": "price", "autoWidth": true },
            { "data": "category.name", "autoWidth": true },
            {
                "data": "id", "render": function (data) {
                    return`
                    <a href="/Admin/Product/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:70px;">Edit<a/>
                    <a onClick=DeleteItem("Admin/Product/DeleteProduct/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:70px;">Delete<a/>
                    `
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
                type: "Delete",
                url: url,
                success: function (data) {
                    if (data.success) {
                       
                        dtble.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                       
                        toastr.error(data.message);
                    }
                }
            });
            Swal.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        }
    });
}