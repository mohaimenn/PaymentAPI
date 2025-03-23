using PaymentAPI.Models;
using PaymentAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MollieSettings>(builder.Configuration.GetSection("Mollie"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<AdyenSettings>(builder.Configuration.GetSection("Adyen"));

builder.Services.AddScoped<MolliePaymentService>();
builder.Services.AddScoped<StripePaymentService>();
builder.Services.AddScoped<AdyenPaymentService>();
builder.Services.AddScoped<PaymentServiceFactory>();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", builder =>
    {
        builder.WithOrigins("http://localhost:8080")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVueApp");
app.UseAuthorization();

app.MapControllers();

app.Run();