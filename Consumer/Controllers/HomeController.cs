using Consumer.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;

namespace Consumer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string selectedOption = "orange")
        {
            var messages = ReceiveMessages(selectedOption);
            ViewBag.Messages = messages;
            return View();
        }

        private List<string> ReceiveMessages(string selectedOption)
        {
            var messages = new List<string>();

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://psqyqkas:QYmaSp0TjmkU91ADQ-m0oc8AZFob-vhj@puffin.rmq2.cloudamqp.com/psqyqkas")
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_exchange", type: ExchangeType.Topic);
                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName, exchange: "topic_exchange", routingKey: selectedOption);

                var data = channel.BasicGet(queueName, true);
                if (data != null)
                {
                    var message = Encoding.UTF8.GetString(data.Body.ToArray());
                    messages.Add(message);
                }

            }

            return messages;
        }
    }
}
