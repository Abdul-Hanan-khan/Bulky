//$(document).ready(function () {
//    $('#tblData').DataTable(); // Basic initialization
//});

//const { ajax } = require("jquery");



$(document).ready(function () {
    $('#tblData').DataTable({
        ajax: {
            url: '/Admin/Company/GetAll',
            type: 'GET'
        },
        columns: [
            { data: 'name', width: "10%" },
            { data: 'streetAddress', width: "25%" },
            { data: 'city', width: "10%" },

            { data: 'state', width: "10%" },

            { data: 'phoneNumber', width: "15%" },
            {
                data: 'id',
                render: function (data) {
                    return `<div class="btn-group text-center" role="group">
                    <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit </a>
                    <a onClick=deleteProduct('/admin/company/delete?id=${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> delete </a>
                    </div>`
                },
                width: "15%",
              
            }

        ]
    });
});


function deleteProduct(url) {
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
                url: url,
                type: 'DELETE',
                success: function (data) {
           
                    toastr.success(data.message);
                    $('#tblData').DataTable().ajax.reload(); 
                },
                error: function (xhr, status, error) {
                    toastr.error("An error occurred while deleting the data.");
                }
            })


            //Swal.fire({
            //    title: "Deleted!",
            //    text: "Your file has been deleted.",
            //    icon: "success"
            //});
        }
    });
}
