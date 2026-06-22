from __future__ import annotations

import platform
from datetime import datetime
from threading import Lock

import sklearn

from app.constants import (
    FEATURE_COLUMNS,
    MODEL_FILE_PREFIX,
    MODEL_TYPE_REASON,
    REASON_LABEL_COLUMN,
)
from app.ml.decision_tree_model import train_decision_tree
from app.ml.feature_builder import build_training_frames
from app.ml.model_inspector import validate_model_file
from app.ml.model_storage import (
    copy_model_as_default,
    default_alias_for_type,
    delete_model_files,
    ensure_directories,
    is_active_alias_model,
    list_models,
    load_metadata,
    load_model,
    model_path,
    save_metadata,
    save_model,
)
from app.schemas import (
    CompareModelRequest,
    PredictProjectRequest,
    TestPredictRequest,
    TrainRequest,
    TrainResponse,
    ValidateModelRequest,
    ValidateModelResponse,
)
from app.services.model_compare_service import ModelCompareService
from app.services.prediction_service import PredictionService
from app.services.reason_dataset_policy import build_blocking_errors, classify_reason_dataset


class ModelService:
    def __init__(self) -> None:
        self._lock = Lock()
        self._cached: dict[str, dict] = {
            MODEL_TYPE_REASON: self._empty_cache_entry(),
        }
        self.started_at = datetime.utcnow()

        ensure_directories()

    @staticmethod
    def _empty_cache_entry() -> dict:
        return {
            "model": None,
            "loaded_name": None,
            "alias": None,
            "metadata": {},
        }

    @staticmethod
    def _normalize_model_type(model_type: str | None) -> str:
        raw = (model_type or MODEL_TYPE_REASON).strip().lower()
        if raw == MODEL_TYPE_REASON.lower():
            return MODEL_TYPE_REASON
        raise ValueError("Hệ thống chỉ hỗ trợ modelType=NguyenNhan.")

    @staticmethod
    def _read_report_metric(report: dict, bucket: str, field: str) -> float | None:
        bucket_value = report.get(bucket)
        if not isinstance(bucket_value, dict):
            return None
        value = bucket_value.get(field)
        if value is None:
            return None
        try:
            return float(value)
        except Exception:
            return None

    def startup_load_default_models(self) -> dict[str, str | None]:
        result: dict[str, str | None] = {}
        alias = default_alias_for_type(MODEL_TYPE_REASON)
        try:
            if not model_path(alias).exists():
                self._clear_cache(MODEL_TYPE_REASON)
                result[MODEL_TYPE_REASON] = None
                return result

            self._load_model_to_cache(alias, MODEL_TYPE_REASON)
            result[MODEL_TYPE_REASON] = self.get_loaded_model_name(MODEL_TYPE_REASON)
            return result
        except Exception:
            self._clear_cache(MODEL_TYPE_REASON)
            result[MODEL_TYPE_REASON] = None
            return result

    def _load_model_to_cache(self, model_file: str, model_type: str) -> None:
        model = load_model(model_file)
        metadata = load_metadata(model_file)
        loaded_name = self._resolve_original_model_name(model_file, metadata)

        with self._lock:
            self._cached[model_type] = {
                "model": model,
                "loaded_name": loaded_name,
                "alias": model_file,
                "metadata": metadata or {},
            }

    def _clear_cache(self, model_type: str) -> None:
        with self._lock:
            self._cached[model_type] = self._empty_cache_entry()

    def get_loaded_model_name(self, model_type: str = MODEL_TYPE_REASON) -> str | None:
        model_type = self._normalize_model_type(model_type)
        with self._lock:
            entry = self._cached.get(model_type) or self._empty_cache_entry()
            model = entry.get("model")
            alias = entry.get("alias")
            loaded_name = entry.get("loaded_name")

            if model is None or not alias:
                return None

            try:
                alias_exists = model_path(alias).exists()
            except Exception:
                alias_exists = False

            if not alias_exists:
                self._cached[model_type] = self._empty_cache_entry()
                return None

            if loaded_name:
                try:
                    if model_path(loaded_name).exists():
                        return loaded_name
                except Exception:
                    pass

            entry["loaded_name"] = alias
            self._cached[model_type] = entry
            return alias

    def get_loaded_models(self) -> dict[str, str | None]:
        return {
            MODEL_TYPE_REASON: self.get_loaded_model_name(MODEL_TYPE_REASON),
        }

    def get_loaded_metadata(self, model_type: str = MODEL_TYPE_REASON) -> dict:
        model_type = self._normalize_model_type(model_type)
        if self.get_loaded_model_name(model_type) is None:
            return {}
        entry = self._cached.get(model_type) or {}
        return dict(entry.get("metadata") or {})

    def _require_loaded_model(self, model_type: str):
        model_type = self._normalize_model_type(model_type)
        loaded = self.get_loaded_model_name(model_type)
        entry = self._cached.get(model_type) or {}
        model = entry.get("model")
        if loaded is None or model is None:
            raise ValueError("Chưa có active model NguyenNhan trong memory. Vui lòng train/active/reload model.")
        return model

    def _validate_reason_dataset(self, request: TrainRequest) -> tuple[list[dict], list[str]]:
        classification = classify_reason_dataset(request.dataset, allow_missing_delay_flag=True)
        blocking_errors = build_blocking_errors(classification)
        if blocking_errors:
            raise ValueError(blocking_errors[0])
        return classification.used_records, classification.warnings

    def train_model(self, request: TrainRequest) -> TrainResponse:
        model_type = self._normalize_model_type(request.modelType)
        records, warning_messages = self._validate_reason_dataset(request)
        dataset_stats = classify_reason_dataset(request.dataset, allow_missing_delay_flag=True)
        x, y = build_training_frames(records, label_column=REASON_LABEL_COLUMN)
        model_description = "DecisionTreeClassifier cho phân tích nguyên nhân trễ dự án"

        result = train_decision_tree(x, y, random_state=42)
        classification_report = result["classification_report"] or {}
        precision_macro = self._read_report_metric(classification_report, "macro avg", "precision")
        recall_macro = self._read_report_metric(classification_report, "macro avg", "recall")
        f1_macro = self._read_report_metric(classification_report, "macro avg", "f1-score")
        precision_weighted = self._read_report_metric(classification_report, "weighted avg", "precision")
        recall_weighted = self._read_report_metric(classification_report, "weighted avg", "recall")
        f1_weighted = self._read_report_metric(classification_report, "weighted avg", "f1-score")

        timestamp = datetime.utcnow().strftime("%Y%m%d_%H%M%S")
        model_file = f"{MODEL_FILE_PREFIX}_{timestamp}.joblib"
        model_saved_path = save_model(result["model"], model_file)

        metadata = {
            "model_file": model_file,
            "model_type": result["model_type"],
            "algorithm": result["model_type"],
            "model_category": model_type,
            "accuracy": result["accuracy"],
            "train_time": result["train_time_utc"],
            "train_time_ms": result["train_time_ms"],
            "feature_list": FEATURE_COLUMNS,
            "label_column": REASON_LABEL_COLUMN,
            "class_distribution": result["class_distribution"],
            "valid_rows_before_class_filter": dataset_stats.valid_rows_before_class_filter,
            "used_rows": dataset_stats.used_rows,
            "accumulating_rows": dataset_stats.accumulating_rows,
            "eligible_class_count": dataset_stats.eligible_class_count,
            "accumulating_class_count": dataset_stats.accumulating_class_count,
            "used_class_distribution": dataset_stats.used_class_distribution,
            "dropped_class_distribution": dataset_stats.dropped_class_distribution,
            "feature_importance": result["feature_importance"],
            "classification_report": classification_report,
            "precision_macro": precision_macro,
            "recall_macro": recall_macro,
            "f1_macro": f1_macro,
            "precision_weighted": precision_weighted,
            "recall_weighted": recall_weighted,
            "f1_weighted": f1_weighted,
            "sklearn_version": sklearn.__version__,
            "python_version": platform.python_version(),
            "train_size": result["train_size"],
            "test_size": result["test_size"],
            "requested_test_size": result["requested_test_size"],
            "effective_test_size": result["effective_test_size"],
            "test_size_was_adjusted": result["test_size_was_adjusted"],
            "confusion_matrix": result["confusion_matrix"],
            "confusion_matrix_labels": result["confusion_matrix_labels"],
            "decision_tree_text": result["decision_tree_text"],
            "requested_by_user_id": request.requestedByUserId,
            "requested_by_user_name": request.requestedByUserName,
            "train_note": request.trainNote,
            "created_at": datetime.utcnow().isoformat() + "Z",
            "modelType": model_type,
            "modelFile": model_file,
            "featureList": FEATURE_COLUMNS,
            "labelColumn": REASON_LABEL_COLUMN,
            "classDistribution": result["class_distribution"],
            "validRowsBeforeClassFilter": dataset_stats.valid_rows_before_class_filter,
            "usedRows": dataset_stats.used_rows,
            "accumulatingRows": dataset_stats.accumulating_rows,
            "eligibleClassCount": dataset_stats.eligible_class_count,
            "accumulatingClassCount": dataset_stats.accumulating_class_count,
            "usedClassDistribution": dataset_stats.used_class_distribution,
            "droppedClassDistribution": dataset_stats.dropped_class_distribution,
            "featureImportance": result["feature_importance"],
            "classificationReport": classification_report,
            "precisionMacro": precision_macro,
            "recallMacro": recall_macro,
            "f1Macro": f1_macro,
            "precisionWeighted": precision_weighted,
            "recallWeighted": recall_weighted,
            "f1Weighted": f1_weighted,
            "trainSize": result["train_size"],
            "testSize": result["test_size"],
            "requestedTestSize": result["requested_test_size"],
            "effectiveTestSize": result["effective_test_size"],
            "testSizeWasAdjusted": result["test_size_was_adjusted"],
            "confusionMatrix": result["confusion_matrix"],
            "confusionMatrixLabels": result["confusion_matrix_labels"],
            "decisionTreeText": result["decision_tree_text"],
            "createdAt": datetime.utcnow().isoformat() + "Z",
            "trainNote": request.trainNote,
        }
        meta_path = save_metadata(model_file, metadata)

        suggested_is_active = self.get_loaded_model_name(model_type) is None or result["accuracy"] >= 0.75
        if request.activateAfterTrain:
            self.set_active_model(model_file, model_type)

        return TrainResponse(
            tenModel=model_file,
            modelFile=model_file,
            modelPath=str(model_saved_path),
            metadataFile=str(meta_path.name),
            soLuongDuLieu=int(len(x)),
            doChinhXac=round(float(result["accuracy"]), 4),
            trainSize=int(result["train_size"]),
            testSize=int(result["test_size"]),
            ngayTao=datetime.utcnow(),
            moTaModel=model_description,
            modelType=model_type,
            featureList=FEATURE_COLUMNS,
            labelColumn=REASON_LABEL_COLUMN,
            featureImportance=result["feature_importance"],
            confusionMatrix=result["confusion_matrix"],
            confusionMatrixLabels=result["confusion_matrix_labels"],
            classificationReport=classification_report,
            precisionMacro=precision_macro,
            recallMacro=recall_macro,
            f1Macro=f1_macro,
            precisionWeighted=precision_weighted,
            recallWeighted=recall_weighted,
            f1Weighted=f1_weighted,
            classDistribution=result["class_distribution"],
            validRowsBeforeClassFilter=dataset_stats.valid_rows_before_class_filter,
            usedRows=dataset_stats.used_rows,
            accumulatingRows=dataset_stats.accumulating_rows,
            eligibleClassCount=dataset_stats.eligible_class_count,
            accumulatingClassCount=dataset_stats.accumulating_class_count,
            usedClassDistribution=dataset_stats.used_class_distribution,
            droppedClassDistribution=dataset_stats.dropped_class_distribution,
            decisionTreeText=result["decision_tree_text"],
            warningMessages=warning_messages,
            suggestedIsActive=suggested_is_active,
            createdAt=datetime.utcnow(),
            trainNote=request.trainNote,
            loaiModel=model_type,
        )

    def list_models(self, model_type: str | None = None, include_aliases: bool = False) -> list[dict]:
        models = list_models(include_aliases=include_aliases)
        if not model_type:
            return models

        normalized_type = self._normalize_model_type(model_type)
        return [x for x in models if str(x.get("loaiModel", "")).lower() == normalized_type.lower()]

    def get_model_detail(self, model_file: str) -> dict:
        _ = model_path(model_file)
        model_meta = load_metadata(model_file)
        if not model_meta:
            raise ValueError("Không tìm thấy metadata của model.")
        return model_meta

    def validate_model(self, request: ValidateModelRequest) -> ValidateModelResponse:
        expected_model_type = self._normalize_model_type(request.modelType) if request.modelType else MODEL_TYPE_REASON
        result = validate_model_file(request.modelFile, expected_model_type)
        return ValidateModelResponse(**result)

    def compare_models(self, request: CompareModelRequest):
        comparer = ModelCompareService()
        return comparer.compare_models(request)

    def set_active_model(self, model_file: str, model_type: str = MODEL_TYPE_REASON) -> dict:
        normalized_type = self._normalize_model_type(model_type)
        previous_model = self.get_loaded_model_name(normalized_type)
        validation = validate_model_file(model_file, normalized_type)
        if not validation["modelExists"]:
            raise ValueError("Model không tồn tại.")
        if not validation["canLoad"]:
            raise ValueError("Model không load được.")
        if not validation["schemaValid"]:
            raise ValueError("Model schema không hợp lệ.")

        copy_model_as_default(model_file, normalized_type)
        alias = default_alias_for_type(normalized_type)
        self._load_model_to_cache(alias, normalized_type)
        return {
            "activeModel": model_file,
            "loadedModel": self.get_loaded_model_name(normalized_type),
            "defaultAliasModel": alias,
            "activatedAt": datetime.utcnow(),
            "previousModel": previous_model,
            "modelType": normalized_type,
        }

    def reload_active_model(self, model_type: str = MODEL_TYPE_REASON) -> dict:
        normalized_type = self._normalize_model_type(model_type)
        alias = default_alias_for_type(normalized_type)
        if not model_path(alias).exists():
            self._clear_cache(normalized_type)
            raise ValueError(f"Chưa có model đang hoạt động cho loại {normalized_type}. Vui lòng huấn luyện model đầu tiên.")
        self._load_model_to_cache(alias, normalized_type)
        return {
            "loadedModel": self.get_loaded_model_name(normalized_type),
            "loadedAt": datetime.utcnow(),
            "success": True,
            "modelType": normalized_type,
        }

    def delete_model(self, model_file: str) -> dict:
        if is_active_alias_model(model_file):
            raise ValueError("Không được xóa trực tiếp alias active model.")

        if self.get_loaded_model_name(MODEL_TYPE_REASON) == model_file:
            raise ValueError(
                f"Model {model_file} đang hoạt động cho loại {MODEL_TYPE_REASON}. "
                "Vui lòng kích hoạt model khác hoặc deactivate trước khi xóa."
            )

        delete_model_files(model_file)

        if self.get_loaded_model_name(MODEL_TYPE_REASON) == model_file:
            self._clear_cache(MODEL_TYPE_REASON)

        return {"deletedModel": model_file}

    def export_metadata(self, model_file: str) -> dict:
        metadata = load_metadata(model_file)
        if not metadata:
            raise ValueError("Không tìm thấy metadata của model.")
        return metadata

    def predict_project(self, request: PredictProjectRequest):
        predictor = PredictionService()

        reason_model = None
        reason_model_name = None
        reason_feature_importance: dict[str, float] = {}
        try:
            reason_loaded_name = self.get_loaded_model_name(MODEL_TYPE_REASON)
            if reason_loaded_name:
                reason_model = self._require_loaded_model(MODEL_TYPE_REASON)
                reason_model_name = reason_loaded_name
                reason_feature_importance = self.get_loaded_metadata(MODEL_TYPE_REASON).get("feature_importance", {})
        except Exception:
            reason_model = None
            reason_model_name = None
            reason_feature_importance = {}

        return predictor.predict_project(
            request=request,
            reason_model=reason_model,
            reason_model_name=reason_model_name,
            reason_feature_importance=reason_feature_importance,
        )

    def test_predict(self, request: TestPredictRequest):
        predictor = PredictionService()

        if request.modelFile:
            model = load_model(request.modelFile)
            model_name = request.modelFile
            feature_importance = load_metadata(request.modelFile).get("feature_importance", {})
        else:
            model = self._require_loaded_model(MODEL_TYPE_REASON)
            model_name = self.get_loaded_model_name(MODEL_TYPE_REASON) or default_alias_for_type(MODEL_TYPE_REASON)
            feature_importance = self.get_loaded_metadata(MODEL_TYPE_REASON).get("feature_importance", {})

        return predictor.test_predict(
            feature=request.feature,
            model=model,
            model_name=model_name,
            danh_muc_nguyen_nhan=request.danhMucNguyenNhan,
            feature_importance=feature_importance,
        )

    @staticmethod
    def _resolve_original_model_name(alias_name: str, metadata: dict | None) -> str:
        if not metadata:
            return alias_name

        preferred_keys = (
            "sourceModelFile",
            "originalModelFile",
            "modelVersionFile",
            "model_file",
        )
        for key in preferred_keys:
            candidate = metadata.get(key)
            if isinstance(candidate, str) and candidate.strip():
                source_model_name = candidate.strip()
                try:
                    if model_path(source_model_name).exists():
                        return source_model_name
                except Exception:
                    continue
        return alias_name
