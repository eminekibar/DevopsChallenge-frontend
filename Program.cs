using System.Text.Json;

var builder = WebApplication.CreateBuilder(args); //.NET 6 minimal API kullanarak basit bir web uygulaması oluşturur.
var app = builder.Build(); // Uygulamayı oluşturur.

app.UseHttpsRedirection(); // HTTPS yönlendirmesini etkinleştirir.

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
    button:hover {
        background: #4a5fd2;
    }
";

// Ana sayfa rotası
app.MapGet("/", async (HttpContext context) => // Ana sayfa isteğini işler.
{
    string yourName = context.Request.Query["YourName"].ToString() ?? ""; // Sorgu parametresinden "YourName" değerini alır.

    if (string.IsNullOrWhiteSpace(yourName)) // Eğer "YourName" boşsa, giriş formunu gösterir.
    {
        return Results.Content($@" // HTML içeriğini döner.
            <html>
            <head><meta charset='UTF-8'><style>{css}</style></head> // CSS stilini ekler.
            <body>
                <div class='card'>
                    <form method='get'> // GET yöntemiyle form gönderimi.
                        <label>İsim:</label>
                        <input name='YourName' placeholder='Adınızı girin...'>
                        <button type='submit'>Göster</button>
                    </form>
                </div>
            </body>
            </html>", "text/html"); // İçeriği HTML olarak döner.
    }

    using var client = new HttpClient(); // HTTP istemcisi oluşturur.
    var backendBase = Environment.GetEnvironmentVariable("BACKEND_URL") ?? "http://backend:11130"; // backend servis URL'sini alır.
    var url = $"{backendBase}/api/values?YourName={yourName}"; // Backend API URL'sini oluşturur.
    var response = await client.GetStringAsync(url); // Backend API'sine istek gönderir ve yanıtı alır.
    var message = JsonSerializer.Deserialize<List<string>>(response)?[0]; // Yanıttan mesajı ayrıştırır. JSON formatında beklenir.

    return Results.Content($@" // HTML içeriğini döner.
        <html> 
        <head><meta charset='UTF-8'><style>{css}</style></head>
        <body>
            <div class='card'>
                <h2>{message}</h2> // Mesajı gösterir.
                // forma geri dönme butonu
                <form method='get'>
                    <button type='submit'>Geri Dön</button>
            </div>
        </body>
        </html>", "text/html"); // İçeriği HTML olarak döner.
});

app.Run(); // Uygulamayı çalıştırır.
