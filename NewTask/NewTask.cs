using System;
using System.Text;
using System.Timers;
using RabbitMQ.Client;

namespace Demo2
{
    class NewTask
    {
        private static Timer messageTimer = null;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            setTimer();

            Console.ReadKey();
        }

        private static void setTimer()
        {
            // Create a timer with a two second interval.
            messageTimer = new Timer(2000);
            // Hook up the Elapsed event for the timer. 
            messageTimer.Elapsed += OnTimedEvent;
            messageTimer.AutoReset = true;
            messageTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.15.33",
                UserName = "renatocolaco",
                Password = "secnet123"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "task_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
                var message = GetMessage();
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: "task_queue",
                                     basicProperties: properties,
                                     body: body);
            }

            Console.WriteLine("A new message published to the queue at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }

        private static string GetMessage()
        {
            return $"Hello World! {DateTime.Now:HH:mm:ss}";
        }
    }
}
