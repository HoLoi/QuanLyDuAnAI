from __future__ import annotations

from sklearn.metrics import accuracy_score, confusion_matrix

from app.constants import DELAY_STATUS_COLUMN, REASON_LABEL_COLUMN
from app.ml.feature_builder import build_training_frames, rows_to_dicts
from app.ml.model_storage import load_model
from app.schemas import CompareModelRequest, CompareModelResponse


class ModelCompareService:
    def compare_models(self, request: CompareModelRequest) -> CompareModelResponse:
        records = rows_to_dicts(request.testDataset)
        filtered: list[dict] = []
        for row in records:
            la_du_an_tre = row.get(DELAY_STATUS_COLUMN)
            if la_du_an_tre in (1, True):
                filtered.append(row)

        if not filtered:
            raise ValueError("Không có dữ liệu dự án trễ để so sánh model.")

        x_test, y_test = build_training_frames(filtered, label_column=REASON_LABEL_COLUMN)

        current_model = load_model(request.currentModelFile)
        new_model = load_model(request.newModelFile)

        current_pred = current_model.predict(x_test)
        new_pred = new_model.predict(x_test)

        current_accuracy = float(accuracy_score(y_test, current_pred))
        new_accuracy = float(accuracy_score(y_test, new_pred))
        diff = new_accuracy - current_accuracy

        labels_sorted = sorted(int(v) for v in y_test.unique().tolist())
        current_cm = confusion_matrix(y_test, current_pred, labels=labels_sorted).tolist()
        new_cm = confusion_matrix(y_test, new_pred, labels=labels_sorted).tolist()

        if diff > 0.02:
            recommendation = "Nên chuyển sang model mới vì độ chính xác cải thiện rõ."
        elif diff < -0.02:
            recommendation = "Nên giữ model hiện tại vì model mới kém hơn."
        else:
            recommendation = "Chênh lệch nhỏ, cân nhắc thêm dữ liệu trước khi đổi model."

        return CompareModelResponse(
            currentAccuracy=round(current_accuracy, 4),
            newAccuracy=round(new_accuracy, 4),
            differenceAccuracy=round(diff, 4),
            confusionMatrixCurrent=current_cm,
            confusionMatrixNew=new_cm,
            recommendation=recommendation,
        )
