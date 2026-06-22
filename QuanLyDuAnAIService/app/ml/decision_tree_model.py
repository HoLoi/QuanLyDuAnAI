from __future__ import annotations

from datetime import datetime
from math import ceil
from time import perf_counter

import pandas as pd
from sklearn.metrics import accuracy_score, classification_report, confusion_matrix
from sklearn.model_selection import train_test_split
from sklearn.tree import DecisionTreeClassifier, export_text

from app.constants import MODEL_ALGORITHM


def train_decision_tree(
    x: pd.DataFrame,
    y: pd.Series,
    random_state: int = 42,
    test_size: float = 0.2,
) -> dict:
    if len(x) < 2:
        raise ValueError("Không đủ dữ liệu để train model.")

    class_counts = y.value_counts().to_dict()
    min_class_count = min(class_counts.values()) if class_counts else 0
    class_count = len(class_counts)
    if class_count < 2:
        raise ValueError("Cần tối thiểu 2 lớp nguyên nhân để train model.")
    if min_class_count < 2:
        raise ValueError("Mỗi lớp nguyên nhân cần tối thiểu 2 dòng để chia train/test.")

    requested_test_count = ceil(len(x) * test_size)
    adjusted_test_size = test_size
    test_size_was_adjusted = False
    if requested_test_count < class_count:
        adjusted_test_count = class_count
        if len(x) - adjusted_test_count < class_count:
            raise ValueError("Không đủ dữ liệu để chia train/test có stratify cho tất cả lớp nguyên nhân.")
        adjusted_test_size = adjusted_test_count
        test_size_was_adjusted = True

    x_train, x_test, y_train, y_test = train_test_split(
        x,
        y,
        test_size=adjusted_test_size,
        random_state=random_state,
        stratify=y,
    )

    start = perf_counter()
    model = DecisionTreeClassifier(random_state=random_state)
    model.fit(x_train, y_train)
    duration_ms = int((perf_counter() - start) * 1000)

    y_pred = model.predict(x_test)
    accuracy = float(accuracy_score(y_test, y_pred))
    labels_sorted = sorted(int(v) for v in y.unique().tolist())
    confusion = confusion_matrix(y_test, y_pred, labels=labels_sorted).tolist()
    report = classification_report(y_test, y_pred, output_dict=True, zero_division=0)
    tree_text = export_text(model, feature_names=x.columns.tolist())
    feature_importance = {
        feature: float(score)
        for feature, score in zip(x.columns.tolist(), model.feature_importances_, strict=True)
    }

    distribution = {str(int(k)): int(v) for k, v in y.value_counts().to_dict().items()}

    return {
        "model": model,
        "accuracy": accuracy,
        "confusion_matrix": confusion,
        "feature_importance": feature_importance,
        "classification_report": report,
        "confusion_matrix_labels": labels_sorted,
        "decision_tree_text": tree_text,
        "class_distribution": distribution,
        "train_size": int(len(x_train)),
        "test_size": int(len(x_test)),
        "requested_test_size": test_size,
        "effective_test_size": float(len(x_test) / len(x)),
        "test_size_was_adjusted": test_size_was_adjusted,
        "train_time_ms": duration_ms,
        "train_time_utc": datetime.utcnow().isoformat() + "Z",
        "model_type": MODEL_ALGORITHM,
    }

