from __future__ import annotations

import json
from datetime import datetime
from pathlib import Path
from threading import Lock
from typing import Any

from app.config import BASE_DIR
from app.constants import EVENT_ANALYZE, EVENT_TRAIN


class LogService:
    def __init__(self) -> None:
        self._lock = Lock()
        self._log_dir = BASE_DIR / "logs"
        self._log_dir.mkdir(parents=True, exist_ok=True)
        self._log_file = self._log_dir / "ai_service.log"

    def log_event(
        self,
        endpoint: str,
        event_type: str,
        success: bool,
        model_name: str | None = None,
        duration_ms: int | None = None,
        error: str | None = None,
        extra: dict[str, Any] | None = None,
    ) -> None:
        payload = {
            "time": datetime.utcnow().isoformat() + "Z",
            "endpoint": endpoint,
            "eventType": event_type,
            "success": success,
            "modelName": model_name,
            "durationMs": duration_ms,
            "error": error,
            "extra": extra or {},
        }
        with self._lock:
            with self._log_file.open("a", encoding="utf-8") as f:
                f.write(json.dumps(payload, ensure_ascii=False) + "\n")

    def get_summary(self) -> dict:
        total_train_success = 0
        total_train_failed = 0
        total_analyze_success = 0
        total_analyze_failed = 0
        latest_error = None
        latest_error_time = None

        if not self._log_file.exists():
            return {
                "totalTrainSuccess": total_train_success,
                "totalTrainFailed": total_train_failed,
                "totalAnalyzeSuccess": total_analyze_success,
                "totalAnalyzeFailed": total_analyze_failed,
                "latestError": latest_error,
                "latestErrorTime": latest_error_time,
            }

        with self._log_file.open("r", encoding="utf-8") as f:
            for line in f:
                line = line.strip()
                if not line:
                    continue
                try:
                    payload = json.loads(line)
                except json.JSONDecodeError:
                    continue

                event_type = payload.get("eventType")
                success = bool(payload.get("success"))
                if event_type == EVENT_TRAIN:
                    if success:
                        total_train_success += 1
                    else:
                        total_train_failed += 1
                elif event_type in (EVENT_ANALYZE, "predict"):
                    if success:
                        total_analyze_success += 1
                    else:
                        total_analyze_failed += 1

                if not success and payload.get("error"):
                    latest_error = str(payload.get("error"))
                    latest_error_time = payload.get("time")

        return {
            "totalTrainSuccess": total_train_success,
            "totalTrainFailed": total_train_failed,
            "totalAnalyzeSuccess": total_analyze_success,
            "totalAnalyzeFailed": total_analyze_failed,
            "latestError": latest_error,
            "latestErrorTime": latest_error_time,
        }
