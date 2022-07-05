namespace GitlabSlackNotifier.Core;

public static class GlobalConstants
{
    public static class Jira
    {
        public static string JiraTicketRegex = @"[A-Z][A-Z0-9]{2,}-\d+";
        public static string JiraIssuePlaceholder = "[ISSUE]";
        public static string JiraOwnerPlaceHolder = "[OWNER]";
    }
    
    public static class Slack
    {
        public static string ReminderStringStart = "Reminder: ";
        public static string HereMention = "<!here>";
    }
}