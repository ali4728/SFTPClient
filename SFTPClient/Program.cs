using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Params.parseAppConfig();
            Params.ConfigLogging();
            //Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);

            Client client = new Client();
            int result = client.Execute();



            Console.WriteLine("result is: " + result);
            Console.WriteLine("click enter to exit");
            Console.ReadLine();

        }
    }
}
