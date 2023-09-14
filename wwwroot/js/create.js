$(function () {
    $("#example1").DataTable({
        "responsive": true,
        "lengthChange": true,
        "autoWidth": true,
        "info": true,
        "ordering": true,
        "searching": true,
        "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
    }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');
    $('#example2').DataTable({
        "paging": true,
        "lengthChange": false,
        "searching": false,
        "ordering": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
    });
});


document.getElementById("Input_UserName").addEventListener("keyup", async () => {
    const username = document.getElementById("Input_UserName").value;
    const response = await fetch(`https://localhost:5141/UserApi/username?username=${username}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json-patch+json",
        },
    });
    const isValid = await response.json();
    if (isValid.includes("Error")) {
        document.getElementById("usernameOutput").classList = "text-danger";
        document.getElementById("usernameIcon").classList = "fa-solid fa-circle-xmark text-danger mr-1"
    } else {
        document.getElementById("usernameOutput").classList = "text-success";
        document.getElementById("usernameIcon").classList = "fa-solid fa-circle-check text-success mr-1"
    }
    document.getElementById("usernameOutput").innerText = isValid;
})