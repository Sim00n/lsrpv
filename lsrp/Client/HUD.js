var hud = {};
hud.money = 0;
hud.bankmoney = 0;

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == 'lsrp_update_hud') {
		hud.money = args[0];
		hud.bankmoney = args[1];
	}
});

API.onUpdate.connect(function () {
	API.drawText("$" + hud.money, API.getScreenResolutionMantainRatio().Width - 15, 50, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
	API.drawText("$" + hud.bankmoney, API.getScreenResolutionMantainRatio().Width - 15, 110, 0.8, 115, 179, 186, 255, 4, 2, false, true, 0);
});