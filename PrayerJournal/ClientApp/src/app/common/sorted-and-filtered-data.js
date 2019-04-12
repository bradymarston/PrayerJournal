"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var SortedAndFilteredData = /** @class */ (function () {
    function SortedAndFilteredData(data, filterStrings, filterKeys, sortByKeys) {
        if (data === void 0) { data = []; }
        if (filterStrings === void 0) { filterStrings = []; }
        if (filterKeys === void 0) { filterKeys = []; }
        if (sortByKeys === void 0) { sortByKeys = []; }
        this._data = data;
        this._filterStrings = filterStrings;
        this._sortByKeys = sortByKeys;
        this._filterKeys = filterKeys;
    }
    Object.defineProperty(SortedAndFilteredData.prototype, "data", {
        get: function () {
            return this._data;
        },
        set: function (data) {
            this._data = data;
            this.refreshList();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(SortedAndFilteredData.prototype, "filterStrings", {
        set: function (filterStrings) {
            this._filterStrings = filterStrings;
            this.refreshList();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(SortedAndFilteredData.prototype, "filterKeys", {
        set: function (keys) {
            this._filterKeys = keys;
            this.refreshList();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(SortedAndFilteredData.prototype, "sortByKeys", {
        set: function (keys) {
            console.log(keys);
            this._sortByKeys = keys;
            this.refreshList();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(SortedAndFilteredData.prototype, "list", {
        get: function () {
            return this._list;
        },
        enumerable: true,
        configurable: true
    });
    SortedAndFilteredData.prototype.remove = function (item) {
        var dataIndex = this._data.indexOf(item);
        var listIndex = this._list.indexOf(item);
        this._data.splice(dataIndex, 1);
        this._list.splice(listIndex, 1);
        return listIndex;
    };
    SortedAndFilteredData.prototype.add = function (item, listIndex) {
        if (listIndex === void 0) { listIndex = -1; }
        this._data.push(item);
        if (listIndex === -1 || listIndex > this._list.length)
            this.refreshList();
        else
            this._list.splice(listIndex, 0, item);
    };
    SortedAndFilteredData.prototype.refreshList = function () {
        var tempList = [];
        tempList = this.doFilter();
        this._list = this.doSort(tempList);
    };
    SortedAndFilteredData.prototype.doFilter = function () {
        var _this = this;
        var list = this._data.slice(0);
        var _loop_1 = function (filterText) {
            list = list.filter(function (item) {
                if (!filterText)
                    return true;
                for (var _i = 0, _a = _this._filterKeys; _i < _a.length; _i++) {
                    var filterKey = _a[_i];
                    if (_this.stringContains(item[filterKey], filterText))
                        return true;
                }
                return false;
            });
        };
        for (var _i = 0, _a = this._filterStrings; _i < _a.length; _i++) {
            var filterText = _a[_i];
            _loop_1(filterText);
        }
        return list;
    };
    SortedAndFilteredData.prototype.doSort = function (list) {
        var _this = this;
        if (this._sortByKeys.length > 0)
            list = list.sort(function (itemA, itemB) {
                var i = 0;
                var returnValue = 0;
                do {
                    var key = _this._sortByKeys[i];
                    var aString = (itemA[key]) ? itemA[key] : "";
                    var bString = (itemB[key]) ? itemB[key] : "";
                    if (aString > bString)
                        returnValue = 1;
                    if (aString < bString)
                        returnValue = -1;
                    i++;
                } while (returnValue === 0 && i < _this._sortByKeys.length);
                return returnValue;
            });
        return list;
    };
    SortedAndFilteredData.prototype.stringContains = function (text, search) {
        if (text && search)
            return text.toUpperCase().indexOf(search.toUpperCase()) !== -1;
        return false;
    };
    return SortedAndFilteredData;
}());
exports.SortedAndFilteredData = SortedAndFilteredData;
//# sourceMappingURL=sorted-and-filtered-data.js.map