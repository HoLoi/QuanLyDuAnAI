# 1. System Overview

The current system is an enterprise-style project management platform that combines operational management and AI-assisted delay cause analysis. Its primary objective is to manage end-to-end project execution (planning, assignment, progress tracking, budget/cost control, team/member operations, evaluation, and audit logging) while adding AI support for analyzing delay causes in completed historical projects.

The implemented user groups include system roles (`Admin`, `Manager`, `Employee`) and project/team leadership scope (`Leader`) represented through project/team membership roles. Core modules include project management (`DU_AN`), task and task-detail management (`CONG_VIEC`, `CT_CONG_VIEC`), progress reporting and approval (`TIEN_DO_CONG_VIEC`), task/budget proposals and approvals (`DE_XUAT_CONG_VIEC`, `DE_XUAT_NGAN_SACH`), budget/cost (`NGAN_SACH`, `CHI_PHI`), membership/team allocation (`NHAN_VIEN_DU_AN`, `TEAM`, `TEAM_DU_AN`), evaluations (`DANH_GIA_DU_AN`, `DANH_GIA_NHAN_VIEN`), chat (`PHONG_CHAT`, `TIN_NHAN`), logs (`NHAT_KY_*`), and AI modules (`AI_DATASET`, `AI_MODEL`, `AI_KET_QUA`, `AI_NGUYEN_NHAN`, `DM_NGUYEN_NHAN`).

# 2. System Architecture

The architecture is split into three layers: ASP.NET MVC application, SQL Server database, and FastAPI AI service.

- ASP.NET MVC acts as the orchestration and business layer: authentication/authorization, workflow enforcement, status transitions, data aggregation, validation, and persistence.
- SQL Server stores all operational and AI-related business records.
- FastAPI is an AI compute microservice for dataset validation, model training, model utilities, and delay-cause analysis.

Boundary evidence in source/docs indicates:

- MVC is the system-of-record for operational and AI business tables.
- FastAPI is compute-only and does not write directly to operational SQL tables.
- AI integration is contract-based (HTTP endpoints), while final data persistence remains in MVC.

# 3. Workflow and Business Process

Project workflow is explicitly state-driven (`KhoiTao`, `DangThucHien`, `ChoXacNhanHoanThanh`, `HoanThanh`, `TamDung`, with locked/end states including `LuuTru`/`DaHuy`). Projects can auto-transition from `KhoiTao` to `DangThucHien` when prerequisites are satisfied, request completion, be confirmed complete, paused, and reopened with reason logging.

Task/task-detail workflow is synchronized through a central status-sync service. Task-detail status drives parent task status; task status then drives project status. Progress reports are submitted as approval records (`ChoDuyet`, `DaDuyet`, `YeuCauBoSung`, `TuChoi`). Only approved progress updates write effective task-detail status; afterward, chain synchronization updates task and project states.

Proposal workflows are implemented for both work and budget: creation/submission, manager approval/rejection, and state transitions (`ChoDuyet`, `DaDuyet`, `TuChoi`, `DaHuy`). Approved work proposals materialize operational tasks/cost entries; approved budget proposals create a new approved active budget version.

Completion confirmation is human-controlled: tasks and projects move to final completion only after required checks. The system also supports rollback/reopen logic when new incomplete items appear after intermediate completion states.

# 4. Role and Permission Model

The platform uses claim-based authorization with permission keys (e.g., `DuAn.*`, `CongViec.*`, `TienDo.*`, `AI.*`, `ThongKe.*`). Claims are aggregated from role claims and user claims at login time.

Role and scope behavior is layered:

- `Admin`: broad administrative visibility; some business-operation flows explicitly restrict direct admin operational actions (e.g., progress/evaluation operations in service logic).
- `Manager`: primary approval and control authority within managed project scope.
- `Employee`: execution/reporting scope based on assignment and membership.
- `Leader`: project/team leadership scope derived from `VaiTroTrongDuAn` / team leadership records, not seeded as a standalone default system role.

