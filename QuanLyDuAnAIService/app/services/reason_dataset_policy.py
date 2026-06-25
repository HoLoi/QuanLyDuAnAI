from __future__ import annotations

from dataclasses import dataclass, field

from app.config import settings
from app.constants import DELAY_STATUS_COLUMN, FEATURE_COLUMNS, REASON_LABEL_COLUMN
from app.ml.feature_builder import get_missing_features, rows_to_dicts


@dataclass
class ReasonDatasetClassification:
    total_rows: int = 0
    valid_rows_before_class_filter: int = 0
    used_rows: int = 0
    accumulating_rows: int = 0
    eligible_class_count: int = 0
    accumulating_class_count: int = 0
    all_class_distribution: dict[str, int] = field(default_factory=dict)
    used_class_distribution: dict[str, int] = field(default_factory=dict)
    dropped_class_distribution: dict[str, int] = field(default_factory=dict)
    used_records: list[dict] = field(default_factory=list)
    warnings: list[str] = field(default_factory=list)
    dropped_not_delay: int = 0
    dropped_missing_label: int = 0
    dropped_missing_feature: int = 0

    @property
    def is_trainable(self) -> bool:
        return (
            self.used_rows >= settings.min_reason_train_rows
            and self.eligible_class_count >= settings.min_reason_class_count
        )


def classify_reason_dataset(rows: list[object], allow_missing_delay_flag: bool = False) -> ReasonDatasetClassification:
    records = rows_to_dicts(rows)
    valid_records: list[dict] = []
    result = ReasonDatasetClassification(total_rows=len(records))

    for record in records:
        is_tre = record.get(DELAY_STATUS_COLUMN)
        if allow_missing_delay_flag:
            is_delay = is_tre in (None, 1, True)
        else:
            is_delay = is_tre in (1, True)
        if not is_delay:
            result.dropped_not_delay += 1
            continue

        if get_missing_features(record):
            result.dropped_missing_feature += 1
            continue

        reason_label = record.get(REASON_LABEL_COLUMN)
        try:
            reason_label = int(reason_label)
        except Exception:
            result.dropped_missing_label += 1
            continue

        if reason_label <= 0:
            result.dropped_missing_label += 1
            continue

        normalized = dict(record)
        normalized[REASON_LABEL_COLUMN] = reason_label
        valid_records.append(normalized)

    class_counts: dict[int, int] = {}
    for row in valid_records:
        key = int(row[REASON_LABEL_COLUMN])
        class_counts[key] = class_counts.get(key, 0) + 1

    eligible_classes = {
        key for key, count in class_counts.items()
        if count >= settings.min_reason_rows_per_class
    }
    used_records = [
        row for row in valid_records
        if int(row[REASON_LABEL_COLUMN]) in eligible_classes
    ]
    dropped_distribution = {
        str(key): int(count)
        for key, count in sorted(class_counts.items())
        if key not in eligible_classes
    }
    used_distribution = {
        str(key): int(count)
        for key, count in sorted(class_counts.items())
        if key in eligible_classes
    }

    result.valid_rows_before_class_filter = len(valid_records)
    result.used_records = used_records
    result.used_rows = len(used_records)
    result.accumulating_rows = sum(dropped_distribution.values())
    result.eligible_class_count = len(used_distribution)
    result.accumulating_class_count = len(dropped_distribution)
    result.all_class_distribution = {str(key): int(count) for key, count in sorted(class_counts.items())}
    result.used_class_distribution = used_distribution
    result.dropped_class_distribution = dropped_distribution

    if result.dropped_not_delay > 0:
        result.warnings.append(f"?? b? {result.dropped_not_delay} d?ng kh?ng thu?c nh?m LaDuAnTre=1.")
    if result.dropped_missing_feature > 0:
        result.warnings.append(f"?? b? {result.dropped_missing_feature} d?ng thi?u feature train.")
    if result.dropped_missing_label > 0:
        result.warnings.append(f"?? b? {result.dropped_missing_label} d?ng thi?u ho?c sai MaDMNguyenNhan.")
    if result.accumulating_class_count > 0:
        result.warnings.append("M?t s? nguy?n nh?n ?ang ti?p t?c t?ch l?y d? li?u.")

    return result


def build_blocking_errors(classification: ReasonDatasetClassification) -> list[str]:
    if classification.is_trainable:
        return []
    errors: list[str] = []
    if classification.used_rows < settings.min_reason_train_rows:
        errors.append("Ch?a ?? 30 d?ng ho?c 2 nguy?n nh?n sau khi l?c c?c l?p d??i 5 d?ng.")
    if classification.eligible_class_count < settings.min_reason_class_count:
        errors.append("Ch?a ?? 30 d?ng ho?c 2 nguy?n nh?n sau khi l?c c?c l?p d??i 5 d?ng.")
    return list(dict.fromkeys(errors))


def null_feature_ratios(records: list[object]) -> dict[str, float]:
    rows = rows_to_dicts(records)
    total = len(rows)
    if total == 0:
        return {feature: 0.0 for feature in FEATURE_COLUMNS}
    null_counts = {feature: 0 for feature in FEATURE_COLUMNS}
    for record in rows:
        for feature in FEATURE_COLUMNS:
            if record.get(feature) is None:
                null_counts[feature] += 1
    return {key: round(value / total, 4) for key, value in null_counts.items()}
