var WebStorage = {
    SetLocalStorage: function (key, value) {
        localStorage.setItem(UTF8ToString(key), UTF8ToString(value));
    },

    GetLocalStorage: function (key) {
        var value = localStorage.getItem(UTF8ToString(key));
        if (value === null) return 0;

        var lengthBytes = lengthBytesUTF8(value) + 1;
        var stringOnWasmHeap = _malloc(lengthBytes);
        stringToUTF8(value, stringOnWasmHeap, lengthBytes);

        return stringOnWasmHeap;
    }
};

mergeInto(LibraryManager.library, WebStorage);
