using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketConnector;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace Server
{
    class Program
    {
        private static List<ClientData> clients;
        private static Socket listener;
        private static TextWriter writer;
        private static bool log = true;

        static void Main(string[] args)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 4242);
            listener.Bind(ip);

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();


            writer = new StreamWriter("log.txt");

            Console.WriteLine("Success... Address: " + ip);

            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd.Length >= 5 && cmd.Substring(0, 5) == "SEND ")
                {
                    Console.WriteLine("Sending '" + cmd.Substring(5, cmd.Length - 5) + "'");
                    Packet p = new Packet(PacketType.Chat, "server");
                    p.data.Add("SERVER: " + cmd.Substring(5, cmd.Length - 5));
                    SendMessageToCLients(p);
                }
                else if (cmd == "x")
                {
                    Console.WriteLine("Clients: " + clients.Count);
                }
                else if (cmd == "l")
                {
                    if (log)
                    {
                        log = false;
                        Console.WriteLine("Log Disabled!");
                    }
                    else
                    {
                        log = true;
                        Console.WriteLine("Log Enabled!");
                    }
                }
                else if (cmd.Length == 4 && (cmd.Substring(0, 4) == "exit" || cmd.Substring(0, 4) == "stop"))
                {
                    writer.Close();
                    Environment.Exit(0);
                }
                else if (cmd == "d")
                {
                    Console.WriteLine("Sending large data");
                    Packet p = new Packet(PacketType.Chat, "server");
                    p.data.Add("SERVER: aiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sgh43aiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496´hi6s305ß4g4´khh6ß093s5356g5khlh40ß34s3khg´0953k5h4h0968s3k5g404h63hg935sik05494tzigß0k3352h45i6h40ß24skh635igß6497hh84j334s065hk34g01´46hhk3´03496h8sj4gh3´036h5j0s´9g4h34j66h83s45jmh64g0h´334k6hs50g34h465hjk6s3304gjh64h3543hsj5gh4trsihj0h5er384thm3s5ewlrgthmh4oeir6t8skh4gwrekthjho3pi5w4sh4glwhmtr38hop4swjrhü35t4shjüg6h544p5shmwr5gtjhüw34tmhü35s6wjh4thnms68ü0h3244g5jhh35w4sktmjhp934580h56f4ojhim3üw4ghm5f4h60ß235js5hgpimh48w0thgf4jsmhwpt4hsmg5wüpohf34thjs3fwgih8h4ß0429535shjh041g5´w4m,f3h6hw440ü956shjpgf4h35sghaiksdhfoiwehyfoihadijfhwuiehfljabndlfiuhaqwljkfnal8fhloiqwhyiohnfdlahf;oiewhoahsdofjhaosfhaohfalkjhfkajudhflkajhflkjahsflkjahfljahflkajhfslkjshfalkjdfhglakdjfg2h439038gl5jhajfgh92ß84jlga590ß824fjjhgg5ß0´824jlagdkjß9fhgla24kdj4j5fg0h92g4ijglak0jd´ß2jf4hgg0la´j24d5kj0f´gghjla8d0k2jf34ggh´ß08j245lgdkj0´9afjhg324l9aj0´ghj´09h8f3g4hjl35a46j09fh8ghgl0´93k4jadhfgkjzhl0kg39kj4h6g0ßh9lkkj345w9skhhß93l24kgzjkh6hß349khsfß09g3j45khsß9g3h4h9kß0653w544s5kh6´0ßh34555k4g6+6h08´35hh,40´34ks66h5´ß0g3h4496???");
                    SendMessageToCLients(p);
                }
            }
        }

        static void ListenThread()
        {
            while (true)
            {
                listener.Listen(0);
                clients.Add(new ClientData(listener.Accept()));
            }
        }

        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = ((ClientData)cSocket).clientSocket;

            byte[] Buffer;
            int readBytes;

            while (true)
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        Packet p = new Packet(Buffer);
                        if (log)
                        {
                            writer.WriteLine("Type: " + p.packetType + " -> Data Count: " + p.data.Count );                            
                            Console.WriteLine("Type: " + p.packetType + " -> Data Count: " + p.data.Count );
                            foreach(string values in p.data){
                                Console.WriteLine("      "+values);
                            }
                        }
                        DataManager(cSocket,p);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Client Disconnected.");
                    clients.Remove(((ClientData)cSocket));
                    Thread.CurrentThread.Abort();
                }
            }
        }

        public static void DataManager(object client,Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.getNews:
                    Packet news = new Packet(PacketType.getNews, "SERVER");
                    news.data.Add("There are no News!");
                    ((ClientData)client).clientSocket.Send(news.ToBytes());
                    break;
                case PacketType.Chat:
                    SendMessageToCLients(p);
                    break;


                case PacketType.CloseConnection:
                    var exitClient = GetClientByID(p);

                    CloseClientConnection(exitClient);
                    RemoveClientFromList(exitClient);
                    SendMessageToCLients(p);
                    AbortClientThread(exitClient);
                    break;
            }
        }

        public static void SendMessageToCLients(Packet p)
        {
            foreach (ClientData c in clients)
            {
                c.clientSocket.Send(p.ToBytes());
            }
        }
        private static ClientData GetClientByID(Packet p)
        {
            return (from client in clients
                    where client.id == p.senderID
                    select client)
                    .FirstOrDefault();

        }
        private static void CloseClientConnection(ClientData c)
        {
            c.clientSocket.Close();
        }
        private static void RemoveClientFromList(ClientData c)
        {
            clients.Remove(c);
        }
        private static void AbortClientThread(ClientData c)
        {
            c.clientThread.Abort();
        }
    }
}