from __future__ import annotations

from fastapi import APIRouter
from fastapi.responses import JSONResponse

from app.schemas import (
    CompareModelRequest,
    DatasetValidateRequest,
    TestPredictRequest,
    ValidateModelRequest,
    error_response,
    success_response,
)
from app.services import admin_ai_service, model_service, validation_service

router = APIRouter(prefix="/admin", tags=["Admin"])


@router.get("/ai-status")
def ai_status():
    try:
        data = admin_ai_service.get_ai_status()
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi lấy AI status.", [str(ex)]))


@router.post("/dataset/quality-report")
def dataset_quality_report(request: DatasetValidateRequest):
    try:
        data = validation_service.quality_report(request)
        return success_response(data.model_dump())
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi quality report.", [str(ex)]))


@router.post("/model/train-recommendation")
def train_recommendation(request: DatasetValidateRequest):
    try:
        data = validation_service.train_recommendation(request)
        return success_response(data.model_dump())
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi train recommendation.", [str(ex)]))


@router.post("/model/validate")
def validate_model(request: ValidateModelRequest):
    try:
        data = model_service.validate_model(request)
        return success_response(data.model_dump())
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi validate model.", [str(ex)]))


@router.post("/model/compare")
def compare_model(request: CompareModelRequest):
    try:
        data = model_service.compare_models(request)
        return success_response(data.model_dump())
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi compare model.", [str(ex)]))


@router.post("/model/set-active")
def set_active_model(request: ValidateModelRequest):
    try:
        data = model_service.set_active_model(request.modelFile, request.modelType or "NguyenNhan")
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi set active model.", [str(ex)]))


@router.post("/model/reload")
def reload_active_model(modelType: str = "NguyenNhan"):
    try:
        data = model_service.reload_active_model(modelType)
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi reload model.", [str(ex)]))


@router.delete("/model/{model_file}")
def delete_model_local(model_file: str):
    try:
        data = model_service.delete_model(model_file)
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi xóa model.", [str(ex)]))


@router.get("/model/export-metadata/{model_file}")
def export_model_metadata(model_file: str):
    try:
        data = model_service.export_metadata(model_file)
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi export metadata.", [str(ex)]))


@router.get("/model/{model_file}")
def model_detail(model_file: str):
    try:
        data = model_service.get_model_detail(model_file)
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi lấy chi tiết model.", [str(ex)]))


@router.post("/model/test-reason")
def test_reason(request: TestPredictRequest):
    try:
        data = model_service.test_predict(request)
        return success_response(data.model_dump())
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi test reason.", [str(ex)]))


@router.post("/model/test-predict")
def test_predict_legacy(request: TestPredictRequest):
    return test_reason(request)


@router.get("/logs/summary")
def logs_summary():
    try:
        data = admin_ai_service.get_logs_summary()
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi lấy tổng hợp log.", [str(ex)]))


@router.get("/system-info")
def system_info():
    try:
        data = admin_ai_service.get_system_info()
        return success_response(data)
    except Exception as ex:
        return JSONResponse(status_code=400, content=error_response("Lỗi lấy system info.", [str(ex)]))
