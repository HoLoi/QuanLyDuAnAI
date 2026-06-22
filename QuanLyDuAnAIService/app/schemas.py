from __future__ import annotations

from datetime import datetime
from typing import Any

from pydantic import BaseModel, ConfigDict, Field


class StrictBaseModel(BaseModel):
    model_config = ConfigDict(extra="forbid")


class ApiResponse(StrictBaseModel):
    success: bool
    message: str
    data: Any | None = None
    errors: list[str] = Field(default_factory=list)


def success_response(data: Any, message: str = "Success") -> dict[str, Any]:
    return ApiResponse(success=True, message=message, data=data, errors=[]).model_dump()


def error_response(message: str, errors: list[str] | None = None) -> dict[str, Any]:
    return ApiResponse(success=False, message=message, data=None, errors=errors or []).model_dump()


class DatasetRow(StrictBaseModel):
    MaDuAn: str | int | None = None
    SoNhanVienDuAn: float | None = None
    TongSoCongViec: float | None = None
    SoCongViecTre: float | None = None
    TyLeCongViecTre: float | None = None
    ChiPhiDuKien: float | None = None
    ChiPhiThucTe: float | None = None
    ChenhLechChiPhi: float | None = None
    SoLanThayDoiNhanSu: float | None = None
    SoLanThayDoiQuanLy: float | None = None
    SoNgayTreTienDo: float | None = None
    SoDeXuatCongViecChoDuyet: float | None = None
    SoDeXuatCongViecBiTuChoi: float | None = None
    ThoiGianDuyetCongViecTrungBinh: float | None = None
    SoDeXuatNganSachChoDuyet: float | None = None
    SoDeXuatNganSachBiTuChoi: float | None = None
    ThoiGianDuyetNganSachTrungBinh: float | None = None
    SoBaoCaoTienDoChoDuyet: float | None = None
    SoBaoCaoTienDoBiTuChoi: float | None = None
    SoBaoCaoTienDoYeuCauBoSung: float | None = None
    TyLeBaoCaoTienDoBiTuChoi: float | None = None
    SoLanCapNhatTienDo: float | None = None
    SoNgayChamCapNhatTienDo: float | None = None
    LaDuAnTre: int | bool | None = None
    MaDMNguyenNhan: int | None = None
    NgayTongHop: datetime | None = None
    GhiChuDataset: str | None = None


class DatasetValidateRequest(StrictBaseModel):
    dataset: list[DatasetRow]


class DatasetValidateResponse(StrictBaseModel):
    tongSoDong: int
    soDongHopLe: int
    soDongThieuDuLieu: int
    soDongDuAnTre: int
    soDongCoNguyenNhan: int
    validRowsBeforeClassFilter: int = 0
    usedRows: int = 0
    accumulatingRows: int = 0
    eligibleClassCount: int = 0
    accumulatingClassCount: int = 0
    canhBaoDataset: list[str] = Field(default_factory=list)


class DatasetQualityReportResponse(StrictBaseModel):
    tongSoDong: int
    soDongHopLe: int
    soDongThieuLabel: int
    soDongThieuFeature: int
    tyLeNullTungFeature: dict[str, float]
    tyLeClass: dict[str, float]
    validRowsBeforeClassFilter: int = 0
    usedRows: int = 0
    accumulatingRows: int = 0
    eligibleClassCount: int = 0
    accumulatingClassCount: int = 0
    usedClassDistribution: dict[str, int] = Field(default_factory=dict)
    droppedClassDistribution: dict[str, int] = Field(default_factory=dict)
    duTrainHayKhong: bool
    warningMessages: list[str]
    blockingErrors: list[str]


class TrainRecommendationResponse(StrictBaseModel):
    nenTrain: bool
    lyDo: str
    canhBaoImbalance: bool
    canhBaoThieuDuLieu: bool
    recommendationMessages: list[str]


class TrainRequest(StrictBaseModel):
    dataset: list[DatasetRow]
    requestedByUserId: str | int | None = None
    requestedByUserName: str | None = None
    trainNote: str | None = None
    activateAfterTrain: bool = False
    modelType: str = "NguyenNhan"


class TrainResponse(StrictBaseModel):
    tenModel: str
    modelFile: str
    modelPath: str
    metadataFile: str
    soLuongDuLieu: int
    doChinhXac: float
    trainSize: int
    testSize: int
    ngayTao: datetime
    moTaModel: str
    modelType: str
    featureList: list[str] = Field(default_factory=list)
    labelColumn: str
    featureImportance: dict[str, float]
    confusionMatrix: list[list[int]]
    confusionMatrixLabels: list[int] = Field(default_factory=list)
    classificationReport: dict[str, Any] = Field(default_factory=dict)
    precisionMacro: float | None = None
    recallMacro: float | None = None
    f1Macro: float | None = None
    precisionWeighted: float | None = None
    recallWeighted: float | None = None
    f1Weighted: float | None = None
    classDistribution: dict[str, int]
    validRowsBeforeClassFilter: int = 0
    usedRows: int = 0
    accumulatingRows: int = 0
    eligibleClassCount: int = 0
    accumulatingClassCount: int = 0
    usedClassDistribution: dict[str, int] = Field(default_factory=dict)
    droppedClassDistribution: dict[str, int] = Field(default_factory=dict)
    decisionTreeText: str | None = None
    warningMessages: list[str]
    suggestedIsActive: bool
    createdAt: datetime
    trainNote: str | None = None
    loaiModel: str = "NguyenNhan"


