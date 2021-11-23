using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ_NewTaskWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue2",
                                durable: true,//false
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);//

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    #region meşgulmüşüz gibi gösterecek olan kod bloğu
                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000); 
                    #endregion

                    Console.WriteLine(" [x] Done");

                    //Message acknowledgment
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); 

                };
                channel.BasicConsume(queue: "task_queue2",
                                     autoAck: false, ///true
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
