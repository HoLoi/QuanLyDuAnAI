# CHÆ¯Æ NG 1: Tá»”NG QUAN Há»† THá»NG

## 1.1 Äáº¶C Táº¢ Dá»® LIá»†U
Há»‡ thá»‘ng Ä‘Æ°á»£c xĂ¢y dá»±ng theo mĂ´ hĂ¬nh ASP.NET MVC, sá»­ dá»¥ng Entity Framework Core lĂ m lá»›p truy cáº­p dá»¯ liá»‡u, SQL Server lĂ m nÆ¡i lÆ°u trá»¯ chĂ­nh, vĂ  FastAPI AI lĂ m dá»‹ch vá»¥ tĂ­nh toĂ¡n ngoĂ i há»‡ thá»‘ng nghiá»‡p vá»¥ lĂµi. Dá»¯ liá»‡u ngÆ°á»i dĂ¹ng Ä‘Æ°á»£c quáº£n lĂ½ theo cá»¥m báº£ng `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetRoleClaims`, `AspNetUserClaims`; dá»¯ liá»‡u há»“ sÆ¡ nhĂ¢n sá»± nghiá»‡p vá»¥ náº±m táº¡i `NGUOI_DUNG`, liĂªn káº¿t `CHUC_DANH` Ä‘á»ƒ xĂ¡c Ä‘á»‹nh chá»©c danh vĂ  hiá»ƒn thá»‹ thĂ´ng tin nhĂ¢n sá»± trong cĂ¡c quy trĂ¬nh dá»± Ă¡n.

Pháº§n tá»• chá»©c váº­n hĂ nh dá»± Ă¡n Ä‘Æ°á»£c quáº£n lĂ½ báº±ng `TEAM`, `NHAN_VIEN_TEAM`, `DU_AN`, `TEAM_DU_AN`, `NHAN_VIEN_DU_AN`. Trong Ä‘Ă³ `DU_AN` lÆ°u ngÆ°á»i quáº£n lĂ½ hiá»‡n táº¡i qua `MaNguoiDung`, `TEAM_DU_AN` gĂ¡n nhĂ³m phá»¥ trĂ¡ch dá»± Ă¡n, `NHAN_VIEN_DU_AN` quáº£n lĂ½ pháº¡m vi tham gia dá»± Ă¡n vĂ  vai trĂ² ná»™i bá»™ dá»± Ă¡n (Leader/Member á»Ÿ má»©c nghiá»‡p vá»¥, khĂ´ng tĂ¡ch actor UML riĂªng). Luá»“ng thay Ä‘á»•i quáº£n lĂ½ Ä‘Æ°á»£c tĂ¡ch riĂªng báº±ng báº£ng `YEU_CAU_DOI_QUAN_LY` vĂ  xá»­ lĂ½ duyá»‡t bá»Ÿi mĂ´-Ä‘un duyá»‡t tÆ°Æ¡ng á»©ng.

Khá»‘i cĂ´ng viá»‡c gá»“m `DANH_MUC_CONG_VIEC`, `CONG_VIEC`, `CT_CONG_VIEC`, `PHAN_CONG_CONG_VIEC`, `PHAN_CONG_CT_CONG_VIEC`, `TIEN_DO_CONG_VIEC`, `FILE_TIEN_DO_CONG_VIEC`, `FILE_CONG_VIEC`, `FILE_CT_CONG_VIEC`, `FILE_DU_AN`. Tráº¡ng thĂ¡i váº­n hĂ nh thá»±c táº¿ dĂ¹ng háº±ng sá»‘ trong `TrangThai.cs` vĂ  Ä‘á»“ng bá»™ chuá»—i qua `TrangThaiWorkflowService` theo hÆ°á»›ng chi tiáº¿t cĂ´ng viá»‡c -> cĂ´ng viá»‡c -> dá»± Ă¡n. Khá»‘i ngĂ¢n sĂ¡ch vĂ  chi phĂ­ gá»“m `DE_XUAT_NGAN_SACH`, `NGAN_SACH`, `CHI_PHI`, `NHAT_KY_NGAN_SACH`, `NHAT_KY_CHI_PHI`. Khá»‘i Ä‘á» xuáº¥t cĂ´ng viá»‡c gá»“m `DE_XUAT_CONG_VIEC` vĂ  nháº­t kĂ½ quáº£n lĂ½ dá»± Ă¡n liĂªn quan.

Khá»‘i Ä‘Ă¡nh giĂ¡ gá»“m `TIEU_CHI_DANH_GIA`, `DANH_GIA_DU_AN`, `CT_DANH_GIA_DU_AN`, `DANH_GIA_NHAN_VIEN`, `CT_DANH_GIA_NHAN_VIEN`. Khá»‘i chat dá»± Ă¡n gá»“m `PHONG_CHAT`, `THANH_VIEN_PHONG_CHAT`, `TIN_NHAN` (Ä‘á»“ng bá»™ thĂ nh viĂªn theo pháº¡m vi dá»± Ă¡n vĂ  khĂ´ng má»Ÿ cho Admin trong nghiá»‡p vá»¥ chat dá»± Ă¡n). Khá»‘i AI gá»“m `AI_DATASET`, `AI_MODEL`, `AI_KET_QUA`, `AI_NGUYEN_NHAN`, `DM_NGUYEN_NHAN`; trong Ä‘Ă³ MVC giá»¯ quyá»n ghi nghiá»‡p vá»¥ vĂ o DB, FastAPI chá»‰ xá»­ lĂ½ suy luáº­n/train theo contract API.

ToĂ n bá»™ há»‡ thá»‘ng sá»­ dá»¥ng soft delete (`IsDeleted`, `DeletedAt`, `DeletedBy`) trĂªn pháº§n lá»›n báº£ng nghiá»‡p vá»¥ Ä‘á»ƒ báº£o toĂ n lá»‹ch sá»­ vĂ  trĂ¡nh máº¥t dá»¯ liá»‡u cá»©ng.

## 1.2 CĂC YĂU Cáº¦U CHá»¨C NÄ‚NG
Há»‡ thá»‘ng Ä‘Ă¡p á»©ng Ä‘áº§y Ä‘á»§ chu trĂ¬nh quáº£n trá»‹ dá»± Ă¡n tá»« ná»n táº£ng tĂ i khoáº£n vĂ  phĂ¢n quyá»n Ä‘áº¿n váº­n hĂ nh cĂ´ng viá»‡c chi tiáº¿t. NgÆ°á»i dĂ¹ng Ä‘Äƒng nháº­p báº±ng tĂ i khoáº£n `AspNetUsers`; sau Ä‘Äƒng nháº­p, quyá»n thao tĂ¡c Ä‘Æ°á»£c xĂ¡c Ä‘á»‹nh qua claim permission (`permission`) vĂ  kiá»ƒm tra theo module báº±ng háº±ng sá»‘ `Permissions.cs`. NhĂ³m quáº£n trá»‹ viĂªn cĂ³ quyá»n quáº£n trá»‹ nhĂ¢n sá»±, chá»©c danh, phĂ¢n quyá»n; quáº£n lĂ½ dá»± Ă¡n cĂ³ quyá»n Ä‘iá»u hĂ nh dá»± Ă¡n, duyá»‡t Ä‘á» xuáº¥t cĂ´ng viá»‡c, duyá»‡t Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch, duyá»‡t bĂ¡o cĂ¡o tiáº¿n Ä‘á»™ trong pháº¡m vi; nhĂ¢n viĂªn vĂ  leader thá»±c hiá»‡n Ä‘á» xuáº¥t, cáº­p nháº­t tiáº¿n Ä‘á»™, tham gia chat vĂ  Ä‘Ă¡nh giĂ¡ theo quyá»n.

Chá»©c nÄƒng nghiá»‡p vá»¥ chĂ­nh gá»“m quáº£n lĂ½ dá»± Ă¡n, quáº£n lĂ½ thĂ nh viĂªn dá»± Ă¡n, phĂ¢n cĂ´ng team phá»¥ trĂ¡ch, yĂªu cáº§u Ä‘á»•i quáº£n lĂ½ vĂ  duyá»‡t Ä‘á»•i quáº£n lĂ½, quáº£n lĂ½ danh má»¥c cĂ´ng viá»‡c, Ä‘á» xuáº¥t cĂ´ng viá»‡c vĂ  duyá»‡t Ä‘á» xuáº¥t cĂ´ng viá»‡c, quáº£n lĂ½ cĂ´ng viá»‡c, quáº£n lĂ½ chi tiáº¿t cĂ´ng viá»‡c, phĂ¢n cĂ´ng cĂ´ng viá»‡c/chi tiáº¿t cĂ´ng viá»‡c, cáº­p nháº­t tiáº¿n Ä‘á»™ vĂ  duyá»‡t bĂ¡o cĂ¡o tiáº¿n Ä‘á»™, Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch vĂ  duyá»‡t Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch, theo dĂµi ngĂ¢n sĂ¡ch vĂ  chi phĂ­, Ä‘Ă¡nh giĂ¡ nhĂ¢n viĂªn, Ä‘Ă¡nh giĂ¡ dá»± Ă¡n, chat dá»± Ă¡n, dashboard tá»•ng há»£p.

Khá»‘i AI há»— trá»£ nghiá»‡p vá»¥ gá»“m tá»•ng há»£p dataset tá»« dá»¯ liá»‡u váº­n hĂ nh, kiá»ƒm tra cháº¥t lÆ°á»£ng dá»¯ liá»‡u train, huáº¥n luyá»‡n model `TreHan`/`NguyenNhan`, quáº£n lĂ½ active model theo loáº¡i, dá»± Ä‘oĂ¡n AI cho dá»± Ă¡n, xĂ¡c nháº­n nguyĂªn nhĂ¢n AI bá»Ÿi quáº£n lĂ½, vĂ  náº¡p káº¿t quáº£ dá»± Ä‘oĂ¡n gáº§n nháº¥t tá»« `AI_KET_QUA` Ä‘á»ƒ tham chiáº¿u Ä‘Ă¡nh giĂ¡. Quy táº¯c triá»ƒn khai hiá»‡n táº¡i báº£o toĂ n nguyĂªn táº¯c MVC lĂ  há»‡ thá»‘ng báº£n ghi chĂ­nh, AI khĂ´ng tá»± thay Ä‘á»•i workflow nghiá»‡p vá»¥ cá»‘t lĂµi.

## 1.3 CĂC YĂU Cáº¦U PHI CHá»¨C NÄ‚NG
Há»‡ thá»‘ng dĂ¹ng xĂ¡c thá»±c cookie, Ă¡p dá»¥ng authorize filter toĂ n cá»¥c táº¡i `Program.cs`, trá»« cĂ¡c action `AllowAnonymous` cáº§n thiáº¿t (Ä‘Äƒng nháº­p). Táº¥t cáº£ thao tĂ¡c ghi dá»¯ liá»‡u quan trá»ng Ä‘á»u Ä‘i qua service layer vĂ  dĂ¹ng kiá»ƒm tra dá»¯ liá»‡u Ä‘áº§u vĂ o trÆ°á»›c khi ghi DB; cĂ¡c nghiá»‡p vá»¥ cĂ³ rá»§i ro tranh cháº¥p dĂ¹ng transaction (duyá»‡t Ä‘á» xuáº¥t cĂ´ng viá»‡c, duyá»‡t Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch, duyá»‡t yĂªu cáº§u Ä‘á»•i quáº£n lĂ½, duyá»‡t bĂ¡o cĂ¡o tiáº¿n Ä‘á»™). CĂ¡c cáº­p nháº­t workflow Ä‘Æ°á»£c ghi nháº­t kĂ½ (`NHAT_KY_*`) Ä‘á»ƒ truy váº¿t.

Há»‡ thá»‘ng dĂ¹ng chiáº¿n lÆ°á»£c async Ä‘á»“ng bá»™ vá»›i EF Core (`SaveChangesAsync`) vĂ  kiá»ƒm tra quyá»n theo claim/role/scope dá»¯ liá»‡u. Soft delete Ä‘Æ°á»£c Æ°u tiĂªn cho tĂ­nh toĂ n váº¹n dá»¯ liá»‡u lá»‹ch sá»­. TĂ­nh sáºµn sĂ ng Ä‘Æ°á»£c tÄƒng cÆ°á»ng báº±ng rĂ ng buá»™c kiá»ƒm tra tráº¡ng thĂ¡i trÆ°á»›c thao tĂ¡c (khĂ³a cáº­p nháº­t khi dá»± Ă¡n/cĂ´ng viá»‡c Ä‘Ă³ng), chá»‘ng lĂ¹i tráº¡ng thĂ¡i trĂ¡i quy táº¯c á»Ÿ tiáº¿n Ä‘á»™ chi tiáº¿t, kiá»ƒm tra pending Ä‘á»™c nháº¥t cho Ä‘á» xuáº¥t vĂ  bĂ¡o cĂ¡o chá» duyá»‡t.

Äá»‘i vá»›i AI, há»‡ thá»‘ng báº£o Ä‘áº£m boundary rĂµ rĂ ng giá»¯a MVC vĂ  FastAPI: dá»¯ liá»‡u train/predict Ä‘Æ°á»£c chuáº©n hĂ³a táº¡i MVC, FastAPI tráº£ káº¿t quáº£ theo contract, káº¿t quáº£ Ä‘Æ°á»£c MVC quyáº¿t Ä‘á»‹nh ghi vĂ o DB. Khi model nguyĂªn nhĂ¢n khĂ´ng kháº£ dá»¥ng, há»‡ thá»‘ng fallback theo rule nghiá»‡p vá»¥; khi thiáº¿u map nguyĂªn nhĂ¢n thĂ¬ fallback danh má»¥c máº·c Ä‘á»‹nh trong `DM_NGUYEN_NHAN` trÆ°á»›c khi lÆ°u `AI_KET_QUA`.

# CHÆ¯Æ NG 2: USE CASE DIAGRAM

## 2.1 SÆ¡ Ä‘á»“ Use Case tá»•ng quĂ¡t
```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Quáº£n trá»‹ viĂªn" as Admin
actor "NgÆ°á»i dĂ¹ng" as User
actor "NhĂ¢n viĂªn" as Employee
actor "Quáº£n lĂ½" as Manager

Employee --|> User
Manager --|> User

rectangle "Há»‡ thá»‘ng quáº£n lĂ½ dá»± Ă¡n" {
  usecase "ÄÄƒng nháº­p" as UC_Login
  usecase "Quáº£n lĂ½ phĂ¢n quyá»n" as UC_PhanQuyen
  usecase "Quáº£n lĂ½ nhĂ¢n sá»±" as UC_NhanSu
  usecase "Quáº£n lĂ½ chá»©c danh" as UC_ChucDanh
  usecase "Quáº£n lĂ½ team" as UC_Team
  usecase "Quáº£n lĂ½ dá»± Ă¡n" as UC_DuAn
  usecase "Quáº£n lĂ½ thĂ nh viĂªn dá»± Ă¡n" as UC_ThanhVienDuAn
  usecase "PhĂ¢n cĂ´ng team phá»¥ trĂ¡ch dá»± Ă¡n" as UC_TeamDuAn
  usecase "YĂªu cáº§u Ä‘á»•i quáº£n lĂ½ dá»± Ă¡n" as UC_YCDoiQL
  usecase "Duyá»‡t yĂªu cáº§u Ä‘á»•i quáº£n lĂ½ dá»± Ă¡n" as UC_DuyetYCDoiQL
  usecase "Quáº£n lĂ½ danh má»¥c cĂ´ng viá»‡c" as UC_DMCV
  usecase "Äá» xuáº¥t cĂ´ng viá»‡c" as UC_DXCV
  usecase "Duyá»‡t Ä‘á» xuáº¥t cĂ´ng viá»‡c" as UC_DuyetDXCV
  usecase "Quáº£n lĂ½ cĂ´ng viá»‡c" as UC_CV
  usecase "Quáº£n lĂ½ chi tiáº¿t cĂ´ng viá»‡c" as UC_CTCV
  usecase "PhĂ¢n cĂ´ng cĂ´ng viá»‡c" as UC_PCCV
  usecase "PhĂ¢n cĂ´ng chi tiáº¿t cĂ´ng viá»‡c" as UC_PCCTCV
  usecase "Cáº­p nháº­t tiáº¿n Ä‘á»™ cĂ´ng viá»‡c" as UC_TienDo
  usecase "Duyá»‡t bĂ¡o cĂ¡o tiáº¿n Ä‘á»™" as UC_DuyetTienDo
  usecase "Äá» xuáº¥t ngĂ¢n sĂ¡ch dá»± Ă¡n" as UC_DXNS
  usecase "Duyá»‡t Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch dá»± Ă¡n" as UC_DuyetDXNS
  usecase "Quáº£n lĂ½ ngĂ¢n sĂ¡ch" as UC_NS
  usecase "Quáº£n lĂ½ chi phĂ­" as UC_ChiPhi
  usecase "ÄĂ¡nh giĂ¡ nhĂ¢n viĂªn" as UC_DGNV
  usecase "ÄĂ¡nh giĂ¡ dá»± Ă¡n" as UC_DGDA
  usecase "Chat dá»± Ă¡n" as UC_Chat
  usecase "Quáº£n lĂ½ bĂ¡o cĂ¡o" as UC_BaoCao
  usecase "Dashboard AI" as UC_AIDashboard
  usecase "Äá»“ng bá»™ dá»¯ liá»‡u AI" as UC_AIDataset
  usecase "Huáº¥n luyá»‡n model AI" as UC_AITrain
  usecase "Dá»± Ä‘oĂ¡n AI" as UC_AIPredict
  usecase "XĂ¡c nháº­n nguyĂªn nhĂ¢n AI" as UC_AIXacNhan
}

Admin --> UC_PhanQuyen
Admin --> UC_NhanSu
Admin --> UC_ChucDanh

Manager --> UC_DuAn
Manager --> UC_ThanhVienDuAn
Manager --> UC_TeamDuAn
Manager --> UC_DuyetYCDoiQL
Manager --> UC_DuyetDXCV
Manager --> UC_DuyetDXNS
Manager --> UC_DuyetTienDo
Manager --> UC_NS
Manager --> UC_ChiPhi
Manager --> UC_DGNV
Manager --> UC_DGDA
Manager --> UC_AIXacNhan

Employee --> UC_YCDoiQL
Employee --> UC_DMCV
Employee --> UC_DXCV
Employee --> UC_CV
Employee --> UC_CTCV
Employee --> UC_PCCV
Employee --> UC_PCCTCV
Employee --> UC_TienDo
Employee --> UC_DXNS
Employee --> UC_Chat
Employee --> UC_BaoCao
Employee --> UC_AIDashboard
Employee --> UC_AIDataset
Employee --> UC_AITrain
Employee --> UC_AIPredict

User --> UC_Login

UC_PhanQuyen .> UC_Login : <<include>>
UC_NhanSu .> UC_Login : <<include>>
UC_ChucDanh .> UC_Login : <<include>>
UC_Team .> UC_Login : <<include>>
UC_DuAn .> UC_Login : <<include>>
UC_ThanhVienDuAn .> UC_Login : <<include>>
UC_TeamDuAn .> UC_Login : <<include>>
UC_YCDoiQL .> UC_Login : <<include>>
UC_DuyetYCDoiQL .> UC_Login : <<include>>
UC_DMCV .> UC_Login : <<include>>
UC_DXCV .> UC_Login : <<include>>
UC_DuyetDXCV .> UC_Login : <<include>>
UC_CV .> UC_Login : <<include>>
UC_CTCV .> UC_Login : <<include>>
UC_PCCV .> UC_Login : <<include>>
UC_PCCTCV .> UC_Login : <<include>>
UC_TienDo .> UC_Login : <<include>>
UC_DuyetTienDo .> UC_Login : <<include>>
UC_DXNS .> UC_Login : <<include>>
UC_DuyetDXNS .> UC_Login : <<include>>
UC_NS .> UC_Login : <<include>>
UC_ChiPhi .> UC_Login : <<include>>
UC_DGNV .> UC_Login : <<include>>
UC_DGDA .> UC_Login : <<include>>
UC_Chat .> UC_Login : <<include>>
UC_BaoCao .> UC_Login : <<include>>
UC_AIDashboard .> UC_Login : <<include>>
UC_AIDataset .> UC_Login : <<include>>
UC_AITrain .> UC_Login : <<include>>
UC_AIPredict .> UC_Login : <<include>>
UC_AIXacNhan .> UC_Login : <<include>>
@enduml
```

