using QuantDeriv.Back.Hubs;
using QuantDeriv.Back.Interfaces;
using QuantDeriv.Back.Repositories;
using QuantDeriv.Back.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton<ITradeDataRepository, TradeDataRepository>();

// 주문 서비스 및 매칭 서비스 등록
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<IOrderMatchService, OrderMatchService>();

//자동 매매 시뮬레이션 서비스 등록
builder.Services.AddHostedService<TradeSimulator>();

//builder.Services.AddCors(options => {
//    options.AddDefaultPolicy(policy =>
//        policy.WithOrigins("http://localhost", "null")
//              .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
//});

var app = builder.Build();

//app.UseCors();

app.MapHub<TradeHubs>("/tradeHub");

app.Run();