AI-related permissions include `AI.Dataset`, `AI.Train`, `AI.PhanTichNguyenNhan`, `AI.Dashboard`, and `AI.XacNhan`. Scope checks in services further restrict actions to relevant project context.

# 5. Project Data and Operational Features

Operational data managed by the current system includes:

- projects (`DU_AN`)
- tasks (`CONG_VIEC`)
- task details (`CT_CONG_VIEC`)
- progress reports (`TIEN_DO_CONG_VIEC`)
- budget and budget proposals (`NGAN_SACH`, `DE_XUAT_NGAN_SACH`)
- cost (`CHI_PHI`)
- project members and teams (`NHAN_VIEN_DU_AN`, `TEAM`, `TEAM_DU_AN`)
- evaluations (`DANH_GIA_DU_AN`, `DANH_GIA_NHAN_VIEN`)
- chat (`PHONG_CHAT`, `TIN_NHAN`, `THANH_VIEN_PHONG_CHAT`)
- logs/audit (`NHAT_KY_DU_AN`, `NHAT_KY_QUAN_LY_DU_AN`, `NHAT_KY_PHU_TRACH_DU_AN`, and related log tables)

Data most directly linked to delay cause analysis includes schedule deviation, delayed tasks, progress approval history, cost overrun patterns, personnel/manager changes, and project staffing size.

# 6. AI Integration

AI is integrated as decision support for delay cause analysis, not as autonomous final decision logic.

- MVC aggregates operational data into `AI_DATASET`.
- MVC requests FastAPI to train/analyze.
- Analysis outputs are stored in `AI_KET_QUA`.
- Human confirmation is stored in `AI_NGUYEN_NHAN`, and confirmed labels are synchronized back to `AI_DATASET.MaDMNguyenNhan`.

Manager confirmation is a required governance step for ground-truth labeling. `DM_NGUYEN_NHAN` provides the controlled reason catalog.

# 7. AI Dataset and Features

Current training/analyze feature set is 22 fields:

`SoNhanVienDuAn`, `TongSoCongViec`, `SoCongViecTre`, `TyLeCongViecTre`, `ChiPhiDuKien`, `ChiPhiThucTe`, `ChenhLechChiPhi`, `SoLanThayDoiNhanSu`, `SoLanThayDoiQuanLy`, `SoNgayTreTienDo`, `SoDeXuatCongViecChoDuyet`, `SoDeXuatCongViecBiTuChoi`, `ThoiGianDuyetCongViecTrungBinh`, `SoDeXuatNganSachChoDuyet`, `SoDeXuatNganSachBiTuChoi`, `ThoiGianDuyetNganSachTrungBinh`, `SoBaoCaoTienDoChoDuyet`, `SoBaoCaoTienDoBiTuChoi`, `SoBaoCaoTienDoYeuCauBoSung`, `TyLeBaoCaoTienDoBiTuChoi`, `SoLanCapNhatTienDo`, `SoNgayChamCapNhatTienDo`.

Feature sources are operational tables and logs (project/task/progress/proposal/budget/cost/member/change logs). The label for reason-model training is `MaDMNguyenNhan`, used only for delayed projects (`LaDuAnTre=1`).

Training-valid rows require delayed status, valid reason label, and full required feature completeness. Integration rules explicitly state that `AI_KET_QUA` must not be used as ground truth; manager-confirmed reasons in `AI_NGUYEN_NHAN` are the label source synchronized to `AI_DATASET`.

# 8. AI Model and Prediction/Analysis Flow

The implemented model family in current FastAPI source is `DecisionTreeClassifier` (reason model type `NguyenNhan`).

FastAPI contracts are strict (`StrictBaseModel(extra="forbid")`) and expose endpoints for health, dataset checks, training, model management, and delay-reason analysis. MVC calls AI service for:

- dataset validation/quality checks,
- training (`/model/train`),
- reason analysis (`/analyze/delay-reason`),
- model operations (list/validate/compare/activate/reload/test).