## 2.2 PhĂ¢n rĂ£ Use Case
### a) NhĂ³m quáº£n lĂ½ cĂ´ng viá»‡c
```plantuml
@startuml
left to right direction
actor "NhĂ¢n viĂªn" as NV
actor "Quáº£n lĂ½" as QL
usecase "Quáº£n lĂ½ danh má»¥c cĂ´ng viá»‡c" as A
usecase "Äá» xuáº¥t cĂ´ng viá»‡c" as B
usecase "Duyá»‡t Ä‘á» xuáº¥t cĂ´ng viá»‡c" as C
usecase "Quáº£n lĂ½ cĂ´ng viá»‡c" as D
usecase "Quáº£n lĂ½ chi tiáº¿t cĂ´ng viá»‡c" as E
usecase "PhĂ¢n cĂ´ng cĂ´ng viá»‡c" as F
usecase "PhĂ¢n cĂ´ng chi tiáº¿t cĂ´ng viá»‡c" as G
usecase "Cáº­p nháº­t tiáº¿n Ä‘á»™" as H
usecase "Duyá»‡t bĂ¡o cĂ¡o tiáº¿n Ä‘á»™" as I
NV --> A
NV --> B
NV --> D
NV --> E
NV --> F
NV --> G
NV --> H
QL --> C
QL --> I
@enduml
```

### b) NhĂ³m quáº£n lĂ½ dá»± Ă¡n vĂ  ngĂ¢n sĂ¡ch
```plantuml
@startuml
left to right direction
actor "NhĂ¢n viĂªn" as NV
actor "Quáº£n lĂ½" as QL
usecase "Quáº£n lĂ½ dá»± Ă¡n" as A
usecase "Quáº£n lĂ½ thĂ nh viĂªn dá»± Ă¡n" as B
usecase "PhĂ¢n cĂ´ng team phá»¥ trĂ¡ch dá»± Ă¡n" as C
usecase "YĂªu cáº§u Ä‘á»•i quáº£n lĂ½ dá»± Ă¡n" as D
usecase "Duyá»‡t yĂªu cáº§u Ä‘á»•i quáº£n lĂ½ dá»± Ă¡n" as E
usecase "Äá» xuáº¥t ngĂ¢n sĂ¡ch" as F
usecase "Duyá»‡t Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch" as G
usecase "Quáº£n lĂ½ ngĂ¢n sĂ¡ch" as H
usecase "Quáº£n lĂ½ chi phĂ­" as I
QL --> A
QL --> B
QL --> C
QL --> E
QL --> G
QL --> H
QL --> I
NV --> D
NV --> F
@enduml
```

### c) NhĂ³m quáº£n lĂ½ phĂ¢n quyá»n
```plantuml
@startuml
left to right direction
actor "Quáº£n trá»‹ viĂªn" as AD
usecase "Quáº£n lĂ½ phĂ¢n quyá»n" as A
usecase "Quáº£n lĂ½ vai trĂ²" as B
usecase "Quáº£n lĂ½ nhĂ¢n sá»±" as C
usecase "Quáº£n lĂ½ chá»©c danh" as D
usecase "Quáº£n lĂ½ team" as E
usecase "Quáº£n lĂ½ thĂ nh viĂªn team" as F
AD --> A
AD --> B
AD --> C
AD --> D
AD --> E
AD --> F
@enduml
```

### d) NhĂ³m Ä‘Ă¡nh giĂ¡
```plantuml
@startuml
left to right direction
actor "Quáº£n lĂ½" as QL
actor "NhĂ¢n viĂªn" as NV
usecase "ÄĂ¡nh giĂ¡ nhĂ¢n viĂªn" as A
usecase "ÄĂ¡nh giĂ¡ dá»± Ă¡n" as B
usecase "XĂ¡c nháº­n nguyĂªn nhĂ¢n AI" as C
NV --> A
NV --> B
QL --> A
QL --> B
QL --> C
@enduml
```

### e) NhĂ³m AI
```plantuml
@startuml
left to right direction
actor "NhĂ¢n viĂªn" as NV
actor "Quáº£n lĂ½" as QL
usecase "Dashboard AI" as A
usecase "Äá»“ng bá»™ dá»¯ liá»‡u AI" as B
usecase "Huáº¥n luyá»‡n model AI" as C
usecase "Dá»± Ä‘oĂ¡n AI" as D
usecase "XĂ¡c nháº­n nguyĂªn nhĂ¢n AI" as E
NV --> A
NV --> B
NV --> C
NV --> D
QL --> E
@enduml
```

### f) NhĂ³m chat vĂ  bĂ¡o cĂ¡o
```plantuml
@startuml
left to right direction
actor "NhĂ¢n viĂªn" as NV
actor "Quáº£n lĂ½" as QL
usecase "Chat dá»± Ă¡n" as A
usecase "Quáº£n lĂ½ bĂ¡o cĂ¡o" as B
NV --> A
NV --> B
QL --> B
@enduml
```

## 2.3 MĂ´ táº£ Use Case
### 1) ÄÄƒng nháº­p
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | ÄÄƒng nháº­p |
| MĂ´ táº£ ngáº¯n gá»n              | NgÆ°á»i dĂ¹ng Ä‘Äƒng nháº­p báº±ng tĂ i khoáº£n há»‡ thá»‘ng. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | TĂ i khoáº£n tá»“n táº¡i, chÆ°a bá»‹ khĂ³a. |
| Äiá»u kiá»‡n háº­u               | Táº¡o cookie xĂ¡c thá»±c vĂ  náº¡p claim role/permission. |
| TĂ¬nh huá»‘ng lá»—i              | Sai tĂ i khoáº£n/máº­t kháº©u hoáº·c tĂ i khoáº£n bá»‹ khĂ³a. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng táº¡o phiĂªn Ä‘Äƒng nháº­p. |
| CĂ¡c Actor giao tiáº¿p         | NgÆ°á»i dĂ¹ng |
| Trigger                     | Nháº¥n nĂºt Ä‘Äƒng nháº­p táº¡i `/Account/Login`. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p thĂ´ng tin Ä‘Äƒng nháº­p. 2) Há»‡ thá»‘ng xĂ¡c thá»±c `AspNetUsers`. 3) Náº¡p role/claim vĂ  chuyá»ƒn vá» Dashboard. |
| Quy trĂ¬nh thay tháº¿          | 1) ThĂ´ng tin khĂ´ng há»£p lá»‡. 2) Tráº£ thĂ´ng bĂ¡o lá»—i vĂ  giá»¯ láº¡i form. |

### 2) Quáº£n lĂ½ phĂ¢n quyá»n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ phĂ¢n quyá»n |
| MĂ´ táº£ ngáº¯n gá»n              | Cáº¥u hĂ¬nh danh sĂ¡ch permission theo role. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄĂ£ Ä‘Äƒng nháº­p, cĂ³ `PhanQuyen.Xem` vĂ  `PhanQuyen.Luu`. |
| Äiá»u kiá»‡n háº­u               | Role claim Ä‘Æ°á»£c cáº­p nháº­t trong `AspNetRoleClaims`. |
| TĂ¬nh huá»‘ng lá»—i              | Role khĂ´ng há»£p lá»‡ hoáº·c thiáº¿u permission báº¯t buá»™c. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng thay Ä‘á»•i dá»¯ liá»‡u phĂ¢n quyá»n. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n trá»‹ viĂªn |
| Trigger                     | Má»Ÿ trang phĂ¢n quyá»n vĂ  báº¥m lÆ°u. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n role. 2) Chá»n permission theo mĂ n hĂ¬nh. 3) LÆ°u transaction cáº­p nháº­t role claims. |
| Quy trĂ¬nh thay tháº¿          | 1) Dá»¯ liá»‡u khĂ´ng há»£p lá»‡. 2) BĂ¡o lá»—i vĂ  khĂ´ng commit. |

### 3) Quáº£n lĂ½ vai trĂ²
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ vai trĂ² |
| MĂ´ táº£ ngáº¯n gá»n              | Sá»­ dá»¥ng role há»‡ thá»‘ng Ä‘á»ƒ gĂ¡n scope quyá»n cho ngÆ°á»i dĂ¹ng. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄĂ£ Ä‘Äƒng nháº­p vĂ  cĂ³ quyá»n quáº£n trá»‹ phĂ¢n quyá»n. |
| Äiá»u kiá»‡n háº­u               | Cáº¥u hĂ¬nh role vĂ  permission nháº¥t quĂ¡n vá»›i quy Ä‘á»‹nh. |
| TĂ¬nh huá»‘ng lá»—i              | Vai trĂ² khĂ´ng tá»“n táº¡i hoáº·c dá»¯ liá»‡u quyá»n sai. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Vai trĂ² khĂ´ng Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n trá»‹ viĂªn |
| Trigger                     | Chá»n role táº¡i trang phĂ¢n quyá»n. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n role cáº§n cáº¥u hĂ¬nh. 2) Há»‡ thá»‘ng náº¡p quyá»n hiá»‡n táº¡i. 3) Cáº­p nháº­t quyá»n vĂ  lÆ°u. |
| Quy trĂ¬nh thay tháº¿          | 1) Role khĂ´ng há»£p lá»‡. 2) Dá»«ng thao tĂ¡c vĂ  bĂ¡o lá»—i. |

### 4) Quáº£n lĂ½ nhĂ¢n sá»±
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ nhĂ¢n sá»± |
| MĂ´ táº£ ngáº¯n gá»n              | Táº¡o/sá»­a/xĂ³a má»m/khĂ³a má»Ÿ khĂ³a tĂ i khoáº£n nhĂ¢n sá»±. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `NhanSu.*` tÆ°Æ¡ng á»©ng thao tĂ¡c. |
| Äiá»u kiá»‡n háº­u               | Báº£ng `NGUOI_DUNG` vĂ  `AspNetUsers` Ä‘Æ°á»£c Ä‘á»“ng bá»™ dá»¯ liá»‡u. |
| TĂ¬nh huá»‘ng lá»—i              | Thiáº¿u dá»¯ liá»‡u báº¯t buá»™c hoáº·c khĂ´ng cĂ³ quyá»n. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng ghi thay Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n trá»‹ viĂªn |
| Trigger                     | Thao tĂ¡c táº¡i mĂ n hĂ¬nh NhĂ¢n sá»±. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p thĂ´ng tin nhĂ¢n sá»±. 2) Validate. 3) LÆ°u vĂ  cáº­p nháº­t tráº¡ng thĂ¡i tĂ i khoáº£n. |
| Quy trĂ¬nh thay tháº¿          | 1) Dá»¯ liá»‡u trĂ¹ng/sai. 2) Hiá»ƒn thá»‹ lá»—i vĂ  giá»¯ dá»¯ liá»‡u nháº­p. |

### 5) Quáº£n lĂ½ chá»©c danh
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ chá»©c danh |
| MĂ´ táº£ ngáº¯n gá»n              | Quáº£n lĂ½ danh má»¥c chá»©c danh nhĂ¢n sá»±. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `ChucDanh.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t báº£ng `CHUC_DANH`. |
| TĂ¬nh huá»‘ng lá»—i              | TrĂ¹ng dá»¯ liá»‡u hoáº·c tham chiáº¿u khĂ´ng há»£p lá»‡. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Dá»¯ liá»‡u chá»©c danh khĂ´ng Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n trá»‹ viĂªn |
| Trigger                     | ThĂªm/sá»­a/xĂ³a chá»©c danh. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n thao tĂ¡c. 2) Validate. 3) LÆ°u `CHUC_DANH`. |
| Quy trĂ¬nh thay tháº¿          | 1) Lá»—i validate. 2) BĂ¡o lá»—i vĂ  há»§y ghi. |

### 6) Quáº£n lĂ½ team
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ team |
| MĂ´ táº£ ngáº¯n gá»n              | Táº¡o, cáº­p nháº­t, xĂ³a má»m team vĂ  tráº¡ng thĂ¡i team. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `Nhom.*`. |
| Äiá»u kiá»‡n háº­u               | Báº£ng `TEAM` cáº­p nháº­t Ä‘Ăºng tráº¡ng thĂ¡i. |
| TĂ¬nh huá»‘ng lá»—i              | Team khĂ´ng há»£p lá»‡ hoáº·c khĂ´ng cĂ³ quyá»n. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Team khĂ´ng thay Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n trá»‹ viĂªn |
| Trigger                     | Má»Ÿ module Team vĂ  thá»±c hiá»‡n lÆ°u/xĂ³a. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p thĂ´ng tin team. 2) Kiá»ƒm tra dá»¯ liá»‡u. 3) LÆ°u vĂ o `TEAM`. |
| Quy trĂ¬nh thay tháº¿          | 1) Lá»—i dá»¯ liá»‡u. 2) Tráº£ thĂ´ng bĂ¡o lá»—i. |

### 7) Quáº£n lĂ½ thĂ nh viĂªn team
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ thĂ nh viĂªn team |
| MĂ´ táº£ ngáº¯n gá»n              | GĂ¡n/thu há»“i nhĂ¢n sá»± vĂ o team vĂ  cáº¥u hĂ¬nh leader team. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `ThanhVienNhom.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `NHAN_VIEN_TEAM`, cĂ³ thá»ƒ gĂ¡n `IsLeader=true`. |
| TĂ¬nh huá»‘ng lá»—i              | NhĂ¢n sá»± hoáº·c team khĂ´ng há»£p lá»‡. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | ThĂ nh viĂªn team giá»¯ nguyĂªn. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n trá»‹ viĂªn |
| Trigger                     | Thao tĂ¡c thĂªm/sá»­a/xĂ³a thĂ nh viĂªn team. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n team vĂ  nhĂ¢n sá»±. 2) Cáº­p nháº­t vai trĂ²/team leader. 3) LÆ°u vĂ o `NHAN_VIEN_TEAM`. |
| Quy trĂ¬nh thay tháº¿          | 1) KhĂ´ng Ä‘á»§ Ä‘iá»u kiá»‡n gĂ¡n leader. 2) Dá»«ng lÆ°u vĂ  bĂ¡o lá»—i. |

### 8) Quáº£n lĂ½ dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | Táº¡o/sá»­a/xĂ³a má»m dá»± Ă¡n vĂ  Ä‘iá»u khiá»ƒn tráº¡ng thĂ¡i dá»± Ă¡n. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `DuAn.*`. |
| Äiá»u kiá»‡n háº­u               | `DU_AN` vĂ  nháº­t kĂ½ quáº£n lĂ½ dá»± Ă¡n cáº­p nháº­t. |
| TĂ¬nh huá»‘ng lá»—i              | Chuyá»ƒn tráº¡ng thĂ¡i khĂ´ng há»£p lá»‡ hoáº·c thiáº¿u ghi chĂº báº¯t buá»™c. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Tráº¡ng thĂ¡i dá»± Ă¡n khĂ´ng Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½ |
| Trigger                     | Thao tĂ¡c táº¡i mĂ n hĂ¬nh Dá»± Ă¡n. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p thĂ´ng tin dá»± Ă¡n. 2) Validate tráº¡ng thĂ¡i/ngĂ y. 3) LÆ°u vĂ  ghi nháº­t kĂ½. |
| Quy trĂ¬nh thay tháº¿          | 1) Dá»¯ liá»‡u sai luá»“ng. 2) Tá»« chá»‘i cáº­p nháº­t. |

### 9) YĂªu cáº§u Ä‘á»•i quáº£n lĂ½ dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | YĂªu cáº§u Ä‘á»•i quáº£n lĂ½ dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | Táº¡o yĂªu cáº§u thay Ä‘á»•i ngÆ°á»i quáº£n lĂ½ cho dá»± Ă¡n. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `YeuCauDoiQuanLy.*`, dá»± Ă¡n há»£p lá»‡. |
| Äiá»u kiá»‡n háº­u               | Táº¡o báº£n ghi `YEU_CAU_DOI_QUAN_LY` á»Ÿ tráº¡ng thĂ¡i chá» duyá»‡t. |
| TĂ¬nh huá»‘ng lá»—i              | TrĂ¹ng yĂªu cáº§u chá» duyá»‡t, dá»± Ă¡n khĂ´ng cho phĂ©p thao tĂ¡c. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng táº¡o yĂªu cáº§u má»›i. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | Nháº¥n táº¡o yĂªu cáº§u Ä‘á»•i quáº£n lĂ½. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n dá»± Ă¡n vĂ  quáº£n lĂ½ Ä‘á» xuáº¥t. 2) Validate scope dá»¯ liá»‡u. 3) LÆ°u yĂªu cáº§u `ChoDuyet`. |
| Quy trĂ¬nh thay tháº¿          | 1) ÄĂ£ cĂ³ yĂªu cáº§u pending. 2) BĂ¡o lá»—i, khĂ´ng lÆ°u. |

### 10) Duyá»‡t yĂªu cáº§u Ä‘á»•i quáº£n lĂ½ dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Duyá»‡t yĂªu cáº§u Ä‘á»•i quáº£n lĂ½ dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | Xá»­ lĂ½ duyá»‡t/tá»« chá»‘i yĂªu cáº§u Ä‘á»•i quáº£n lĂ½. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ quyá»n `DuyetYeuCauDoiQuanLy.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `DU_AN.MaNguoiDung` vĂ  tráº¡ng thĂ¡i yĂªu cáº§u. |
| TĂ¬nh huá»‘ng lá»—i              | YĂªu cáº§u khĂ´ng cĂ²n pending hoáº·c khĂ´ng Ä‘á»§ quyá»n. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Dá»¯ liá»‡u dá»± Ă¡n vĂ  yĂªu cáº§u giá»¯ nguyĂªn. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½, Quáº£n trá»‹ viĂªn |
| Trigger                     | Chá»n duyá»‡t hoáº·c tá»« chá»‘i yĂªu cáº§u. |
| Quy trĂ¬nh chuáº©n             | 1) Má»Ÿ chi tiáº¿t yĂªu cáº§u. 2) Validate tráº¡ng thĂ¡i pending. 3) Transaction cáº­p nháº­t dá»± Ă¡n + nháº­t kĂ½ + tráº¡ng thĂ¡i duyá»‡t. |
| Quy trĂ¬nh thay tháº¿          | 1) YĂªu cáº§u Ä‘Ă£ xá»­ lĂ½. 2) Tá»« chá»‘i thao tĂ¡c. |

