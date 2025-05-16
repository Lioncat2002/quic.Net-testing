
using System.Runtime.InteropServices;
using System.Text;
using QuicClient;
using QuicNet;
using QuicNet.Connections;
using QuicNet.Streams;


   public class Program
    {
        
        public static byte[] StructToBytes<T>(T strct) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(strct, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return arr;
        }
        
        public static byte[] SerializeQueryData<T>(int function, T payload) where T : struct
        {
            byte[] functionBytes = BitConverter.GetBytes(function);
            byte[] payloadBytes = StructToBytes(payload);

            byte[] result = new byte[functionBytes.Length + payloadBytes.Length];
            Buffer.BlockCopy(functionBytes, 0, result, 0, functionBytes.Length);
            Buffer.BlockCopy(payloadBytes, 0, result, functionBytes.Length, payloadBytes.Length);

            return result;
        }



        static void Main(string[] args)
        {
            QuicNet.QuicClient client = new QuicNet.QuicClient();

            // Connect to peer (Server)
            QuicConnection connection = client.Connect("127.0.0.1", 11000);
            // Create a data stream
            
            var player = new PlayerData
            {
                id = 1,
                x = 1.0f,
                y = 7.0f,
            };
            // Send Data
            for (int i = 0; i < 30; i++)
            {
                QuicStream stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);

                var queryBytes = SerializeQueryData(0x01, player);
                stream.Send(queryBytes);
                
                byte[] data = stream.Receive();
                Console.WriteLine(i+" "+ Encoding.UTF8.GetString(data));
            }
            var nstream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);

            var resp = SerializeQueryData(0x02, player);
            nstream.Send(resp);
            var ndata = nstream.Receive();
            Console.WriteLine(Encoding.UTF8.GetString(ndata));
            

            Console.ReadKey();
        }
    }
