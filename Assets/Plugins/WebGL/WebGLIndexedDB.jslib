mergeInto(LibraryManager.library, {
    SaveData: function (key, value) {

        let keyStr = UTF8ToString(key);
        let valueStr = UTF8ToString(value);

        let request = indexedDB.open("idlecolors", 1);

        request.onupgradeneeded = function (event) {
            console.log("onupgradeneeded ... ");
            let db = event.target.result;
            if (!db.objectStoreNames.contains("table")) {
                db.createObjectStore("table");
            }
        };

        request.onsuccess = function (event) {
            let db = event.target.result;

            if (!db.objectStoreNames.contains("table")) {
                console.log("DB does not contain table with name:table");
                return;
            }

            let transaction = db.transaction("table", "readwrite");
            let store = transaction.objectStore("table");
            store.put(valueStr, keyStr);
        };

        request.onerror = function (event) {
            console.error("IndexedDB Fehler:", event.target.error);
        };
    },

    LoadData: function (key) {
        let keyStr = UTF8ToString(key);
        let request = indexedDB.open("idlecolors", 1);

        request.onsuccess = function (event) {
            let db = event.target.result;

            if (!db.objectStoreNames.contains("table")) {
                console.error("data never saved before ... returning with empty string to reset the gamedata in Gamemanager");
                if (typeof unityInstance !== "undefined" && unityInstance !== null) {
                    unityInstance.SendMessage('Gamemanager', "HandleGameData", "");
                } else {
                    console.error("Unity instance not found. Retrying in 500ms...");
                    setTimeout(() => {
                        if (typeof unityInstance !== "undefined" && unityInstance !== null) {
                            unityInstance.SendMessage('Gamemanager', "HandleGameData", "");
                        } else {
                            console.error("Unity instance is still not available.");
                        }
                    }, 500);
                }

                indexedDB.deleteDatabase("idlecolors").onsuccess = function () {
                    console.log("Database deleted");
                    // location.reload(); // Refresh to reopen and trigger upgrade
                };
                return;
            }

            let transaction = db.transaction("table", "readonly");
            let store = transaction.objectStore("table");
            let getRequest = store.get(keyStr);

            getRequest.onsuccess = function () {
                let result = getRequest.result ? getRequest.result : "";

                if (typeof unityInstance !== "undefined" && unityInstance !== null) {
                    unityInstance.SendMessage('Gamemanager', "HandleGameData", result);
                } else {
                    console.error("Unity instance not found. Retrying in 500ms...");
                    setTimeout(() => {
                        if (typeof unityInstance !== "undefined" && unityInstance !== null) {
                            unityInstance.SendMessage('Gamemanager', "HandleGameData", result);
                        } else {
                            console.error("Unity instance is still not available.");
                        }
                    }, 500);
                }
            };
        };

        request.onerror = function (event) {
            console.error("IndexedDB Ladefehler:", event.target.error);
        };
    }
});
