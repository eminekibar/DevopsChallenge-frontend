using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

string css = @"
    body {
        font-family: Arial, sans-serif;
        background: #f7f7fa;
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100vh;
        margin: 0;
    }
    .card {
        background: #fff;
        padding: 20px;
        border-radius: 8px;
        box-shadow: 0 2px 6px rgba(0,0,0,0.1);
        width: 280px;
        text-align: center;
    }
    input, button {
        width: 100%;
        padding: 8px;
        margin-top: 10px;
        font-size: 14px;
    }
    button {
        background: #5e72e4;
        color: #fff;
        border: none;
        border-radius: 4px;
        cursor: pointer;
    }
    button:hover { background: #4a5fd2; }
";

app.MapGet("/", async (HttpContext context) =>
{
    sstring yourName = context.Request.Query["YourName"].ToString() ?? "";

    if (string.IsNullOrWhiteSpace(yourName))
    {
        return Results.Content($@"
            <html>
            <head><meta charset='UTF-8'><style>{css}</style></head>
            <body>
                <div class='card'>
                    <form method='get'>
                        <label>İsim:</label>
                        <input name='YourName' placeholder='Adınızı girin...'>
                        <button type='submit'>Göster</button>
                    </form>
                </div>
            </body>
            </html>", "text/html");
    }

    using var client = new HttpClient();
    var backendBase = Environment.GetEnvironmentVariable("BACKEND_URL")
                     ?? "http://backend:11130";
    var url = $"{backendBase}/api/values?YourName={yourName}";
    var response = await client.GetStringAsync(url);
    var message = JsonSerializer.Deserialize<List<string>>(response)?[0];

    return Results.Content($@"
        <html>
        <head><meta charset='UTF-8'><style>{css}</style></head>
        <body>
            <div class='card'>
                <h2>{message}</h2>
            </div>
        </body>
        </html>", "text/html");
});

app.Run();
