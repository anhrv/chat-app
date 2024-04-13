using ChatApplicationAPI.Hub;
using ChatApplicationAPI.Models;

namespace ChatApplicationAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();
			builder.Services.AddSignalR();

			builder.Services.AddSingleton<IDictionary<string, UserRoomConnection>>(opt =>
				new Dictionary<string, UserRoomConnection>()
			);

			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
					builder.WithOrigins("http://localhost:4200")
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
				});
			});

			var app = builder.Build();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors();

			app.UseEndpoints(endpoint =>
			{
				endpoint.MapHub<ChatHub>("/chat");
			});

			app.MapControllers();

			app.Run();
		}
	}
}
