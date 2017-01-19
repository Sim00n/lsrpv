var items_list_menu = null;
var items_list_use_menu = null;
var items_list_nearby = null;

var current_item_list = chosen_name = chosen_iid = null;
var current_nearby_items_list = null;

API.onResourceStart.connect(function () {
	// nic
});

API.onResourceStop.connect(function () {
	// nic
});

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == 'lsrp_show_own_items') {
		showItemList(args);
	} else if (eventName == 'lsrp_show_nearby_items') {
		listItemsNearby(args);
	}
});

API.onUpdate.connect(function () {
	if (items_list_menu != null) {
		API.drawMenu(items_list_menu);
	}
	if (items_list_use_menu != null) {
		API.drawMenu(items_list_use_menu);
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
		if (items_list_use_menu != null) {
			items_list_use_menu.Visible = false;
			items_list_use_menu = null;
			API.showCursor(false);
		}
		if (items_list_nearby != null) {
			items_list_nearby.Visible = false;
			items_list_nearby = null;
			API.showCursor(false);
		}

		current_item_list = null;
	}
});

/**
 * List items in player's inventory.
 */
function showItemList(args) {
	var items = current_item_list = JSON.parse(args[0]);

	items_list_menu = API.createMenu("LS-RP V", "Przedmioty", 0, 0, 4);

	items_list_menu.AddItem(API.createMenuItem("~o~-- Przedmioty w pobliżu --", "Pokaż przedmioty na ziemi"));
	for (i = 0; i < items.length; i++) {
		items_list_menu.AddItem(API.createMenuItem("~w~" + items[i].name, items[i].name));
	}

	items_list_menu.OnItemSelect.connect(function (sender, item, index) {
		items_list_menu.Visible = false;
		items_list_menu = null;
		API.showCursor(false);

		if (index == 0) {
			// We'll call use because that will call us back with a nearby list.
			API.triggerServerEvent("lsrp_use_item", -1);
			return;
		}

		// Add one because we have the nearby items button in the first slot.
		index -= 1;

		chosen_name = current_item_list[index].name;
		chosen_iid = current_item_list[index].iid;

		listItemUseOptions(item, chosen_name, chosen_iid);
	});

	API.showCursor(true);
	items_list_menu.Visible = true;
}

/**
 * Show options to act upon a chosen item.
 */
function listItemUseOptions(menu_element, item_name, item_iid) {
	items_list_use_menu = API.createMenu(item_name, "Wybierz akcję", 0, 0, 4);

	items_list_use_menu.AddItem(API.createMenuItem("~g~Użyj", "Przedmiot zostanie użyty"));
	items_list_use_menu.AddItem(API.createMenuItem("~r~Odłóż", "Przedmiot zostanie odłożony"));
	
	items_list_use_menu.OnItemSelect.connect(function (sender, item, index) {
		items_list_use_menu.Visible = false;
		items_list_use_menu = null;
		API.showCursor(false);

		if (index == 0) {
			API.triggerServerEvent("lsrp_use_item", chosen_iid);
		} else if (index == 1) {
			API.triggerServerEvent("lsrp_drop_item", chosen_iid);
		}
	});

	API.showCursor(true);
	items_list_use_menu.Visible = true;
}


/**
 * List items nearby the player.
 */
function listItemsNearby(args) {
	var items = current_nearby_items_list = JSON.parse(args[0]);

	items_list_nearby = API.createMenu("LS-RP V", "Przedmioty w pobliżu", 0, 0, 4);

	for (i = 0; i < items.length; i++) {
		items_list_nearby.AddItem(API.createMenuItem(items[i].name, items[i].name));
	}

	items_list_nearby.OnItemSelect.connect(function (sender, item, index) {
		items_list_nearby.Visible = false;
		items_list_nearby = null;
		API.showCursor(false);

		var iid = current_nearby_items_list[index].iid;
		API.triggerServerEvent("lsrp_pickup_item", iid);
	});

	API.showCursor(true);
	items_list_nearby.Visible = true;
}