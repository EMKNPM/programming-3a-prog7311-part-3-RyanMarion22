using TechMoveMVC.Interfaces;

namespace TechMoveMVC.Services
{
    public class NotificationService : IObserver
    {
        public void Update(string message)
        {
            Console.WriteLine("Notification: " + message);
        }
    }
}