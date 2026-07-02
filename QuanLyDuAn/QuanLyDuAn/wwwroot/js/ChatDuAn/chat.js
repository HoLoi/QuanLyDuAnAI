(function () {
    "use strict";

    const app = document.getElementById("chat-du-an-app");
    if (!app) {
        return;
    }

    const roomUrl = app.dataset.roomUrl;
    const roomBatchUrl = app.dataset.roomBatchUrl;
    const olderMessagesUrl = app.dataset.olderMessagesUrl;
    const newMessagesUrl = app.dataset.newMessagesUrl;
    const sendUrl = app.dataset.sendUrl;
    const hubUrl = app.dataset.chatHubUrl;
    const currentUserId = Number.parseInt(app.dataset.currentUserId || "", 10);
    const realtimeStatus = document.getElementById("chat-realtime-status");
    const signalREvent = Object.freeze({ messageCreated: "MessageCreated" });
    const roomEmptyText = app.dataset.roomEmptyText || "Không có phòng phù hợp.";
    let roomRequestRunning = false;
    let olderMessagesRequestRunning = false;
    let sendRequestRunning = false;
    let roomSelectionController = null;
    let syncRequestRunning = false;
    const processedRealtimeIds = new Set();

    function buildUrl(baseUrl, parameters) {
        const url = new URL(baseUrl, window.location.origin);
        Object.entries(parameters).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== "") {
                url.searchParams.set(key, value);
            }
        });
        return url.toString();
    }

    async function readError(response) {
        try {
            const payload = await response.json();
            return payload.message || "Không thể hoàn tất yêu cầu.";
        } catch {
            return "Không thể hoàn tất yêu cầu.";
        }
    }

    function parseSingleElement(html, selector) {
        const template = document.createElement("template");
        template.innerHTML = html.trim();
        return template.content.querySelector(selector);
    }

    function scrollMessagesToBottom() {
        const messageList = document.getElementById("chat-message-list");
        if (messageList) {
            messageList.scrollTop = messageList.scrollHeight;
        }
    }

    function isNearMessageBottom() {
        const list = document.getElementById("chat-message-list");
        return !list || list.scrollHeight - list.scrollTop - list.clientHeight <= 120;
    }

    function hasMessage(messageId) {
        return document.querySelector(`[data-message-id="${Number(messageId)}"]`) !== null;
    }

    function formatMessageTime(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return "";
        }
        return new Intl.DateTimeFormat("vi-VN", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit"
        }).format(date);
    }

    function getSafeAvatarUrl(value) {
        if (!value) {
            return null;
        }
        try {
            const url = new URL(value, window.location.origin);
            return url.protocol === "http:" || url.protocol === "https:" ? url.toString() : null;
        } catch {
            return null;
        }
    }

    function createMessageElement(payload) {
        const messageId = Number(payload.maTinNhan);
        const roomId = Number(payload.maPhongChat);
        const senderId = Number(payload.maNguoiDung);
        if (!Number.isInteger(messageId) || messageId <= 0
            || !Number.isInteger(roomId) || roomId <= 0
            || !Number.isInteger(senderId) || senderId <= 0
            || typeof payload.noiDungTinNhan !== "string") {
            return null;
        }

        const senderName = typeof payload.tenNguoiGui === "string" && payload.tenNguoiGui.trim()
            ? payload.tenNguoiGui.trim()
            : `Nhân viên ${senderId}`;
        const row = document.createElement("div");
        row.className = senderId === currentUserId ? "message-row mine" : "message-row";
        row.dataset.messageId = String(messageId);
        row.dataset.roomId = String(roomId);

        const avatar = document.createElement("div");
        avatar.className = "message-avatar";
        const avatarUrl = getSafeAvatarUrl(payload.avatarUrl);
        if (avatarUrl) {
            const image = document.createElement("img");
            image.src = avatarUrl;
            image.alt = `Avatar ${senderName}`;
            avatar.appendChild(image);
        } else {
            const fallback = document.createElement("span");
            fallback.textContent = senderName.charAt(0).toUpperCase() || "U";
            avatar.appendChild(fallback);
        }

        const bubble = document.createElement("div");
        bubble.className = "message-bubble";
        const author = document.createElement("div");
        author.className = "message-author";
        author.textContent = senderName;
        const content = document.createElement("div");
        content.className = "message-content";
        content.textContent = payload.noiDungTinNhan;
        const meta = document.createElement("div");
        meta.className = "message-meta";
        meta.textContent = formatMessageTime(payload.thoiGianGui);
        bubble.append(author, content, meta);
        row.append(avatar, bubble);
        return row;
    }

    function appendRealtimeMessage(payload, allowNotice) {
        const activeRoomId = Number(document.querySelector(".chat-content")?.dataset.activeRoomId);
        const roomId = Number(payload.maPhongChat);
        if (roomId !== activeRoomId || hasMessage(payload.maTinNhan)) {
            return false;
        }

        const messageItems = document.getElementById("chat-message-items");
        if (!messageItems) {
            return false;
        }
        const wasNearBottom = isNearMessageBottom();
        const message = createMessageElement(payload);
        if (!message) {
            return false;
        }

        let lastBatch = messageItems.querySelector(".chat-message-batch:last-child");
        if (!lastBatch) {
            lastBatch = document.createElement("div");
            lastBatch.className = "chat-message-batch";
            lastBatch.dataset.roomId = String(roomId);
            messageItems.appendChild(lastBatch);
        }
        lastBatch.appendChild(message);
        document.getElementById("chat-message-empty")?.remove();

        if (wasNearBottom) {
            scrollMessagesToBottom();
            document.getElementById("chat-new-message-notice")?.remove();
        } else if (allowNotice && !document.getElementById("chat-new-message-notice")) {
            const notice = document.createElement("button");
            notice.type = "button";
            notice.id = "chat-new-message-notice";
            notice.className = "chat-new-message-notice";
            notice.textContent = "Có tin nhắn mới";
            document.getElementById("chat-message-list")?.appendChild(notice);
        }
        return true;
    }

    function setStatus(element, message, isError) {
        if (!element) {
            return;
        }
        element.textContent = message || "";
        element.classList.toggle("is-error", Boolean(isError));
    }

    function getRoomItemsContainer() {
        return document.getElementById("chat-room-items");
    }

    function getRoomLoadArea() {
        return getRoomItemsContainer()?.querySelector(".chat-room-load") || null;
    }

    function getCurrentRoomCount() {
        return document.querySelectorAll("#chat-room-items [data-room-id]").length;
    }

    function getRoomBatchItemCount(batch) {
        if (!batch) {
            return 0;
        }

        const declaredCount = Number.parseInt(batch.dataset.roomCount || "", 10);
        if (Number.isFinite(declaredCount)) {
            return declaredCount;
        }

        return batch.querySelectorAll("[data-room-id]").length;
    }

    function getRoomEmptyState() {
        return document.getElementById("chat-room-empty");
    }

    function removeRoomEmptyState() {
        getRoomEmptyState()?.remove();
    }

    function showRoomEmptyState() {
        if (getCurrentRoomCount() > 0 || getRoomEmptyState()) {
            return;
        }

        const roomItems = getRoomItemsContainer();
        const loadArea = getRoomLoadArea();
        if (!roomItems || !loadArea) {
            return;
        }

        const emptyState = document.createElement("div");
        emptyState.id = "chat-room-empty";
        emptyState.className = "chat-room-empty app-empty-state";
        emptyState.textContent = roomEmptyText;
        roomItems.insertBefore(emptyState, loadArea);
    }

    function updateActiveRoom(roomId) {
        document.querySelectorAll(".chat-room-item").forEach((item) => {
            item.classList.toggle("active", item.dataset.roomId === String(roomId));
        });
    }

    async function selectRoom(link) {
        const roomId = link.dataset.roomId;
        const projectId = link.dataset.projectId;
        const chatMain = document.getElementById("chat-main");
        if (!roomId || !chatMain) {
            return;
        }

        if (roomSelectionController) {
            roomSelectionController.abort();
        }
        roomSelectionController = new AbortController();
        chatMain.classList.add("is-loading");

        try {
            const response = await fetch(buildUrl(roomUrl, { maPhongChat: roomId }), {
                headers: { "X-Requested-With": "XMLHttpRequest" },
                signal: roomSelectionController.signal
            });
            if (!response.ok) {
                throw new Error(await readError(response));
            }

            chatMain.innerHTML = await response.text();
            updateActiveRoom(roomId);
            scrollMessagesToBottom();

            const url = new URL(window.location.href);
            url.searchParams.set("maDuAn", projectId);
            window.history.pushState({ roomId }, "", url);
        } catch (error) {
            if (error.name !== "AbortError") {
                window.alert(error.message);
            }
        } finally {
            chatMain.classList.remove("is-loading");
        }
    }

    function getLastRoomBatch() {
        const batches = document.querySelectorAll("#chat-room-items .chat-room-batch");
        return batches.length ? batches[batches.length - 1] : null;
    }

    function updateRoomLoadButton(hasMore) {
        const button = document.getElementById("chat-load-more-rooms");
        if (button) {
            button.hidden = !hasMore;
        }
    }

    function syncRoomLoadButtonFromBatch() {
        const batch = getLastRoomBatch();
        updateRoomLoadButton(batch ? batch.dataset.hasMore === "true" : false);
    }

    function removeDuplicateRooms(batch) {
        const knownIds = new Set(
            Array.from(document.querySelectorAll(".chat-room-item"))
                .map((item) => item.dataset.roomId)
        );
        batch.querySelectorAll(".chat-room-item").forEach((item) => {
            if (knownIds.has(item.dataset.roomId)) {
                item.remove();
            } else {
                knownIds.add(item.dataset.roomId);
            }
        });
    }

    async function loadRoomBatch(reset) {
        if (roomRequestRunning) {
            return;
        }

        const roomItems = getRoomItemsContainer();
        const searchInput = document.querySelector('#chat-room-search-form input[name="tuKhoa"]');
        const status = document.getElementById("chat-room-status");
        const button = document.getElementById("chat-load-more-rooms");
        if (!roomItems) {
            return;
        }

        const cursorBatch = reset ? null : getLastRoomBatch();
        roomRequestRunning = true;
        if (button) {
            button.disabled = true;
        }
        setStatus(status, "Đang tải…", false);

        try {
            const response = await fetch(buildUrl(roomBatchUrl, {
                tuKhoa: searchInput ? searchInput.value.trim() : "",
                lastRoomId: cursorBatch ? cursorBatch.dataset.nextRoomId : null,
                pageSize: 20
            }), {
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });
            if (!response.ok) {
                throw new Error(await readError(response));
            }

            const batch = parseSingleElement(await response.text(), ".chat-room-batch");
            if (!batch) {
                throw new Error("Dữ liệu phòng chat không hợp lệ.");
            }

            const loadArea = getRoomLoadArea();
            const hasMore = batch.dataset.hasMore === "true";
            if (reset) {
                removeRoomEmptyState();
                roomItems.querySelectorAll(".chat-room-batch").forEach((item) => item.remove());
            } else {
                removeDuplicateRooms(batch);
            }

            if (loadArea && getRoomBatchItemCount(batch) > 0) {
                roomItems.insertBefore(batch, loadArea);
            }

            updateRoomLoadButton(hasMore);

            if (getCurrentRoomCount() > 0) {
                removeRoomEmptyState();
            } else if (reset) {
                showRoomEmptyState();
            }

            setStatus(status, "", false);
        } catch (error) {
            setStatus(status, error.message, true);
        } finally {
            roomRequestRunning = false;
            if (button) {
                button.disabled = false;
            }
        }
    }

    function getOldestMessageBatch() {
        return document.querySelector("#chat-message-items .chat-message-batch");
    }

    function removeDuplicateMessages(batch) {
        const knownIds = new Set(
            Array.from(document.querySelectorAll("[data-message-id]"))
                .map((item) => item.dataset.messageId)
        );
        batch.querySelectorAll("[data-message-id]").forEach((item) => {
            if (knownIds.has(item.dataset.messageId)) {
                item.remove();
            } else {
                knownIds.add(item.dataset.messageId);
            }
        });
    }

    async function loadOlderMessages(button) {
        if (olderMessagesRequestRunning) {
            return;
        }

        const batch = getOldestMessageBatch();
        const messageItems = document.getElementById("chat-message-items");
        const messageList = document.getElementById("chat-message-list");
        const status = document.getElementById("chat-message-status");
        if (!batch || !messageItems || !messageList) {
            return;
        }

        olderMessagesRequestRunning = true;
        button.disabled = true;
        setStatus(status, "Đang tải…", false);
        const oldHeight = messageList.scrollHeight;
        const oldTop = messageList.scrollTop;

        try {
            const response = await fetch(buildUrl(olderMessagesUrl, {
                maPhongChat: batch.dataset.roomId,
                cursorThoiGianGui: batch.dataset.cursorTime,
                cursorMaTinNhan: batch.dataset.cursorMessageId,
                pageSize: 30
            }), {
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });
            if (!response.ok) {
                throw new Error(await readError(response));
            }

            const olderBatch = parseSingleElement(await response.text(), ".chat-message-batch");
            if (!olderBatch || olderBatch.dataset.roomId !== batch.dataset.roomId) {
                throw new Error("Dữ liệu tin nhắn không hợp lệ.");
            }

            removeDuplicateMessages(olderBatch);
            messageItems.insertBefore(olderBatch, messageItems.firstChild);
            button.hidden = olderBatch.dataset.hasMore !== "true";
            messageList.scrollTop = oldTop + (messageList.scrollHeight - oldHeight);
            setStatus(status, "", false);
        } catch (error) {
            setStatus(status, error.message, true);
        } finally {
            olderMessagesRequestRunning = false;
            button.disabled = false;
        }
    }

    function updateRoomPreview(roomId, content, time, incrementCount) {
        const room = document.querySelector(`.chat-room-item[data-room-id="${roomId}"]`);
        if (!room) {
            return;
        }

        const preview = room.querySelector(".room-latest");
        if (preview) {
            preview.textContent = content;
        }
        const countPill = room.querySelector("[data-room-message-count]");
        if (incrementCount && countPill) {
            const current = Number.parseInt(countPill.dataset.roomMessageCount || "", 10);
            if (Number.isFinite(current)) {
                const next = current + 1;
                countPill.dataset.roomMessageCount = String(next);
                const icon = countPill.querySelector("i");
                countPill.textContent = ` ${next} tin`;
                if (icon) {
                    countPill.prepend(icon);
                }
            }
        }
        let timePill = room.querySelector("[data-room-message-time]");
        if (!timePill && time) {
            timePill = document.createElement("span");
            timePill.className = "meta-pill";
            timePill.dataset.roomMessageTime = "";
            const icon = document.createElement("i");
            icon.className = "bi bi-clock";
            timePill.appendChild(icon);
            room.querySelector(".room-meta")?.appendChild(timePill);
        }
        if (timePill && time) {
            const icon = timePill.querySelector("i");
            timePill.textContent = ` ${formatMessageTime(time)}`;
            if (icon) {
                timePill.prepend(icon);
            }
        }
    }

    async function sendMessage(form) {
        if (sendRequestRunning) {
            return;
        }

        const submitButton = form.querySelector('button[type="submit"]');
        const input = form.querySelector('textarea[name="NoiDungTinNhan"]');
        const status = form.querySelector("#chat-send-status");
        if (!input || !input.value.trim()) {
            setStatus(status, "Vui lòng nhập nội dung tin nhắn.", true);
            return;
        }

        sendRequestRunning = true;
        if (submitButton) {
            submitButton.disabled = true;
        }
        setStatus(status, "Đang gửi…", false);

        try {
            const response = await fetch(sendUrl, {
                method: "POST",
                body: new FormData(form),
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });
            if (!response.ok) {
                throw new Error(await readError(response));
            }

            const message = parseSingleElement(await response.text(), "[data-message-id]");
            const messageItems = document.getElementById("chat-message-items");
            if (!message || !messageItems) {
                throw new Error("Phản hồi gửi tin nhắn không hợp lệ.");
            }
            const wasAlreadyRendered = hasMessage(message.dataset.messageId);
            if (!wasAlreadyRendered) {
                let lastBatch = messageItems.querySelector(".chat-message-batch:last-child");
                if (!lastBatch) {
                    lastBatch = document.createElement("div");
                    lastBatch.className = "chat-message-batch";
                    messageItems.appendChild(lastBatch);
                }
                lastBatch.appendChild(message);
            }

            document.getElementById("chat-message-empty")?.remove();
            const roomId = message.dataset.roomId
                || document.querySelector(".chat-content")?.dataset.activeRoomId;
            if (!wasAlreadyRendered) {
                updateRoomPreview(roomId, input.value.trim(), new Date().toISOString(), true);
            }
            input.value = "";
            setStatus(status, "", false);
            scrollMessagesToBottom();
        } catch (error) {
            setStatus(status, error.message, true);
        } finally {
            sendRequestRunning = false;
            if (submitButton) {
                submitButton.disabled = false;
            }
        }
    }

    document.addEventListener("click", (event) => {
        const newMessageNotice = event.target.closest("#chat-new-message-notice");
        if (newMessageNotice) {
            scrollMessagesToBottom();
            newMessageNotice.remove();
            return;
        }

        const roomLink = event.target.closest(".chat-room-item");
        if (roomLink) {
            event.preventDefault();
            selectRoom(roomLink);
            return;
        }

        const loadRoomsButton = event.target.closest("#chat-load-more-rooms");
        if (loadRoomsButton) {
            loadRoomBatch(false);
            return;
        }

        const loadMessagesButton = event.target.closest("#chat-load-older-messages");
        if (loadMessagesButton) {
            loadOlderMessages(loadMessagesButton);
        }
    });

    document.addEventListener("submit", (event) => {
        if (event.target.matches("#chat-room-search-form")) {
            event.preventDefault();
            loadRoomBatch(true);
            return;
        }
        if (event.target.matches("#chat-message-form")) {
            event.preventDefault();
            sendMessage(event.target);
        }
    });

    syncRoomLoadButtonFromBatch();
    scrollMessagesToBottom();

    function getLargestRenderedMessageId() {
        return Array.from(document.querySelectorAll("#chat-message-items [data-message-id]"))
            .reduce((largest, item) => {
                const value = Number.parseInt(item.dataset.messageId || "", 10);
                return Number.isFinite(value) ? Math.max(largest, value) : largest;
            }, 0);
    }

    async function syncMissedMessages() {
        if (syncRequestRunning || !newMessagesUrl) {
            return;
        }
        const roomId = Number(document.querySelector(".chat-content")?.dataset.activeRoomId);
        if (!Number.isInteger(roomId) || roomId <= 0) {
            return;
        }

        syncRequestRunning = true;
        let afterMessageId = getLargestRenderedMessageId();
        let batchCount = 0;
        try {
            while (batchCount < 20) {
                const response = await fetch(buildUrl(newMessagesUrl, {
                    maPhongChat: roomId,
                    afterMessageId,
                    pageSize: 50
                }), {
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });
                if (!response.ok) {
                    throw new Error(await readError(response));
                }

                const batch = await response.json();
                const messages = Array.isArray(batch.danhSachTinNhan)
                    ? batch.danhSachTinNhan
                    : [];
                for (const message of messages) {
                    const messageId = Number(message.maTinNhan);
                    if (Number.isInteger(messageId)) {
                        afterMessageId = Math.max(afterMessageId, messageId);
                    }
                    if (!hasMessage(messageId)) {
                        appendRealtimeMessage(message, true);
                        updateRoomPreview(
                            message.maPhongChat,
                            message.noiDungTinNhan,
                            message.thoiGianGui,
                            true);
                    }
                }

                batchCount += 1;
                if (!batch.hasMore || messages.length === 0) {
                    break;
                }
            }
        } catch {
            setStatus(
                realtimeStatus,
                "Không thể đồng bộ tin nhắn mới. Vui lòng thử lại.",
                true);
        } finally {
            syncRequestRunning = false;
        }
    }

    function initializeRealtime() {
        if (!hubUrl || typeof signalR === "undefined") {
            setStatus(
                realtimeStatus,
                "Không thể kết nối realtime. Tin nhắn vẫn có thể gửi bình thường.",
                true);
            return;
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        connection.on(signalREvent.messageCreated, (payload) => {
            const messageId = Number(payload?.maTinNhan);
            if (!Number.isInteger(messageId)
                || messageId <= 0
                || processedRealtimeIds.has(messageId)
                || hasMessage(messageId)) {
                return;
            }
            processedRealtimeIds.add(messageId);

            appendRealtimeMessage(payload, true);
            updateRoomPreview(
                payload.maPhongChat,
                payload.noiDungTinNhan,
                payload.thoiGianGui,
                true);
        });

        connection.onreconnecting(() => {
            setStatus(realtimeStatus, "Mất kết nối, đang thử lại...", false);
        });
        connection.onreconnected(() => {
            setStatus(realtimeStatus, "Đã kết nối lại.", false);
            syncMissedMessages();
        });
        connection.onclose(() => {
            setStatus(
                realtimeStatus,
                "Không thể kết nối realtime. Tin nhắn vẫn có thể gửi bình thường.",
                true);
        });

        setStatus(realtimeStatus, "Đang kết nối...", false);
        connection.start()
            .then(() => setStatus(realtimeStatus, "", false))
            .catch(() => {
                setStatus(
                    realtimeStatus,
                    "Không thể kết nối realtime. Tin nhắn vẫn có thể gửi bình thường.",
                    true);
            });
    }

    initializeRealtime();
})();
