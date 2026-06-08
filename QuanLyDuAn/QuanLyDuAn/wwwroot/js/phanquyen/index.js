(function () {
    const form = document.getElementById("permission-form");
    if (!form) {
        return;
    }

    const checkboxes = Array.from(form.querySelectorAll(".permission-checkbox"));
    const checkboxByPermission = new Map();
    const alertBox = document.getElementById("permission-client-alert");
    const searchInput = document.getElementById("permission-search");
    const emptySearch = document.getElementById("permission-empty-search");

    checkboxes.forEach(function (checkbox) {
        checkboxByPermission.set(checkbox.dataset.permission, checkbox);
    });

    function isRoleLocked(checkbox) {
        return checkbox.dataset.deniedByRole === "true" || checkbox.dataset.requiredByRole === "true";
    }

    function showAlert(messages) {
        if (!alertBox) {
            return;
        }

        if (!messages.length) {
            alertBox.classList.add("d-none");
            alertBox.innerHTML = "";
            return;
        }

        const items = messages.map(function (message) {
            return "<li>" + message + "</li>";
        }).join("");
        alertBox.innerHTML = "<strong>Không thể lưu phân quyền.</strong><ul>" + items + "</ul>";
        alertBox.classList.remove("d-none");
        alertBox.scrollIntoView({ behavior: "smooth", block: "start" });
    }

    function syncDependencyState() {
        checkboxes.forEach(function (checkbox) {
            const parentPermission = checkbox.dataset.parentPermission;
            const pill = checkbox.closest("[data-permission-pill]");

            if (!parentPermission || isRoleLocked(checkbox)) {
                if (pill) {
                    pill.classList.remove("dependency-invalid");
                }
                return;
            }

            const parent = checkboxByPermission.get(parentPermission);
            const parentChecked = parent && parent.checked;
            checkbox.disabled = !parentChecked;

            if (!parentChecked) {
                checkbox.checked = false;
                if (pill) {
                    pill.classList.add("dependency-invalid");
                }
            } else if (pill) {
                pill.classList.remove("dependency-invalid");
            }
        });
    }

    function selectCheckbox(checkbox) {
        if (!checkbox || checkbox.disabled || isRoleLocked(checkbox)) {
            return;
        }

        const parentPermission = checkbox.dataset.parentPermission;
        if (parentPermission) {
            const parent = checkboxByPermission.get(parentPermission);
            if (parent && !parent.checked && !parent.disabled && !isRoleLocked(parent)) {
                parent.checked = true;
            }
        }

        checkbox.checked = true;
    }

    function clearChildren(parentCheckbox) {
        const parentPermission = parentCheckbox.dataset.permission;
        checkboxes.forEach(function (checkbox) {
            if (checkbox.dataset.parentPermission === parentPermission && !isRoleLocked(checkbox)) {
                checkbox.checked = false;
            }
        });
    }

    checkboxes.forEach(function (checkbox) {
        checkbox.addEventListener("change", function () {
            if (checkbox.checked) {
                const parentPermission = checkbox.dataset.parentPermission;
                if (parentPermission) {
                    const parent = checkboxByPermission.get(parentPermission);
                    if (parent && !parent.checked && !parent.disabled && !isRoleLocked(parent)) {
                        parent.checked = true;
                    }
                }
            } else {
                clearChildren(checkbox);
            }

            syncDependencyState();
            showAlert([]);
        });
    });

    const selectAllButton = document.getElementById("select-all");
    if (selectAllButton) {
        selectAllButton.addEventListener("click", function () {
            checkboxes
                .filter(function (checkbox) { return !checkbox.dataset.parentPermission; })
                .forEach(selectCheckbox);
            syncDependencyState();
            checkboxes
                .filter(function (checkbox) { return checkbox.dataset.parentPermission; })
                .forEach(selectCheckbox);
            syncDependencyState();
        });
    }

    const clearAllButton = document.getElementById("clear-all");
    if (clearAllButton) {
        clearAllButton.addEventListener("click", function () {
            checkboxes.forEach(function (checkbox) {
                if (!isRoleLocked(checkbox)) {
                    checkbox.checked = false;
                }
            });
            syncDependencyState();
        });
    }

    form.querySelectorAll("[data-select-group]").forEach(function (button) {
        button.addEventListener("click", function () {
            const groupKey = button.dataset.selectGroup;
            const groupCheckboxes = checkboxes.filter(function (checkbox) {
                return checkbox.dataset.group === groupKey;
            });

            groupCheckboxes
                .filter(function (checkbox) { return !checkbox.dataset.parentPermission; })
                .forEach(selectCheckbox);
            syncDependencyState();
            groupCheckboxes
                .filter(function (checkbox) { return checkbox.dataset.parentPermission; })
                .forEach(selectCheckbox);
            syncDependencyState();
        });
    });

    form.querySelectorAll("[data-clear-group]").forEach(function (button) {
        button.addEventListener("click", function () {
            const groupKey = button.dataset.clearGroup;
            checkboxes.forEach(function (checkbox) {
                if (checkbox.dataset.group === groupKey && !isRoleLocked(checkbox)) {
                    checkbox.checked = false;
                }
            });
            syncDependencyState();
        });
    });

    if (searchInput) {
        searchInput.addEventListener("input", function () {
            const query = searchInput.value.trim().toLocaleLowerCase("vi");
            const cards = Array.from(form.querySelectorAll("[data-screen-card]"));
            let visibleCount = 0;

            cards.forEach(function (card) {
                const text = (card.dataset.searchText || "").toLocaleLowerCase("vi");
                const visible = !query || text.includes(query);
                card.classList.toggle("d-none", !visible);
                if (visible) {
                    visibleCount += 1;
                }
            });

            form.querySelectorAll(".permission-group").forEach(function (group) {
                const hasVisibleCard = Array.from(group.querySelectorAll("[data-screen-card]"))
                    .some(function (card) { return !card.classList.contains("d-none"); });
                group.classList.toggle("d-none", !hasVisibleCard);
            });

            if (emptySearch) {
                emptySearch.classList.toggle("d-none", visibleCount > 0);
            }
        });
    }

    form.addEventListener("submit", function (event) {
        const messages = [];

        checkboxes.forEach(function (checkbox) {
            const displayName = checkbox.dataset.displayName || checkbox.dataset.permission;

            if (checkbox.dataset.deniedByRole === "true" && checkbox.checked) {
                messages.push("Role hiện tại không được giữ quyền " + displayName + ".");
            }

            const parentPermission = checkbox.dataset.parentPermission;
            if (checkbox.checked && parentPermission) {
                const parent = checkboxByPermission.get(parentPermission);
                if (!parent || !parent.checked) {
                    messages.push("Quyền " + displayName + " cần quyền Xem.");
                }
            }

            if (checkbox.dataset.requiredByRole === "true" && !checkbox.checked) {
                messages.push("Role hiện tại bắt buộc giữ quyền " + displayName + ".");
            }
        });

        if (messages.length) {
            event.preventDefault();
            showAlert(Array.from(new Set(messages)));
        }
    });

    syncDependencyState();
})();
