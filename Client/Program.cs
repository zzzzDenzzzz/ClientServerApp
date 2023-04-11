using System.Net.Sockets;
using System.Text;

// слова для отправки для получения перевода
var words = new string[] { "red", "yellow", "blue" };

using var tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

try
{
    await tcpClient.ConnectAsync("127.0.0.1", 8888);
    // буфер для входящих данных
    var response = new List<byte>();
    foreach (var word in words)
    {
        // считыванием строку в массив байт
        // при отправке добавляем маркер завершения сообщения - \n
        byte[] data = Encoding.UTF8.GetBytes(word + '\n');
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
        var translation = Encoding.UTF8.GetString(response.ToArray());
        Console.WriteLine($"Слово {word}: {translation}");
        response.Clear();
    }

    // отправляем маркер завершения подключения - END
    //await tcpClient.SendAsync(Encoding.UTF8.GetBytes("END\n"), SocketFlags.None);
    //Console.WriteLine("Все сообщения отправлены");

    while (true)
    {
        string mes = Console.ReadLine()!;

        byte[] data = Encoding.UTF8.GetBytes(mes + '\n');
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
        var translation = Encoding.UTF8.GetString(response.ToArray());
        Console.WriteLine($"Слово {mes}: {translation}");
        response.Clear();
    }
    
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
