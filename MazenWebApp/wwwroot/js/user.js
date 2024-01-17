var dataTable;

$(document).ready(function () {
    populateUsersTable();
});

function populateUsersTable() {

    dataTable = $('#tblData').DataTable(
        {
            ajax: {
                url: '/Admin/User/GetAll'
            },
            columns: [
                {
                    data: "name",
                    width: "10%"
                },
                {
                    data: "email",
                    width: "10%"
                },
                {
                    data: "phoneNumber",
                    width: "10%"
                },
                {
                    data: "company.name",
                    width: "10%"
                },
                {
                    data: "role",
                    width: "10%"
                },
                {
                    data: {
                        id: "id",
                        lockoutEnd: "lockoutEnd"
                    },
                    render: function (data) {

                        let today = new Date().getTime();
                        let lockout = new Date(data.lockoutEnd).getTime();
                        let returnHtml = "";

                        if (lockout > today) {
                            returnHtml =
                                `<div class="text-center">
                                    <a onclick="lockUnlock('${ data.id }')" class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                        <i class="bi bi-lock-fill"></i> Lock
                                    </a>
                                    <a href="/Admin/User/RoleManagement?userId=${ data.id }" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                        <i class="bi bi-pencil-square"></i> Permission
                                    </a>
                                </div>`;
                        }
                        else {
                            returnHtml =
                                `<div class="text-center">
                                    <a onclick="lockUnlock('${ data.id }')" class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                        <i class="bi bi-unlock-fill"></i> Unlock
                                    </a>
                                    <a href="/Admin/User/RoleManagement?userId=${ data.id }" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                        <i class="bi bi-pencil-square"></i> Permission
                                    </a>
                                </div>`;
                        }

                        return returnHtml;
                    }
                },
            ]
        }
    );
}

function lockUnlock(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/User/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}