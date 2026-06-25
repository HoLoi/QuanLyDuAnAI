using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Options;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Services.Implementations
{
    public class AiApiService : IAiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AiApiOptions _options;
        private readonly ILogger<AiApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public AiApiService(
            HttpClient httpClient,
            IOptions<AiApiOptions> options,
            ILogger<AiApiService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public Task<AiOperationResultViewModel<AiHealthResponseViewModel>> KiemTraSucKhoeAsync(CancellationToken cancellationToken = default)
            => SendForDataAsync<object?, AiHealthResponseViewModel>(HttpMethod.Get, "/health", null, cancellationToken);

        public Task<AiOperationResultViewModel<AiAdminStatusViewModel>> LayTrangThaiAdminAsync(CancellationToken cancellationToken = default)
            => SendForDataAsync<object?, AiAdminStatusViewModel>(HttpMethod.Get, "/admin/ai-status", null, cancellationToken);

        public Task<AiOperationResultViewModel<AiSystemInfoViewModel>> LayThongTinHeThongAsync(CancellationToken cancellationToken = default)
            => SendForDataAsync<object?, AiSystemInfoViewModel>(HttpMethod.Get, "/admin/system-info", null, cancellationToken);

        public Task<AiOperationResultViewModel<AiLogSummaryViewModel>> LayTongHopLogAsync(CancellationToken cancellationToken = default)
            => SendForDataAsync<object?, AiLogSummaryViewModel>(HttpMethod.Get, "/admin/logs/summary", null, cancellationToken);

        public Task<AiOperationResultViewModel<AiDatasetValidateResponseViewModel>> ValidateDatasetAsync(AiDatasetValidateRequestViewModel request, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiDatasetValidateRequestViewModel, AiDatasetValidateResponseViewModel>(HttpMethod.Post, "/dataset/validate", request, cancellationToken);

        public Task<AiOperationResultViewModel<AiDatasetQualityReportViewModel>> BaoCaoChatLuongDatasetAsync(AiDatasetValidateRequestViewModel request, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiDatasetValidateRequestViewModel, AiDatasetQualityReportViewModel>(HttpMethod.Post, "/admin/dataset/quality-report", request, cancellationToken);

        public Task<AiOperationResultViewModel<AiTrainRecommendationViewModel>> KhuyenNghiTrainAsync(AiDatasetValidateRequestViewModel request, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiDatasetValidateRequestViewModel, AiTrainRecommendationViewModel>(HttpMethod.Post, "/admin/model/train-recommendation", request, cancellationToken);

        public Task<AiOperationResultViewModel<AiTrainResponseViewModel>> TrainModelAsync(AiTrainRequestViewModel request, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiTrainRequestViewModel, AiTrainResponseViewModel>(HttpMethod.Post, "/model/train", request, cancellationToken);

        public Task<AiOperationResultViewModel<List<AiModelInfoViewModel>>> LayDanhSachModelAsync(string? modelType = null, CancellationToken cancellationToken = default, bool includeAliases = false)
        {
            var url = "/model/list";
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(modelType))
            {
                queryParams.Add($"modelType={Uri.EscapeDataString(modelType.Trim())}");
            }
            queryParams.Add($"includeAliases={includeAliases.ToString().ToLowerInvariant()}");

            if (queryParams.Count > 0)
            {
                url += $"?{string.Join("&", queryParams)}";
            }

            return SendForDataAsync<object?, List<AiModelInfoViewModel>>(HttpMethod.Get, url, null, cancellationToken);
        }

        public Task<AiOperationResultViewModel<Dictionary<string, object?>>> LayChiTietModelAsync(string modelFile, CancellationToken cancellationToken = default)
            => SendForDataAsync<object?, Dictionary<string, object?>>(HttpMethod.Get, $"/admin/model/{Uri.EscapeDataString(modelFile)}", null, cancellationToken);

        public Task<AiOperationResultViewModel<AiValidateModelResponseViewModel>> ValidateModelAsync(string modelFile, string? modelType, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiValidateModelRequestViewModel, AiValidateModelResponseViewModel>(
                HttpMethod.Post,
                "/admin/model/validate",
                new AiValidateModelRequestViewModel { ModelFile = modelFile, ModelType = modelType },
                cancellationToken);

        public Task<AiOperationResultViewModel<AiCompareModelResponseViewModel>> CompareModelAsync(AiCompareModelRequestViewModel request, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiCompareModelRequestViewModel, AiCompareModelResponseViewModel>(HttpMethod.Post, "/admin/model/compare", request, cancellationToken);

        public Task<AiOperationResultViewModel<AiModelActivationResultViewModel>> DatModelHoatDongAsync(string modelFile, string modelType, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiValidateModelRequestViewModel, AiModelActivationResultViewModel>(
                HttpMethod.Post,
                "/admin/model/set-active",
                new AiValidateModelRequestViewModel { ModelFile = modelFile, ModelType = modelType },
                cancellationToken);

        public Task<AiOperationResultViewModel<AiModelReloadResultViewModel>> TaiLaiModelHoatDongAsync(string modelType, CancellationToken cancellationToken = default)
            => SendForDataAsync<object?, AiModelReloadResultViewModel>(
                HttpMethod.Post,
                $"/admin/model/reload?modelType={Uri.EscapeDataString(modelType)}",
                null,
                cancellationToken);

        public Task<AiOperationResultViewModel<AiModelDeleteResultViewModel>> XoaModelAsync(string modelFile, CancellationToken cancellationToken = default)
            => SendForDataAsync<object?, AiModelDeleteResultViewModel>(HttpMethod.Delete, $"/admin/model/{Uri.EscapeDataString(modelFile)}", null, cancellationToken);

        public Task<AiOperationResultViewModel<Dictionary<string, object?>>> ExportMetadataAsync(string modelFile, CancellationToken cancellationToken = default)
            => SendForDataAsync<object?, Dictionary<string, object?>>(HttpMethod.Get, $"/admin/model/export-metadata/{Uri.EscapeDataString(modelFile)}", null, cancellationToken);

        public Task<AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>> DuDoanDuAnAsync(AiAnalyzeDelayReasonRequestViewModel request, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiAnalyzeDelayReasonRequestViewModel, AiAnalyzeDelayReasonResponseViewModel>(HttpMethod.Post, "/analyze/delay-reason", request, cancellationToken);

        public Task<AiOperationResultViewModel<AiTestReasonResponseViewModel>> TestPredictAsync(AiTestReasonRequestViewModel request, CancellationToken cancellationToken = default)
            => SendForDataAsync<AiTestReasonRequestViewModel, AiTestReasonResponseViewModel>(HttpMethod.Post, "/admin/model/test-reason", request, cancellationToken);

        private async Task<AiOperationResultViewModel<TResponse>> SendForDataAsync<TRequest, TResponse>(
            HttpMethod method,
            string url,
            TRequest? body,
            CancellationToken cancellationToken)
        {
            var maxAttempts = Math.Max(1, _options.RetryCount + 1);
            var delayMs = Math.Max(50, _options.RetryDelayMilliseconds);

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using var request = new HttpRequestMessage(method, url);
                    string? payloadForLog = null;
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (body != null)
                    {
                        var payload = JsonSerializer.Serialize(body, _jsonOptions);
                        payloadForLog = payload;
                        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                    }

                    using var response = await _httpClient.SendAsync(request, cancellationToken);
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var envelope = ParseEnvelope(content);

                    if (response.IsSuccessStatusCode && envelope?.Success == true)
                    {
                        return new AiOperationResultViewModel<TResponse>
                        {
                            ThanhCong = true,
                            ThongBao = string.IsNullOrWhiteSpace(envelope.Message) ? "Thanh cong." : envelope.Message,
                            DuLieu = ConvertData<TResponse>(envelope.Data),
                            Loi = []
                        };
                    }

                    if (ShouldRetry(response.StatusCode, attempt, maxAttempts))
                    {
                        await Task.Delay(delayMs, cancellationToken);
                        continue;
                    }

                    var errorPayload = BuildErrorPayload(response.StatusCode, response.ReasonPhrase, content, envelope);
                    _logger.LogWarning(
                        "AI API tra loi {Method} {Url} - HTTP {StatusCode}. Payload: {Payload}. Body: {Body}. Errors: {Errors}",
                        method,
                        url,
                        (int)response.StatusCode,
                        TruncateForLog(payloadForLog),
                        TruncateForLog(content),
                        string.Join(" | ", errorPayload.Errors));

                    return new AiOperationResultViewModel<TResponse>
                    {
                        ThanhCong = false,
                        ThongBao = errorPayload.Message,
                        Loi = errorPayload.Errors
                    };
                }
                catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "Timeout khi goi AI API {Method} {Url}. Lan thu {Attempt}/{MaxAttempts}.", method, url, attempt, maxAttempts);
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                        continue;
                    }

                    return new AiOperationResultViewModel<TResponse>
                    {
                        ThanhCong = false,
                        ThongBao = "Không kết nối được AI service do quá thời gian chờ.",
                        Loi = ["AI service đang phản hồi chậm hoặc tạm thời không sẵn sàng."],
                        LaDuLieuFallback = true,
                        LaLoiTimeout = true
                    };
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "Loi mang khi goi AI API {Method} {Url}. Lan thu {Attempt}/{MaxAttempts}.", method, url, attempt, maxAttempts);
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                        continue;
                    }

                    return new AiOperationResultViewModel<TResponse>
                    {
                        ThanhCong = false,
                        ThongBao = "Không kết nối được AI service.",
                        Loi = [ex.Message],
                        LaDuLieuFallback = true
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Loi khong mong muon khi goi AI API {Method} {Url}.", method, url);
                    return new AiOperationResultViewModel<TResponse>
                    {
                        ThanhCong = false,
                        ThongBao = "Co loi xay ra khi xu ly du lieu AI.",
                        Loi = [ex.Message]
                    };
                }
            }

            return new AiOperationResultViewModel<TResponse>
            {
                ThanhCong = false,
                ThongBao = "Không thể hoàn tất yêu cầu AI.",
                Loi = ["Yêu cầu AI thất bại sau nhiều lần thử."],
                LaDuLieuFallback = true
            };
        }

        private static bool ShouldRetry(HttpStatusCode statusCode, int attempt, int maxAttempts)
        {
            if (attempt >= maxAttempts)
            {
                return false;
            }

            var code = (int)statusCode;
            return code >= 500 || statusCode == HttpStatusCode.RequestTimeout || statusCode == HttpStatusCode.TooManyRequests;
        }

        private AiApiEnvelopeViewModel<JsonElement>? ParseEnvelope(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            return JsonSerializer.Deserialize<AiApiEnvelopeViewModel<JsonElement>>(content, _jsonOptions);
        }

        private T? ConvertData<T>(JsonElement dataElement)
        {
            if (dataElement.ValueKind == JsonValueKind.Undefined || dataElement.ValueKind == JsonValueKind.Null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(dataElement.GetRawText(), _jsonOptions);
        }

        private static AiErrorPayload BuildErrorPayload(
            HttpStatusCode statusCode,
            string? reasonPhrase,
            string? content,
            AiApiEnvelopeViewModel<JsonElement>? envelope)
        {
            var errors = new List<string>();

            if (envelope?.Errors != null && envelope.Errors.Count > 0)
            {
                errors.AddRange(envelope.Errors.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
            }

            if (statusCode == HttpStatusCode.UnprocessableEntity)
            {
                var parsedValidationErrors = ParseValidationErrors(content);
                if (parsedValidationErrors.Count > 0)
                {
                    errors.AddRange(parsedValidationErrors);
                }
            }

            if (errors.Count == 0 && !string.IsNullOrWhiteSpace(content))
            {
                errors.Add(content.Trim());
            }

            if (errors.Count == 0)
            {
                errors.Add($"HTTP {(int)statusCode} - {reasonPhrase}");
            }

            if (statusCode == HttpStatusCode.UnprocessableEntity)
            {
                var featureList = "SoNhanVienDuAn, TongSoCongViec, SoCongViecTre, TyLeCongViecTre, ChiPhiDuKien, ChiPhiThucTe, ChenhLechChiPhi, SoLanThayDoiNhanSu, SoLanThayDoiQuanLy, SoNgayTreTienDo, SoDeXuatCongViecChoDuyet, SoDeXuatCongViecBiTuChoi, ThoiGianDuyetCongViecTrungBinh, SoDeXuatNganSachChoDuyet, SoDeXuatNganSachBiTuChoi, ThoiGianDuyetNganSachTrungBinh, SoBaoCaoTienDoChoDuyet, SoBaoCaoTienDoBiTuChoi, SoBaoCaoTienDoYeuCauBoSung, TyLeBaoCaoTienDoBiTuChoi, SoLanCapNhatTienDo, SoNgayChamCapNhatTienDo";
                errors.Insert(0, $"Schema hiện tại chỉ nhận đầy đủ bộ feature hợp lệ: {featureList}.");

                if (errors.Any(x => x.Contains("LaDuAnTre", StringComparison.OrdinalIgnoreCase) && x.Contains("feature", StringComparison.OrdinalIgnoreCase)))
                {
                    errors.Insert(1, "Không gửi LaDuAnTre trong payload phân tích nguyên nhân.");
                }

                return new AiErrorPayload(
                    "422 Validation Error: request không khớp schema FastAPI. Kiểm tra danh sách lỗi chi tiết bên dưới.",
                    errors);
            }

            var message = !string.IsNullOrWhiteSpace(envelope?.Message)
                ? envelope.Message
                : $"Yêu cầu AI thất bại ({(int)statusCode} {reasonPhrase}).";

            return new AiErrorPayload(message, errors);
        }

        private static List<string> ParseValidationErrors(string? content)
        {
            var parsed = new List<string>();
            if (string.IsNullOrWhiteSpace(content))
            {
                return parsed;
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (!doc.RootElement.TryGetProperty("detail", out var detailElement))
                {
                    return parsed;
                }

                if (detailElement.ValueKind != JsonValueKind.Array)
                {
                    return parsed;
                }

                foreach (var item in detailElement.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    var fieldPath = BuildFieldPath(item);
                    var errorMessage = item.TryGetProperty("msg", out var msgElement) ? msgElement.GetString() : null;
                    var errorType = item.TryGetProperty("type", out var typeElement) ? typeElement.GetString() : null;

                    var pieces = new List<string>();
                    if (!string.IsNullOrWhiteSpace(fieldPath))
                    {
                        pieces.Add($"field \"{fieldPath}\"");
                    }

                    if (!string.IsNullOrWhiteSpace(errorMessage))
                    {
                        pieces.Add(errorMessage!);
                    }

                    if (!string.IsNullOrWhiteSpace(errorType))
                    {
                        pieces.Add($"type={errorType}");
                    }

                    if (pieces.Count > 0)
                    {
                        parsed.Add(string.Join(" - ", pieces));
                    }
                }
            }
            catch
            {
                // De fallback o lop ngoai khi body khong parse duoc.
            }

            return parsed;
        }

        private static string BuildFieldPath(JsonElement item)
        {
            if (!item.TryGetProperty("loc", out var locElement) || locElement.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var parts = new List<string>();
            foreach (var part in locElement.EnumerateArray())
            {
                switch (part.ValueKind)
                {
                    case JsonValueKind.String:
                    {
                        var value = part.GetString();
                        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "body", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        parts.Add(value);
                        break;
                    }
                    case JsonValueKind.Number:
                        parts.Add(part.GetRawText());
                        break;
                }
            }

            return string.Join('.', parts);
        }

        private static string TruncateForLog(string? content, int maxLength = 4000)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            return content.Length <= maxLength
                ? content
                : content[..maxLength] + "...(truncated)";
        }

        private sealed record AiErrorPayload(string Message, List<string> Errors);
    }
}

