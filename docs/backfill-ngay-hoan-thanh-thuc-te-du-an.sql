USE [QuanLyDuAnAI];
GO

SET NOCOUNT ON;

UPDATE da
SET NgayHoanThanhThucTeDuAn = cvMax.NgayHoanThanhThucTe
FROM dbo.DU_AN AS da
CROSS APPLY
(
    SELECT MAX(cv.NgayKetThucCVThucTe) AS NgayHoanThanhThucTe
    FROM dbo.DANH_MUC_CONG_VIEC AS dm
    INNER JOIN dbo.CONG_VIEC AS cv ON cv.MaDanhMucCV = dm.MaDanhMucCV
    WHERE dm.MaDuAn = da.MaDuAn
      AND ISNULL(dm.IsDeleted, 0) = 0
      AND ISNULL(cv.IsDeleted, 0) = 0
      AND cv.NgayKetThucCVThucTe IS NOT NULL
) AS cvMax
WHERE da.NgayHoanThanhThucTeDuAn IS NULL
  AND cvMax.NgayHoanThanhThucTe IS NOT NULL
  AND ISNULL(da.IsDeleted, 0) = 0
  AND da.TrangThaiDuAn IN (N'HoanThanh', N'Hoàn thành', N'Completed');