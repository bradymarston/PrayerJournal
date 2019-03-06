"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var PasswordMatchErrorMatcher = /** @class */ (function () {
    function PasswordMatchErrorMatcher() {
    }
    PasswordMatchErrorMatcher.prototype.isErrorState = function (control, form) {
        return control.dirty;
    };
    return PasswordMatchErrorMatcher;
}());
exports.PasswordMatchErrorMatcher = PasswordMatchErrorMatcher;
//# sourceMappingURL=PasswordMatchErrorMatcher.js.map