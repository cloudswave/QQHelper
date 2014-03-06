define(["dojo/_base/lang",
    "dojo/_base/connect",
    "dojo/dom","dojo/dom-class",
    "dijit/registry",
    "dojo/data/ItemFileWriteStore",
    "dojox/mobile/iconUtils",
    "dojox/mobile/RoundRectDataList"],
    function(lang, connect, dom, domClass, registry, ItemFileWriteStore, iconUtils, RoundRectDataList) {

            var static_data = {
                items: [
                    {label: "Apple"},
                    {label: "Banana"},
                    {label: "Cherry"},
                    {label: "Grape"},
                    {label: "Kiwi"}
                ]
            };
            var _store2 = new ItemFileWriteStore({data: lang.clone(static_data)});
            return {
                listsStore : _store2

            };

    });