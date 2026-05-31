from __future__ import annotations

from typing import Iterable

import pandas as pd

from app.constants import FEATURE_COLUMNS, REASON_LABEL_COLUMN


def rows_to_dicts(rows: Iterable[object]) -> list[dict]:
    data: list[dict] = []
    for row in rows:
        if hasattr(row, "model_dump"):
            data.append(row.model_dump())
        elif isinstance(row, dict):
            data.append(row)
        else:
            data.append(dict(row))
    return data


def get_missing_features(record: dict) -> list[str]:
    return [feature for feature in FEATURE_COLUMNS if record.get(feature) is None]


def build_feature_frame(records: list[dict], strict: bool = True) -> pd.DataFrame:
    matrix: list[dict] = []
    for idx, record in enumerate(records):
        missing = get_missing_features(record)
        if strict and missing:
            raise ValueError(f"Dong thu {idx + 1} thieu feature: {', '.join(missing)}")
        row = {feature: record.get(feature, 0.0) for feature in FEATURE_COLUMNS}
        matrix.append(row)
    return pd.DataFrame(matrix, columns=FEATURE_COLUMNS)


def build_training_frames(records: list[dict], label_column: str = REASON_LABEL_COLUMN) -> tuple[pd.DataFrame, pd.Series]:
    clean_records: list[dict] = []
    for idx, record in enumerate(records):
        missing = get_missing_features(record)
        label = record.get(label_column)
        if missing:
            raise ValueError(f"Dong thu {idx + 1} thieu feature: {', '.join(missing)}")
        if label is None:
            raise ValueError(f"Dòng thứ {idx + 1} thiếu label {label_column}.")

        try:
            parsed = int(label)
        except Exception as ex:
            raise ValueError(f"Dòng thứ {idx + 1} có label {label_column} không hợp lệ.") from ex

        if label_column == REASON_LABEL_COLUMN and parsed <= 0:
            raise ValueError(f"Dòng thứ {idx + 1} có MaDMNguyenNhan không hợp lệ. Chỉ chấp nhận số nguyên > 0.")

        normalized = dict(record)
        normalized[label_column] = parsed
        clean_records.append(normalized)

    frame = pd.DataFrame(clean_records)
    x = frame[FEATURE_COLUMNS].copy()
    y = frame[label_column].astype(int)
    return x, y
