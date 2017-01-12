function signinfun() {
	var username = $("#login").val();
	var password = $("#password").val();

	resourceCall("login", username, password);
}

function showError(error) {
	$(function () {
		$("#message").html('a' + error);
	});
}