### 11) Quáº£n lĂ½ thĂ nh viĂªn tham gia dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ thĂ nh viĂªn tham gia dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | ThĂªm, cáº­p nháº­t vai trĂ², xĂ³a thĂ nh viĂªn dá»± Ă¡n. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `ThanhVienDuAn.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `NHAN_VIEN_DU_AN`. |
| TĂ¬nh huá»‘ng lá»—i              | NhĂ¢n sá»± khĂ´ng thuá»™c pháº¡m vi hoáº·c dá»± Ă¡n khĂ´ng há»£p lá»‡. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Danh sĂ¡ch thĂ nh viĂªn khĂ´ng Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½ |
| Trigger                     | Thao tĂ¡c táº¡i mĂ n hĂ¬nh thĂ nh viĂªn dá»± Ă¡n. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n dá»± Ă¡n. 2) ThĂªm/cáº­p nháº­t/xĂ³a thĂ nh viĂªn. 3) LÆ°u dá»¯ liá»‡u vĂ  nháº­t kĂ½ liĂªn quan. |
| Quy trĂ¬nh thay tháº¿          | 1) TrĂ¹ng thĂ nh viĂªn. 2) BĂ¡o lá»—i vĂ  khĂ´ng lÆ°u. |

### 12) PhĂ¢n cĂ´ng team phá»¥ trĂ¡ch dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | PhĂ¢n cĂ´ng team phá»¥ trĂ¡ch dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | GĂ¡n team vĂ o dá»± Ă¡n vĂ  cáº­p nháº­t nháº­t kĂ½ phá»¥ trĂ¡ch. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `TeamDuAn.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `TEAM_DU_AN`, `NHAT_KY_DU_AN`, `NHAT_KY_PHU_TRACH_DU_AN`. |
| TĂ¬nh huá»‘ng lá»—i              | Team hoáº·c dá»± Ă¡n khĂ´ng há»£p lá»‡. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Mapping team dá»± Ă¡n khĂ´ng Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½ |
| Trigger                     | ThĂªm hoáº·c xĂ³a team phá»¥ trĂ¡ch. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n dá»± Ă¡n vĂ  team. 2) Kiá»ƒm tra Ä‘iá»u kiá»‡n. 3) LÆ°u mapping team-dá»± Ă¡n vĂ  nháº­t kĂ½. |
| Quy trĂ¬nh thay tháº¿          | 1) Team Ä‘Ă£ tá»“n táº¡i trong dá»± Ă¡n. 2) Dá»«ng lÆ°u. |

### 13) Quáº£n lĂ½ danh má»¥c cĂ´ng viá»‡c
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ danh má»¥c cĂ´ng viá»‡c |
| MĂ´ táº£ ngáº¯n gá»n              | Táº¡o/sá»­a/xĂ³a má»m danh má»¥c cĂ´ng viá»‡c theo dá»± Ă¡n. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `DanhMucCongViec.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `DANH_MUC_CONG_VIEC`. |
| TĂ¬nh huá»‘ng lá»—i              | Dá»± Ă¡n khĂ´ng há»£p lá»‡ hoáº·c dá»¯ liá»‡u trĂ¹ng. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Danh má»¥c khĂ´ng Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | Thao tĂ¡c táº¡i module danh má»¥c cĂ´ng viá»‡c. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n dá»± Ă¡n. 2) ThĂªm/sá»­a danh má»¥c. 3) LÆ°u dá»¯ liá»‡u. |
| Quy trĂ¬nh thay tháº¿          | 1) KhĂ´ng Ä‘á»§ quyá»n theo scope. 2) Dá»«ng thao tĂ¡c. |

### 14) Äá» xuáº¥t cĂ´ng viá»‡c
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Äá» xuáº¥t cĂ´ng viá»‡c |
| MĂ´ táº£ ngáº¯n gá»n              | Táº¡o Ä‘á» xuáº¥t cĂ´ng viá»‡c má»›i theo ngĂ¢n sĂ¡ch hiá»‡n hĂ nh. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `DeXuatCongViec.Them`, Ä‘á»§ scope dá»± Ă¡n. |
| Äiá»u kiá»‡n háº­u               | Táº¡o `DE_XUAT_CONG_VIEC` tráº¡ng thĂ¡i `ChoDuyet`. |
| TĂ¬nh huá»‘ng lá»—i              | VÆ°á»£t ngĂ¢n sĂ¡ch cĂ²n láº¡i, trĂ¹ng Ä‘á» xuáº¥t pending, sai má»‘c thá»i gian. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng táº¡o Ä‘á» xuáº¥t. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ (trong pháº¡m vi cho phĂ©p nghiá»‡p vá»¥) |
| Trigger                     | Gá»­i form Ä‘á» xuáº¥t cĂ´ng viá»‡c. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p dá»¯ liá»‡u Ä‘á» xuáº¥t. 2) Validate ngĂ¢n sĂ¡ch/scope/tráº¡ng thĂ¡i dá»± Ă¡n. 3) LÆ°u Ä‘á» xuáº¥t vĂ  nháº­t kĂ½. |
| Quy trĂ¬nh thay tháº¿          | 1) Thiáº¿u ngĂ¢n sĂ¡ch hoáº·c khĂ´ng Ä‘á»§ scope. 2) Tráº£ lá»—i nghiá»‡p vá»¥. |

### 15) Duyá»‡t Ä‘á» xuáº¥t cĂ´ng viá»‡c
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Duyá»‡t Ä‘á» xuáº¥t cĂ´ng viá»‡c |
| MĂ´ táº£ ngáº¯n gá»n              | Duyá»‡t hoáº·c tá»« chá»‘i Ä‘á» xuáº¥t cĂ´ng viá»‡c. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `DuyetDeXuatCongViec.*`, lĂ  quáº£n lĂ½ dá»± Ă¡n. |
| Äiá»u kiá»‡n háº­u               | Táº¡o `CONG_VIEC`, ghi `CHI_PHI` tÆ°Æ¡ng á»©ng, cáº­p nháº­t tráº¡ng thĂ¡i Ä‘á» xuáº¥t. |
| TĂ¬nh huá»‘ng lá»—i              | Äá» xuáº¥t Ä‘Ă£ xá»­ lĂ½, dá»± Ă¡n Ä‘Ă³ng, ngĂ¢n sĂ¡ch khĂ´ng há»£p lá»‡. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng táº¡o cĂ´ng viá»‡c/chi phĂ­ má»›i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½ |
| Trigger                     | Chá»n duyá»‡t hoáº·c tá»« chá»‘i Ä‘á» xuáº¥t cĂ´ng viá»‡c. |
| Quy trĂ¬nh chuáº©n             | 1) Má»Ÿ Ä‘á» xuáº¥t pending. 2) Transaction táº¡o cĂ´ng viá»‡c + chi phĂ­ + nháº­t kĂ½. 3) ÄĂ¡nh dáº¥u Ä‘á» xuáº¥t Ä‘Ă£ duyá»‡t. |
| Quy trĂ¬nh thay tháº¿          | 1) Tá»« chá»‘i Ä‘á» xuáº¥t. 2) Cáº­p nháº­t tráº¡ng thĂ¡i `TuChoi` vĂ  ghi lĂ½ do. |

### 16) Quáº£n lĂ½ cĂ´ng viá»‡c
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ cĂ´ng viá»‡c |
| MĂ´ táº£ ngáº¯n gá»n              | Theo dĂµi danh sĂ¡ch cĂ´ng viá»‡c, xĂ¡c nháº­n hoĂ n thĂ nh, má»Ÿ láº¡i cĂ´ng viá»‡c. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `CongViec.Xem` vĂ  scope phĂ¹ há»£p. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `CONG_VIEC.TrangThaiCongViec`, Ä‘á»“ng bá»™ tráº¡ng thĂ¡i dá»± Ă¡n náº¿u cáº§n. |
| TĂ¬nh huá»‘ng lá»—i              | KhĂ´ng Ä‘á»§ quyá»n hoáº·c tráº¡ng thĂ¡i khĂ´ng cho phĂ©p chuyá»ƒn. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | CĂ´ng viá»‡c giá»¯ tráº¡ng thĂ¡i cÅ©. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | Nháº¥n xĂ¡c nháº­n hoĂ n thĂ nh hoáº·c má»Ÿ láº¡i. |
| Quy trĂ¬nh chuáº©n             | 1) Kiá»ƒm tra quyá»n theo dá»± Ă¡n. 2) Cáº­p nháº­t tráº¡ng thĂ¡i cĂ´ng viá»‡c. 3) Gá»i Ä‘á»“ng bá»™ workflow dá»± Ă¡n. |
| Quy trĂ¬nh thay tháº¿          | 1) CĂ´ng viá»‡c khĂ´ng á»Ÿ tráº¡ng thĂ¡i Ä‘Ă­ch há»£p lá»‡. 2) BĂ¡o lá»—i. |

### 17) Quáº£n lĂ½ chi tiáº¿t cĂ´ng viá»‡c
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ chi tiáº¿t cĂ´ng viá»‡c |
| MĂ´ táº£ ngáº¯n gá»n              | ThĂªm/sá»­a/xĂ³a chi tiáº¿t cĂ´ng viá»‡c vĂ  tráº¡ng thĂ¡i chi tiáº¿t. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `ChiTietCongViec.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `CT_CONG_VIEC`, Ä‘á»“ng bá»™ workflow chuá»—i. |
| TĂ¬nh huá»‘ng lá»—i              | Tráº¡ng thĂ¡i cĂ´ng viá»‡c cha bá»‹ khĂ³a hoáº·c dá»¯ liá»‡u khĂ´ng há»£p lá»‡. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng thay Ä‘á»•i chi tiáº¿t cĂ´ng viá»‡c. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | Gá»­i form thĂªm/sá»­a/xĂ³a chi tiáº¿t cĂ´ng viá»‡c. |
| Quy trĂ¬nh chuáº©n             | 1) Validate tráº¡ng thĂ¡i/permission. 2) Ghi dá»¯ liá»‡u chi tiáº¿t. 3) Äá»“ng bá»™ tráº¡ng thĂ¡i cĂ´ng viá»‡c vĂ  dá»± Ă¡n. |
| Quy trĂ¬nh thay tháº¿          | 1) CĂ´ng viá»‡c cha Ä‘Ă£ hoĂ n thĂ nh/táº¡m dá»«ng/há»§y. 2) Cháº·n cáº­p nháº­t. |

### 18) PhĂ¢n cĂ´ng cĂ´ng viá»‡c
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | PhĂ¢n cĂ´ng cĂ´ng viá»‡c |
| MĂ´ táº£ ngáº¯n gá»n              | Giao cĂ´ng viá»‡c cho nhĂ¢n sá»± dá»± Ă¡n vĂ  ghi nháº­t kĂ½ phĂ¢n cĂ´ng. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `PhanCongCongViec.ThucHien`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `PHAN_CONG_CONG_VIEC`, `NHAT_KY_PHAN_CONG_CONG_VIEC`. |
| TĂ¬nh huá»‘ng lá»—i              | NhĂ¢n sá»± khĂ´ng thuá»™c pháº¡m vi dá»± Ă¡n hoáº·c trĂ¹ng phĂ¢n cĂ´ng. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng thĂªm phĂ¢n cĂ´ng má»›i. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | ThĂªm/xĂ³a phĂ¢n cĂ´ng cĂ´ng viá»‡c. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n cĂ´ng viá»‡c vĂ  nhĂ¢n sá»±. 2) Validate scope vĂ  tráº¡ng thĂ¡i. 3) LÆ°u phĂ¢n cĂ´ng + nháº­t kĂ½. |
| Quy trĂ¬nh thay tháº¿          | 1) KhĂ´ng Ä‘á»§ Ä‘iá»u kiá»‡n giao viá»‡c. 2) BĂ¡o lá»—i. |

### 19) PhĂ¢n cĂ´ng chi tiáº¿t cĂ´ng viá»‡c
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | PhĂ¢n cĂ´ng chi tiáº¿t cĂ´ng viá»‡c |
| MĂ´ táº£ ngáº¯n gá»n              | Giao chi tiáº¿t cĂ´ng viá»‡c cho nhĂ¢n sá»± thá»±c hiá»‡n. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `PhanCongChiTietCongViec.ThucHien`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `PHAN_CONG_CT_CONG_VIEC`, `NHAT_KY_PHAN_CONG_CT_CONG_VIEC`. |
| TĂ¬nh huá»‘ng lá»—i              | Chi tiáº¿t cĂ´ng viá»‡c hoáº·c nhĂ¢n sá»± khĂ´ng há»£p lá»‡. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng cáº­p nháº­t phĂ¢n cĂ´ng. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | ThĂªm/xĂ³a phĂ¢n cĂ´ng chi tiáº¿t. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n chi tiáº¿t cĂ´ng viá»‡c. 2) Chá»n nhĂ¢n sá»±. 3) LÆ°u phĂ¢n cĂ´ng vĂ  nháº­t kĂ½. |
| Quy trĂ¬nh thay tháº¿          | 1) TrĂ¹ng phĂ¢n cĂ´ng. 2) Há»‡ thá»‘ng tá»« chá»‘i ghi. |

### 20) Cáº­p nháº­t tiáº¿n Ä‘á»™ cĂ´ng viá»‡c
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Cáº­p nháº­t tiáº¿n Ä‘á»™ cĂ´ng viá»‡c |
| MĂ´ táº£ ngáº¯n gá»n              | Gá»­i bĂ¡o cĂ¡o tiáº¿n Ä‘á»™ chi tiáº¿t cĂ´ng viá»‡c Ä‘á»ƒ chá» duyá»‡t. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `TienDo.CapNhat`, cĂ³ quyá»n theo scope. |
| Äiá»u kiá»‡n háº­u               | ThĂªm báº£n ghi `TIEN_DO_CONG_VIEC` tráº¡ng thĂ¡i `ChoDuyet`. |
| TĂ¬nh huá»‘ng lá»—i              | Äang cĂ³ bĂ¡o cĂ¡o pending hoáº·c Ä‘á» xuáº¥t lĂ¹i tráº¡ng thĂ¡i. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng táº¡o bĂ¡o cĂ¡o má»›i. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn |
| Trigger                     | Gá»­i form cáº­p nháº­t tiáº¿n Ä‘á»™. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p ghi chĂº/tráº¡ng thĂ¡i Ä‘á» xuáº¥t/tá»‡p. 2) Validate quy táº¯c tráº¡ng thĂ¡i. 3) LÆ°u bĂ¡o cĂ¡o chá» duyá»‡t. |
| Quy trĂ¬nh thay tháº¿          | 1) Dá»¯ liá»‡u rá»—ng vĂ  khĂ´ng thay Ä‘á»•i tráº¡ng thĂ¡i. 2) Há»‡ thá»‘ng tá»« chá»‘i. |

### 21) Duyá»‡t bĂ¡o cĂ¡o tiáº¿n Ä‘á»™
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Duyá»‡t bĂ¡o cĂ¡o tiáº¿n Ä‘á»™ |
| MĂ´ táº£ ngáº¯n gá»n              | Duyá»‡t, yĂªu cáº§u bá»• sung, hoáº·c tá»« chá»‘i bĂ¡o cĂ¡o tiáº¿n Ä‘á»™. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `TienDo.Duyet`, Ä‘Ăºng scope duyá»‡t dá»± Ă¡n. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t tráº¡ng thĂ¡i bĂ¡o cĂ¡o; náº¿u duyá»‡t thĂ¬ cáº­p nháº­t tráº¡ng thĂ¡i `CT_CONG_VIEC`. |
| TĂ¬nh huá»‘ng lá»—i              | BĂ¡o cĂ¡o khĂ´ng cĂ²n `ChoDuyet` hoáº·c khĂ´ng Ä‘á»§ quyá»n. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng Ä‘á»•i tráº¡ng thĂ¡i bĂ¡o cĂ¡o. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½ |
| Trigger                     | Chá»n duyá»‡t/bá»• sung/tá»« chá»‘i bĂ¡o cĂ¡o. |
| Quy trĂ¬nh chuáº©n             | 1) Má»Ÿ lá»‹ch sá»­ bĂ¡o cĂ¡o. 2) Chá»n tráº¡ng thĂ¡i xá»­ lĂ½. 3) Transaction cáº­p nháº­t bĂ¡o cĂ¡o + Ä‘á»“ng bá»™ workflow chuá»—i. |
| Quy trĂ¬nh thay tháº¿          | 1) BĂ¡o cĂ¡o bá»‹ khĂ³a do tráº¡ng thĂ¡i dá»± Ă¡n/cĂ´ng viá»‡c. 2) Dá»«ng thao tĂ¡c. |

### 22) Äá» xuáº¥t ngĂ¢n sĂ¡ch dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Äá» xuáº¥t ngĂ¢n sĂ¡ch dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | Táº¡o Ä‘á» xuáº¥t Ä‘iá»u chá»‰nh ngĂ¢n sĂ¡ch dá»± Ă¡n. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `DeXuatNganSach.Them`, Ä‘á»§ scope dá»± Ă¡n. |
| Äiá»u kiá»‡n háº­u               | Táº¡o `DE_XUAT_NGAN_SACH` tráº¡ng thĂ¡i `ChoDuyet`. |
| TĂ¬nh huá»‘ng lá»—i              | ÄĂ£ cĂ³ Ä‘á» xuáº¥t pending hoáº·c ngĂ¢n sĂ¡ch Ä‘á» xuáº¥t < tá»•ng chi Ä‘Ă£ dĂ¹ng. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng táº¡o Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn |
| Trigger                     | Gá»­i form Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p sá»‘ tiá»n vĂ  lĂ½ do. 2) Validate scope/tráº¡ng thĂ¡i dá»± Ă¡n. 3) LÆ°u Ä‘á» xuáº¥t vĂ  nháº­t kĂ½ ngĂ¢n sĂ¡ch. |
| Quy trĂ¬nh thay tháº¿          | 1) Dá»± Ă¡n khĂ´ng cho phĂ©p Ä‘á» xuáº¥t. 2) Há»‡ thá»‘ng tá»« chá»‘i. |

### 23) Duyá»‡t Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Duyá»‡t Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | Duyá»‡t hoáº·c tá»« chá»‘i Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `DuyetNganSach.Duyet`, lĂ  quáº£n lĂ½ dá»± Ă¡n. |
| Äiá»u kiá»‡n háº­u               | Táº¡o phiĂªn báº£n `NGAN_SACH` má»›i active vĂ  cáº­p nháº­t Ä‘á» xuáº¥t. |
| TĂ¬nh huá»‘ng lá»—i              | Äá» xuáº¥t Ä‘Ă£ xá»­ lĂ½ hoáº·c dá»± Ă¡n Ä‘Ă³ng. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng thay Ä‘á»•i ngĂ¢n sĂ¡ch active. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½ |
| Trigger                     | Chá»n duyá»‡t/tá»« chá»‘i Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch. |
| Quy trĂ¬nh chuáº©n             | 1) Má»Ÿ Ä‘á» xuáº¥t pending. 2) Transaction Ä‘Ă³ng version cÅ©, má»Ÿ version má»›i. 3) Ghi nháº­t kĂ½ vĂ  cáº­p nháº­t tráº¡ng thĂ¡i Ä‘á» xuáº¥t. |
| Quy trĂ¬nh thay tháº¿          | 1) Tá»« chá»‘i Ä‘á» xuáº¥t. 2) Cáº­p nháº­t tráº¡ng thĂ¡i `TuChoi`. |

