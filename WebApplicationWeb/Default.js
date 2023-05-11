
    function newUser()
    {
        $.ajax({
            type: "POST", //POST
            url: "Default.aspx/AddUser",
            data: JSON.stringify({
                username: $('#username').val(),
                age: $('#age').val(),
                birthday: $('#birthday').val()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function () {
                alert("SUCCESS");
            },
            failure: function () {
                alert("ERROR");
            }
        });
}
