namespace SmartMedia.MCore
{
    public interface IModules
    {
        int OrderIndex { get; } // 越大越靠后

        string Title { get; }
        Image Ico { get; }
    }
}
