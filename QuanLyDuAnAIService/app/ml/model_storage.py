from __future__ import annotations

import json
import re
import shutil
from datetime import datetime
from pathlib import Path

import joblib

from app.config import settings
from app.constants import (
    ACTIVE_MODEL_ALIAS_BY_TYPE,
    FEATURE_COLUMNS,
    LEGACY_DEFAULT_MODEL_ALIAS,
    METADATA_EXTENSION,
    MODEL_EXTENSION,
    MODEL_TYPE_REASON,
)

SAFE_FILE_PATTERN = re.compile(r"^[A-Za-z0-9._-]+$")


def ensure_directories() -> None:
    settings.model_dir.mkdir(parents=True, exist_ok=True)


def ensure_safe_model_file(model_file: str) -> str:
    if not model_file or not SAFE_FILE_PATTERN.match(model_file):
        raise ValueError("Tên model không hợp lệ.")
    if not model_file.endswith(MODEL_EXTENSION):
        raise ValueError(f"Model file phải kết thúc bằng {MODEL_EXTENSION}.")
    return model_file


def model_path(model_file: str) -> Path:
    safe_name = ensure_safe_model_file(model_file)
    path = (settings.model_dir / safe_name).resolve()
    if path.parent != settings.model_dir.resolve():
        raise ValueError("Đường dẫn model không hợp lệ.")
    return path


def metadata_path(model_file: str) -> Path:
    safe_name = ensure_safe_model_file(model_file)
    meta_name = safe_name.replace(MODEL_EXTENSION, METADATA_EXTENSION)
    path = (settings.model_dir / meta_name).resolve()
    if path.parent != settings.model_dir.resolve():
        raise ValueError("Đường dẫn metadata không hợp lệ.")
    return path


def default_alias_for_type(model_type: str | None = None) -> str:
    normalized = (model_type or "").strip()
    if normalized.lower() == MODEL_TYPE_REASON.lower():
        return settings.default_reason_model_name
    return ACTIVE_MODEL_ALIAS_BY_TYPE.get(MODEL_TYPE_REASON, settings.default_reason_model_name)


def is_active_alias_model(model_file: str) -> bool:
    alias_names = {
        settings.default_reason_model_name,
        LEGACY_DEFAULT_MODEL_ALIAS,
    }
    return model_file in alias_names


def save_model(model, model_file: str) -> Path:
    ensure_directories()
    path = model_path(model_file)
    joblib.dump(model, path)
    return path


def save_metadata(model_file: str, metadata: dict) -> Path:
    ensure_directories()
    meta_path = metadata_path(model_file)
    with meta_path.open("w", encoding="utf-8") as f:
        json.dump(metadata, f, ensure_ascii=False, indent=2)
    return meta_path


def load_model(model_file: str):
    path = model_path(model_file)
    if not path.exists():
        raise FileNotFoundError(f"Không tìm thấy model: {model_file}")
    return joblib.load(path)


def load_metadata(model_file: str) -> dict:
    meta_path = metadata_path(model_file)
    if not meta_path.exists():
        return {}
    with meta_path.open("r", encoding="utf-8") as f:
        return json.load(f)


def _infer_model_type(model_file: str, metadata: dict) -> str:
    model_type = str(metadata.get("model_category") or "").strip()
    if model_type:
        return model_type
    lower_name = model_file.lower()
    if lower_name.startswith("reason_") or "reason" in lower_name:
        return MODEL_TYPE_REASON
    return MODEL_TYPE_REASON