MVC does not call reason analysis for non-delayed projects (`LaDuAnTre != true`). Analysis results are persisted in `AI_KET_QUA`, while manager-confirmed final reason is persisted in `AI_NGUYEN_NHAN`. Confidence and warning-related fields are present (`DoTinCayKetQua`, `CanhBaoNguyenNhan`, `NoiDungPhanTich`, `ReasonSource`), with `RuleFallback` used when confidence/consistency constraints are not satisfied.

# 9. Delay Cause Analysis Logic

Project delay determination in dataset aggregation is rule-based in MVC. A project can be marked delayed (`LaDuAnTre=true`) when one or more conditions are met, including overdue completion, delayed tasks, high delayed-task ratio threshold, or positive delay-day indicators.

The implemented delay-related factors include:

- delayed task count (`SoCongViecTre`)
- delayed task ratio (`TyLeCongViecTre`)
- planned vs actual cost (`ChiPhiDuKien`, `ChiPhiThucTe`, `ChenhLechChiPhi`)
- personnel change count (`SoLanThayDoiNhanSu`)
- manager-change count (`SoLanThayDoiQuanLy`)
- delay-day indicators (`SoNgayTreTienDo`)
- project staffing size (`SoNhanVienDuAn`)

Manager confirmation finalizes the business-accepted cause in `AI_NGUYEN_NHAN`; `DM_NGUYEN_NHAN` is the canonical reason dictionary.

# 10. Explainability, Human Confirmation, and Trustworthiness

Current system behavior supports a human-in-the-loop pattern:

- AI outputs are recommendations/support signals.
- Final business confirmation is performed by authorized human users (manager scope and claims).
- Confirmed reason is explicitly stored and traceable.

Trust-related artifacts in current source include confidence values, warning text, reason-source markers (`NguyenNhanModel` vs `RuleFallback`), and stored analysis text. This provides a practical explainability baseline, although full formal explainability frameworks are not yet implemented as dedicated subsystems.

# 11. Dashboard, Reporting, and Monitoring

Dashboard/reporting currently provides aggregated monitoring for project and execution health, including project counts by status, completion timeliness, task delays, budget/cost totals, budget utilization, over-budget projects, pending proposals, manager-change requests, workload pressure (`NhanSuQuaTai`), and projects lacking AI dataset rows.

These metrics support delay/risk monitoring by combining schedule, resource, and financial signals, with export capability guarded by `ThongKe.XuatFile`.

# 12. Data Quality and Training Conditions

Current train-gating conditions are explicit in MVC/FastAPI:

- minimum valid rows: 30 (`MIN_REASON_TRAIN_ROWS`)
- minimum class count: 2 (`MIN_REASON_CLASS_COUNT`)
- minimum rows per class: 5 (`MIN_REASON_ROWS_PER_CLASS`)
- valid train row: delayed project + valid reason label + complete feature vector.

Dataset quality checks also report missing labels/features and class distribution imbalance warnings. Source/docs also indicate a key data-governance rule: avoid using prediction outputs (`AI_KET_QUA`) as training labels.

# 13. Research Relevance

This system is directly relevant to AI-assisted project management research because it integrates operational workflow data with a production-like human-governed AI loop for delay cause analysis. It concretely supports studies on project delay analysis, delay root-cause modeling, workflow-aware monitoring, and human-in-the-loop decision support. Its tree-based tabular ML implementation (Decision Tree) and structured operational features (schedule, cost, staffing, approvals, progress) also make it suitable for empirical project risk analysis in enterprise process settings.

# 14. Potential Research Gaps and Future Work

**Current system:** reason-focused analysis with Decision Tree-based FastAPI inference, MVC-governed workflow, and manager-confirmed labels.

**Potential future work (not currently implemented):**

