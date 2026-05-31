from __future__ import annotations

import platform
from datetime import datetime
from time import perf_counter

import pandas as pd
import sklearn

from app.config import settings
from app.constants import SERVICE_VERSION
from app.ml.model_storage import list_models, model_path
from app.services.log_service import LogService
from app.services.model_service import ModelService


class AdminAIService:
    def __init__(self, model_service: ModelService, log_service: LogService) -> None:
        self._model_service = model_service
        self._log_service = log_service
        self._started_at_perf = perf_counter()

    def get_ai_status(self) -> dict:
        reason_alias_path = model_path(settings.default_reason_model_name)
        models = list_models(include_aliases=True)
        loaded_models = self._model_service.get_loaded_models()
        loaded_reason_model = loaded_models.get("NguyenNhan")

        loaded_reason_model_in_local_list = False
        if loaded_reason_model:
            loaded_reason_model_in_local_list = any(
                str(item.get("tenFile", "")).strip().lower() == loaded_reason_model.strip().lower()
                for item in models
            )

        return {
            "serviceStatus": "running",
            "serviceVersion": SERVICE_VERSION,
            "defaultReasonModelName": settings.default_reason_model_name,
            "defaultReasonModelExists": reason_alias_path.exists(),
            "totalLocalModels": len(models),
            "loadedReasonModel": loaded_reason_model,
            "loadedReasonModelInLocalList": loaded_reason_model_in_local_list,
            "modelDir": str(settings.model_dir),
            "minReasonTrainRows": settings.min_reason_train_rows,
            "minReasonClassCount": settings.min_reason_class_count,
            "minReasonRowsPerClass": settings.min_reason_rows_per_class,
            "reasonConfidenceThreshold": settings.reason_confidence_threshold,
            "checkedAt": datetime.utcnow(),
        }

    def get_logs_summary(self) -> dict:
        return self._log_service.get_summary()

    def get_system_info(self) -> dict:
        uptime_seconds = int(perf_counter() - self._started_at_perf)
        memory_usage = None
        try:
            import psutil  # type: ignore

            process = psutil.Process()
            memory_usage = int(process.memory_info().rss)
        except Exception:
            memory_usage = None

        loaded_models = self._model_service.get_loaded_models()
        return {
            "pythonVersion": platform.python_version(),
            "sklearnVersion": sklearn.__version__,
            "pandasVersion": pd.__version__,
            "uptimeSeconds": uptime_seconds,
            "loadedReasonModel": loaded_models.get("NguyenNhan"),
            "memoryUsageBytes": memory_usage,
        }
