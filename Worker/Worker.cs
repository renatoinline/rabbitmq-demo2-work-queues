using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Worker
{
    class Worker
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var factory = new ConnectionFactory()
            {
                HostName = "192.168.15.33",
                UserName = "renatocolaco",
                Password = "secnet123"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // This tells RabbitMQ not to give more than one message to a worker at a time
                // prefetchCount: 1
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                channel.QueueDeclare(
                    queue: "task_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] message received {0}", message);
                                        
                    Thread.Sleep(4000);

                    Console.WriteLine(" [#] Done");

                    // manual ack
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(
                    queue: "task_queue",
                    autoAck: false, // manual ack in order to not lost messages if consumer dies
                    consumer: consumer
                    );

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
