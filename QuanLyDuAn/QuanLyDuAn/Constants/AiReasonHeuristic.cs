namespace QuanLyDuAn.Constants
{
    public static class AiReasonHeuristic
    {
        // Đồng bộ với ngưỡng FastAPI (.env/.env.example) cho reason-only.
        public const double HighDelayRatioThreshold = 0.2d;
        public const double SevereDelayRatioThreshold = 0.5d;
        public const double HighCostOverrunThreshold = 0.15d;
        public const double HighStaffChangeThreshold = 2d;
        public const double HighManagerChangeThreshold = 1d;
        public const int LongDelayDaysThreshold = 14;
        public const int SevereOverdueTasksThreshold = 5;
    }
}
