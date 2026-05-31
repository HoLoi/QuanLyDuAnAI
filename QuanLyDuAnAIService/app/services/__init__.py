from app.services.log_service import LogService
from app.services.model_service import ModelService
from app.services.validation_service import ValidationService
from app.services.admin_ai_service import AdminAIService

log_service = LogService()
model_service = ModelService()
validation_service = ValidationService()
admin_ai_service = AdminAIService(model_service=model_service, log_service=log_service)

