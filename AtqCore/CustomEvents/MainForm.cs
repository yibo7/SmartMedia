

using XS.WinFormsTools;

namespace SmartMedia.AtqCore.CustomEvents
{

    public class TipEventArgs : EventArgs
    {
        public string Title = string.Empty;
        public TipsState State = TipsState.Default;
        public ContentAlignment Alignment = ContentAlignment.BottomCenter;
        public TipEventArgs(string title, TipsState tipsState = TipsState.Default, ContentAlignment alignment = ContentAlignment.BottomCenter)
        {
            State = tipsState;
            Alignment = alignment;
            Title = title;
        }
    }

    internal static class MainForm
    {
        // 声明事件，使用内置的 EventHandler 委托类型
        static public event EventHandler<TipEventArgs> EvShowTips;

        // 触发事件的方法
        static public void OnShowTips(string sTitle, TipsState tipsState = TipsState.Default, ContentAlignment alignment = ContentAlignment.BottomCenter)
        {
            if (EvShowTips != null)
            {
                EvShowTips?.Invoke(sTitle, new TipEventArgs(sTitle, tipsState, alignment));
            }
        }
        // 声明事件，使用内置的 EventHandler 委托类型
        static public event EventHandler EvCopy;

        // 触发事件的方法
        static public void OnCopy(string txt)
        {
            if (EvCopy != null)
            {
                // 触发事件
                EvCopy?.Invoke(txt, EventArgs.Empty);
            }
        }

    }
}
