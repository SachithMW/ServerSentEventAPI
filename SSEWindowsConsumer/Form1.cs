using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSEWindowsConsumer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			
		}

		public static async Task Consumer()
		{
			HttpClientHandler clientHandler = new HttpClientHandler();
			clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

			// Pass the handler to httpclient(from you are calling api)
			HttpClient client = new HttpClient(clientHandler);
			client.Timeout = TimeSpan.FromSeconds(10);
			string CIF = "5";

			string url = $"https://localhost:5003/messages/subscribe/{CIF}";

			while (true)
			{
				try
				{
					Console.WriteLine("Establishing connection");
					using (var streamReader = new StreamReader(await client.GetStreamAsync(url)))
					{
						while (!streamReader.EndOfStream)
						{
							var message = await streamReader.ReadLineAsync();
							Console.WriteLine($"Received price update: {message}");
						}
					}
					Console.ReadKey();

				}
				catch (Exception ex)
				{
					//Here you can check for 
					//specific types of errors before continuing
					//Since this is a simple example, i'm always going to retry
					Console.WriteLine($"Error: {ex.Message}");
					Console.WriteLine("Retrying in 5 seconds");
					await Task.Delay(TimeSpan.FromSeconds(5));
				}
			}
		}

        private async void btnStart_ClickAsync(object sender, EventArgs e)
        {
			await Consumer();
		}
    }
}