class ModelInfoResponse(StrictBaseModel):
    tenFile: str
    dungLuong: int
    createdAt: datetime
    updatedAt: datetime
    isDefault: bool
    canLoad: bool
    algorithm: str | None = None
    expectedFeatures: list[str] = Field(default_factory=list)
    schemaValid: bool = False
    canActivate: bool = False
    validationMessage: str | None = None
    accuracy: float | None = None
    trainSize: int | None = None
    testSize: int | None = None
    classDistribution: dict[str, int] = Field(default_factory=dict)
    featureImportance: dict[str, float] = Field(default_factory=dict)
    confusionMatrix: list[list[int]] = Field(default_factory=list)
    confusionMatrixLabels: list[int] = Field(default_factory=list)
    precisionMacro: float | None = None
    recallMacro: float | None = None
    f1Macro: float | None = None
    precisionWeighted: float | None = None
    recallWeighted: float | None = None
    f1Weighted: float | None = None
    decisionTreeText: str | None = None
    loaiModel: str = "NguyenNhan"


class ValidateModelRequest(StrictBaseModel):
    modelFile: str
    modelType: str | None = None


class ValidateModelResponse(StrictBaseModel):
    modelExists: bool
    canLoad: bool
    expectedFeatures: list[str]
    schemaValid: bool
    errors: list[str]


class CompareModelRequest(StrictBaseModel):
    currentModelFile: str
    newModelFile: str
    testDataset: list[DatasetRow]
    modelType: str = "NguyenNhan"


class CompareModelResponse(StrictBaseModel):
    currentAccuracy: float
    newAccuracy: float
    differenceAccuracy: float
    confusionMatrixCurrent: list[list[int]]
    confusionMatrixNew: list[list[int]]
    recommendation: str


class DanhMucNguyenNhanItem(StrictBaseModel):
    maDMNguyenNhan: str | int
    tenNguyenNhan: str


class PredictFeatureInput(StrictBaseModel):
    SoNhanVienDuAn: float
    TongSoCongViec: float
    SoCongViecTre: float
    TyLeCongViecTre: float
    ChiPhiDuKien: float
    ChiPhiThucTe: float
    ChenhLechChiPhi: float
    SoLanThayDoiNhanSu: float
    SoLanThayDoiQuanLy: float
    SoNgayTreTienDo: float
    SoDeXuatCongViecChoDuyet: float
    SoDeXuatCongViecBiTuChoi: float
    ThoiGianDuyetCongViecTrungBinh: float
    SoDeXuatNganSachChoDuyet: float
    SoDeXuatNganSachBiTuChoi: float
    ThoiGianDuyetNganSachTrungBinh: float
    SoBaoCaoTienDoChoDuyet: float
    SoBaoCaoTienDoBiTuChoi: float
    SoBaoCaoTienDoYeuCauBoSung: float
    TyLeBaoCaoTienDoBiTuChoi: float
    SoLanCapNhatTienDo: float
    SoNgayChamCapNhatTienDo: float


class PredictProjectRequest(StrictBaseModel):
    maDuAn: str | int
    feature: PredictFeatureInput
    danhMucNguyenNhan: list[DanhMucNguyenNhanItem] | None = None
    reasonConfidenceThreshold: float | None = None


class PredictProjectResponse(StrictBaseModel):
    maDMNguyenNhanDuDoan: int | None = None
    tenNguyenNhanDuDoan: str | None = None
    doTinCayKetQua: float
    mucPhuHop: str | None = None
    danhSachNguyenNhanLienQuan: list["RelatedReasonItem"] | None = None
    reasonSource: str
    modelNguyenNhanUsed: str | None = None
    canhBaoNguyenNhan: str | None = None
    noiDungPhanTich: str | None = None


class RelatedReasonItem(StrictBaseModel):
    maDMNguyenNhan: int | None = None
    tenNguyenNhan: str
    score: float
    mucPhuHop: str


class TestPredictRequest(StrictBaseModel):
    modelFile: str | None = None
    feature: PredictFeatureInput
    danhMucNguyenNhan: list[DanhMucNguyenNhanItem] | None = None


class TestPredictResponse(StrictBaseModel):
    confidence: float
    suggestedReasonCode: int | None = None
    suggestedReason: str
    explanation: str
    modelUsed: str
    reasonSource: str


class HealthResponse(StrictBaseModel):
    serviceStatus: str
    version: str
    loadedReasonModel: str | None
    checkedAt: datetime
