using System.Net.Sockets;
using System.Text;

using var tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

try
{
    await tcpClient.ConnectAsync("127.0.0.1", 8888);
    // буфер для входящих данных
    var response = new List<byte>();

    while (true)
    {
        Console.Write("Ведите запрос: ");
        string request = Console.ReadLine()!;

        byte[] data = Encoding.UTF8.GetBytes(request + "\n");
        // отправляем данные
        await tcpClient.SendAsync(data, SocketFlags.None);

        // буфер для считывания одного байта
        var bytesRead = new byte[1];
        // считываем данные до конечного символа
        while (true)
        {
            var count = tcpClient.Receive(bytesRead);
            // смотрим, если считанный байт представляет конечный символ, выходим
            if (count == 0 || bytesRead[0] == '\n') break;
            // иначе добавляем в буфер
            response.Add(bytesRead[0]);
        }

        if (!string.Equals(request, "end", StringComparison.OrdinalIgnoreCase))
        {
            var translation = Encoding.UTF8.GetString(response.ToArray());
            Console.WriteLine($"Ответ от сервера: {translation}");
            response.Clear();
        }
        else
        {
            Console.WriteLine("Клиент завершил работу");
            break;
        }
    }
    
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
