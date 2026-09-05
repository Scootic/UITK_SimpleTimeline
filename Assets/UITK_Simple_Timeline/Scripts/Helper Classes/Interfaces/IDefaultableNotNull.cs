namespace UITK_SimpleTimeline
{
    public interface IDefaultableNotNull<T> where T : notnull
    {
        public T Default();
    }
}
