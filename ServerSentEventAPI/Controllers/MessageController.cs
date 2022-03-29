using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerSentEventAPI.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerSentEventAPI.Controllers
{
    [ApiController]
    public class MessageController : ControllerBase
    {
		private readonly IMessageQueue MessageQueue;

        public MessageController(IMessageQueue messageQueue)
        {
			MessageQueue = messageQueue;
		}
		[HttpGet]
		[Route("messages/subscribe/{cif}")]
		public async Task Subscribe(string cif)
		{
			Response.ContentType = "text/event-stream";
			Response.StatusCode = 200;

			StreamWriter streamWriter = new StreamWriter(Response.Body);

			MessageQueue.Register(cif);

			try
			{
				await MessageQueue.EnqueueAsync(cif, $"Subscribed to CIF {cif}", HttpContext.RequestAborted);

				await foreach (var message in MessageQueue.DequeueAsync(cif, HttpContext.RequestAborted))
				{
					await streamWriter.WriteLineAsync($"{DateTime.Now} {message}");
					await streamWriter.FlushAsync();
				}
			}
			catch (OperationCanceledException)
			{
				//this is expected when the client disconnects the connection
			}
			catch (Exception)
			{
				Response.StatusCode = 400;
			}
			finally
			{
				MessageQueue.Unregister(cif);
			}
		}



		[HttpGet]
		[Route("messages/sse/{cif}")]
		public async Task SimpleSSE(string cif)
		{
			//1. Set content type
			Response.ContentType = "text/event-stream";
			Response.StatusCode = 200;

			StreamWriter streamWriter = new StreamWriter(Response.Body);

			while (!HttpContext.RequestAborted.IsCancellationRequested)
			{
				//2. Await something that generates messages
				await Task.Delay(5000, HttpContext.RequestAborted);

				//3. Write to the Response.Body stream
				await streamWriter.WriteLineAsync($"{DateTime.Now} Looping");
				await streamWriter.FlushAsync();

			}
		}


		[HttpPost]
		[Route("messages")]
		public async Task<IActionResult> PostMessage([FromBody] MessageRequest messageRequest)
		{
			try
			{
				await MessageQueue.EnqueueAsync(messageRequest.cif, messageRequest.Message, HttpContext.RequestAborted);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
