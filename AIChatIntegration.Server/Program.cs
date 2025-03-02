using FastEndpoints;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();


// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


// Register and configure IChatClient
IChatClient ollamaClient = new OllamaChatClient(new Uri("http://host.docker.internal:11434/"), modelId: "llama3.2");
IChatClient client = new ChatClientBuilder(ollamaClient).UseFunctionInvocation().Build();
builder.Services.AddChatClient(client)
    .ConfigureOptions(options =>
    {
        options.Temperature = 1;
        
    })
    .UseLogging();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFastEndpoints();

app.Run();
