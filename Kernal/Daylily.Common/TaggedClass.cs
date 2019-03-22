namespace Daylily.Common
{
    public struct TaggedClass<T>
    {
        public TaggedClass(string tag, T instance) : this()
        {
            Tag = tag;
            Instance = instance;
        }

        public string Tag { get; set; }
        public T Instance { get; set; }
    }
}