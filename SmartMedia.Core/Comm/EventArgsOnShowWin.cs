using WeifenLuo.WinFormsUI.Docking;

namespace SmartMedia.Core.Comm;

public class EventArgsOnShowWin : EventArgs
{
    public EventArgsOnShowWin(DockContent _DockWin, string Title)
    {
        this.Name = Title;
        this.DockWin = _DockWin;
    }
    public string Name { get; set; }
    public DockContent DockWin { get; set; }


}