### 24) Quáº£n lĂ½ ngĂ¢n sĂ¡ch cá»§a dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ ngĂ¢n sĂ¡ch cá»§a dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | Theo dĂµi ngĂ¢n sĂ¡ch Ä‘Ă£ duyá»‡t, tráº¡ng thĂ¡i version vĂ  má»©c sá»­ dá»¥ng. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `NganSach.Xem`. |
| Äiá»u kiá»‡n háº­u               | KhĂ´ng báº¯t buá»™c ghi má»›i; cung cáº¥p sá»‘ liá»‡u giĂ¡m sĂ¡t. |
| TĂ¬nh huá»‘ng lá»—i              | Dá»± Ă¡n khĂ´ng cĂ³ ngĂ¢n sĂ¡ch duyá»‡t. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Hiá»ƒn thá»‹ cáº£nh bĂ¡o thiáº¿u dá»¯ liá»‡u ngĂ¢n sĂ¡ch. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½, NhĂ¢n viĂªn |
| Trigger                     | Má»Ÿ module ngĂ¢n sĂ¡ch. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n dá»± Ă¡n. 2) Náº¡p ngĂ¢n sĂ¡ch version active + lá»‹ch sá»­. 3) Hiá»ƒn thá»‹ tĂ¬nh tráº¡ng sá»­ dá»¥ng. |
| Quy trĂ¬nh thay tháº¿          | 1) ChÆ°a cĂ³ ngĂ¢n sĂ¡ch. 2) Tráº£ tráº¡ng thĂ¡i cáº£nh bĂ¡o. |

### 25) Quáº£n lĂ½ chi phĂ­
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ chi phĂ­ |
| MĂ´ táº£ ngáº¯n gá»n              | Theo dĂµi vĂ  ghi nháº­n chi phĂ­ gáº¯n ngĂ¢n sĂ¡ch/cĂ´ng viá»‡c. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ permission `ChiPhi.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `CHI_PHI` vĂ  `NHAT_KY_CHI_PHI`. |
| TĂ¬nh huá»‘ng lá»—i              | KhĂ´ng cĂ³ ngĂ¢n sĂ¡ch active hoáº·c dá»¯ liá»‡u chi phĂ­ sai. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng ghi chi phĂ­ má»›i. |
| CĂ¡c Actor giao tiáº¿p         | Quáº£n lĂ½ |
| Trigger                     | Ghi nháº­n/sá»­a chi phĂ­. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p thĂ´ng tin chi phĂ­. 2) Validate liĂªn káº¿t ngĂ¢n sĂ¡ch. 3) LÆ°u chi phĂ­ vĂ  nháº­t kĂ½. |
| Quy trĂ¬nh thay tháº¿          | 1) Thiáº¿u ngĂ¢n sĂ¡ch há»£p lá»‡. 2) Tá»« chá»‘i thao tĂ¡c. |

### 26) ÄĂ¡nh giĂ¡ nhĂ¢n viĂªn
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | ÄĂ¡nh giĂ¡ nhĂ¢n viĂªn |
| MĂ´ táº£ ngáº¯n gá»n              | Láº­p, gá»­i duyá»‡t, duyá»‡t/tá»« chá»‘i phiáº¿u Ä‘Ă¡nh giĂ¡ nhĂ¢n viĂªn. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `DanhGiaNhanVien.*` theo thao tĂ¡c. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `DANH_GIA_NHAN_VIEN` vĂ  `CT_DANH_GIA_NHAN_VIEN`. |
| TĂ¬nh huá»‘ng lá»—i              | Thiáº¿u tiĂªu chĂ­ hoáº·c khĂ´ng Ä‘Ăºng scope dá»± Ă¡n. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Phiáº¿u Ä‘Ă¡nh giĂ¡ khĂ´ng Ä‘á»•i. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | Táº¡o/lÆ°u/gá»­i duyá»‡t/duyá»‡t Ä‘Ă¡nh giĂ¡ nhĂ¢n viĂªn. |
| Quy trĂ¬nh chuáº©n             | 1) Nháº­p dá»¯ liá»‡u Ä‘Ă¡nh giĂ¡ theo tiĂªu chĂ­. 2) LÆ°u báº£n nhĂ¡p hoáº·c gá»­i duyá»‡t. 3) Quáº£n lĂ½ duyá»‡t hoáº·c tá»« chá»‘i. |
| Quy trĂ¬nh thay tháº¿          | 1) Tá»« chá»‘i duyá»‡t. 2) Ghi lĂ½ do tá»« chá»‘i. |

### 27) ÄĂ¡nh giĂ¡ dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | ÄĂ¡nh giĂ¡ dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | Láº­p vĂ  duyá»‡t Ä‘Ă¡nh giĂ¡ tá»•ng thá»ƒ dá»± Ă¡n, cĂ³ tham chiáº¿u AI. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `DanhGiaDuAn.*`. |
| Äiá»u kiá»‡n háº­u               | Cáº­p nháº­t `DANH_GIA_DU_AN`, `CT_DANH_GIA_DU_AN`. |
| TĂ¬nh huá»‘ng lá»—i              | Dá»± Ă¡n khĂ´ng há»£p lá»‡ hoáº·c dá»¯ liá»‡u tiĂªu chĂ­ thiáº¿u. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng Ä‘á»•i dá»¯ liá»‡u Ä‘Ă¡nh giĂ¡ dá»± Ă¡n. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | Táº¡o/lÆ°u/gá»­i duyá»‡t/duyá»‡t Ä‘Ă¡nh giĂ¡ dá»± Ă¡n. |
| Quy trĂ¬nh chuáº©n             | 1) Láº¥y thá»‘ng kĂª dá»± Ă¡n vĂ  dá»¯ liá»‡u AI tham chiáº¿u. 2) Nháº­p Ä‘iá»ƒm/nháº­n xĂ©t. 3) LÆ°u vĂ  xá»­ lĂ½ duyá»‡t. |
| Quy trĂ¬nh thay tháº¿          | 1) Tá»« chá»‘i Ä‘Ă¡nh giĂ¡. 2) Ghi lĂ½ do vĂ  tráº£ tráº¡ng thĂ¡i. |

### 28) Chat dá»± Ă¡n
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Chat dá»± Ă¡n |
| MĂ´ táº£ ngáº¯n gá»n              | Trao Ä‘á»•i tin nháº¯n trong phĂ²ng chat theo dá»± Ă¡n. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ `Chat.Xem`/`Chat.Gui`, thuá»™c scope dá»± Ă¡n. |
| Äiá»u kiá»‡n háº­u               | Ghi `TIN_NHAN`, Ä‘á»“ng bá»™ `THANH_VIEN_PHONG_CHAT`. |
| TĂ¬nh huá»‘ng lá»—i              | Dá»± Ă¡n Ä‘Ă³ng (`DaHuy`/`LuuTru`) hoáº·c khĂ´ng cĂ²n scope. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | KhĂ´ng lÆ°u tin nháº¯n. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | Gá»­i tin nháº¯n táº¡i mĂ n hĂ¬nh chat. |
| Quy trĂ¬nh chuáº©n             | 1) Chá»n phĂ²ng chat dá»± Ă¡n. 2) Kiá»ƒm tra quyá»n vĂ  tráº¡ng thĂ¡i dá»± Ă¡n. 3) LÆ°u tin nháº¯n má»›i. |
| Quy trĂ¬nh thay tháº¿          | 1) KhĂ´ng thuá»™c phĂ²ng chat há»£p lá»‡. 2) Tá»« chá»‘i gá»­i tin. |

### 29) Quáº£n lĂ½ bĂ¡o cĂ¡o
| Má»¥c                         | Ná»™i dung |
| --------------------------- | -------- |
| TĂªn                         | Quáº£n lĂ½ bĂ¡o cĂ¡o |
| MĂ´ táº£ ngáº¯n gá»n              | Xem dashboard tá»•ng há»£p vĂ  bĂ¡o cĂ¡o váº­n hĂ nh theo dá»¯ liá»‡u nghiá»‡p vá»¥. |
| Äiá»u kiá»‡n tiĂªn quyáº¿t        | ÄÄƒng nháº­p, cĂ³ quyá»n xem module liĂªn quan (`ThongKe.Xem`, quyá»n mĂ n hĂ¬nh). |
| Äiá»u kiá»‡n háº­u               | Sinh sá»‘ liá»‡u theo thá»i Ä‘iá»ƒm truy váº¥n. |
| TĂ¬nh huá»‘ng lá»—i              | Thiáº¿u dá»¯ liá»‡u nguá»“n hoáº·c khĂ´ng Ä‘á»§ quyá»n. |
| Tráº¡ng thĂ¡i há»‡ thá»‘ng khi lá»—i | Tráº£ cáº£nh bĂ¡o dá»¯ liá»‡u/permission. |
| CĂ¡c Actor giao tiáº¿p         | NhĂ¢n viĂªn, Quáº£n lĂ½ |
| Trigger                     | Má»Ÿ Dashboard hoáº·c mĂ n hĂ¬nh tá»•ng há»£p. |
| Quy trĂ¬nh chuáº©n             | 1) Náº¡p sá»‘ liá»‡u dá»± Ă¡n/cĂ´ng viá»‡c/ngĂ¢n sĂ¡ch/chi phĂ­. 2) TĂ­nh chá»‰ sá»‘ cáº£nh bĂ¡o. 3) Hiá»ƒn thá»‹ biá»ƒu Ä‘á»“ vĂ  danh sĂ¡ch gá»£i Ă½. |
| Quy trĂ¬nh thay tháº¿          | 1) KhĂ´ng cĂ³ dá»¯ liá»‡u. 2) Hiá»ƒn thá»‹ tráº¡ng thĂ¡i rá»—ng. |

### 30) Đồng bộ dữ liệu AI

| Mục | Nội dung |
|------|----------|
| Tên | Đồng bộ dữ liệu AI |
| Mô tả ngắn gọn | Tổng hợp dữ liệu nghiệp vụ vào `AI_DATASET` và đánh giá chất lượng train. |
| Điều kiện tiên quyết | Đăng nhập, có permission `AI.Dataset`. |
| Điều kiện hậu | `AI_DATASET` được tạo mới/cập nhật theo dự án. |
| Tình huống lỗi | Không có dự án hợp lệ hoặc thiếu dữ liệu nguồn. |
| Trạng thái hệ thống khi lỗi | Dataset không thay đổi. |
| Các Actor giao tiếp | Nhân viên, Quản lý |
| Trigger | Bấm tổng hợp dataset AI. |
| Quy trình chuẩn | 1) Thu thập số liệu dự án/công việc/ngân sách/nhân sự. 2) Chuẩn hóa feature và label. 3) Lưu `AI_DATASET` và báo cáo chất lượng. |
| Quy trình thay thế | 1) Dữ liệu chưa đạt điều kiện train. 2) Trả danh sách cảnh báo. |

### 31) Huấn luyện model AI

| Mục | Nội dung |
|------|----------|
| Tên | Huấn luyện model AI |
| Mô tả ngắn gọn | Gửi yêu cầu train model `TreHan` hoặc `NguyenNhan`. |
| Điều kiện tiên quyết | Đăng nhập, có `AI.Train`, dataset đạt chuẩn tối thiểu. |
| Điều kiện hậu | Cập nhật `AI_MODEL`, có thể đặt active model theo loại. |
| Tình huống lỗi | Dataset thiếu dòng, thiếu nhãn, không đủ lớp, API train lỗi. |
| Trạng thái hệ thống khi lỗi | Không ghi model active mới. |
| Các Actor giao tiếp | Nhân viên, Quản lý |
| Trigger | Bấm train model trên màn hình AI. |
| Quy trình chuẩn | 1) Kiểm tra chất lượng dataset. 2) Gọi FastAPI train. 3) Lưu metadata model và trạng thái active. |
| Quy trình thay thế | 1) Chất lượng dataset không đạt. 2) Chặn train và hiển thị lý do. |

### 32) Dự đoán AI

| Mục | Nội dung |
|------|----------|
| Tên | Dự đoán AI |
| Mô tả ngắn gọn | Dự đoán trễ hạn/nguyên nhân cho dự án và lưu kết quả tham chiếu. |
| Điều kiện tiên quyết | Đăng nhập, có permission `AI.DuDoan`, dữ liệu đầu vào hợp lệ. |
| Điều kiện hậu | Lưu `AI_KET_QUA` (nếu map dữ liệu hợp lệ) và hiển thị kết quả. |
| Tình huống lỗi | Lỗi schema/payload AI, thiếu danh mục nguyên nhân fallback. |
| Trạng thái hệ thống khi lỗi | Không lưu kết quả dự đoán vào DB. |
| Các Actor giao tiếp | Nhân viên, Quản lý |
| Trigger | Gửi yêu cầu dự đoán AI theo dự án. |
| Quy trình chuẩn | 1) Chuẩn hóa feature gửi FastAPI. 2) Nhận kết quả dự đoán. 3) Map nguyên nhân và lưu `AI_KET_QUA`. |
| Quy trình thay thế | 1) Không có model nguyên nhân hợp lệ. 2) Fallback rule và cảnh báo. |

### 33) Xác nhận nguyên nhân AI

| Mục | Nội dung |
|------|----------|
| Tên | Xác nhận nguyên nhân AI |
| Mô tả ngắn gọn | Quản lý xác nhận nguyên nhân cuối cùng cho dự án. |
| Điều kiện tiên quyết | Đăng nhập, là Manager hoặc có claim `AI.XacNhan`. |
| Điều kiện hậu | Cập nhật/ghi mới `AI_NGUYEN_NHAN`. |
| Tình huống lỗi | Mã nguyên nhân không hợp lệ hoặc không có quyền xác nhận. |
| Trạng thái hệ thống khi lỗi | Dữ liệu xác nhận nguyên nhân không đổi. |
| Các Actor giao tiếp | Quản lý |
| Trigger | Chọn xác nhận nguyên nhân trên màn hình AI hoặc đánh giá dự án. |
| Quy trình chuẩn | 1) Chọn dự án và nguyên nhân. 2) Kiểm tra quyền + danh mục nguyên nhân. 3) Lưu `AI_NGUYEN_NHAN`. |
| Quy trình thay thế | 1) Không đủ quyền. 2) Trả lỗi và dừng thao tác. |

# CHÆ¯Æ NG 3: CLASS DIAGRAM

## 3.1 SÆ¡ Ä‘á»“ quan há»‡ (Relationship Overview)
```plantuml
@startuml
hide circle
skinparam linetype ortho

class AspNetUsers
class AspNetRoles
class AspNetUserRoles
class AspNetRoleClaims
class AspNetUserClaims

class NGUOI_DUNG
class CHUC_DANH
class DANH_MUC_MAN_HINH
class DANH_MUC_QUYEN

class DU_AN
class LOAI_DU_AN
class TEAM
class NHAN_VIEN_TEAM
class TEAM_DU_AN
class NHAN_VIEN_DU_AN

class DANH_MUC_CONG_VIEC
class MUC_DO_UU_TIEN
class DE_XUAT_CONG_VIEC
class CONG_VIEC
class CT_CONG_VIEC
class PHAN_CONG_CONG_VIEC
class PHAN_CONG_CT_CONG_VIEC
class TIEN_DO_CONG_VIEC

class FILE_DU_AN
class FILE_CONG_VIEC
class FILE_CT_CONG_VIEC
class FILE_TIEN_DO_CONG_VIEC

class DE_XUAT_NGAN_SACH
class NGAN_SACH
class CHI_PHI

class NHAT_KY_NGAN_SACH
class NHAT_KY_CHI_PHI
class NHAT_KY_DU_AN
class NHAT_KY_QUAN_LY_DU_AN
class NHAT_KY_PHU_TRACH_DU_AN
class NHAT_KY_PHAN_CONG_CONG_VIEC
class NHAT_KY_PHAN_CONG_CT_CONG_VIEC

class TIEU_CHI_DANH_GIA
class DANH_GIA_DU_AN
class CT_DANH_GIA_DU_AN
class DANH_GIA_NHAN_VIEN
class CT_DANH_GIA_NHAN_VIEN

class PHONG_CHAT
class THANH_VIEN_PHONG_CHAT
class TIN_NHAN

class AI_DATASET
class AI_MODEL
class AI_KET_QUA
class AI_NGUYEN_NHAN
class DM_NGUYEN_NHAN
class YEU_CAU_DOI_QUAN_LY

AspNetUsers -- NGUOI_DUNG
AspNetUsers -- AspNetUserRoles
AspNetRoles -- AspNetUserRoles
AspNetRoles -- AspNetRoleClaims
AspNetUsers -- AspNetUserClaims
CHUC_DANH -- NGUOI_DUNG

DANH_MUC_MAN_HINH -- DANH_MUC_QUYEN

NGUOI_DUNG -- DU_AN
LOAI_DU_AN -- DU_AN
TEAM -- NHAN_VIEN_TEAM
NGUOI_DUNG -- NHAN_VIEN_TEAM
DU_AN -- TEAM_DU_AN
TEAM -- TEAM_DU_AN
DU_AN -- NHAN_VIEN_DU_AN
NGUOI_DUNG -- NHAN_VIEN_DU_AN

DU_AN -- DANH_MUC_CONG_VIEC
DANH_MUC_CONG_VIEC -- CONG_VIEC
MUC_DO_UU_TIEN -- CONG_VIEC
DE_XUAT_CONG_VIEC -- CONG_VIEC
CONG_VIEC -- CT_CONG_VIEC
CONG_VIEC -- PHAN_CONG_CONG_VIEC
NGUOI_DUNG -- PHAN_CONG_CONG_VIEC
CT_CONG_VIEC -- PHAN_CONG_CT_CONG_VIEC
NGUOI_DUNG -- PHAN_CONG_CT_CONG_VIEC
CT_CONG_VIEC -- TIEN_DO_CONG_VIEC
NGUOI_DUNG -- TIEN_DO_CONG_VIEC

DU_AN -- FILE_DU_AN
CONG_VIEC -- FILE_CONG_VIEC
CT_CONG_VIEC -- FILE_CT_CONG_VIEC
TIEN_DO_CONG_VIEC -- FILE_TIEN_DO_CONG_VIEC

DU_AN -- DE_XUAT_NGAN_SACH
DU_AN -- NGAN_SACH
NGAN_SACH -- CHI_PHI
CONG_VIEC -- CHI_PHI

NGAN_SACH -- NHAT_KY_NGAN_SACH
CHI_PHI -- NHAT_KY_CHI_PHI
DU_AN -- NHAT_KY_DU_AN
DU_AN -- NHAT_KY_QUAN_LY_DU_AN
DU_AN -- NHAT_KY_PHU_TRACH_DU_AN
CONG_VIEC -- NHAT_KY_PHAN_CONG_CONG_VIEC
CT_CONG_VIEC -- NHAT_KY_PHAN_CONG_CT_CONG_VIEC

DU_AN -- DANH_GIA_DU_AN
DANH_GIA_DU_AN -- CT_DANH_GIA_DU_AN
TIEU_CHI_DANH_GIA -- CT_DANH_GIA_DU_AN
DU_AN -- DANH_GIA_NHAN_VIEN
DANH_GIA_NHAN_VIEN -- CT_DANH_GIA_NHAN_VIEN
TIEU_CHI_DANH_GIA -- CT_DANH_GIA_NHAN_VIEN
CONG_VIEC -- CT_DANH_GIA_NHAN_VIEN

DU_AN -- PHONG_CHAT
PHONG_CHAT -- THANH_VIEN_PHONG_CHAT
NGUOI_DUNG -- THANH_VIEN_PHONG_CHAT
PHONG_CHAT -- TIN_NHAN
NGUOI_DUNG -- TIN_NHAN

DU_AN -- AI_DATASET
AI_MODEL -- AI_KET_QUA
AI_DATASET -- AI_KET_QUA
DU_AN -- AI_KET_QUA
DM_NGUYEN_NHAN -- AI_KET_QUA
DU_AN -- AI_NGUYEN_NHAN
DM_NGUYEN_NHAN -- AI_NGUYEN_NHAN

