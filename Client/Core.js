var mainBrowser = null;
var vehicles = null;
var debug = false;
var cef_enabled = true;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "LOGIN_PASSWORD_OK") {
        
        API.setActiveCamera(null);
        API.showCursor(false);
        API.setCanOpenChat(true);
        API.setHudVisible(true);
        API.destroyCefBrowser(mainBrowser);
        mainBrowser = null;
    }
    else if (eventName == "LOGIN_INCORRECT") {
        mainBrowser.call("onLoginError", args[0] + "", false);
    }
    else if (eventName == "SHOW_LOGIN_SCREEN") {
        // cef_enabled = args[0];
        API.sleep(2);
        LoginScreen();
    }
    else if (eventName == "SELECT_SKIN") {
        //API.sendChatMessage("Podane has³o lub login jest niepoprawne");
        //API.
        API.sleep(2);
        skinCamera();
    }
    else if (eventName == "vehicleList") {
        //mainBrowser.showElements(args[0]);
        if (mainBrowser == null)
        {
            API.sendChatMessage("MainBrowser == null");
        }
        mainBrowser.call("show", "test");
        API.sendChatMessage(args[0]);
    }
    else if (eventName == "VEHICLES_SHOW") {
        API.sleep(2);
        showVehicles();
    }
    

});

function LoginScreen() {
    LoginCamera();
    if (cef_enabled == true) {
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.setHudVisible(false);
        LoadPage("Client/Login/login.html");
    }
    API.triggerServerEvent("loginShow");
}

function LoadPage(targetPage) {
    if (mainBrowser == null) {
        var resolution = API.getScreenResolution();

        mainBrowser = API.createCefBrowser(resolution.Width, resolution.Height);
        API.waitUntilCefBrowserInit(mainBrowser);
        API.sleep(1);

    }

    API.loadPageCefBrowser(mainBrowser, targetPage);
    API.setCefBrowserHeadless(mainBrowser, false);
}

function LoginCamera() {
    //var main_camera = API.createCamera(new Vector3(-396.6894, 6370.951, 201.492), new Vector3(0.00, 0.00, 0.00));
    var destination_camera = API.createCamera(new Vector3(-208.9449, 6372.965, 63.49265), new Vector3());




    var newCam = API.createCamera(new Vector3(-396.6894, 6370.951, 201.492), new Vector3());
    API.pointCameraAtPosition(newCam, new Vector3(-208.9449, 6372.965, 63.49265));
    API.setActiveCamera(newCam);
    API.interpolateCameras(newCam, destination_camera, 60000, true, true);
}

function skinCamera() {
    var newCam = API.createCamera(new Vector3(122.6022, 545.6385, 180.4973), new Vector3());
    API.pointCameraAtPosition(newCam, new Vector3(122.0416, 548.738, 180.4973));
    API.setActiveCamera(newCam);
}

///[00:37:22] X: 122.0416 Y: 548.738 Z: 180.4973
///[00:37:31] X: 122.6022 Y: 545.6385 Z: 180.4973

function Login(username, password) {
    if (debug) API.sendChatMessage("Sprawdzam u¿ytkownika '" + username + "'");
    // Do some magic
    // Hide the browser while we process the data

    API.triggerServerEvent("doLogin", username, password);
}
function showVehicles() {
    if (cef_enabled == true) {
        API.showCursor(true);
        LoadPage("Client/Vehicle/vehiclelist.html");
    }
    API.triggerServerEvent("showVehicles");
}
