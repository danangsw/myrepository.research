using MyRepository.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallFmxRepository
{
    class Program
    {
        static IFmxRepository repository;
        static void Main(string[] args)
        {
            repository = new FmxRepository();
            repository.Initialize();

            Console.WriteLine("ENTER TO START TEST");
            Console.ReadLine();
            SampleValidRegisterRepository();
            Console.ReadLine();
            SampleInValidContentRegisterRepository();
            Console.ReadLine();
            SampleInvalidDuplicateRegisterRepository();
            Console.ReadLine();
            SampleValidDeleteRegisterRepository();
            Console.ReadLine();
            SampleInvalidDuplicateInstanceRegisterRepository();
            Console.ReadLine();
        }

        private static void SampleValidRegisterRepository()
        {
              Console.WriteLine("START TEST: SampleValidRegisterRepository");

              try
              {
                  string contentJson = File.ReadAllText(@".\testingdata\validjson.json");
                  string contentXml = File.ReadAllText(@".\testingdata\validxml.xml");

                  repository.Register("1", contentJson, 1);
                  repository.Register("2", contentXml, 2);

                  Console.WriteLine(repository.Retrieve("1"));
                  Console.WriteLine(repository.Retrieve("2"));

                  Console.WriteLine("SUCCEEDED!");
              }
              catch (Exception ex)
              {
                  Console.WriteLine(ex.Message);
                  Console.WriteLine("FAILED!");
              }

              Console.WriteLine("END TEST: SampleValidRegisterRepository");
        }

        private static void SampleInValidContentRegisterRepository()
        {
            Console.WriteLine("START TEST: SampleInValidContentRegisterRepository");

            try
            {
                string contentJson = File.ReadAllText(@".\testingdata\validjson.json");
                string contentXml = File.ReadAllText(@".\testingdata\validxml.xml");

                repository.Register("3", contentJson, 2); //Raise invalid format here, since itemType and itemContent are not match.
                repository.Register("4", contentXml, 1);

                Console.WriteLine(repository.Retrieve("3"));
                Console.WriteLine(repository.Retrieve("4"));

                Console.WriteLine("SUCCEEDED!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("FAILED!");
            }

            Console.WriteLine("END TEST: SampleInValidContentRegisterRepository");
        }

        private static void SampleInvalidDuplicateRegisterRepository()
        {
            Console.WriteLine("START TEST: SampleInvalidDuplicateRegisterRepository");

            try
            {
                string contentJson = File.ReadAllText(@".\testingdata\validjson.json");
                string contentXml = File.ReadAllText(@".\testingdata\validxml.xml");

                repository.Register("1", contentJson, 1); //Raise duplicate exception here
                repository.Register("2", contentXml, 2);

                Console.WriteLine(repository.Retrieve("1"));
                Console.WriteLine(repository.Retrieve("2"));

                Console.WriteLine("SUCCEEDED!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("FAILED!");
            }

            Console.WriteLine("END TEST: SampleInvalidDuplicateRegisterRepository");
        }

        private static void SampleValidDeleteRegisterRepository()
        {
            Console.WriteLine("START TEST: SampleValidDeleteRegisterRepository");

            try
            {
                string contentJson = File.ReadAllText(@".\testingdata\validjson.json");
                string contentXml = File.ReadAllText(@".\testingdata\validxml.xml");

                repository.Deregister("1");
                repository.Deregister("2");

                Console.WriteLine(repository.Retrieve("1")); //Raise not found exception here
                Console.WriteLine(repository.Retrieve("2"));

                Console.WriteLine("SUCCEEDED!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("FAILED!");
            }

            Console.WriteLine("END TEST: SampleValidDeleteRegisterRepository");
        }

        private static void SampleInvalidDuplicateInstanceRegisterRepository()
        {
            Console.WriteLine("START TEST: SampleInvalidDuplicateInstanceRegisterRepository");

            try
            {
                repository.Initialize(); //Raise duplicate instance exception here
                Console.WriteLine("SUCCEEDED!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("FAILED!");
            }

            Console.WriteLine("END TEST: SampleInvalidDuplicateInstanceRegisterRepository");
        }
    }
}