DU_AN -- YEU_CAU_DOI_QUAN_LY
NGUOI_DUNG -- YEU_CAU_DOI_QUAN_LY
@enduml
```

## 3.2 SÆ¡ Ä‘á»“ cáº¥u trĂºc lá»›p (Class Structure)
```plantuml
@startuml
hide circle
skinparam classAttributeIconSize 0

class AspNetUsers {
  +Id (PK)
  +MaNguoiDung (FK -> NGUOI_DUNG)
  +UserName
  +NormalizedUserName
  +PasswordHash
  +LockoutEnd
}
class AspNetRoles {
  +Id (PK)
  +Name
}
class AspNetUserRoles {
  +Asp_Id (PK, FK -> AspNetUsers)
  +Id (PK, FK -> AspNetRoles)
}
class AspNetRoleClaims {
  +Id (PK)
  +Asp_Id (FK -> AspNetRoles)
  +MaDanhMucQuyen (FK -> DANH_MUC_QUYEN)
  +ClaimType
  +ClaimValue
}
class AspNetUserClaims {
  +Id (PK)
  +Asp_Id (FK -> AspNetUsers)
  +ClaimType
  +ClaimValue
}
class NGUOI_DUNG {
  +MaNguoiDung (PK)
  +MaChucDanh (FK -> CHUC_DANH)
  +Id (FK -> AspNetUsers)
  +HoTenNguoiDung
  +SdtNguoiDung
  +IsDeleted
}
class CHUC_DANH {
  +MaChucDanh (PK)
  +TenChucDanh
}
class DANH_MUC_MAN_HINH {
  +MaManHinh (PK)
  +TenManHinh
}
class DANH_MUC_QUYEN {
  +MaDanhMucQuyen (PK)
  +MaManHinh (FK -> DANH_MUC_MAN_HINH)
  +TenDanhMucQuyen
}
@enduml
```

```plantuml
@startuml
hide circle
skinparam classAttributeIconSize 0

class DU_AN {
  +MaDuAn (PK)
  +MaNguoiDung (FK -> NGUOI_DUNG)
  +MaLoaiDuAn (FK -> LOAI_DU_AN)
  +TenDuAn
  +TrangThaiDuAn
  +PhanTramHoanThanh
}
class LOAI_DU_AN {
  +MaLoaiDuAn (PK)
  +TenLoai
}
class TEAM {
  +MaTeam (PK)
  +TenTeam
  +TrangThaiTeam
}
class NHAN_VIEN_TEAM {
  +MaNguoiDung (PK, FK -> NGUOI_DUNG)
  +MaTeam (PK, FK -> TEAM)
  +IsLeader
}
class TEAM_DU_AN {
  +MaTeam (PK, FK -> TEAM)
  +MaDuAn (PK, FK -> DU_AN)
}
class NHAN_VIEN_DU_AN {
  +MaDuAn (PK, FK -> DU_AN)
  +MaNguoiDung (PK, FK -> NGUOI_DUNG)
  +VaiTroTrongDuAn
}
class YEU_CAU_DOI_QUAN_LY {
  +MaYeuCauDoiQuanLy (PK)
  +MaDuAn (FK -> DU_AN)
  +MaQuanLyHienTai (FK -> NGUOI_DUNG)
  +MaQuanLyDeXuat (FK -> NGUOI_DUNG)
  +MaNguoiDungDuyet (FK -> NGUOI_DUNG)
  +TrangThaiYeuCauDoiQuanLy
}

class DANH_MUC_CONG_VIEC {
  +MaDanhMucCV (PK)
  +MaDuAn (FK -> DU_AN)
  +TenDanhMucCV
}
class MUC_DO_UU_TIEN {
  +MaMucDo (PK)
  +TenMucDo
}
class DE_XUAT_CONG_VIEC {
  +MaDeXuatCV (PK)
  +MaDuAn (FK -> DU_AN)
  +MaDanhMucCV (FK -> DANH_MUC_CONG_VIEC)
  +MaMucDo (FK -> MUC_DO_UU_TIEN)
  +MaNguoiDungDeXuat (FK -> NGUOI_DUNG)
  +MaNguoiDungDuyet (FK -> NGUOI_DUNG)
  +TrangThaiCongViecDeXuat
}
class CONG_VIEC {
  +MaCongViec (PK)
  +MaDeXuatCV (FK -> DE_XUAT_CONG_VIEC)
  +MaDanhMucCV (FK -> DANH_MUC_CONG_VIEC)
  +MaMucDo (FK -> MUC_DO_UU_TIEN)
  +TrangThaiCongViec
}
class CT_CONG_VIEC {
  +MaChiTietCV (PK)
  +MaCongViec (FK -> CONG_VIEC)
  +TrangThaiCTCV
}
class PHAN_CONG_CONG_VIEC {
  +MaNguoiDung (PK, FK -> NGUOI_DUNG)
  +MaCongViec (PK, FK -> CONG_VIEC)
  +NgayGiaoViec
}
class PHAN_CONG_CT_CONG_VIEC {
  +MaNguoiDung (PK, FK -> NGUOI_DUNG)
  +MaChiTietCV (PK, FK -> CT_CONG_VIEC)
  +NgayGiaoCTCV
}
class TIEN_DO_CONG_VIEC {
  +MaTienDo (PK)
  +MaChiTietCV (FK -> CT_CONG_VIEC)
  +MaNguoiDung (FK -> NGUOI_DUNG)
  +MaNguoiDungDuyet (FK -> NGUOI_DUNG)
  +TrangThaiCTCVDeXuat
  +TrangThaiTienDo
}
@enduml
```

```plantuml
@startuml
hide circle
skinparam classAttributeIconSize 0

class FILE_DU_AN {
  +MaFileDA (PK)
  +MaDuAn (FK -> DU_AN)
}
class FILE_CONG_VIEC {
  +MaFileCV (PK)
  +MaCongViec (FK -> CONG_VIEC)
}
class FILE_CT_CONG_VIEC {
  +MaFileCTCV (PK)
  +MaChiTietCV (FK -> CT_CONG_VIEC)
}
class FILE_TIEN_DO_CONG_VIEC {
  +MaFileTDCV (PK)
  +MaTienDo (FK -> TIEN_DO_CONG_VIEC)
}

class DE_XUAT_NGAN_SACH {
  +MaDeXuatNS (PK)
  +MaDuAn (FK -> DU_AN)
  +MaNganSachCu (FK -> NGAN_SACH)
  +MaNguoiDungDeXuat (FK -> NGUOI_DUNG)
  +MaNguoiDungDuyet (FK -> NGUOI_DUNG)
  +TrangThaiDeXuat
}
class NGAN_SACH {
  +MaNganSach (PK)
  +MaDuAn (FK -> DU_AN)
  +MaNguoiDungDeXuat (FK -> NGUOI_DUNG)
  +MaNguoiDungDuyet (FK -> NGUOI_DUNG)
  +NganSach
  +TrangThaiNganSach
}
class CHI_PHI {
  +MaChiPhi (PK)
  +MaCongViec (FK -> CONG_VIEC)
  +MaNganSach (FK -> NGAN_SACH)
  +SoTienDaChi
  +TrangThaiChiPhi
}

class NHAT_KY_NGAN_SACH {
  +MaNhatKyNS (PK)
  +MaNganSach (FK -> NGAN_SACH)
  +MaDuAn (FK -> DU_AN)
}
class NHAT_KY_CHI_PHI {
  +MaNhatKyCP (PK)
  +MaChiPhi (FK -> CHI_PHI)
  +MaCongViec (FK -> CONG_VIEC)
}
class NHAT_KY_DU_AN {
  +MaNhatKyTeamDA (PK)
  +MaTeam (FK -> TEAM)
  +MaDuAn (FK -> DU_AN)
}
class NHAT_KY_QUAN_LY_DU_AN {
  +MaNhatKyQLDA (PK)
  +MaDuAn (FK -> DU_AN)
  +MaNguoiDung (FK -> NGUOI_DUNG)
}
class NHAT_KY_PHU_TRACH_DU_AN {
  +MaNhatKyPTDA (PK)
  +MaDuAn (FK -> DU_AN)
  +MaNguoiDung (FK -> NGUOI_DUNG)
}
class NHAT_KY_PHAN_CONG_CONG_VIEC {
  +MaNhatKyPCCV (PK)
  +MaCongViec (FK -> CONG_VIEC)
  +MaNguoiDung (FK -> NGUOI_DUNG)
}
class NHAT_KY_PHAN_CONG_CT_CONG_VIEC {
  +MaNhatKyPCCTCV (PK)
  +MaChiTietCV (FK -> CT_CONG_VIEC)
  +MaNguoiDung (FK -> NGUOI_DUNG)
}

class TIEU_CHI_DANH_GIA {
  +MaTieuChi (PK)
  +TenTieuChi
}
class DANH_GIA_DU_AN {
  +MaDanhGiaDuAn (PK)
  +MaDuAn (FK -> DU_AN)
  +MaNguoiDung (FK -> NGUOI_DUNG)
  +MaNguoiDungDuyet (FK -> NGUOI_DUNG)
}
class CT_DANH_GIA_DU_AN {
  +MaChiTietDGDA (PK)
  +MaDanhGiaDuAn (FK -> DANH_GIA_DU_AN)
  +MaTieuChi (FK -> TIEU_CHI_DANH_GIA)
}
class DANH_GIA_NHAN_VIEN {
  +MaDanhGiaNhanVien (PK)
  +MaNguoiDung (FK -> NGUOI_DUNG)
  +MaDuAn (FK -> DU_AN)
  +MaNguoiDungDanhGia (FK -> NGUOI_DUNG)
  +MaNguoiDungDuyet (FK -> NGUOI_DUNG)
}
class CT_DANH_GIA_NHAN_VIEN {
  +MaChiTietDGNV (PK)
  +MaDanhGiaNhanVien (FK -> DANH_GIA_NHAN_VIEN)
  +MaTieuChi (FK -> TIEU_CHI_DANH_GIA)
  +MaCongViec (FK -> CONG_VIEC)
}

class PHONG_CHAT {
  +MaPhongChat (PK)
  +MaDuAn (FK -> DU_AN)
}
class THANH_VIEN_PHONG_CHAT {
  +MaPhongChat (PK, FK -> PHONG_CHAT)
  +MaNguoiDung (PK, FK -> NGUOI_DUNG)
}
class TIN_NHAN {
  +MaTinNhan (PK)
  +MaPhongChat (FK -> PHONG_CHAT)
  +MaNguoiDung (FK -> NGUOI_DUNG)
}

class AI_DATASET {
  +MaData (PK)
  +MaDuAn (FK -> DU_AN)
  +IsTre
}
class AI_MODEL {
  +MaModel (PK)
  +TenModel
  +LoaiModel
  +IsActive
}
class AI_KET_QUA {
  +MaAiKetQua (PK)
  +MaModel (FK -> AI_MODEL)
  +MaData (FK -> AI_DATASET)
  +MaDuAn (FK -> DU_AN)
  +MaDMNguyenNhan (FK -> DM_NGUYEN_NHAN)
}
class AI_NGUYEN_NHAN {
  +MaAINguyenNhan (PK)
  +MaDuAn (FK -> DU_AN)
  +MaDMNguyenNhan (FK -> DM_NGUYEN_NHAN)
}
class DM_NGUYEN_NHAN {
  +MaDMNguyenNhan (PK)
  +TenNguyenNhan
}
@enduml
```

## 3.3 SÆ¡ Ä‘á»“ phá»¥ thuá»™c workflow
```plantuml
@startuml
left to right direction

rectangle "Workflow Ä‘á» xuáº¥t cĂ´ng viá»‡c" {
  [DE_XUAT_CONG_VIEC: ChoDuyet] --> [DuyetDeXuatCongViecService.Approve]
  [DuyetDeXuatCongViecService.Approve] --> [CONG_VIEC: ChuaBatDau]
  [DuyetDeXuatCongViecService.Approve] --> [CHI_PHI: DaDuyet]
}

rectangle "Workflow tiáº¿n Ä‘á»™" {
  [TIEN_DO_CONG_VIEC: ChoDuyet] --> [TienDoCongViecService.DuyetBaoCaoTienDoAsync]
  [TienDoCongViecService.DuyetBaoCaoTienDoAsync] --> [CT_CONG_VIEC: TrangThaiCTCV]
  [CT_CONG_VIEC: TrangThaiCTCV] --> [TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync]
  [CONG_VIEC: TrangThaiCongViec] --> [TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync]
  [DU_AN: DangThucHien/ChoXacNhanHoanThanh] --> [DuAnService.XacNhanHoanThanhAsync]
}

rectangle "Workflow Ä‘á» xuáº¥t ngĂ¢n sĂ¡ch" {
  [DE_XUAT_NGAN_SACH: ChoDuyet] --> [DuyetDeXuatNganSachService.Approve]
  [DuyetDeXuatNganSachService.Approve] --> [NGAN_SACH version má»›i: DaDuyet + IsActive]
  [NGAN_SACH version cÅ©] --> [DaThayThe + IsActive=false]
}

rectangle "Workflow Ä‘á»•i quáº£n lĂ½" {
  [YEU_CAU_DOI_QUAN_LY: ChoDuyet] --> [DuyetYeuCauDoiQuanLyService.Approve]
  [DuyetYeuCauDoiQuanLyService.Approve] --> [DU_AN.MaNguoiDung cáº­p nháº­t]
}

rectangle "Workflow AI" {
  [AiDatasetService.TongHop] --> [AI_DATASET]
  [AiService.TrainAsync] --> [AI_MODEL]
  [AiService.DuDoanDuAnAsync] --> [AI_KET_QUA]
  [AiService.XacNhanNguyenNhanAsync] --> [AI_NGUYEN_NHAN]
}
@enduml
```

# CHƯƠNG 4: SEQUENCE DIAGRAM


## 4.1 Đăng nhập hệ thống
- Mô tả: Người dùng đăng nhập qua `AccountController.Login`, xác thực bằng `AccountService.AuthenticateAsync`, nạp role + permission claim để cấp quyền truy cập các module.
- Actor: Người dùng
```plantuml
@startuml
actor "Người dùng" as ND
participant "AccountController" as ACC
participant "AccountService" as ACS
database "Database" as DB

ND -> ACC : POST /Account/Login(userName,password)
ACC -> ACC : Validate ModelState
alt ModelState không hợp lệ
  ACC --> ND : Trả lại form + lỗi validate
else Hợp lệ
  ACC -> ACS : AuthenticateAsync(userName,password)
  ACS -> DB : Tìm AspNetUsers + kiểm tra IsActive
  DB --> ACS : Bản ghi người dùng
  alt Không tồn tại / bị khóa / sai mật khẩu
    ACS --> ACC : Throw Exception
    ACC --> ND : Thông báo đăng nhập thất bại
  else Hợp lệ
    par Nạp role và claim
      ACS -> DB : Load AspNetUserRoles + AspNetRoles
      ACS -> DB : Load AspNetRoleClaims(type=permission)
    and Tạo principal
      ACS -> ACS : Build ClaimsIdentity + permission claim
    end
    ACS --> ACC : ClaimsPrincipal
    ACC -> ACC : HttpContext.SignInAsync(cookie)
    ACC --> ND : Redirect returnUrl/Home/Index
  end
end
@enduml
```

## 4.2 Phân quyền hệ thống
- Mô tả: Admin mở trang phân quyền và lưu danh sách permission cho role qua `PhanQuyenService`, ghi vào `AspNetRoleClaims` theo transaction.
- Actor: Quản trị viên
```plantuml
@startuml
actor "Quản trị viên" as AD
participant "PhanQuyenController" as PQC
participant "PermissionHelper" as PH
participant "PhanQuyenService" as PQS
database "Database" as DB

ref over AD,PQC
Đăng nhập hệ thống
end ref

AD -> PQC : POST /PhanQuyen/Save(roleId, permissions[])
PQC -> PH : HasPermissionAsync(User, PhanQuyen.Luu)
PH -> DB : Kiểm tra claim permission
DB --> PH : Kết quả
alt Không có quyền
  PH --> PQC : false
  PQC --> AD : 403/AccessDenied
else Có quyền
  PH --> PQC : true
  PQC -> PQS : SaveAsync(model)
  PQS -> PQS : Validate roleId, validate permission hợp lệ
  alt Thiếu role / role không tồn tại
    PQS --> PQC : Throw Exception
    PQC --> AD : Thông báo lỗi dữ liệu
  else Hợp lệ
    PQS -> DB : BeginTransaction
    PQS -> DB : Xóa RoleClaims(permission) cũ của role
    PQS -> DB : Thêm RoleClaims(permission) mới
    alt Role Admin thiếu PhanQuyen.Xem/Luu
      PQS -> DB : Rollback
      PQS --> PQC : Throw Exception
      PQC --> AD : Báo lỗi rule bắt buộc Admin
    else Đạt rule
      PQS -> DB : SaveChanges + Commit
      PQS --> PQC : Thành công
      PQC --> AD : Thông báo lưu phân quyền thành công
    end
  end
end
@enduml
```

## 4.3 Tạo dự án
- Mô tả: Manager/Admin tạo mới dự án qua `DuAnController.LuuDuAn` và `DuAnService.SaveAsync`, kiểm tra quyền, validate dữ liệu ngày, loại dự án và trạng thái khởi tạo.
- Actor: Quản lý dự án
```plantuml
@startuml
actor "Quản lý dự án" as QL
participant "DuAnController" as DAC
participant "PermissionHelper" as PH
participant "DuAnService" as DAS
database "Database" as DB

ref over QL,DAC
Đăng nhập hệ thống
end ref

QL -> DAC : POST /DuAn/LuuDuAn(vm)
DAC -> PH : HasPermissionAsync(User, DuAn.Them)
PH -> DB : Kiểm tra claim DuAn.Them
DB --> PH : Kết quả
alt Không có quyền
  DAC --> QL : 403/AccessDenied
else Có quyền
  DAC -> DAS : SaveAsync(vm)
  DAS -> DAS : Validate TenDuAn, MaLoaiDuAn, ngày bắt đầu/kết thúc
  alt Dữ liệu không hợp lệ
    DAS --> DAC : Throw Exception
    DAC --> QL : Trả form + lỗi validate
  else Hợp lệ
    DAS -> DB : Kiểm tra trùng tên theo scope/soft delete
    DB --> DAS : Kết quả
    alt Trùng dữ liệu/ràng buộc không đạt
      DAS --> DAC : Throw Exception
      DAC --> QL : Báo lỗi nghiệp vụ
    else Hợp lệ
      DAS -> DB : INSERT DU_AN(TrangThaiDuAn=KhoiTao)
      DAS -> DB : INSERT NHAT_KY_DU_AN/NHAT_KY_QUAN_LY_DU_AN nếu có
      DAS -> DB : SaveChanges
      DAS --> DAC : Thành công
      DAC --> QL : Redirect danh sách + thông báo tạo thành công
    end
  end
end
@enduml
```

## 4.4 Chuyển trạng thái dự án
- Mô tả: Chuyển trạng thái dự án qua các action `BatDauDuAn`, `YeuCauHoanThanh`, `XacNhanHoanThanh`, `MoLaiDuAn`, `TamDungDuAn`; service kiểm tra workflow và đồng bộ trạng thái.
- Actor: Quản lý dự án
```plantuml
@startuml
actor "Quản lý dự án" as QL
participant "DuAnController" as DAC
participant "PermissionHelper" as PH
participant "DuAnService" as DAS
participant "TrangThaiWorkflowService" as WFS
database "Database" as DB