- dynamic delay prediction (time-evolving forecasts, not only reason analysis)
- process mining over structured workflow event logs
- richer workflow event-log engineering for temporal causality
- uncertainty estimation/calibration beyond raw confidence scores
- deeper explainable AI methods (e.g., local/global explanation modules)
- graph-based dependency analysis across tasks/teams/work items
- sequence/graph models (e.g., LSTM/GNN) for longitudinal dependency-aware prediction
- broader model benchmarking (Decision Tree vs Random Forest vs XGBoost) under controlled datasets
- larger, cleaner, and more balanced `AI_DATASET` construction pipelines

# 15. Business Motivation

**Current System.** The implemented platform addresses practical limitations of traditional project management in multi-actor workflows. In this system, project control depends on many interdependent operational streams (tasks, task details, progress approvals, budget proposals, staffing changes, and manager-change requests). Without centralized integration, identifying why delays occur is difficult because evidence is distributed across multiple tables and process states.

Manual monitoring alone is also limited in this context: decision makers must continuously reconcile status transitions, approval history, budget consumption, and staffing volatility. The existing system therefore integrates AI into the established workflow to support delay-cause interpretation from operational data, while preserving human managerial control for final business confirmation.

**Potential Future Work.** Broader motivation extensions (e.g., autonomous decisioning or predictive scheduling automation) are not part of the currently implemented workflow.

# 16. Delay Cause Taxonomy

**Current System.** The delay-cause taxonomy is stored in `DM_NGUYEN_NHAN`, used by AI outputs (`AI_KET_QUA`) and manager confirmation (`AI_NGUYEN_NHAN`). Based on SQL seed/default initialization and AI docs, the active taxonomy is a 10-category scheme:

1. **Lack of Personnel** (`Thiếu nhân sự`): delay associated with insufficient or unstable staffing capacity.
2. **Continuous Requirement Changes** (`Thay đổi yêu cầu liên tục`): delay pressure caused by changing requirements.
3. **Approval/Process Delay** (`Chậm phê duyệt` or normalized label `Quy trình xử lý chậm`): delay linked to approval/processing bottlenecks.
4. **Budget Overrun** (`Vượt ngân sách`): delay context associated with budget over-consumption.
5. **Technical Risk** (`Rủi ro kỹ thuật`): delay due to technical uncertainty/implementation risk.
6. **Dependency/Coordination Delay** (`Công việc phụ thuộc bị chậm` or normalized label `Phối hợp công việc chưa tốt`): delay propagated from weak inter-task coordination.
7. **Insufficient Input Information** (`Thiếu dữ liệu hoặc tài liệu` or normalized label `Thông tin đầu vào chưa đầy đủ`): delay due to incomplete input artifacts.
8. **Inaccurate Time Estimation** (`Ước lượng thời gian chưa chính xác`): delay from planning estimation error.
9. **Incomplete Progress Updating** (`Tiến độ cập nhật không đầy đủ`): delay linked to weak progress update discipline.
10. **Other** (`Khác`): residual category when no dominant predefined cause is confirmed.

`AI_NGUYEN_NHAN` records the manager-confirmed cause per delayed project and is synchronized back to `AI_DATASET.MaDMNguyenNhan` for trustworthy training labels.

**Potential Future Work.** Hierarchical or multi-label taxonomy structures are not implemented in the current system.

# 17. Feature Source Mapping

**Current System.** `AI_DATASET` currently uses 22 features. Their sources in `AiDatasetService` map to operational tables as follows:

