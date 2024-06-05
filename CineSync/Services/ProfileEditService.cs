using CineSync.Components.Utils;

namespace CineSync.Services
{
    public class ProfileEditService
    {
        public readonly ICollection<IObserver> observers = new List<IObserver>();

        public void Subscribe(IObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Unsubscribe(IObserver observer)
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }

        public async void NotifyProfileEdit()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }
    }
}