QL -> DAC : POST chuyển trạng thái dự án
DAC -> PH : HasPermissionAsync(User, DuAn.Sua)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DAC --> QL : 403/AccessDenied
else Có quyền
  DAC -> DAS : Transition/Request/Confirm/MoLai/Pause
  DAS -> DB : Load DU_AN + quyền manager theo scope
  DB --> DAS : Dự án hiện tại
  alt Dự án không tồn tại
    DAS --> DAC : Throw Exception
    DAC --> QL : Báo lỗi không tìm thấy
  else Tồn tại
    DAS -> DAS : Validate trạng thái hiện tại theo TrangThai.cs
    alt Sai workflow (ví dụ chưa đủ điều kiện hoàn thành)
      DAS --> DAC : Throw Exception
      DAC --> QL : Thông báo trạng thái không hợp lệ
    else Hợp lệ
      DAS -> DB : Update TrangThaiDuAn + ghi nhật ký
      alt Luồng xác nhận hoàn thành
        DAS -> DAS : ValidateCompletionAsync(maDuAn)
        DAS -> DB : Kiểm tra toàn bộ CONG_VIEC/CT_CONG_VIEC đã hoàn tất
      else Luồng mở lại
        DAS -> WFS : DongBoTrangThaiDuAnTheoCongViecAsync(maDuAn,...)
        WFS -> DB : Đồng bộ rollback/forward trạng thái DU_AN
      end
      DAS -> DB : SaveChanges
      DAS --> DAC : Thành công
      DAC --> QL : Hiển thị thông báo cập nhật trạng thái thành công
    end
  end
end
@enduml
```

## 4.5 Yêu cầu đổi quản lý dự án
- Mô tả: Manager tạo yêu cầu đổi quản lý qua `YeuCauDoiQuanLyService.CreateAsync`, kiểm tra pending request và ứng viên quản lý mới.
- Actor: Quản lý dự án hiện tại
```plantuml
@startuml
actor "Quản lý hiện tại" as QL
participant "YeuCauDoiQuanLyController" as YCC
participant "PermissionHelper" as PH
participant "YeuCauDoiQuanLyService" as YCS
database "Database" as DB

QL -> YCC : POST /YeuCauDoiQuanLy/Create(vm)
YCC -> PH : HasPermissionAsync(User, YeuCauDoiQuanLy.Them)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  YCC --> QL : 403/AccessDenied
else Có quyền
  YCC -> YCS : CreateAsync(model)
  YCS -> YCS : Validate MaDuAn, MaQuanLyDeXuat, LyDo
  YCS -> DB : Load DU_AN + kiểm tra manager hiện tại
  DB --> YCS : Dữ liệu dự án
  alt Không thuộc scope quản lý
    YCS --> YCC : Throw Exception
    YCC --> QL : Báo lỗi không đủ quyền
  else Hợp lệ
    YCS -> DB : Kiểm tra request ChoDuyet đang tồn tại
    DB --> YCS : Kết quả
    alt Đã có request chờ duyệt
      YCS --> YCC : Throw Exception
      YCC --> QL : Báo lỗi trùng yêu cầu
    else Chưa có request pending
      YCS -> DB : Validate quản lý đề xuất tồn tại + đang hoạt động
      DB --> YCS : Kết quả
      alt Ứng viên không hợp lệ
        YCS --> YCC : Throw Exception
        YCC --> QL : Báo lỗi dữ liệu quản lý mới
      else Hợp lệ
        YCS -> DB : INSERT YEU_CAU_DOI_QUAN_LY(TrangThai=ChoDuyet)
        YCS -> DB : SaveChanges
        YCS --> YCC : Thành công
        YCC --> QL : Thông báo gửi yêu cầu thành công
      end
    end
  end
end
@enduml
```

## 4.6 Duyệt yêu cầu đổi quản lý
- Mô tả: Người có quyền `DuyetYeuCauDoiQuanLy.Duyet` phê duyệt hoặc từ chối; khi duyệt sẽ đổi `DU_AN.MaNguoiDung` và cập nhật nhật ký trong transaction.
- Actor: Quản trị viên/Manager có quyền duyệt
```plantuml
@startuml
actor "Người duyệt" as DD
participant "DuyetYeuCauDoiQuanLyController" as DYC
participant "PermissionHelper" as PH
participant "DuyetYeuCauDoiQuanLyService" as DYS
database "Database" as DB

DD -> DYC : POST /DuyetYeuCauDoiQuanLy/Approve(id) hoặc Reject(id)
DYC -> PH : HasPermissionAsync(User, DuyetYeuCauDoiQuanLy.Duyet)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DYC --> DD : 403/AccessDenied
else Có quyền
  DYC -> DYS : ApproveAsync(id) / RejectAsync(id,lyDo)
  DYS -> DB : Load YEU_CAU_DOI_QUAN_LY + DU_AN
  DB --> DYS : Bản ghi yêu cầu
  alt Không tồn tại hoặc không ở ChoDuyet
    DYS --> DYC : Throw Exception
    DYC --> DD : Báo lỗi trạng thái duyệt
  else Hợp lệ
    alt Reject
      DYS -> DB : Update TrangThai=TuChoi + LyDo + MaNguoiDungDuyet
      DYS -> DB : SaveChanges
      DYS --> DYC : Thành công
      DYC --> DD : Thông báo từ chối thành công
    else Approve
      DYS -> DB : BeginTransaction
      DYS -> DB : Update DU_AN.MaNguoiDung = MaQuanLyDeXuat
      DYS -> DB : Update YEU_CAU_DOI_QUAN_LY.TrangThai=DaDuyet
      par Ghi lịch sử
        DYS -> DB : INSERT NHAT_KY_QUAN_LY_DU_AN
      and Đồng bộ nhật ký dự án
        DYS -> DB : INSERT NHAT_KY_DU_AN
      end
      alt Lỗi khi cập nhật
        DYS -> DB : Rollback
        DYS --> DYC : Throw Exception
        DYC --> DD : Báo lỗi duyệt
      else Thành công
        DYS -> DB : Commit
        DYS --> DYC : Thành công
        DYC --> DD : Thông báo duyệt thành công
      end
    end
  end
end
@enduml
```

## 4.7 Thêm thành viên dự án
- Mô tả: `NhanVienDuAnService.AddAsync` thêm thành viên vào `NHAN_VIEN_DU_AN`, kiểm tra trạng thái dự án, trùng dữ liệu và đồng bộ room chat dự án.
- Actor: Quản lý dự án
```plantuml
@startuml
actor "Quản lý dự án" as QL
participant "NhanVienDuAnController" as NVC
participant "PermissionHelper" as PH
participant "NhanVienDuAnService" as NVS
participant "ChatDuAnService" as CDS
database "Database" as DB

QL -> NVC : POST /NhanVienDuAn/ThemNhanVien(maDuAn,danhSachNguoiDung)
NVC -> PH : HasPermissionAsync(User, ThanhVienDuAn.Them)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  NVC --> QL : 403/AccessDenied
else Có quyền
  NVC -> NVS : AddAsync(maDuAn, users)
  NVS -> DB : Load DU_AN + kiểm tra scope manager
  DB --> NVS : Dự án
  alt Dự án đóng/hoàn thành hoặc không thuộc scope
    NVS --> NVC : Throw Exception
    NVC --> QL : Báo lỗi workflow/quyền
  else Hợp lệ
    NVS -> DB : Lọc user đã có trong NHAN_VIEN_DU_AN
    DB --> NVS : Danh sách tồn tại
    alt Danh sách thêm rỗng
      NVS --> NVC : Throw Exception
      NVC --> QL : Báo lỗi không có thành viên mới
    else Có bản ghi mới
      NVS -> DB : INSERT NHAN_VIEN_DU_AN
      NVS -> DB : SaveChanges
      NVS -> CDS : DongBoThanhVienPhongChatTheoDuAnAsync(maDuAn)
      CDS -> DB : Upsert PHONG_CHAT + THANH_VIEN_PHONG_CHAT
      CDS --> NVS : Đồng bộ xong
      NVS --> NVC : Thành công
      NVC --> QL : Thông báo thêm thành viên thành công
    end
  end
end
@enduml
```

## 4.8 Phân công công việc
- Mô tả: `PhanCongCongViecService.AddAsync` gán người thực hiện cho `CONG_VIEC`, có kiểm tra quyền `PhanCongCongViec.ThucHien`, trạng thái công việc và thành viên dự án.
- Actor: Trưởng nhóm/Leader dự án
```plantuml
@startuml
actor "Trưởng nhóm/Leader" as TL
participant "PhanCongCongViecController" as PCC
participant "PermissionHelper" as PH
participant "PhanCongCongViecService" as PCS
database "Database" as DB

TL -> PCC : POST /PhanCongCongViec/ThemPhanCong(maCongViec,maNguoiDung)
PCC -> PH : HasPermissionAsync(User, PhanCongCongViec.ThucHien)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  PCC --> TL : 403/AccessDenied
else Có quyền
  PCC -> PCS : AddAsync(input)
  PCS -> DB : Load CONG_VIEC + DANH_MUC_CONG_VIEC + DU_AN
  DB --> PCS : Context
  alt Không tìm thấy công việc
    PCS --> PCC : Throw Exception
    PCC --> TL : Báo lỗi dữ liệu
  else Tồn tại
    PCS -> PCS : Kiểm tra trạng thái công việc/dự án có bị khóa cập nhật
    alt Công việc đã hoàn thành/tạm dừng/hủy
      PCS --> PCC : Từ chối phân công
      PCC --> TL : Báo lỗi workflow
    else Hợp lệ
      PCS -> DB : Kiểm tra người nhận là thành viên DU_AN
      DB --> PCS : Kết quả
      alt Không thuộc dự án
        PCS --> PCC : Throw Exception
        PCC --> TL : Báo lỗi phạm vi dữ liệu
      else Hợp lệ
        PCS -> DB : Kiểm tra trùng PHAN_CONG_CONG_VIEC
        DB --> PCS : Kết quả
        alt Đã phân công
          PCS --> PCC : Throw Exception
          PCC --> TL : Báo lỗi trùng phân công
        else Chưa phân công
          PCS -> DB : INSERT PHAN_CONG_CONG_VIEC
          PCS -> DB : INSERT NHAT_KY_PHAN_CONG_CONG_VIEC
          PCS -> DB : SaveChanges
          PCS --> PCC : Thành công
          PCC --> TL : Thông báo phân công thành công
        end
      end
    end
  end
end
@enduml
```

## 4.9 Phân công chi tiết công việc
- Mô tả: `PhanCongChiTietCongViecService.AddAsync` phân công trên `CT_CONG_VIEC`, bắt buộc người được gán đã có phân công ở cấp công việc cha.
- Actor: Trưởng nhóm/Leader dự án
```plantuml
@startuml
actor "Trưởng nhóm/Leader" as TL
participant "PhanCongChiTietCongViecController" as PCTC
participant "PermissionHelper" as PH
participant "PhanCongChiTietCongViecService" as PCTS
database "Database" as DB

TL -> PCTC : POST /PhanCongChiTietCongViec/ThemPhanCong(maChiTietCv,maNguoiDung)
PCTC -> PH : HasPermissionAsync(User, PhanCongChiTietCongViec.ThucHien)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  PCTC --> TL : 403/AccessDenied
else Có quyền
  PCTC -> PCTS : AddAsync(input)
  PCTS -> DB : Load CT_CONG_VIEC + CONG_VIEC + DU_AN
  DB --> PCTS : Context
  alt Không tìm thấy chi tiết
    PCTS --> PCTC : Throw Exception
    PCTC --> TL : Báo lỗi dữ liệu
  else Tồn tại
    PCTS -> PCTS : Validate trạng thái công việc cha/chi tiết
    alt Workflow không cho phép cập nhật
      PCTS --> PCTC : Từ chối phân công
      PCTC --> TL : Báo lỗi trạng thái
    else Hợp lệ
      PCTS -> DB : Kiểm tra user đã được phân công ở PHAN_CONG_CONG_VIEC
      DB --> PCTS : Kết quả
      alt Chưa có phân công cha
        PCTS --> PCTC : Throw Exception
        PCTC --> TL : Báo lỗi "chưa phân công công việc cha"
      else Đủ điều kiện
        PCTS -> DB : Kiểm tra trùng PHAN_CONG_CT_CONG_VIEC
        DB --> PCTS : Kết quả
        alt Đã tồn tại
          PCTS --> PCTC : Throw Exception
          PCTC --> TL : Báo lỗi trùng dữ liệu
        else Chưa tồn tại
          PCTS -> DB : INSERT PHAN_CONG_CT_CONG_VIEC
          PCTS -> DB : INSERT NHAT_KY_PHAN_CONG_CT_CONG_VIEC
          PCTS -> DB : SaveChanges
          PCTS --> PCTC : Thành công
          PCTC --> TL : Thông báo phân công chi tiết thành công
        end
      end
    end
  end
end
@enduml
```

## 4.10 Tạo công việc
- Mô tả: Công việc được tạo theo workflow duyệt đề xuất công việc; `DyetDeXuatCongViecService.ApproveAsync` sinh `CONG_VIEC` và nghiệp vụ chi phí.
- Actor: Người duyệt đề xuất công việc
```plantuml
@startuml
actor "Người duyệt" as DD
participant "DuyetDeXuatCongViecController" as DXC
participant "PermissionHelper" as PH
participant "DyetDeXuatCongViecService" as DXS
participant "TrangThaiWorkflowService" as WFS
database "Database" as DB

ref over DD,DXC
Đề xuất công việc đã ở trạng thái ChoDuyet
end ref

DD -> DXC : POST /DuyetDeXuatCongViec/Duyet(maDeXuatCv)
DXC -> PH : HasPermissionAsync(User, DuyetDeXuatCongViec.Duyet)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DXC --> DD : 403/AccessDenied
else Có quyền
  DXC -> DXS : ApproveAsync(maDeXuatCv)
  DXS -> DB : Load DE_XUAT_CONG_VIEC + DU_AN + danh mục
  DB --> DXS : Bản ghi đề xuất
  alt Không tồn tại/không ở ChoDuyet
    DXS --> DXC : Throw Exception
    DXC --> DD : Báo lỗi không thể tạo công việc
  else Hợp lệ
    DXS -> DB : BeginTransaction
    DXS -> DB : INSERT CONG_VIEC(TrangThai=ChuaBatDau)
    par Nghiệp vụ chi phí
      DXS -> DB : INSERT CHI_PHI/NHAT_KY_CHI_PHI nếu có ChiPhiDeXuat
    and Cập nhật đề xuất
      DXS -> DB : UPDATE DE_XUAT_CONG_VIEC(TrangThai=DaDuyet,NgayDuyet,MaNguoiDungDuyet)
    end
    DXS -> WFS : DongBoTrangThaiDuAnTheoCongViecAsync(maDuAn,...)
    WFS -> DB : Đồng bộ DU_AN nếu cần rollback/forward
    alt Lỗi trong transaction
      DXS -> DB : Rollback
      DXS --> DXC : Throw Exception
      DXC --> DD : Báo lỗi tạo công việc
    else Thành công
      DXS -> DB : Commit
      DXS --> DXC : Thành công
      DXC --> DD : Thông báo tạo công việc thành công
    end
  end
end
@enduml
```

## 4.11 Tạo chi tiết công việc
- Mô tả: `ChiTietCongViecService.AddAsync` thêm `CT_CONG_VIEC`, validate dữ liệu ngày/trạng thái và đồng bộ chuỗi trạng thái cha-con.
- Actor: Trưởng nhóm/Leader dự án
```plantuml
@startuml
actor "Trưởng nhóm/Leader" as TL
participant "ChiTietCongViecController" as CTC
participant "PermissionHelper" as PH
participant "ChiTietCongViecService" as CTS
participant "TrangThaiWorkflowService" as WFS
database "Database" as DB

TL -> CTC : POST /ChiTietCongViec/Them(form)
CTC -> PH : HasPermissionAsync(User, ChiTietCongViec.Them)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  CTC --> TL : 403/AccessDenied
else Có quyền
  CTC -> CTS : AddAsync(form)
  CTS -> DB : Load CONG_VIEC + DU_AN theo MaCongViec
  DB --> CTS : Context
  alt Không tìm thấy công việc
    CTS --> CTC : Throw Exception
    CTC --> TL : Báo lỗi dữ liệu
  else Tồn tại
    CTS -> CTS : Validate TenCTCV/NoiDung/NgayBatDau/TrangThaiCTCV
    CTS -> CTS : KiemTraQuyenCapNhatAsync + KiemTraTrangThaiCongViecTruocKhiThemAsync
    alt Không hợp lệ hoặc không đủ quyền
      CTS --> CTC : Throw Exception
      CTC --> TL : Hiển thị lỗi validate/quyền/workflow
    else Hợp lệ
      CTS -> DB : INSERT CT_CONG_VIEC
      CTS -> DB : SaveChanges
      CTS -> WFS : DongBoChuoiTrangThaiTuCongViecAsync(maCongViec,...)
      par Đồng bộ công việc
        WFS -> DB : UPDATE CONG_VIEC theo tổng hợp CT_CONG_VIEC
      and Đồng bộ dự án
        WFS -> DB : UPDATE DU_AN theo tổng hợp CONG_VIEC
      end
      CTS -> DB : SaveChanges
      CTS --> CTC : Thành công
      CTC --> TL : Thông báo thêm chi tiết thành công
    end
  end
end
@enduml
```

## 4.12 Cập nhật tiến độ công việc
- Mô tả: `TienDoCongViecService.CapNhatTienDoAsync` tạo bản ghi `TIEN_DO_CONG_VIEC` trạng thái `ChoDuyet`, kiểm tra chống lùi trạng thái và bắt buộc minh chứng cho trạng thái hoàn thành.
- Actor: Nhân viên thực hiện
```plantuml
@startuml
actor "Nhân viên" as NV
participant "TienDoCongViecController" as TDC
participant "PermissionHelper" as PH
participant "TienDoCongViecService" as TDS
database "Database" as DB

ref over NV,TDC
Đăng nhập + đã được phân công CT_CONG_VIEC
end ref

NV -> TDC : POST /TienDoCongViec/CapNhatTienDo(form, files)
TDC -> PH : HasPermissionAsync(User, TienDo.CapNhat)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  TDC --> NV : 403/AccessDenied
else Có quyền
  TDC -> TDS : CapNhatTienDoAsync(form)
  TDS -> DB : Load CT_CONG_VIEC + CONG_VIEC + DU_AN
  DB --> TDS : Context
  TDS -> TDS : CoTheTacNghiepTienDoChiTietAsync(scope + trạng thái)
  alt Bị khóa do workflow/scope
    TDS --> TDC : Throw Exception
    TDC --> NV : Báo lỗi không được cập nhật
  else Được cập nhật
    TDS -> TDS : Validate trạng thái đề xuất không lùi
    TDS -> TDS : Validate nội dung báo cáo + file minh chứng
    alt Không hợp lệ
      TDS --> TDC : Throw Exception
      TDC --> NV : Hiển thị lỗi validate cụ thể
    else Hợp lệ
      TDS -> DB : BeginTransaction
      TDS -> DB : INSERT TIEN_DO_CONG_VIEC(TrangThaiTienDo=ChoDuyet)
      alt Có file minh chứng
        TDS -> DB : INSERT FILE_TIEN_DO_CONG_VIEC
      else Không có file nhưng trạng thái yêu cầu minh chứng
        TDS -> DB : Rollback
        TDS --> TDC : Throw Exception
        TDC --> NV : Báo lỗi bắt buộc minh chứng
      end
      TDS -> DB : Commit
      TDS --> TDC : Thành công
      TDC --> NV : Thông báo gửi báo cáo chờ duyệt
    end
  end