def list_models(include_aliases: bool = False) -> list[dict]:
    ensure_directories()
    results: list[dict] = []
    for path in sorted(settings.model_dir.glob(f"*{MODEL_EXTENSION}")):
        stat = path.stat()
        model_file = path.name
        if not include_aliases and is_active_alias_model(model_file):
            continue
        can_load = True
        algorithm = None
        expected_features: list[str] = []
        schema_valid = False
        validation_message = None
        accuracy = None
        train_size = None
        test_size = None
        class_distribution: dict[str, int] = {}
        feature_importance: dict[str, float] = {}
        confusion_matrix: list[list[int]] = []
        confusion_matrix_labels: list[int] = []
        precision_macro = None
        recall_macro = None
        f1_macro = None
        precision_weighted = None
        recall_weighted = None
        f1_weighted = None
        decision_tree_text = None
        metadata: dict = {}
        try:
            metadata = load_metadata(model_file)
            algorithm = metadata.get("model_type")
            expected_features = metadata.get("feature_list", []) or []
            accuracy = metadata.get("accuracy")
            train_size = metadata.get("train_size", metadata.get("trainSize"))
            test_size = metadata.get("test_size", metadata.get("testSize"))
            class_distribution = metadata.get("class_distribution", metadata.get("classDistribution", {})) or {}
            feature_importance = metadata.get("feature_importance", metadata.get("featureImportance", {})) or {}
            confusion_matrix = metadata.get("confusion_matrix", metadata.get("confusionMatrix", [])) or []
            confusion_matrix_labels = metadata.get("confusion_matrix_labels", metadata.get("confusionMatrixLabels", [])) or []
            try:
                confusion_matrix_labels = [int(x) for x in confusion_matrix_labels]
            except Exception:
                confusion_matrix_labels = []
            precision_macro = metadata.get("precision_macro", metadata.get("precisionMacro"))
            recall_macro = metadata.get("recall_macro", metadata.get("recallMacro"))
            f1_macro = metadata.get("f1_macro", metadata.get("f1Macro"))
            precision_weighted = metadata.get("precision_weighted", metadata.get("precisionWeighted"))
            recall_weighted = metadata.get("recall_weighted", metadata.get("recallWeighted"))
            f1_weighted = metadata.get("f1_weighted", metadata.get("f1Weighted"))
            decision_tree_text = metadata.get("decision_tree_text", metadata.get("decisionTreeText"))
            _ = load_model(model_file)
            schema_valid = expected_features == FEATURE_COLUMNS
            if not schema_valid:
                if not expected_features:
                    validation_message = "Thiếu metadata feature_list. Vui lòng train lại model."
                else:
                    validation_message = "Schema feature của model không khớp contract hiện tại. Vui lòng train lại."
        except Exception:
            can_load = False
            validation_message = "Không thể tải model hoặc metadata model bị lỗi."

        model_type = _infer_model_type(model_file, metadata)
        results.append(
            {
                "tenFile": model_file,
                "dungLuong": int(stat.st_size),
                "createdAt": datetime.fromtimestamp(stat.st_ctime),
                "updatedAt": datetime.fromtimestamp(stat.st_mtime),
                "isDefault": is_active_alias_model(model_file),
                "canLoad": can_load,
                "algorithm": algorithm,
                "expectedFeatures": expected_features,
                "schemaValid": schema_valid,
                "canActivate": can_load and schema_valid,
                "validationMessage": validation_message,
                "accuracy": float(accuracy) if accuracy is not None else None,
                "trainSize": int(train_size) if train_size is not None else None,
                "testSize": int(test_size) if test_size is not None else None,
                "classDistribution": class_distribution,
                "featureImportance": feature_importance,
                "confusionMatrix": confusion_matrix,
                "confusionMatrixLabels": confusion_matrix_labels,
                "precisionMacro": float(precision_macro) if precision_macro is not None else None,
                "recallMacro": float(recall_macro) if recall_macro is not None else None,
                "f1Macro": float(f1_macro) if f1_macro is not None else None,
                "precisionWeighted": float(precision_weighted) if precision_weighted is not None else None,
                "recallWeighted": float(recall_weighted) if recall_weighted is not None else None,
                "f1Weighted": float(f1_weighted) if f1_weighted is not None else None,
                "decisionTreeText": str(decision_tree_text) if decision_tree_text is not None else None,
                "loaiModel": model_type,
            }
        )
    return results


def copy_model_as_default(model_file: str, model_type: str) -> tuple[Path, Path | None]:
    src_model_path = model_path(model_file)
    if not src_model_path.exists():
        raise FileNotFoundError(f"Không tìm thấy model: {model_file}")

    alias_name = default_alias_for_type(model_type)
    dst_model_path = model_path(alias_name)
    shutil.copy2(src_model_path, dst_model_path)

    src_meta = metadata_path(model_file)
    dst_meta = metadata_path(alias_name)
    if src_meta.exists():
        shutil.copy2(src_meta, dst_meta)

    try:
        metadata = load_metadata(alias_name) if dst_meta.exists() else {}
        metadata["sourceModelFile"] = model_file
        metadata["originalModelFile"] = model_file
        metadata["modelVersionFile"] = model_file
        metadata["activeAliasFile"] = alias_name
        metadata["isAliasMetadata"] = True
        if not str(metadata.get("model_file") or "").strip():
            metadata["model_file"] = model_file
        save_metadata(alias_name, metadata)
        return dst_model_path, dst_meta
    except Exception:
        return dst_model_path, (dst_meta if dst_meta.exists() else None)


def delete_model_files(model_file: str) -> None:
    if is_active_alias_model(model_file):
        raise ValueError("Không được xóa file alias active model.")

    m_path = model_path(model_file)
    meta = metadata_path(model_file)

    if m_path.exists():
        m_path.unlink()
    if meta.exists():
        meta.unlink()
