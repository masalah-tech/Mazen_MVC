var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable(
        {
            ajax: {
                url: '/Admin/Product/GetAll'
            }
            ,
            columns: [
                {
                    data: 'title',
                    width: '25%'
                },
                {
                    data: 'isbn',
                    width: '15%'
                },
                {
                    data: 'listPrice',
                    width: '10%'
                },
                {
                    data: 'author',
                    width: '15%'
                },
                {
                    data: 'category.name',
                    width: '10%'
                },
                {
                    data: 'id',
                    render: function (data) {
                        return `<div class="btn-group w-75" role="group">
                            <a href="/Admin/Product/Upsert/${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a onClick='deleteItem("/Admin/Product/Delete/${data}")' class="btn btn-danger mx-2">
                                <i class="bi bi-trash-fill"></i> Delete
                            </a>
                        </div>`;
                    },
                    width: '25%'
                },

            ]
        });
}

function deleteItem (url) {
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
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            });
        }
    });
}