end
@enduml
```

## 4.13 Duyệt báo cáo tiến độ
- Mô tả: `TienDoCongViecService.DuyetBaoCaoTienDoAsync` / `YeuCauBoSungBaoCaoTienDoAsync` / `TuChoiBaoCaoTienDoAsync`; khi duyệt sẽ cập nhật `CT_CONG_VIEC` và đồng bộ `CT_CONG_VIEC -> CONG_VIEC -> DU_AN`.
- Actor: Manager/Leader duyệt tiến độ
```plantuml
@startuml
actor "Người duyệt tiến độ" as DD
participant "TienDoCongViecController" as TDC
participant "PermissionHelper" as PH
participant "TienDoCongViecService" as TDS
participant "TrangThaiWorkflowService" as WFS
database "Database" as DB

DD -> TDC : POST DuyetBaoCaoTienDo/YeuCauBoSung/TuChoi(form)
TDC -> PH : HasPermissionAsync(User, TienDo.Duyet)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  TDC --> DD : 403/AccessDenied
else Có quyền
  TDC -> TDS : XuLyDuyetBaoCaoTienDoAsync(form,trangThaiDich)
  TDS -> DB : Load TIEN_DO_CONG_VIEC + context dự án
  DB --> TDS : Bản ghi báo cáo
  alt Không tồn tại/không ở ChoDuyet
    TDS --> TDC : Throw Exception
    TDC --> DD : Báo lỗi trạng thái duyệt
  else Hợp lệ
    TDS -> TDS : CoTheDuyetBaoCaoTheoScopeAsync(maDuAn,currentUser)
    alt Không đủ scope duyệt
      TDS --> TDC : Throw Exception
      TDC --> DD : Báo lỗi quyền duyệt
    else Đủ quyền
      TDS -> DB : BeginTransaction
      alt Duyệt (DaDuyet)
        TDS -> DB : UPDATE TIEN_DO_CONG_VIEC(TrangThai=DaDuyet,NguoiDuyet,ThoiGianDuyet)
        TDS -> DB : UPDATE CT_CONG_VIEC theo TrangThaiCTCVDeXuat
        TDS -> DB : SaveChanges
        TDS -> WFS : DongBoChuoiTrangThaiTuCongViecAsync(maCongViec,...)
        par Đồng bộ CÔNG_VIỆC
          WFS -> DB : UPDATE CONG_VIEC
        and Đồng bộ DỰ_ÁN
          WFS -> DB : UPDATE DU_AN
        end
      else Yêu cầu bổ sung
        TDS -> DB : UPDATE TIEN_DO_CONG_VIEC(TrangThai=YeuCauBoSung)
      else Từ chối
        TDS -> DB : UPDATE TIEN_DO_CONG_VIEC(TrangThai=TuChoi)
      end
      alt Lỗi transaction
        TDS -> DB : Rollback
        TDS --> TDC : Throw Exception
        TDC --> DD : Báo lỗi xử lý duyệt
      else Thành công
        TDS -> DB : Commit
        TDS --> TDC : Thành công
        TDC --> DD : Thông báo xử lý báo cáo thành công
      end
    end
  end
end
@enduml
```

## 4.14 Đề xuất công việc
- Mô tả: `DeXuatCongViecService.CreateAsync` tạo đề xuất công việc ở trạng thái `ChoDuyet`, kiểm tra dự án, danh mục, ngân sách và workflow.
- Actor: Nhân viên/Leader dự án
```plantuml
@startuml
actor "Người đề xuất" as DX
participant "DeXuatCongViecController" as DXC
participant "PermissionHelper" as PH
participant "DeXuatCongViecService" as DXS
database "Database" as DB

DX -> DXC : POST /DeXuatCongViec/TaoDeXuat(vm)
DXC -> PH : HasPermissionAsync(User, DeXuatCongViec.Them)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DXC --> DX : 403/AccessDenied
else Có quyền
  DXC -> DXS : CreateAsync(model)
  DXS -> DXS : Validate bắt buộc, ngày bắt đầu/kết thúc, chi phí
  DXS -> DB : Load DU_AN + DANH_MUC_CONG_VIEC + NGAN_SACH đang dùng
  DB --> DXS : Context
  alt Dự án không hợp lệ hoặc ngoài scope
    DXS --> DXC : Throw Exception
    DXC --> DX : Báo lỗi quyền/scope
  else Hợp lệ
    DXS -> DXS : Kiểm tra trạng thái dự án có cho phép đề xuất
    alt Dự án đã đóng/khóa workflow
      DXS --> DXC : Throw Exception
      DXC --> DX : Báo lỗi trạng thái dự án
    else Hợp lệ
      DXS -> DB : INSERT DE_XUAT_CONG_VIEC(TrangThai=ChoDuyet)
      DXS -> DB : SaveChanges
      DXS --> DXC : Thành công
      DXC --> DX : Thông báo gửi đề xuất công việc thành công
    end
  end
end
@enduml
```

## 4.15 Duyệt đề xuất công việc
- Mô tả: `DyetDeXuatCongViecService.ApproveAsync/RejectAsync`; nhánh duyệt tạo `CONG_VIEC` + ghi `CHI_PHI`, nhánh từ chối cập nhật trạng thái và lý do.
- Actor: Manager/Người có quyền duyệt đề xuất công việc
```plantuml
@startuml
actor "Người duyệt" as DD
participant "DuyetDeXuatCongViecController" as DYC
participant "PermissionHelper" as PH
participant "DyetDeXuatCongViecService" as DXS
participant "TrangThaiWorkflowService" as WFS
database "Database" as DB

DD -> DYC : POST Duyet(maDeXuatCv) / TuChoi(maDeXuatCv,lyDo)
DYC -> PH : HasPermissionAsync(User, DuyetDeXuatCongViec.Duyet)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DYC --> DD : 403/AccessDenied
else Có quyền
  alt Từ chối
    DYC -> DXS : RejectAsync(maDeXuatCv,lyDo)
    DXS -> DB : Load DE_XUAT_CONG_VIEC
    DB --> DXS : Bản ghi đề xuất
    alt Không ở ChoDuyet
      DXS --> DYC : Throw Exception
      DYC --> DD : Báo lỗi trạng thái
    else Hợp lệ
      DXS -> DB : UPDATE DE_XUAT_CONG_VIEC(TrangThai=TuChoi, LyDo, NguoiDuyet)
      DXS -> DB : SaveChanges
      DXS --> DYC : Thành công
      DYC --> DD : Thông báo từ chối thành công
    end
  else Duyệt
    DYC -> DXS : ApproveAsync(maDeXuatCv)
    DXS -> DB : Load DE_XUAT_CONG_VIEC + DU_AN + ngân sách
    DB --> DXS : Context
    alt Không hợp lệ (không pending/không đủ ngân sách)
      DXS --> DYC : Throw Exception
      DYC --> DD : Báo lỗi duyệt
    else Hợp lệ
      DXS -> DB : BeginTransaction
      DXS -> DB : UPDATE DE_XUAT_CONG_VIEC(TrangThai=DaDuyet)
      DXS -> DB : INSERT CONG_VIEC
      DXS -> DB : INSERT CHI_PHI + NHAT_KY_CHI_PHI (nếu phát sinh)
      DXS -> WFS : DongBoTrangThaiDuAnTheoCongViecAsync(maDuAn,...)
      WFS -> DB : Update DU_AN theo tổng hợp CONG_VIEC
      alt Lỗi lưu
        DXS -> DB : Rollback
        DXS --> DYC : Throw Exception
        DYC --> DD : Báo lỗi và không phát sinh dữ liệu nửa chừng
      else Thành công
        DXS -> DB : Commit
        DXS --> DYC : Thành công
        DYC --> DD : Thông báo duyệt đề xuất thành công
      end
    end
  end
end
@enduml
```

## 4.16 Đề xuất ngân sách
- Mô tả: `DeXuatNganSachService.CreateAsync` tạo `DE_XUAT_NGAN_SACH` với trạng thái `ChoDuyet`, kiểm tra ngân sách cũ và quyền theo scope dự án.
- Actor: Quản lý/Leader dự án
```plantuml
@startuml
actor "Người đề xuất ngân sách" as DX
participant "DeXuatNganSachController" as NSC
participant "PermissionHelper" as PH
participant "DeXuatNganSachService" as NSS
database "Database" as DB

DX -> NSC : POST /DeXuatNganSach/TaoDeXuat(vm)
NSC -> PH : HasPermissionAsync(User, DeXuatNganSach.Them)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  NSC --> DX : 403/AccessDenied
else Có quyền
  NSC -> NSS : CreateAsync(model)
  NSS -> NSS : Validate NganSachDeXuat, LyDo, MaDuAn
  NSS -> DB : Load DU_AN + NGAN_SACH active
  DB --> NSS : Context
  alt Dự án không hợp lệ/ngoài scope
    NSS --> NSC : Throw Exception
    NSC --> DX : Báo lỗi quyền dữ liệu
  else Hợp lệ
    NSS -> DB : Kiểm tra DE_XUAT_NGAN_SACH đang ChoDuyet
    DB --> NSS : Kết quả
    alt Đã có đề xuất chờ duyệt
      NSS --> NSC : Throw Exception
      NSC --> DX : Báo lỗi pending request
    else Chưa có pending
      NSS -> DB : INSERT DE_XUAT_NGAN_SACH(TrangThai=ChoDuyet)
      NSS -> DB : SaveChanges
      NSS --> NSC : Thành công
      NSC --> DX : Thông báo gửi đề xuất ngân sách thành công
    end
  end
end
@enduml
```

## 4.17 Duyệt đề xuất ngân sách
- Mô tả: `DuyetDeXuatNganSachService.ApproveAsync/RejectAsync`; nhánh duyệt thay ngân sách active cũ bằng ngân sách mới và ghi nhật ký.
- Actor: Người duyệt ngân sách
```plantuml
@startuml
actor "Người duyệt ngân sách" as DD
participant "DuyetDeXuatNganSachController" as DNC
participant "PermissionHelper" as PH
participant "DuyetDeXuatNganSachService" as DNS
database "Database" as DB

DD -> DNC : POST Duyet(maDeXuatNs) / TuChoi(maDeXuatNs,lyDo)
DNC -> PH : HasPermissionAsync(User, DuyetNganSach.Duyet)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DNC --> DD : 403/AccessDenied
else Có quyền
  alt Từ chối
    DNC -> DNS : RejectAsync(maDeXuatNs,lyDo)
    DNS -> DB : Load DE_XUAT_NGAN_SACH
    DB --> DNS : Bản ghi
    alt Không ở ChoDuyet
      DNS --> DNC : Throw Exception
      DNC --> DD : Báo lỗi trạng thái
    else Hợp lệ
      DNS -> DB : UPDATE DE_XUAT_NGAN_SACH(TrangThai=TuChoi,LyDo)
      DNS -> DB : SaveChanges
      DNS --> DNC : Thành công
      DNC --> DD : Thông báo từ chối thành công
    end
  else Duyệt
    DNC -> DNS : ApproveAsync(maDeXuatNs)
    DNS -> DB : Load đề xuất + ngân sách active hiện tại
    DB --> DNS : Context
    alt Không hợp lệ (không pending/sai scope)
      DNS --> DNC : Throw Exception
      DNC --> DD : Báo lỗi duyệt
    else Hợp lệ
      DNS -> DB : BeginTransaction
      DNS -> DB : UPDATE NGAN_SACH cũ -> DaThayThe/NgungHoatDong
      DNS -> DB : INSERT NGAN_SACH mới (DangSuDung/HoatDong)
      DNS -> DB : UPDATE DE_XUAT_NGAN_SACH(TrangThai=DaDuyet)
      DNS -> DB : INSERT NHAT_KY_NGAN_SACH
      alt Lỗi transaction
        DNS -> DB : Rollback
        DNS --> DNC : Throw Exception
        DNC --> DD : Báo lỗi duyệt
      else Thành công
        DNS -> DB : Commit
        DNS --> DNC : Thành công
        DNC --> DD : Thông báo duyệt ngân sách thành công
      end
    end
  end
end
@enduml
```

## 4.18 Đánh giá nhân viên
- Mô tả: `DanhGiaNhanVienService.LuuDanhGiaAsync` và `GuiDuyetAsync`; workflow `Nhap -> ChoDuyet` trước khi duyệt chính thức.
- Actor: Quản lý/Leader đánh giá
```plantuml
@startuml
actor "Người đánh giá" as DG
participant "DanhGiaNhanVienController" as NVC
participant "PermissionHelper" as PH
participant "DanhGiaNhanVienService" as NVS
database "Database" as DB

DG -> NVC : POST /DanhGiaNhanVien/Luu(form)
NVC -> PH : HasPermissionAsync(User, DanhGiaNhanVien.DanhGia, DanhGiaNhanVien.Sua)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  NVC --> DG : 403/AccessDenied
else Có quyền
  NVC -> NVS : LuuDanhGiaAsync(form)
  NVS -> DB : Load DU_AN + NHAN_VIEN_DU_AN + tiêu chí
  DB --> NVS : Context
  NVS -> NVS : Validate điểm từng tiêu chí + tổng điểm + scope
  alt Không hợp lệ
    NVS --> NVC : Throw Exception
    NVC --> DG : Báo lỗi validate/scope
  else Hợp lệ
    alt Chưa có bản ghi
      NVS -> DB : INSERT DANH_GIA_NHAN_VIEN(TrangThai=Nhap)
      NVS -> DB : INSERT CT_DANH_GIA_NHAN_VIEN
    else Đã có bản ghi nháp
      NVS -> DB : UPDATE DANH_GIA_NHAN_VIEN + CT_DANH_GIA_NHAN_VIEN
    end
    NVS -> DB : SaveChanges
    NVS --> NVC : Thành công
    NVC --> DG : Thông báo lưu nháp thành công
  end
end

DG -> NVC : POST /DanhGiaNhanVien/GuiDuyet(maDanhGiaNhanVien)
NVC -> NVS : GuiDuyetAsync(maDanhGiaNhanVien)
NVS -> DB : UPDATE TrangThaiDanhGia = ChoDuyet
NVS -> DB : SaveChanges
NVS --> NVC : Thành công
NVC --> DG : Thông báo gửi duyệt thành công
@enduml
```

## 4.19 Duyệt đánh giá nhân viên
- Mô tả: `DanhGiaNhanVienService.DuyetAsync/TuChoiAsync`, kiểm tra quyền `DanhGiaNhanVien.Duyet`, trạng thái phải là `ChoDuyet`.
- Actor: Người duyệt đánh giá nhân viên
```plantuml
@startuml
actor "Người duyệt" as DD
participant "DanhGiaNhanVienController" as NVC
participant "PermissionHelper" as PH
participant "DanhGiaNhanVienService" as NVS
database "Database" as DB

DD -> NVC : POST /DanhGiaNhanVien/Duyet hoặc TuChoi
NVC -> PH : HasPermissionAsync(User, DanhGiaNhanVien.Duyet)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  NVC --> DD : 403/AccessDenied
else Có quyền
  alt Duyệt
    NVC -> NVS : DuyetAsync(maDanhGiaNhanVien)
    NVS -> DB : Load DANH_GIA_NHAN_VIEN
    DB --> NVS : Bản ghi
    alt Không ở ChoDuyet
      NVS --> NVC : Throw Exception
      NVC --> DD : Báo lỗi trạng thái
    else Hợp lệ
      NVS -> DB : UPDATE TrangThaiDanhGia=DaDuyet, MaNguoiDungDuyet, NgayDuyet
      NVS -> DB : SaveChanges
      NVS --> NVC : Thành công
      NVC --> DD : Thông báo duyệt thành công
    end
  else Từ chối
    NVC -> NVS : TuChoiAsync(maDanhGiaNhanVien,lyDo)
    NVS -> DB : UPDATE TrangThaiDanhGia=TuChoi, LyDoTuChoi
    NVS -> DB : SaveChanges
    NVS --> NVC : Thành công
    NVC --> DD : Thông báo từ chối thành công
  end
end
@enduml
```

## 4.20 Đánh giá dự án
- Mô tả: `DanhGiaDuAnService.LuuDanhGiaAsync` và `GuiDuyetAsync`; tích hợp dữ liệu AI tham khảo nhưng MVC vẫn là nơi quyết định lưu và workflow đánh giá.
- Actor: Manager dự án
```plantuml
@startuml
actor "Manager dự án" as MG
participant "DanhGiaDuAnController" as DAC
participant "PermissionHelper" as PH
participant "DanhGiaDuAnService" as DAS
participant "AiService" as AIS
database "Database" as DB

MG -> DAC : POST /DanhGiaDuAn/Luu(form)
DAC -> PH : HasPermissionAsync(User, DanhGiaDuAn.DanhGia, DanhGiaDuAn.Sua)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DAC --> MG : 403/AccessDenied
else Có quyền
  DAC -> DAS : LuuDanhGiaAsync(form)
  DAS -> DB : Load DU_AN + tiêu chí đánh giá
  DB --> DAS : Context
  DAS -> DAS : Validate trạng thái dự án cho phép đánh giá
  alt Không hợp lệ
    DAS --> DAC : Throw Exception
    DAC --> MG : Báo lỗi workflow/validate
  else Hợp lệ
    par Nạp AI tham khảo gần nhất
      DAS -> AIS : NapDuLieuAiThamKhaoAsync(maDuAn)
      AIS -> DB : Read AI_KET_QUA mới nhất + AI_NGUYEN_NHAN
      DB --> AIS : Dữ liệu tham khảo
      AIS --> DAS : Gợi ý AI (read-only)
    and Lưu nghiệp vụ đánh giá
      DAS -> DB : INSERT/UPDATE DANH_GIA_DU_AN + CT_DANH_GIA_DU_AN(TrangThai=Nhap)
    end
    DAS -> DB : SaveChanges
    DAS --> DAC : Thành công
    DAC --> MG : Thông báo lưu đánh giá dự án thành công
  end
end

MG -> DAC : POST /DanhGiaDuAn/GuiDuyet(maDanhGiaDuAn)
DAC -> DAS : GuiDuyetAsync(maDanhGiaDuAn)
DAS -> DB : UPDATE TrangThaiDanhGia=ChoDuyet
DAS -> DB : SaveChanges
DAS --> DAC : Thành công
DAC --> MG : Thông báo gửi duyệt thành công
@enduml
```

## 4.21 Duyệt đánh giá dự án
- Mô tả: `DanhGiaDuAnService.DuyetAsync/TuChoiAsync`; áp dụng workflow `ChoDuyet -> DaDuyet/TuChoi`.
- Actor: Người duyệt đánh giá dự án
```plantuml
@startuml
actor "Người duyệt" as DD
participant "DanhGiaDuAnController" as DAC
participant "PermissionHelper" as PH
participant "DanhGiaDuAnService" as DAS
database "Database" as DB

DD -> DAC : POST /DanhGiaDuAn/Duyet hoặc TuChoi
DAC -> PH : HasPermissionAsync(User, DanhGiaDuAn.Duyet)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DAC --> DD : 403/AccessDenied
else Có quyền
  alt Duyệt
    DAC -> DAS : DuyetAsync(maDanhGiaDuAn)
    DAS -> DB : Load DANH_GIA_DU_AN
    DB --> DAS : Bản ghi
    alt Không ở ChoDuyet
      DAS --> DAC : Throw Exception
      DAC --> DD : Báo lỗi trạng thái duyệt
    else Hợp lệ
      DAS -> DB : UPDATE TrangThaiDanhGia=DaDuyet + metadata duyệt
      DAS -> DB : SaveChanges
      DAS --> DAC : Thành công
      DAC --> DD : Thông báo duyệt đánh giá dự án thành công
    end
  else Từ chối
    DAC -> DAS : TuChoiAsync(maDanhGiaDuAn,lyDo)
    DAS -> DB : UPDATE TrangThaiDanhGia=TuChoi + LyDoTuChoi
    DAS -> DB : SaveChanges
    DAS --> DAC : Thành công
    DAC --> DD : Thông báo từ chối đánh giá dự án
  end
