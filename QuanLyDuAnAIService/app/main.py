from __future__ import annotations

import logging

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app import __version__
from app.config import settings
from app.routers.admin_router import router as admin_router
from app.routers.health_router import router as health_router
from app.routers.model_router import router as model_router
from app.routers.prediction_router import router as prediction_router
from app.services import model_service

logger = logging.getLogger("QuanLyDuAnAIService")
logging.basicConfig(level=logging.INFO, format="%(asctime)s %(levelname)s %(name)s %(message)s")

app = FastAPI(
    title="QuanLyDuAn AI FastAPI Service",
    version=__version__,
    description=(
        "Microservice AI hỗ trợ phân tích nguyên nhân trễ dự án cho hệ thống QuanLyDuAn. "
        "Service này chỉ xử lý AI compute và không kết nối DB."
    ),
    openapi_tags=[
        {"name": "Health", "description": "Trạng thái service"},
        {"name": "Dataset", "description": "Validate và quality dữ liệu"},
        {"name": "Models", "description": "Train và quản lý model nguyên nhân"},
        {"name": "Prediction", "description": "Phân tích nguyên nhân trễ dự án"},
        {"name": "Admin", "description": "Công cụ quản trị AI"},
    ],
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.allow_origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.on_event("startup")
def startup_event():
    loaded_models = model_service.startup_load_default_models()
    logger.info(
        "Khởi động AI service với model nguyên nhân đang nạp: %s",
        loaded_models.get("NguyenNhan"),
    )


app.include_router(health_router)
app.include_router(model_router)
app.include_router(prediction_router)
app.include_router(admin_router)
