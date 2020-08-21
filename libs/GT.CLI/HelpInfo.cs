namespace GT.CLI
{
    /// <summary>
    /// 帮助提示信息
    /// </summary>
    public class HelpInfo
    {
        public string Version { get; set; }
        public HelpInfo() { }

        public string GetWelcomeInfo()
        {
            return "\nWelcome!";
        }
        public string GetHelpInfo()
        {
            return "\nHelp!";
        }
        public string UnRecognized()
        {
            return "\n No exist command!";
        }

    }
}
