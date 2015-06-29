using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace LoginServer
{
    [Serializable]
    class MessageData
    {
        public int type = 0;
        public string stringData = "";

        public static MessageData FromByteArray(byte[] input)
        {
            MemoryStream stream = new MemoryStream(input);
            BinaryFormatter formatter = new BinaryFormatter();
            MessageData data = new MessageData();
            data.type = (int)formatter.Deserialize(stream);
            data.stringData = (string)formatter.Deserialize(stream);

            if (data.stringData == "")
            {
                data.type = 999;
                data.stringData = "No Command Included";
            }
            return data;
        }

        public static byte[] ToByteArray(MessageData msg)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, msg.type);
            formatter.Serialize(stream, msg.stringData);

            return stream.ToArray();
        }
    }
}
