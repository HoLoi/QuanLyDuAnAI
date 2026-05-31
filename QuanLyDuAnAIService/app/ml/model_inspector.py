from __future__ import annotations

from app.constants import FEATURE_COLUMNS
from app.ml import model_storage


def validate_model_file(model_file: str, expected_model_type: str | None = None) -> dict:
    errors: list[str] = []
    model_exists = False
    can_load = False
    schema_valid = False
    expected_features: list[str] = []

    try:
        path = model_storage.model_path(model_file)
        model_exists = path.exists()
        if not model_exists:
            errors.append("Model không tồn tại.")
            return {
                "modelExists": model_exists,
                "canLoad": can_load,
                "expectedFeatures": expected_features,
                "schemaValid": schema_valid,
                "errors": errors,
            }

        model = model_storage.load_model(model_file)
        can_load = True

        meta = model_storage.load_metadata(model_file)
        expected_features = meta.get("feature_list", [])
        if not expected_features and hasattr(model, "feature_names_in_"):
            expected_features = [str(x) for x in model.feature_names_in_]
        if not expected_features and hasattr(model, "n_features_in_"):
            expected_features = FEATURE_COLUMNS[: int(model.n_features_in_)]

        schema_valid = expected_features == FEATURE_COLUMNS
        if not schema_valid:
            errors.append("Feature schema không khớp với schema dự án.")

        if expected_model_type:
            model_type = str(meta.get("model_category") or "").strip()
            if model_type and model_type.lower() != expected_model_type.strip().lower():
                errors.append(f"Model thuộc loại {model_type}, không khớp loại yêu cầu {expected_model_type}.")
                schema_valid = False
    except Exception as ex:
        errors.append(str(ex))

    return {
        "modelExists": model_exists,
        "canLoad": can_load,
        "expectedFeatures": expected_features,
        "schemaValid": schema_valid,
        "errors": errors,
    }
