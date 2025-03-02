using AIChatIntegration.Server.Models.Configuration;
using FastEndpoints;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();
bool isDev = builder.Environment.IsDevelopment();

// Configure CORS
if (isDev)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
    });
}





// Register and configure IChatClient
//pull out the ClientConfig from the appsettings
var config = builder.Configuration.GetSection("ChatClientConfig").Get<ChatClientConfig>();
// run LLM locally with ollama see https://github.com/ollama/ollama,  this is configured to use the llama3.2 model, if you don't change any configuration in the docker-compose.overeride.yml file
IChatClient chatClient = isDev ? new OllamaChatClient(new Uri(config.Uri), modelId: config.ModelName) : new OpenAIClient(Environment.GetEnvironmentVariable(config.APIKey)).AsChatClient(config.ModelName); 
IChatClient client = new ChatClientBuilder(chatClient).UseFunctionInvocation().Build();
builder.Services.AddChatClient(client)
    .ConfigureOptions(options =>
    {
        options.Temperature = 1;
        
    })
    .UseLogging();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFastEndpoints();

app.Run();
