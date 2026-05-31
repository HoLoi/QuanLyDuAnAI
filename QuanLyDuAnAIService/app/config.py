from __future__ import annotations

import os
from dataclasses import dataclass
from pathlib import Path

from dotenv import load_dotenv


BASE_DIR = Path(__file__).resolve().parents[1]
ENV_PATH = BASE_DIR / ".env"
if ENV_PATH.exists():
    load_dotenv(ENV_PATH)
else:
    load_dotenv()


def _to_int(value: str, default: int) -> int:
    try:
        return int(value)
    except (TypeError, ValueError):
        return default


def _to_float(value: str, default: float) -> float:
    try:
        return float(value)
    except (TypeError, ValueError):
        return default


def _resolve_dir(value: str, default: str) -> Path:
    raw = (value or default).strip()
    path = Path(raw)
    if not path.is_absolute():
        path = BASE_DIR / path
    return path.resolve()


def _parse_origins(value: str) -> list[str]:
    if not value:
        return ["http://localhost:5000", "http://localhost:5001"]
    return [x.strip() for x in value.split(",") if x.strip()]


@dataclass(frozen=True)
class Settings:
    model_dir: Path
    default_reason_model_name: str
    allow_origins: list[str]
    min_reason_train_rows: int
    min_reason_class_count: int
    min_reason_rows_per_class: int
    reason_confidence_threshold: float
    high_delay_ratio_threshold: float
    high_cost_overrun_threshold: float
    high_staff_change_threshold: float
    high_manager_change_threshold: float


def load_settings() -> Settings:
    default_reason_model_name = (
        os.getenv("DEFAULT_REASON_MODEL_ALIAS")
        or os.getenv("DEFAULT_REASON_MODEL_NAME")
        or "reason_active.joblib"
    ).strip()
    return Settings(
        model_dir=_resolve_dir(os.getenv("MODEL_DIR", "models"), "models"),
        default_reason_model_name=default_reason_model_name,
        allow_origins=_parse_origins(os.getenv("ALLOW_ORIGINS", "")),
        min_reason_train_rows=_to_int(os.getenv("MIN_REASON_TRAIN_ROWS", "30"), 30),
        min_reason_class_count=_to_int(os.getenv("MIN_REASON_CLASS_COUNT", "2"), 2),
        min_reason_rows_per_class=_to_int(os.getenv("MIN_REASON_ROWS_PER_CLASS", "5"), 5),
        reason_confidence_threshold=_to_float(os.getenv("REASON_CONFIDENCE_THRESHOLD", "0.6"), 0.6),
        high_delay_ratio_threshold=_to_float(os.getenv("HIGH_DELAY_RATIO_THRESHOLD", "0.2"), 0.2),
        high_cost_overrun_threshold=_to_float(os.getenv("HIGH_COST_OVERRUN_THRESHOLD", "0.15"), 0.15),
        high_staff_change_threshold=_to_float(os.getenv("HIGH_STAFF_CHANGE_THRESHOLD", "2"), 2.0),
        high_manager_change_threshold=_to_float(os.getenv("HIGH_MANAGER_CHANGE_THRESHOLD", "1"), 1.0),
    )


settings = load_settings()
