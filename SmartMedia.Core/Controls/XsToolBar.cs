using WeifenLuo.WinFormsUI.Docking;

namespace SmartMedia.Core.Controls;

public class XsToolBar : ToolStrip
{
    public XsToolBar()
    {
        VisualStudioToolStripExtender vsToolStripExtender1 = new VisualStudioToolStripExtender();
        vsToolStripExtender1.SetStyle(this, VisualStudioToolStripExtender.VsVersion.Vs2012, new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme());
    }
}
