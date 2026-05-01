namespace QuanLyDuAn.ViewModels.DuAn
{
    public class ProjectStatusCheckViewModel
    {
        public int MaDuAn { get; set; }
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public int PhanTramHoanThanh { get; set; }
        
        // Status transition conditions
        public bool CanTransitionToDangThucHien { get; set; }
        public bool CanRequestCompletion { get; set; }
        public bool IsInChoXacNhanHoanThanh { get; set; }
        public bool CanConfirmCompletion { get; set; }
        public bool CanPause { get; set; }
        public bool CanDelete { get; set; }
        public bool IsCompleted { get; set; }
        
        // Condition details for UI guidance
        public bool HasMembers { get; set; }
        public bool HasCategories { get; set; }
        public bool HasWorkItems { get; set; }
        public bool AllTasksDone { get; set; }
        public bool HasOngoingTasks { get; set; }
        public bool HasBlockedTasks { get; set; }
    }
}
