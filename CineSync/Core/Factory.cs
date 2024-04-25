namespace CineSync.Core
{
    public class Factory : IFactory
    {
        private ICollection<Type> Constructs { get; set; }

        public Factory(params Type[] types)
        {
            Constructs = new List<Type>(types);
        }

        public T? Create<T>(params object?[]? args)
        {
            var type = typeof(T);

            if (!Constructs.Contains(type)) return Cast<T>(null);
            return Cast<T>(Activator.CreateInstance(type, args));
        }

        public static T? Cast<T>(object? obj)
        {
            return (T?)obj;
        }
    }
}
