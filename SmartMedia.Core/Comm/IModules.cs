namespace SmartMedia.Core.Comm;

public interface IModules
{
    int OrderIndex { get; } // 越大越靠后

    string Title { get; }
    Image Ico { get; }
}
