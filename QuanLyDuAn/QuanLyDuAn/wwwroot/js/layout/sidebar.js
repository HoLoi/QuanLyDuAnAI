(function () {
    var storageKey = "QuanLyDuAnAI.sidebar.collapsed";
    var desktopQuery = window.matchMedia("(min-width: 992px)");

    var collapseToggle = document.querySelector("[data-sidebar-collapse-toggle]");
    var mobileToggleButtons = document.querySelectorAll("[data-sidebar-toggle]");
    var dismissButtons = document.querySelectorAll("[data-sidebar-dismiss]");
    var menuLinks = document.querySelectorAll(".sidebar-menu-link");
    var floatingTooltip = null;

    if (document.body.dataset.sidebarInitialized === "true") {
        return;
    }

    document.body.dataset.sidebarInitialized = "true";

    function readStoredCollapsed() {
        try {
            return window.localStorage.getItem(storageKey) === "true";
        } catch (error) {
            return false;
        }
    }

    function writeStoredCollapsed(isCollapsed) {
        try {
            window.localStorage.setItem(storageKey, isCollapsed ? "true" : "false");
        } catch (error) {
            return;
        }
    }

    function getLinkLabel(link) {
        var text = link.querySelector(".sidebar-menu-text");
        return text ? text.textContent.trim() : "";
    }

    function setMobileSidebarState(isOpen) {
        document.body.classList.toggle("sidebar-open", isOpen);
        mobileToggleButtons.forEach(function (button) {
            button.setAttribute("aria-expanded", isOpen ? "true" : "false");
        });
    }

    function setMenuLabels(isCollapsed) {
        menuLinks.forEach(function (link) {
            var label = link.getAttribute("data-sidebar-label") || getLinkLabel(link);
            if (!label) {
                return;
            }

            link.setAttribute("data-sidebar-label", label);
            link.setAttribute("aria-label", label);

            if (isCollapsed) {
                link.setAttribute("title", label);
            } else if (link.getAttribute("title") === label) {
                link.removeAttribute("title");
            }
        });
    }

    function ensureFloatingTooltip() {
        if (floatingTooltip) {
            return floatingTooltip;
        }

        floatingTooltip = document.createElement("div");
        floatingTooltip.className = "sidebar-floating-tooltip";
        floatingTooltip.setAttribute("role", "tooltip");
        document.body.appendChild(floatingTooltip);
        return floatingTooltip;
    }

    function hideFloatingTooltip() {
        if (floatingTooltip) {
            floatingTooltip.classList.remove("show");
        }
    }

    function showFloatingTooltip(link) {
        if (!desktopQuery.matches || !document.body.classList.contains("sidebar-collapsed")) {
            hideFloatingTooltip();
            return;
        }

        var label = link.getAttribute("data-sidebar-label") || getLinkLabel(link);
        if (!label) {
            hideFloatingTooltip();
            return;
        }

        var tooltip = ensureFloatingTooltip();
        var rect = link.getBoundingClientRect();
        tooltip.textContent = label;
        tooltip.style.left = Math.round(rect.right + 10) + "px";
        tooltip.style.top = Math.round(rect.top + (rect.height / 2)) + "px";
        tooltip.style.transform = "translateY(-50%) translateX(-0.25rem)";
        tooltip.classList.add("show");
        window.requestAnimationFrame(function () {
            tooltip.style.transform = "translateY(-50%) translateX(0)";
        });
    }

    function updateCollapseToggle(isCollapsed) {
        if (!collapseToggle) {
            return;
        }

        var label = isCollapsed ? "Mở rộng menu" : "Thu gọn menu";
        var iconClass = isCollapsed ? "bi bi-chevron-double-right" : "bi bi-chevron-double-left";
        var icon = collapseToggle.querySelector("i");
        var text = collapseToggle.querySelector(".sidebar-collapse-text");

        collapseToggle.setAttribute("aria-label", label);
        collapseToggle.setAttribute("title", label);
        collapseToggle.setAttribute("aria-expanded", isCollapsed ? "false" : "true");

        if (icon) {
            icon.className = iconClass;
            icon.setAttribute("aria-hidden", "true");
        }

        if (text) {
            text.textContent = label;
        }
    }

    function applyDesktopState(isCollapsed, shouldPersist) {
        var canCollapse = desktopQuery.matches;
        document.body.classList.toggle("sidebar-collapsed", canCollapse && isCollapsed);
        updateCollapseToggle(canCollapse && isCollapsed);
        setMenuLabels(canCollapse && isCollapsed);

        if (!canCollapse || !isCollapsed) {
            hideFloatingTooltip();
        }

        if (shouldPersist) {
            writeStoredCollapsed(isCollapsed);
        }
    }

    if (collapseToggle) {
        collapseToggle.addEventListener("click", function () {
            var nextCollapsed = !document.body.classList.contains("sidebar-collapsed");
            applyDesktopState(nextCollapsed, true);
        });
    }

    mobileToggleButtons.forEach(function (button) {
        button.setAttribute("aria-controls", "appSidebar");
        button.setAttribute("aria-expanded", "false");
        button.addEventListener("click", function () {
            setMobileSidebarState(!document.body.classList.contains("sidebar-open"));
        });
    });

    dismissButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            setMobileSidebarState(false);
        });
    });

    menuLinks.forEach(function (link) {
        link.addEventListener("click", function () {
            if (!desktopQuery.matches) {
                setMobileSidebarState(false);
            }
        });
        link.addEventListener("mouseenter", function () {
            showFloatingTooltip(link);
        });
        link.addEventListener("focus", function () {
            showFloatingTooltip(link);
        });
        link.addEventListener("mouseleave", hideFloatingTooltip);
        link.addEventListener("blur", hideFloatingTooltip);
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            setMobileSidebarState(false);
        }
    });

    function handleBreakpointChange() {
        if (desktopQuery.matches) {
            setMobileSidebarState(false);
            applyDesktopState(readStoredCollapsed(), false);
        } else {
            document.body.classList.remove("sidebar-collapsed");
            updateCollapseToggle(false);
            setMenuLabels(false);
            hideFloatingTooltip();
        }
    }

    if (typeof desktopQuery.addEventListener === "function") {
        desktopQuery.addEventListener("change", handleBreakpointChange);
    } else if (typeof desktopQuery.addListener === "function") {
        desktopQuery.addListener(handleBreakpointChange);
    }

    applyDesktopState(readStoredCollapsed(), false);
    document.addEventListener("scroll", hideFloatingTooltip, true);
})();
