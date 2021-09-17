using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace ChatServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IAmazonDynamoDB>(x => DynamoDbClientFactory.CreateClient());
            services.AddSingleton<IDatabaseClient, DatabaseClient>();
            services.AddSingleton<MyAppData>();
            services.AddSingleton<ChatRepository>();
            services.AddSingleton<FriendRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ChatRepository chatRepository,
            FriendRepository friendRepository,
            MyAppData myAppData)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            await chatRepository.CreateTableAsync();
            await friendRepository.CreateTableAsync();

            //Need to scan all friends into memory
            friendRepository.ScanDynamoDBAsync();
            chatRepository.ScanDynamoDBAsync();

            Console.WriteLine("Server up and running!");
        }
    }
}