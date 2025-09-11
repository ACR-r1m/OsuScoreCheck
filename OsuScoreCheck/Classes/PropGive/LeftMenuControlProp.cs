namespace OsuScoreCheck.Classes.PropGive
{
    class LeftMenuControlMessage
    {
        public bool IsButton1Checked { get; }
        public bool IsButton2Checked { get; }
        public bool IsButton3Checked { get; }

        public LeftMenuControlMessage(bool isButton1Checked, bool isButton2Checked, bool isButton3Checked)
        {
            IsButton1Checked = isButton1Checked;
            IsButton2Checked = isButton2Checked;
            IsButton3Checked = isButton3Checked;
        }
    }
}