end
@enduml
```

## 4.22 Chat dự án
- Mô tả: `ChatDuAnService.GuiTinNhanAsync` chỉ cho thành viên dự án gửi tin; tự đồng bộ phòng chat và thành viên trước khi ghi `TIN_NHAN`.
- Actor: Thành viên dự án
```plantuml
@startuml
actor "Thành viên dự án" as TV
participant "ChatDuAnController" as CDC
participant "PermissionHelper" as PH
participant "ChatDuAnService" as CDS
database "Database" as DB

ref over TV,CDC
Đăng nhập + mở màn hình chat dự án
end ref

TV -> CDC : POST /ChatDuAn/GuiTinNhan(form)
CDC -> PH : HasPermissionAsync(User, Chat.Gui)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  CDC --> TV : 403/AccessDenied
else Có quyền
  CDC -> CDS : GuiTinNhanAsync(maDuAn,noiDung)
  CDS -> DB : Load DU_AN + kiểm tra user thuộc NHAN_VIEN_DU_AN
  DB --> CDS : Context
  alt Không thuộc dự án
    CDS --> CDC : Throw Exception
    CDC --> TV : Báo lỗi không có quyền chat dự án này
  else Thuộc phạm vi
    CDS -> CDS : DamBaoPhongChatDuAnAsync(maDuAn)
    CDS -> DB : Upsert PHONG_CHAT + THANH_VIEN_PHONG_CHAT
    alt Dự án ở trạng thái khóa chat (đã đóng)
      CDS --> CDC : Throw Exception
      CDC --> TV : Báo lỗi không thể gửi tin
    else Được gửi
      CDS -> DB : INSERT TIN_NHAN
      CDS -> DB : SaveChanges
      CDS --> CDC : Thành công
      CDC --> TV : Trả lại màn hình chat + tin nhắn mới
    end
  end
end
@enduml
```

## 4.23 Tổng hợp AI_DATASET
- Mô tả: `AiDatasetService.TongHopDatasetAsync/TongHopDatasetChoDuAnAsync` tổng hợp feature từ dữ liệu thực (DU_AN, CONG_VIEC, TIEN_DO, NGAN_SACH...) vào `AI_DATASET`.
- Actor: Người vận hành AI
```plantuml
@startuml
actor "Người vận hành AI" as AIU
participant "AiDatasetController" as ADC
participant "PermissionHelper" as PH
participant "AiDatasetService" as ADS
database "Database" as DB

AIU -> ADC : POST /AiDataset/TongHopDataset hoặc TongHopDatasetChoDuAn
ADC -> PH : HasPermissionAsync(User, AI.Dataset)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  ADC --> AIU : 403/AccessDenied
else Có quyền
  ADC -> ADS : TongHopDataset...Async()
  ADS -> DB : Load DU_AN active cần tổng hợp
  ADS -> DB : Load CONG_VIEC, CT_CONG_VIEC, TIEN_DO_CONG_VIEC
  ADS -> DB : Load NGAN_SACH/CHI_PHI, NHAN_VIEN_DU_AN, NHAT_KY_QUAN_LY
  DB --> ADS : Dữ liệu nguồn
  ADS -> ADS : Tính 10 feature + rule gán IsTre (không tự gán bừa)
  alt Không có dự án hợp lệ
    ADS --> ADC : Result thất bại + lý do
    ADC --> AIU : Hiển thị cảnh báo thiếu dữ liệu
  else Có dữ liệu
    ADS -> DB : UPSERT AI_DATASET theo MaDuAn
    par Đếm thống kê
      ADS -> ADS : SoDuAnTaoMoi / SoDuAnCapNhat / SoDuAnLoi
    and Ghi log ứng dụng
      ADS -> ADS : logger.LogInformation
    end
    ADS -> DB : SaveChanges
    ADS --> ADC : Result thành công + thống kê
    ADC --> AIU : Thông báo tổng hợp dataset thành công
  end
end
@enduml
```

## 4.24 Train model AI
- Mô tả: `AiService.TrainAsync` tách rõ `TreHan` và `NguyenNhan`, kiểm tra chất lượng dataset (`MIN_TRAIN_ROWS`), gọi FastAPI train compute-only, sau đó MVC lưu metadata model.
- Actor: Người vận hành AI
```plantuml
@startuml
actor "Người vận hành AI" as AIU
participant "AiController" as AIC
participant "PermissionHelper" as PH
participant "AiService" as AIS
participant "AiDatasetService" as ADS
participant "AiApiService" as API
participant "FastAPI AI" as FAPI
database "Database" as DB

AIU -> AIC : POST /Ai/Train(modelType,activateAfterTrain)
AIC -> PH : HasPermissionAsync(User, AI.Train)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  AIC --> AIU : 403/AccessDenied
else Có quyền
  AIC -> AIS : TrainAsync(modelType,activateAfterTrain,...)
  alt modelType = TreHan
    AIS -> ADS : KiemTraChatLuongDatasetAsync()
    ADS -> DB : Read AI_DATASET
    DB --> ADS : Dataset
  else modelType = NguyenNhan
    AIS -> ADS : KiemTraChatLuongDatasetNguyenNhanAsync()
    ADS -> DB : Read AI_DATASET + AI_NGUYEN_NHAN xác nhận
    DB --> ADS : Dataset train nguyên nhân
  end
  ADS --> AIS : Summary chất lượng
  alt Không đủ điều kiện train (ít dòng, thiếu lớp, thiếu nhãn)
    AIS --> AIC : Result thất bại + lý do chi tiết
    AIC --> AIU : Hiển thị lỗi quality gate
  else Đủ điều kiện
    AIS -> ADS : LayDataset...HopLeDeTrainAsync()
    ADS --> AIS : Rows hợp lệ
    AIS -> API : TrainModelAsync(payload)
    API -> FAPI : POST /model/train
    FAPI --> API : train result(model_file,metrics)
    alt FastAPI lỗi/timeout/422
      API --> AIS : ThanhCong=false + detail.loc/msg/type
      AIS --> AIC : Result thất bại
      AIC --> AIU : Hiển thị lỗi train cụ thể
    else Train thành công
      API --> AIS : ThanhCong=true
      AIS -> DB : INSERT/UPDATE AI_MODEL(metadata, modelType)
      alt activateAfterTrain=true
        AIS -> API : DatModelHoatDongAsync(modelFile,modelType)
        API -> FAPI : POST /admin/model/activate
        FAPI --> API : kết quả activate
        API --> AIS : trạng thái active
      end
      AIS -> DB : SaveChanges
      AIS --> AIC : Result thành công
      AIC --> AIU : Thông báo train thành công
    end
  end
end
@enduml
```

## 4.25 Kích hoạt model AI
- Mô tả: `AiService.DatModelHoatDongAsync` kích hoạt model theo từng loại (`TreHan` hoặc `NguyenNhan`), đồng bộ trạng thái active ở FastAPI và DB MVC.
- Actor: Người vận hành AI
```plantuml
@startuml
actor "Người vận hành AI" as AIU
participant "AiController" as AIC
participant "PermissionHelper" as PH
participant "AiService" as AIS
participant "AiApiService" as API
participant "FastAPI AI" as FAPI
database "Database" as DB

AIU -> AIC : POST /Ai/SetActiveModel(modelFile,modelType)
AIC -> PH : HasPermissionAsync(User, AI.Train)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  AIC --> AIU : 403/AccessDenied
else Có quyền
  AIC -> AIS : DatModelHoatDongAsync(modelFile,modelType)
  AIS -> DB : Kiểm tra AI_MODEL tồn tại theo modelType
  DB --> AIS : Metadata model
  alt Không tồn tại/không đúng loại
    AIS --> AIC : Result thất bại
    AIC --> AIU : Báo lỗi model không hợp lệ
  else Hợp lệ
    AIS -> API : DatModelHoatDongAsync(modelFile,modelType)
    API -> FAPI : POST /admin/model/activate
    FAPI --> API : Kết quả kích hoạt
    alt FastAPI trả lỗi
      API --> AIS : ThanhCong=false
      AIS --> AIC : Thất bại
      AIC --> AIU : Hiển thị lỗi kích hoạt
    else Thành công
      AIS -> DB : UPDATE AI_MODEL cùng loại (model mới=active, model cũ=inactive)
      AIS -> DB : SaveChanges
      AIS --> AIC : Thành công
      AIC --> AIU : Thông báo kích hoạt model thành công
    end
  end
end
@enduml
```

## 4.26 Dự đoán dự án trễ hạn
- Mô tả: `AiService.DuDoanDuAnAsync` bổ sung feature từ `AI_DATASET` nếu thiếu, gọi FastAPI dự đoán `TreHan` + `NguyenNhan`, rồi lưu kết quả vào `AI_KET_QUA` (MVC ghi DB, FastAPI compute-only).
- Actor: Manager/Người dùng AI có quyền dự đoán
```plantuml
@startuml
actor "Người dùng AI" as AIU
participant "AiController" as AIC
participant "PermissionHelper" as PH
participant "AiService" as AIS
participant "AiApiService" as API
participant "FastAPI AI" as FAPI
database "Database" as DB

AIU -> AIC : POST /Ai/Predict(form)
AIC -> PH : HasPermissionAsync(User, AI.DuDoan)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  AIC --> AIU : 403/AccessDenied
else Có quyền
  AIC -> AIS : DuDoanDuAnAsync(input)
  AIS -> AIS : Validate MaDuAn > 0
  alt MaDuAn không hợp lệ
    AIS --> AIC : Result thất bại
    AIC --> AIU : Báo lỗi chọn dự án
  else Hợp lệ
    AIS -> DB : Load AI_DATASET mới nhất theo MaDuAn
    DB --> AIS : Dataset
    alt Thiếu feature
      AIS -> AIS : BoSungFeatureTuDatasetNeuCanAsync
    else Đã đủ feature
      AIS -> AIS : MapPredictFeature(10 features)
    end
    AIS -> API : DuDoanDuAnAsync(payload)
    API -> FAPI : POST /predict/project
    FAPI --> API : Envelope(success/data/errors)
    alt FastAPI lỗi 422/schema
      API --> AIS : Message + Parse detail.loc/msg/type
      AIS --> AIC : Result thất bại + hướng dẫn 10 feature chuẩn
      AIC --> AIU : Hiển thị lỗi request schema
    else FastAPI timeout/5xx
      API --> AIS : Result lỗi + retry exhausted
      AIS --> AIC : Result thất bại
      AIC --> AIU : Thông báo AI tạm thời không sẵn sàng
    else Dự đoán thành công
      API --> AIS : DuLieu(duAnBiTre,doTinCay,nguyenNhan...)
      AIS -> AIS : RuleFallback nguyên nhân nếu model reason không khả dụng/độ tin cậy thấp
      AIS -> DB : INSERT AI_KET_QUA + map DM_NGUYEN_NHAN
      AIS -> DB : SaveChanges
      AIS --> AIC : Result thành công
      AIC --> AIU : Hiển thị kết quả dự đoán trễ hạn
    end
  end
end
@enduml
```

## 4.27 AI phân tích nguyên nhân trễ
- Mô tả: `DanhGiaDuAnService.PhanTichAiDuAnAsync` chỉ cho Manager theo scope; nếu dự án đúng hạn thì trả thông báo không cần phân tích, nếu trễ sẽ gọi AI dự đoán và cập nhật insight.
- Actor: Manager dự án
```plantuml
@startuml
actor "Manager dự án" as MG
participant "DanhGiaDuAnController" as DGC
participant "PermissionHelper" as PH
participant "DanhGiaDuAnService" as DGS
participant "AiDatasetService" as ADS
participant "AiService" as AIS
database "Database" as DB

MG -> DGC : POST /DanhGiaDuAn/PhanTichAiDuAn(maDuAn)
DGC -> PH : HasPermissionAsync(User, DanhGiaDuAn.DanhGia)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  DGC --> MG : 403/AccessDenied
else Có quyền
  DGC -> DGS : PhanTichAiDuAnAsync(maDuAn)
  DGS -> DB : Load DU_AN + kiểm tra manager scope
  DB --> DGS : Dự án
  alt Không thuộc scope manager
    DGS --> DGC : Throw Exception
    DGC --> MG : Báo lỗi không đủ quyền phân tích
  else Hợp lệ
    DGS -> DGS : XayDungThongKeDuAnAsync + XacDinhTrangThaiThucTeTienDo
    alt Dự án hoàn thành đúng hạn
      DGS --> DGC : Insight "không cần phân tích AI"
      DGC --> MG : Hiển thị trạng thái không trễ
    else Dự án có rủi ro trễ
      DGS -> DB : Load AI_DATASET mới nhất
      DB --> DGS : Dataset
      alt Chưa có dataset
        DGS -> ADS : TongHopDatasetChoDuAnAsync(maDuAn)
        ADS -> DB : UPSERT AI_DATASET
        ADS --> DGS : Kết quả tổng hợp
      end
      DGS -> AIS : DuDoanDuAnAsync(request từ dataset)
      AIS -> DB : Lưu AI_KET_QUA nếu dự đoán thành công
      AIS --> DGS : Predict result
      alt Predict thất bại
        DGS --> DGC : Throw Exception(thông báo chi tiết)
        DGC --> MG : Báo lỗi gọi AI
      else Predict thành công
        DGS -> DGS : BuildAiInsightViewModelAsync
        DGS --> DGC : Insight mới (TreHan + NguyenNhan + confidence)
        DGC --> MG : Hiển thị phân tích nguyên nhân trễ
      end
    end
  end
end
@enduml
```

## 4.28 Manager xác nhận nguyên nhân AI
- Mô tả: Manager xác nhận nguyên nhân cuối cùng qua `DanhGiaDuAnService.XacNhanNguyenNhanAsync`/`AiService.XacNhanNguyenNhanAsync`, ghi `AI_NGUYEN_NHAN` tại MVC.
- Actor: Manager dự án
```plantuml
@startuml
actor "Manager dự án" as MG
participant "DanhGiaDuAnController" as DGC
participant "PermissionHelper" as PH
participant "DanhGiaDuAnService" as DGS
database "Database" as DB

MG -> DGC : POST /DanhGiaDuAn/XacNhanNguyenNhan(maDuAn,maDmNguyenNhan,doTinCay)
DGC -> PH : HasPermissionAsync(User, AI.XacNhan)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền AI.XacNhan
  DGC --> MG : 403/AccessDenied
else Có quyền
  DGC -> DGS : XacNhanNguyenNhanAsync(maDuAn,maDmNguyenNhan,doTinCay)
  DGS -> DB : Load DU_AN + kiểm tra scope manager
  DB --> DGS : Context
  alt Không thuộc scope/du án không tồn tại
    DGS --> DGC : Throw Exception
    DGC --> MG : Báo lỗi quyền dữ liệu
  else Hợp lệ
    DGS -> DB : Kiểm tra DM_NGUYEN_NHAN tồn tại
    DGS -> DGS : XayDungThongKeDuAnAsync để xác nhận dự án có trễ
    alt Dự án không trễ
      DGS --> DGC : Throw Exception("không cần xác nhận nguyên nhân")
      DGC --> MG : Hiển thị cảnh báo
    else Dự án trễ
      DGS -> DB : Load AI_NGUYEN_NHAN mới nhất theo MaDuAn
      DB --> DGS : Bản ghi cũ hoặc null
      alt Chưa có bản ghi
        DGS -> DB : INSERT AI_NGUYEN_NHAN
      else Đã có bản ghi
        DGS -> DB : UPDATE AI_NGUYEN_NHAN
      end
      DGS -> DB : SaveChanges
      DGS --> DGC : Thành công
      DGC --> MG : Thông báo xác nhận nguyên nhân thành công
    end
  end
end
@enduml
```

## 4.29 Dashboard thống kê
- Mô tả: `DashboardService.GetDashboardAsync` tổng hợp số liệu trạng thái dự án/công việc/đề xuất/AI theo `TrangThai.cs` để hiển thị trang tổng quan.
- Actor: Người dùng đã đăng nhập
```plantuml
@startuml
actor "Người dùng" as ND
participant "DashboardController" as DBC
participant "DashboardService" as DBS
database "Database" as DB

ref over ND,DBC
Đăng nhập hệ thống
end ref

ND -> DBC : GET /Dashboard/Index
DBC -> DBS : GetDashboardAsync()
DBS -> DB : Query DU_AN theo trạng thái (KhoiTao,DangThucHien,ChoXacNhanHoanThanh,HoanThanh,...)
DBS -> DB : Query CONG_VIEC/CT_CONG_VIEC/TIEN_DO_CONG_VIEC
DBS -> DB : Query DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, YEU_CAU_DOI_QUAN_LY
DBS -> DB : Query AI_KET_QUA, AI_MODEL khi cần
DB --> DBS : Dữ liệu thô
DBS -> DBS : Chuẩn hóa bằng TrangThai.EqualsValue/GetCommonStatusVariants
par Tổng hợp số liệu
  DBS -> DBS : Tính KPI tổng hợp
and Tổng hợp biểu đồ
  DBS -> DBS : Tính thống kê theo nhóm/trạng thái
end
DBS --> DBC : DashboardViewModel
DBC --> ND : Render dashboard thống kê
@enduml
```

## 4.30 Upload file tiến độ
- Mô tả: `TienDoCongViecController.ThemFileTienDo` gọi `FileTienDoCongViecService.UploadAsync`; kiểm tra quyền cập nhật tiến độ, validate file, lưu vật lý và ghi `FILE_TIEN_DO_CONG_VIEC`.
- Actor: Nhân viên thực hiện
```plantuml
@startuml
actor "Nhân viên" as NV
participant "TienDoCongViecController" as TDC
participant "PermissionHelper" as PH
participant "TienDoCongViecService" as TDS
participant "FileTienDoCongViecService" as FTS
database "Database" as DB

NV -> TDC : POST /TienDoCongViec/ThemFileTienDo(maChiTietCv,file)
TDC -> PH : HasPermissionAsync(User, TienDo.CapNhat)
PH -> DB : Check claim
DB --> PH : Kết quả
alt Không có quyền
  TDC --> NV : 403/AccessDenied
else Có quyền
  TDC -> TDS : CoTheTacNghiepTienDoChiTietAsync(maChiTietCv,currentUser)
  TDS -> DB : Kiểm tra scope + trạng thái DU_AN/CONG_VIEC/CT_CONG_VIEC
  DB --> TDS : Kết quả
  alt Không được thao tác
    TDS --> TDC : false/Exception
    TDC --> NV : Báo lỗi không thể upload
  else Được thao tác
    TDC -> FTS : UploadAsync(maChiTietCv,file)
    FTS -> FTS : Validate tên file, dung lượng, định dạng
    alt File rỗng/sai định dạng/quá dung lượng
      FTS --> TDC : Throw Exception
      TDC --> NV : Hiển thị lỗi validate file
    else Hợp lệ
      FTS -> DB : Kiểm tra CT_CONG_VIEC tồn tại
      DB --> FTS : Context
      FTS -> FTS : Lưu file vật lý vào /uploads/tiendocongviec
      FTS -> DB : INSERT FILE_TIEN_DO_CONG_VIEC
      FTS -> DB : SaveChanges
      FTS --> TDC : Thành công
      TDC --> NV : Thông báo upload file tiến độ thành công
    end
  end
end
@enduml
```
