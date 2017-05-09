using MyRepository.Api;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRepository.Test
{
    [TestFixture]
    public class RepositoryTest
    {
        IFmxRepository repository;

        [OneTimeSetUpAttribute]
        public void Init()
        {
            repository = new FmxRepository();
            repository.Initialize();
        }

        [Test]
        public void TestDuplicateInitilization()
        {
            Exception expected = null;

            try
            {
                repository.Initialize();
            }
            catch (Exception e)
            {
                expected = e;
            }

            Assert.IsNotNull(expected);
        }

        [Test]
        public void TestRepositoryValidInput()
        {
            Exception expected = null;
            string res1 = null;
            string res2 = null;

            try
            {
                string contentJson = File.ReadAllText(@".\testingdata\validjson.json");
                string contentXml = File.ReadAllText(@".\testingdata\validxml.xml");
                
                repository.Register("1", contentJson, 1);
                repository.Register("2", contentXml, 2);

                res1 = repository.Retrieve("1");
                res2 = repository.Retrieve("2");
            }
            catch (Exception e)
            {
                expected = e;
            }

            Assert.IsNull(expected);
            Assert.IsNotNull(res1);
            Assert.IsNotNull(res2);
        }

        [Test]
        public void TestRepositoryInValidInput()
        {
            Exception expected = null;
            string res1 = null;
            string res2 = null;

            try
            {
                string contentJson = File.ReadAllText(@".\testingdata\validjson.json");
                string contentXml = File.ReadAllText(@".\testingdata\validxml.xml");

                repository.Register("3", contentJson, 2); //Raise invalid format here, since itemType and itemContent are not match.
                repository.Register("4", contentXml, 1);

                res1 = repository.Retrieve("3");
                res2 = repository.Retrieve("4");
            }
            catch (Exception e)
            {
                expected = e;
            }

            Assert.IsNotNull(expected);
            Assert.IsNull(res1);
            Assert.IsNull(res2);
        }
    }
}
