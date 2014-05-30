using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mock {
    class Program {
        static void Main(string[] args) {

            var sr = new ServiceHost(typeof(ServiceRepository), new Uri[] { new Uri("net.tcp://127.0.0.1:50000/IServiceRepository") });
            var ar = new ServiceHost(typeof(AccountRepository), new Uri[] { new Uri("net.tcp://127.0.0.1:50000/IAccountRepository") });

            try {
                sr.AddServiceEndpoint(typeof(IServiceRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:50000/IServiceRepository");
                ar.AddServiceEndpoint(typeof(IAccountRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:50000/IAccountRepository");

                sr.Open();
                ar.Open();

                Console.ReadLine();

                sr.Close();
                ar.Close();
            }
            catch (CommunicationException commError) {
                Console.WriteLine("Communication error: " + commError.Message);
                sr.Abort();
                ar.Abort();
                Console.Read();
            }
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ServiceRepository : IServiceRepository {
        public void registerService(String Name, String Address) {
            Console.WriteLine("Registered service " + Name + " at " + Address);
            return;
        }

        public string getServiceAddress(String Name) {
            Console.WriteLine("Service ask about " + Name + " address.");
            if (Name.Equals("IAccountRepository"))
                return "net.tcp://127.0.0.1:11111/IAccountRepository";
            else
                return "";
        }

        public void unregisterService(String Name) {
            Console.WriteLine("Service " + Name + " unregistered.");
            return;
        }

        public void isAlive(String Name) {
            Console.WriteLine("Service " + Name + " is alive.");
            return;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class AccountRepository : IAccountRepository {
        public string CreateAccount(Guid clientId, AccountDetails details) {
            return "account";
        }

        public AccountDetails GetAccountInformation(string accountNumber) {
            AccountDetails acc = new AccountDetails();
            return acc;
        }

        public bool UpdateAccountInformation(AccountDetails details) {
            return true;
        }

        public List<AccountDetails> GetAllAccounts() {
            return null;
        }

        public List<AccountDetails> GetAccountsById(Guid ClientId) {
            List<AccountDetails> list = new List<AccountDetails>();

            AccountDetails acc = new AccountDetails();
            acc.Id = Guid.NewGuid();
            acc.ClientId = ClientId;
            acc.AccountNumber = "22 3333 4444 5555 6666 7777 8888";
            acc.Money = 123456;
            acc.Type = "Deposit";
            acc.Percentage = 3.5;
            acc.EndDate = new System.DateTime(2014, 06, 06);
            acc.StartDate = new System.DateTime(2014, 01, 01);
            list.Add(acc);

            AccountDetails acc2 = new AccountDetails();
            acc2.Id = Guid.NewGuid();
            acc2.ClientId = ClientId;
            acc2.AccountNumber = "22 3333 4455 5555 5566 7777 8888";
            acc2.Money = 654321;
            acc2.Type = "Deposit";
            acc2.Percentage = 5;
            acc2.EndDate = new System.DateTime(2015, 06, 06);
            acc2.StartDate = new System.DateTime(2013, 06, 06);
            list.Add(acc2);

            return list;
        }
    }
}