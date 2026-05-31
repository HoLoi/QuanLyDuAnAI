namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiOperationResultViewModel<T>
    {
        public bool ThanhCong { get; set; }

        public string ThongBao { get; set; } = string.Empty;

        public T? DuLieu { get; set; }

        public List<string> Loi { get; set; } = [];

        public bool LaDuLieuFallback { get; set; }

        public bool LaLoiTimeout { get; set; }
    }
}
