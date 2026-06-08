(function () {
    function disableSubmit(button) {
        if (!button) {
            return;
        }

        button.disabled = true;
        button.classList.add("is-submitting");
    }

    document.querySelectorAll(".js-confirm-submit, .js-review-submit").forEach(function (form) {
        form.addEventListener("submit", function (event) {
            var message = form.getAttribute("data-confirm");
            if (message && !window.confirm(message)) {
                event.preventDefault();
                return;
            }

            disableSubmit(form.querySelector("button[type='submit']"));
        });
    });

    document.querySelectorAll(".js-toggle-detail").forEach(function (button) {
        button.addEventListener("click", function () {
            var targetId = button.getAttribute("data-target");
            if (!targetId) {
                return;
            }

            var detailRow = document.getElementById(targetId);
            if (!detailRow) {
                return;
            }

            var isHidden = detailRow.classList.contains("d-none");
            detailRow.classList.toggle("d-none", !isHidden);
            button.setAttribute("aria-expanded", isHidden ? "true" : "false");
            button.textContent = isHidden ? "Ẩn chi tiết" : "Xem chi tiết";
        });
    });
})();
