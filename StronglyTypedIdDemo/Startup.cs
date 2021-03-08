using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StronglyTypedIdDemo.Data;
using StronglyTypedIdDemo.Infrastructure;
using StronglyTypedIdDemo.Infrastructure.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StronglyTypedIdDemo
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
            services.AddDbContext<StronglyTypedIdDemoDbContext>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StronglyTypedIdDemo", Version = "v1" });

                //// 获取xml文件名
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //// 获取xml文件路径
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //// 添加控制器层注释，true表示显示控制器注释
                //c.IncludeXmlComments(xmlPath, true);

                c.MapTypeOfStronglyTypedId(typeof(Startup).Assembly);
            });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StronglyTypedIdDemo v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetService<StronglyTypedIdDemoDbContext>();

                db.Database.Migrate();

                var orders = db.Orders.ToList();

                if (orders.Count > 0)
                {
                    db.Orders.RemoveRange(orders);
                }

                db.Orders.AddRange(
                    new Order { Id = new OrderId(1), No = "F001" },
                    new Order { Id = new OrderId(2), No = "F002" }
                );

                db.SaveChanges();
            }
        }
    }
}
