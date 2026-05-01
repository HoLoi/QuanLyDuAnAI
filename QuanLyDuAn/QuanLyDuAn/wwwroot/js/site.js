(function () {
    function normalize(value) {
        return (value || "").toString().trim().toLowerCase();
    }

    function containsAny(source, keywords) {
        var text = normalize(source);
        return keywords.some(function (k) { return text.indexOf(k) >= 0; });
    }

    function getActionPath(form) {
        var action = form.getAttribute("action") || "";
        try {
            var url = new URL(action, window.location.origin);
            return normalize(url.pathname);
        } catch (e) {
            return normalize(action);
        }
    }

    function isDeleteAction(form, submitter) {
        var action = getActionPath(form);
        var submitText = submitter ? (submitter.innerText || submitter.value || "") : "";
        var submitName = submitter ? (submitter.getAttribute("name") || "") : "";
        return containsAny(action, ["xoa", "delete"]) ||
            containsAny(submitText, ["xoa", "xóa", "delete"]) ||
            containsAny(submitName, ["xoa", "delete"]);
    }

    function getActionContext(form, submitter) {
        var action = getActionPath(form);
        var submitText = normalize(submitter ? (submitter.innerText || submitter.value || "") : "");

        if (containsAny(action, ["logout"]) || containsAny(submitText, ["đăng xuất", "dang xuat", "logout"])) {
            return "logout";
        }

        if (containsAny(action, ["khoa", "lock"]) || containsAny(submitText, ["khóa", "khoa", "lock"])) {
            return "lock";
        }

        if (containsAny(action, ["mokhoataikhoan", "unlock", "mokhoa"]) || containsAny(submitText, ["mở khóa", "mo khoa", "unlock"])) {
            return "unlock";
        }

        if (containsAny(action, ["gantruongnhom"]) || containsAny(submitText, ["gán trưởng nhóm", "gan truong nhom"])) {
            return "assignLeader";
        }

        if (containsAny(action, ["duyet"]) || containsAny(submitText, ["duyệt", "duyet"])) {
            return "approve";
        }

        if (isDeleteAction(form, submitter)) {
            return "delete";
        }

        if (containsAny(submitText, ["lưu", "luu", "save", "cập nhật", "cap nhat", "thêm", "them"])) {
            return "save";
        }

        return "default";
    }

    function getConfirmMessages(form, submitter) {
        var customFirst = form.getAttribute("data-confirm-first");
        var customSecond = form.getAttribute("data-confirm-second");
        if (customFirst) {
            return {
                first: customFirst,
                second: customSecond || null
            };
        }

        var context = getActionContext(form, submitter);
        switch (context) {
            case "delete":
                return {
                    first: "Bạn có chắc muốn xóa dữ liệu này?",
                    second: "XÁC NHẬN LẦN 2: Hành động xóa sẽ không thể hoàn tác. Bạn vẫn muốn tiếp tục?"
                };
            case "logout":
                return {
                    first: "Bạn có chắc muốn đăng xuất?",
                    second: null
                };
            case "lock":
                return {
                    first: "Bạn có chắc muốn khóa tài khoản này?",
                    second: null
                };
            case "unlock":
                return {
                    first: "Bạn có chắc muốn mở khóa tài khoản này?",
                    second: null
                };
            case "assignLeader":
                return {
                    first: "Bạn có chắc muốn gán thành viên này làm trưởng nhóm?",
                    second: null
                };
            case "approve":
                return {
                    first: "Bạn có chắc muốn duyệt thao tác này?",
                    second: null
                };
            case "save":
                return {
                    first: "Bạn có chắc muốn lưu thay đổi?",
                    second: null
                };
            default:
                return {
                    first: "Bạn có chắc muốn thực hiện thao tác này?",
                    second: null
                };
        }
    }

    function shouldSkip(form) {
        return form.hasAttribute("data-skip-confirm");
    }

    document.addEventListener("submit", function (event) {
        var form = event.target;
        if (!(form instanceof HTMLFormElement) || shouldSkip(form)) {
            return;
        }

        var method = normalize(form.getAttribute("method") || "get");
        if (method !== "post") {
            return;
        }

        if (window.jQuery && typeof jQuery.fn.valid === "function") {
            var $form = window.jQuery(form);
            if ($form.data("validator") && !$form.valid()) {
                event.preventDefault();
                return;
            }
        }

        if (!form.checkValidity()) {
            event.preventDefault();
            form.reportValidity();
            return;
        }

        var submitter = event.submitter || form.querySelector("button[type='submit'],input[type='submit']");
        var messages = getConfirmMessages(form, submitter);

        var ok = window.confirm(messages.first);
        if (!ok) {
            event.preventDefault();
            return;
        }

        if (messages.second) {
            var okSecond = window.confirm(messages.second);
            if (!okSecond) {
                event.preventDefault();
            }
        }
    }, true);
})();