| Feature | Source Table | Description |
| ------- | ------------ | ----------- |
| `SoNhanVienDuAn` | `NHAN_VIEN_DU_AN` | Distinct project member count per project. |
| `TongSoCongViec` | `CONG_VIEC` + `DANH_MUC_CONG_VIEC` | Total task count under the project. |
| `SoCongViecTre` | `CONG_VIEC` + `DANH_MUC_CONG_VIEC` | Number of overdue tasks (finished late or still overdue). |
| `TyLeCongViecTre` | Derived from task aggregates | Delayed-task ratio computed from `SoCongViecTre / TongSoCongViec`. |
| `ChiPhiDuKien` | `NGAN_SACH` | Planned budget total from active approved budget rows. |
| `ChiPhiThucTe` | `CHI_PHI` + `NGAN_SACH` | Actual cost total aggregated via project budgets. |
| `ChenhLechChiPhi` | Derived from budget/cost aggregates | Cost gap computed as `ChiPhiThucTe - ChiPhiDuKien`. |
| `SoLanThayDoiNhanSu` | `NHAT_KY_PHU_TRACH_DU_AN` | Personnel-change count inferred from validated assignment-change actions. |
| `SoLanThayDoiQuanLy` | `YEU_CAU_DOI_QUAN_LY` | Approved manager-change count where manager actually changed. |
| `SoNgayTreTienDo` | `DU_AN` + `CONG_VIEC` + `DANH_MUC_CONG_VIEC` | Delay-day indicator from project/task schedule deviations. |
| `SoDeXuatCongViecChoDuyet` | `DE_XUAT_CONG_VIEC` | Number of task proposals currently pending approval. |
| `SoDeXuatCongViecBiTuChoi` | `DE_XUAT_CONG_VIEC` | Number of task proposals rejected. |
| `ThoiGianDuyetCongViecTrungBinh` | `DE_XUAT_CONG_VIEC` | Average approval time for task proposals (days). |
| `SoDeXuatNganSachChoDuyet` | `DE_XUAT_NGAN_SACH` | Number of budget proposals currently pending approval. |
| `SoDeXuatNganSachBiTuChoi` | `DE_XUAT_NGAN_SACH` | Number of budget proposals rejected. |
| `ThoiGianDuyetNganSachTrungBinh` | `DE_XUAT_NGAN_SACH` | Average approval time for budget proposals (days). |
| `SoBaoCaoTienDoChoDuyet` | `TIEN_DO_CONG_VIEC` + `CT_CONG_VIEC` + `CONG_VIEC` + `DANH_MUC_CONG_VIEC` | Count of progress reports in pending status. |
| `SoBaoCaoTienDoBiTuChoi` | `TIEN_DO_CONG_VIEC` + workflow joins | Count of rejected progress reports. |
| `SoBaoCaoTienDoYeuCauBoSung` | `TIEN_DO_CONG_VIEC` + workflow joins | Count of progress reports marked for supplementary update. |
| `TyLeBaoCaoTienDoBiTuChoi` | Derived from progress aggregates | Rejection ratio among all progress updates. |
| `SoLanCapNhatTienDo` | `TIEN_DO_CONG_VIEC` + workflow joins | Total number of progress updates for project work items. |
| `SoNgayChamCapNhatTienDo` | `TIEN_DO_CONG_VIEC` + `DU_AN` | Days since latest progress update relative to project end-date reference. |

**Potential Future Work.** Additional non-tabular signals (e.g., graph dependencies, sequence logs) are not part of the current feature pipeline.

# 18. AI Governance Rules

**Current System (AI Governance Principles).**

1. **Business Authority Principle:** Final business decisions remain in MVC workflow; AI does not autonomously finalize project decisions.
2. **Human Confirmation Principle:** Delay-cause confirmation is manager-driven and persisted in `AI_NGUYEN_NHAN`.
3. **Separation-of-Concerns Principle:** MVC is the operational system-of-record; FastAPI is compute-only for AI validation/training/analysis.
4. **Non-Intrusive AI Principle:** AI analysis does not directly alter core operational project/task workflow tables.
5. **Ground-Truth Integrity Principle:** `AI_KET_QUA` is prediction output, not training ground truth.
6. **Label Trust Principle:** Only manager-confirmed reasons (synchronized into `AI_DATASET.MaDMNguyenNhan`) are accepted as reliable training labels.
7. **Eligibility Principle:** Reason analysis and confirmation are constrained to delayed projects (`LaDuAnTre=true`) under permission and scope checks.
8. **Model-Risk Control Principle:** Confidence thresholding and `RuleFallback` are used when model confidence/consistency is insufficient.
9. **Contract Robustness Principle:** FastAPI request payloads are strict (`extra="forbid"`) to prevent schema drift.
10. **Freshness Control Principle:** MVC checks dataset staleness against latest operational events before AI analysis.

