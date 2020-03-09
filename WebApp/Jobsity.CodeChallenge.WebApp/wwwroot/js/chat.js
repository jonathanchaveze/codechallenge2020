"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    // Enable send button
    document.getElementById("sendButton").disabled = false;
    connection.invoke("UserJoined").catch(function (err) {
        return console.error(err.toString());
    });

}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("formMessage").addEventListener("submit", function (event) {
    event.preventDefault();
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    $("#error").hide();
    var message = document.getElementById("message").value;
    document.getElementById("message").value = "";
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("ReceiveMessage", function (data) {
    var message = JSON.parse(data);
    var template = '<li class="media my-4"><img src="' + message.profilePic + '?s=50" class="mr-3 rounded-circle" alt="Profile picture for ' + message.fullName + '"><div class="media-body"><h6 class="mt-0 mb-1">' + message.fullName + ' <small> - Posted on: ' + message.publishedOn + '</small></h6>' + message.message + '</div></li>';

    $("#messagesList").append(template);
    $("#room").scrollTop($("#room")[0].scrollHeight);
});

connection.on("Error", function (data) {
    $("#error").html(data);
    $("#error").show();
});