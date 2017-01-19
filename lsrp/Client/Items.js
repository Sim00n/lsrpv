var items_list_menu = null;
var items_list_to_drop_menu = null;
var items_list_nearby = null;

API.onResourceStart.connect(function () {
	// nic
});

API.onResourceStop.connect(function () {
	// nic
});

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == 'lsrp_show_own_items') {
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
	} else if (eventName == 'lsrp_show_own_items_to_drop') {
		var items = JSON.parse(args[0]);

		items_list_to_drop_menu = API.createMenu("LS-RP V", "Przedmiot do odłożenia", 0, 0, 4);

		for (i = 0; i < items.length; i++) {
			items_list_to_drop_menu.AddItem(API.createMenuItem(items[i].name, "" + items[i].iid));
		}

		items_list_to_drop_menu.OnItemSelect.connect(function (sender, item, index) {
			items_list_to_drop_menu.Visible = false;
			items_list_to_drop_menu = null;
			API.showCursor(false);
			API.triggerServerEvent("lsrp_drop_item", item.Description);
		});

		API.showCursor(true);
		items_list_to_drop_menu.Visible = true;
	} else if (eventName == 'lsrp_show_nearby_items') {
		var items = JSON.parse(args[0]);

		items_list_nearby = API.createMenu("LS-RP V", "Przedmioty w pobliżu", 0, 0, 4);

		for (i = 0; i < items.length; i++) {
			items_list_nearby.AddItem(API.createMenuItem(items[i].name, "" + items[i].iid));
		}

		items_list_nearby.OnItemSelect.connect(function (sender, item, index) {
			items_list_nearby.Visible = false;
			items_list_nearby = null;
			API.showCursor(false);
			API.triggerServerEvent("lsrp_pickup_item", item.Description);
		});

		API.showCursor(true);
		items_list_nearby.Visible = true;

	}
});

API.onUpdate.connect(function () {
	if (items_list_menu != null) {
		API.drawMenu(items_list_menu);
	}
	if (items_list_to_drop_menu != null) {
		API.drawMenu(items_list_to_drop_menu);
	}
	if (items_list_nearby != null) {
		API.drawMenu(items_list_nearby);
	}
});

API.onKeyDown.connect(function (sender, e) {
	if (e.KeyCode == Keys.Escape) {
		if (items_list_menu != null) {
			items_list_menu.Visible = false;
			items_list_menu = null;
			API.showCursor(false);
		}
		if (items_list_to_drop_menu != null) {
			items_list_to_drop_menu.Visible = false;
			items_list_to_drop_menu = null;
			API.showCursor(false);
		}
		if (items_list_nearby != null) {
			items_list_nearby.Visible = false;
			items_list_nearby = null;
			API.showCursor(false);
		}
	}
});