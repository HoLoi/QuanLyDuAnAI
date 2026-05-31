(function (global) {
    "use strict";

    var AI_CHART_KEYS = "__ai_chart_keys__";
    var AI_CHART_EMPTY_CLASS = "ai-chart-empty-message";
    var FEATURE_LABEL_MAP = {
        SoNhanVienDuAn: "Số nhân viên dự án",
        TongSoCongViec: "Tổng số công việc",
        SoCongViecTre: "Số công việc trễ",
        TyLeCongViecTre: "Tỷ lệ công việc trễ",
        ChiPhiDuKien: "Chi phí dự kiến",
        ChiPhiThucTe: "Chi phí thực tế",
        ChenhLechChiPhi: "Chênh lệch chi phí",
        SoLanThayDoiNhanSu: "Số lần thay đổi nhân sự",
        SoLanThayDoiQuanLy: "Số lần thay đổi quản lý",
        SoNgayTreTienDo: "Số ngày trễ tiến độ",
        SoDeXuatCongViecChoDuyet: "Số đề xuất công việc chờ duyệt",
        SoDeXuatCongViecBiTuChoi: "Số đề xuất công việc bị từ chối",
        ThoiGianDuyetCongViecTrungBinh: "Thời gian duyệt công việc trung bình",
        SoDeXuatNganSachChoDuyet: "Số đề xuất ngân sách chờ duyệt",
        SoDeXuatNganSachBiTuChoi: "Số đề xuất ngân sách bị từ chối",
        ThoiGianDuyetNganSachTrungBinh: "Thời gian duyệt ngân sách trung bình",
        SoBaoCaoTienDoChoDuyet: "Số báo cáo tiến độ chờ duyệt",
        SoBaoCaoTienDoBiTuChoi: "Số báo cáo tiến độ bị từ chối",
        SoBaoCaoTienDoYeuCauBoSung: "Số báo cáo tiến độ yêu cầu bổ sung",
        TyLeBaoCaoTienDoBiTuChoi: "Tỷ lệ báo cáo tiến độ bị từ chối",
        SoLanCapNhatTienDo: "Số lần cập nhật tiến độ",
        SoNgayChamCapNhatTienDo: "Số ngày chậm cập nhật tiến độ"
    };

    if (!global[AI_CHART_KEYS]) {
        global[AI_CHART_KEYS] = new Set();
    }

    function hasChartJs() {
        return typeof global.Chart !== "undefined" && typeof global.Chart.getChart === "function";
    }

    function getCanvas(canvasId) {
        if (!canvasId) {
            return null;
        }
        return document.getElementById(canvasId);
    }

    function removeEmptyMessage(canvas) {
        if (!canvas || !canvas.parentElement) {
            return;
        }

        var emptyNode = canvas.parentElement.querySelector("." + AI_CHART_EMPTY_CLASS);
        if (emptyNode) {
            emptyNode.remove();
        }
    }

    function showEmptyMessage(canvas, message) {
        if (!canvas || !canvas.parentElement) {
            return;
        }

        var parent = canvas.parentElement;
        removeEmptyMessage(canvas);
        if (global.getComputedStyle(parent).position === "static") {
            parent.style.position = "relative";
        }

        var emptyNode = document.createElement("div");
        emptyNode.className = AI_CHART_EMPTY_CLASS;
        emptyNode.textContent = message || "Chưa có dữ liệu";
        parent.appendChild(emptyNode);
    }

    function hasRenderableData(config) {
        if (!config || !config.data || !Array.isArray(config.data.datasets)) {
            return false;
        }

        for (var i = 0; i < config.data.datasets.length; i += 1) {
            var dataset = config.data.datasets[i];
            if (!dataset || !Array.isArray(dataset.data) || dataset.data.length === 0) {
                continue;
            }

            for (var j = 0; j < dataset.data.length; j += 1) {
                var value = dataset.data[j];
                if (value === null || typeof value === "undefined") {
                    continue;
                }
                if (typeof value === "number" && !Number.isNaN(value)) {
                    return true;
                }
                if (typeof value === "string" && value.trim() !== "") {
                    return true;
                }
            }
        }

        return false;
    }

    function destroy(canvasId) {
        if (!hasChartJs()) {
            return;
        }

        var canvas = getCanvas(canvasId);
        var existing = global.Chart.getChart(canvasId) || (canvas ? global.Chart.getChart(canvas) : null);
        if (existing) {
            existing.destroy();
        }
    }

    function render(canvasId, config, emptyMessage) {
        if (!hasChartJs()) {
            return null;
        }

        var canvas = getCanvas(canvasId);
        if (!canvas) {
            return null;
        }

        if (!hasRenderableData(config)) {
            destroy(canvasId);
            showEmptyMessage(canvas, emptyMessage || "Chưa có dữ liệu");
            return null;
        }

        removeEmptyMessage(canvas);
        destroy(canvasId);
        global[AI_CHART_KEYS].add(canvasId);
        return new global.Chart(canvas, config);
    }

    function destroyAll() {
        if (!hasChartJs()) {
            return;
        }

        global[AI_CHART_KEYS].forEach(function (canvasId) {
            destroy(canvasId);
            removeEmptyMessage(getCanvas(canvasId));
        });
        global[AI_CHART_KEYS].clear();
    }

    function mapFeatureLabel(featureName) {
        if (!featureName) {
            return "";
        }
        return FEATURE_LABEL_MAP[featureName] || featureName;
    }

    if (!global.__ai_chart_unload_bound__) {
        global.addEventListener("beforeunload", destroyAll);
        global.__ai_chart_unload_bound__ = true;
    }

    global.AiChartHelper = {
        render: render,
        destroy: destroy,
        destroyAll: destroyAll,
        mapFeatureLabel: mapFeatureLabel
    };
})(window);
