from __future__ import annotations

from time import perf_counter

from fastapi import APIRouter
from fastapi.responses import JSONResponse

from app.constants import EVENT_ANALYZE
from app.schemas import DatasetValidateRequest, PredictProjectRequest, error_response, success_response
from app.services import log_service, model_service, validation_service

router = APIRouter(tags=["Prediction"])


@router.post("/dataset/validate", tags=["Dataset"])
def validate_dataset(request: DatasetValidateRequest):
    start = perf_counter()
    try:
        result = validation_service.validate_dataset(request)
        duration = int((perf_counter() - start) * 1000)
        log_service.log_event(
            endpoint="/dataset/validate",
            event_type=EVENT_ANALYZE,
            success=True,
            duration_ms=duration,
        )
        return success_response(result.model_dump())
    except Exception as ex:
        duration = int((perf_counter() - start) * 1000)
        log_service.log_event(
            endpoint="/dataset/validate",
            event_type=EVENT_ANALYZE,
            success=False,
            duration_ms=duration,
            error=str(ex),
        )
        return JSONResponse(status_code=400, content=error_response("Lỗi validate dataset.", [str(ex)]))


@router.post("/analyze/delay-reason")
def analyze_delay_reason(request: PredictProjectRequest):
    start = perf_counter()
    try:
        result = model_service.predict_project(request)
        duration = int((perf_counter() - start) * 1000)
        log_service.log_event(
            endpoint="/analyze/delay-reason",
            event_type=EVENT_ANALYZE,
            success=True,
            duration_ms=duration,
            model_name=result.modelNguyenNhanUsed,
        )
        return success_response(result.model_dump())
    except Exception as ex:
        duration = int((perf_counter() - start) * 1000)
        log_service.log_event(
            endpoint="/analyze/delay-reason",
            event_type=EVENT_ANALYZE,
            success=False,
            duration_ms=duration,
            model_name=model_service.get_loaded_model_name("NguyenNhan"),
            error=str(ex),
        )
        return JSONResponse(status_code=400, content=error_response("Lỗi phân tích nguyên nhân trễ.", [str(ex)]))


@router.post("/predict/project")
def predict_project_legacy(request: PredictProjectRequest):
    return analyze_delay_reason(request)
