var cef = null;
var character_choice_menu = null;
var items_list_menu = null;

var hud = {};
hud.money = 0;
hud.bankmoney = 0;

API.onResourceStart.connect(function () {
	// nic
});

API.onResourceStop.connect(function () {
	if (cef != null) {
		cef.destroy();
		cef = null;
	}
});

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == 'lsrp_loginscreen') {
		if (cef != null) {
			cef.destroy();
			cef = null;
		}

		cef = new CefHelper('cef/login/test.html');
		cef.show();

		if (args[0].length > 0) {
			cef.browser.call("showError", args[0]);	// Not working as of now.
			API.sendChatMessage("Bład: " + args[0]);
		}

		API.setCanOpenChat(false);

	} else if (eventName == 'lsrp_hideloginscreen') {
		if (cef != null) {
			cef.destroy();
			cef = null;
		}
		API.setCanOpenChat(true);
	} else if (eventName == 'lsrp_choosecharacter') {
		var chars = JSON.parse(args[0]);

		character_choice_menu = API.createMenu("LS-RP V", "Wybierz postać", 0, 0, 4);

		for (i = 0; i < chars.length; i++) {
			character_choice_menu.AddItem(API.createMenuItem(chars[i].name, "" + chars[i].cid));
		}

		character_choice_menu.OnItemSelect.connect(function (sender, item, index) {
			character_choice_menu.Visible = false;
			character_choice_menu = null;
			API.showCursor(false);
			API.triggerServerEvent("lsrp_characterselection", item.Description);
		});

		API.showCursor(true);
		character_choice_menu.Visible = true;
	} else if (eventName == 'lsrp_show_own_items') {
		var items = JSON.parse(args[0]);

		items_list_menu = API.createMenu("LS-RP V", "Przedmioty", 0, 0, 4);

		for (i = 0; i < items.length; i++) {
			items_list_menu.AddItem(API.createMenuItem(items[i].name, "" + items[i].iid));
		}

		items_list_menu.OnItemSelect.connect(function (sender, item, index) {
			items_list_menu.Visible = false;
			items_list_menu = null;
			API.showCursor(false);
			API.triggerServerEvent("lsrp_use_item", item.Description);
		});

		API.showCursor(true);
		items_list_menu.Visible = true;
	} else if (eventName == 'lsrp_update_hud') {
		hud.money = args[0];
		hud.bankmoney = args[1];
	}
});

API.onUpdate.connect(function () {
	if (character_choice_menu != null) {
		API.drawMenu(character_choice_menu);
	}
	if (items_list_menu != null) {
		API.drawMenu(items_list_menu);
	}

	API.drawText("$" + hud.money, API.getScreenResolutionMantainRatio().Width - 15, 50, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
	API.drawText("$" + hud.bankmoney, API.getScreenResolutionMantainRatio().Width - 15, 110, 0.8, 115, 179, 186, 255, 4, 2, false, true, 0);
});

API.onKeyDown.connect(function (sender, e) {
	if (e.KeyCode == Keys.Escape) {
		items_list_menu.Visible = false;
		items_list_menu = null;
		API.showCursor(false);
	}
});

class CefHelper {
	constructor(resourcePath) {
		this.path = resourcePath
		this.open = false
	}

	show() {
		if (this.open === false) {
			this.open = true;

			var resolution = API.getScreenResolution();

			this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
			API.waitUntilCefBrowserInit(this.browser);
			API.setCefBrowserPosition(this.browser, 0, 0);
			API.loadPageCefBrowser(this.browser, this.path);
			API.showCursor(true);
		}
	}

	destroy() {
		this.open = false;
		API.destroyCefBrowser(this.browser);
		API.showCursor(false);
	}

	eval(string) {
		this.browser.eval(string);
	}
}

function login(username, password) {
	API.triggerServerEvent("lsrp_signin", username, password);
}