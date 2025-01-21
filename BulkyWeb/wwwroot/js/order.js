//$(document).ready(function () {
//    $('#tblData').DataTable(); // Basic initialization
//});

//const { ajax } = require("jquery");



$(document).ready(function () {
    var orderStatus = "all";
    var url = window.location.search;
    if (url.includes("inprocess")) {
        orderStatus = "inprocess";
    } else if (url.includes("completed")) {
        orderStatus = "completed";
    } else if (url.includes("pending")) {
        orderStatus = "pending";
    } else if (url.includes("approved")) {
        orderStatus = "approved";
    } else {
        orderStatus = "all"
    }
    $('#tblData').DataTable({
        ajax: {
            url: '/Admin/Order/GetAll?status=' + orderStatus,
            type: 'GET'
        },
        columns: [
            {
                data: 'id', width: "5%", className: "text-center" },
            { data: 'name', width: "15%" },
            { data: 'phoneNumber', width: "15%", className: "text-start" },

            { data: 'applicationUser.email', width: "15%" },

            { data: 'orderStatus', width: "10%" },
            { data: 'orderTotal', width: "5%",  className:'text-center' },
            { data: 'paymentStatus', width: "15%" },

            {
                data: 'id',
                render: function (data) {
                    return `<div class="btn-group text-center" role="group">
                    <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                width: "3%",
              
            }

        ]
    });
});
