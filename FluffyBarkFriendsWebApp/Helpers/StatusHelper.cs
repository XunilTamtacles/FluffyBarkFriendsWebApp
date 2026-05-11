namespace FluffyBarkFriendsWebApp.Helpers
{
    public static class StatusHelper
    {
        public static string DisplayStatusText(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return "No Status";
            }

            return status.Trim();
        }

        public static string GetBootstrapBadgeClass(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return "badge bg-secondary";
            }

            switch (status.Trim().ToLower())
            {
                case "pending":
                    return "badge bg-warning text-dark";

                case "approved":
                case "scheduled":
                case "confirmed":
                    return "badge bg-info text-dark";

                case "completed":
                    return "badge bg-success";

                case "cancelled":
                case "canceled":
                    return "badge bg-danger";

                default:
                    return "badge bg-secondary";
            }
        }
    }
}
