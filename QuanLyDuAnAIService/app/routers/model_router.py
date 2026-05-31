from __future__ import annotations

from time import perf_counter

from fastapi import APIRouter
from fastapi.responses import JSONResponse

from app.constants import EVENT_TRAIN
from app.schemas import TrainRequest, error_response, success_response
from app.services import log_service, model_service

router = APIRouter(tags=["Models"])


@router.post("/model/train")
def train_model(request: TrainRequest):
    start = perf_counter()
    try:
        result = model_service.train_model(request)
        duration = int((perf_counter() - start) * 1000)
        log_service.log_event(
            endpoint="/model/train",
            event_type=EVENT_TRAIN,
            success=True,
            duration_ms=duration,
            model_name=result.modelFile,
            extra={"soLuongDuLieu": result.soLuongDuLieu, "doChinhXac": result.doChinhXac},
        )
        return success_response(result.model_dump())
    except Exception as ex:
        duration = int((perf_counter() - start) * 1000)
        log_service.log_event(
            endpoint="/model/train",
            event_type=EVENT_TRAIN,
            success=False,
            duration_ms=duration,
            error=str(ex),
        )
        return JSONResponse(status_code=400, content=error_response("Lỗi train model.", [str(ex)]))


@router.get("/model/list")
def get_model_list(modelType: str | None = None, includeAliases: bool = False):
    try:
        data = model_service.list_models(modelType, include_aliases=includeAliases)
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi lay danh sach model.", [str(ex)]))