**Potential Future Work.** Formal governance extensions (e.g., policy versioning dashboards, fairness auditing, automated compliance scoring) are not currently implemented.

# 19. Example Delay Cause Analysis Scenario

**Current System.**

**Illustrative example based on system workflow**

1. **Project data context:** A project has completed operational records (tasks, progress approvals, budget/cost, staffing changes) and is eligible for dataset aggregation.
2. **Extracted features:** MVC aggregates the 22-feature vector into `AI_DATASET`; if delay rules classify the project as delayed, `LaDuAnTre` is set to `1`.
3. **AI analysis request:** MVC sends `maDuAn`, `feature`, `danhMucNguyenNhan`, and `reasonConfidenceThreshold=0.6` to FastAPI (`/analyze/delay-reason`).
4. **Suggested delay cause:** FastAPI returns suggested reason code/name with confidence and explanation metadata; MVC may apply post-check fallback for inconsistency cases.
5. **Manager confirmation:** An authorized manager confirms the final cause through `XacNhanNguyenNhan` flow.
6. **Final stored result:**  
   - AI suggestion is stored in `AI_KET_QUA`.  
   - Manager-confirmed cause is stored in `AI_NGUYEN_NHAN`.  
   - Latest `AI_DATASET.MaDMNguyenNhan` is synchronized from manager confirmation for training reliability.

**Potential Future Work.** Automated closed-loop retraining immediately after each confirmation is not currently part of the default runtime workflow.

# 20. Research Contributions of the Implemented System

**Current System.** The implemented platform contributes the following research-relevant capabilities:

1. Workflow-aware project management with explicit state synchronization from task details to tasks and projects.
2. Centralized operational monitoring through integrated dashboard indicators for schedule, workload, and budget/cost health.
3. AI-assisted delay cause analysis via FastAPI reason model integration.
4. Human-in-the-loop validation where manager confirmation governs final cause acceptance.
5. Structured progress tracking with approval-controlled status updates.
6. Budget and resource monitoring through proposal/version workflows and staffing/manager-change signals.
7. Claim-based authorization and scope-constrained operations across modules.
8. AI dataset generation (`AI_DATASET`) from real operational workflow data.

**Potential Future Work.** Additional contributions around autonomous prediction, process mining, and advanced explainability remain future extensions (Section 14).

# 21. Mapping Between System Components and Research Topics

**Current System.** The mapping below links implemented components to research themes:

| Research Topic | Related System Component |
| -------------- | ------------------------ |
| AI-assisted project management | `AiService` + `AiApiService` + `AiController` |
| Delay cause analysis | FastAPI `prediction_service` + MVC `AiService.DuDoanDuAnAsync` |
| Human-in-the-loop AI | Manager confirmation flow (`XacNhanNguyenNhanAsync`, `AI_NGUYEN_NHAN`) |
| Workflow-aware monitoring | `TrangThaiWorkflowService` + `TienDoCongViecService` |
| Project progress control | `TIEN_DO_CONG_VIEC` workflow + approval statuses |
| Budget-risk monitoring | `NganSach`/`ChiPhi` services + `DE_XUAT_NGAN_SACH` approval flow |
| Resource volatility analysis | `NHAN_VIEN_DU_AN`, `NHAT_KY_PHU_TRACH_DU_AN`, `YEU_CAU_DOI_QUAN_LY` |
| Claim-based governance | `Permissions`, `PermissionHelper`, `PhanQuyenService`, authentication claims |
| Operational data to AI pipeline | `AiDatasetService` aggregation into `AI_DATASET` |
| Tree-based tabular ML for project data | FastAPI `DecisionTreeClassifier` training (`model_service`, `decision_tree_model`) |

**Potential Future Work.** Mapping to advanced topics (e.g., process mining, GNN/LSTM forecasting, uncertainty-aware AI) is currently prospective and not part of deployed modules.
