using System.Net;
using System.Net.Sockets;
using System.Text;

using Socket tcpListener = new (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

try
{
    tcpListener.Bind(new IPEndPoint(IPAddress.Any, 8888));
    tcpListener.Listen();    // запускаем сервер
    Console.WriteLine($"Сервер запущен. Ожидание подключений...{Environment.NewLine}");

    while (true)
    {
        // получаем подключение в виде TcpClient
        using var tcpClient = await tcpListener.AcceptAsync();

        // буфер для накопления входящих данных
        var response = new List<byte>();
        // буфер для считывания одного байта
        var bytesRead = new byte[1];
        while (true)
        {
            // считываем данные до конечного символа
            while (true)
            {
                var count = tcpClient.Receive(bytesRead);
                // смотрим, если считанный байт представляет конечный символ, выходим
                if (count == 0 || bytesRead[0] == '\n') break;
                // иначе добавляем в буфер
                response.Add(bytesRead[0]);
            }
            var request = Encoding.UTF8.GetString(response.ToArray());
            // если прислан маркер окончания взаимодействия,
            // выходим из цикла и завершаем взаимодействие с клиентом
            if (string.Equals(request, "end", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Клиент {tcpClient.LocalEndPoint} завершил работу");
                break;
            } 

            Console.WriteLine($"Запрос: {request}");

            DateTime time = DateTime.Now;
            await tcpClient.SendAsync(Encoding.UTF8.GetBytes(time.ToString("G") + "\n"), SocketFlags.None);
            response.Clear();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
