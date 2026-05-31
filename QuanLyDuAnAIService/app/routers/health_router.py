from __future__ import annotations

from datetime import datetime

from fastapi import APIRouter

from app.constants import SERVICE_VERSION
from app.schemas import HealthResponse, success_response
from app.services import model_service

router = APIRouter(tags=["Health"])


@router.get("/health")
def health_check():
    loaded_models = model_service.get_loaded_models()
    data = HealthResponse(
        serviceStatus="healthy",
        version=SERVICE_VERSION,
        loadedReasonModel=loaded_models.get("NguyenNhan"),
        checkedAt=datetime.utcnow(),
    )
    return success_response(data.model_dump())
