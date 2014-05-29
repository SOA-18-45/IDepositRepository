using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using System.Timers;
using log4net;
using NHibernate;
using Contracts;

namespace IDepositService {
    class Program {
        private const string myAddress = "net.tcp://localhost:50002/IDepositRepository";
        private static IServiceRepository serviceRepository;

        static void Main(string[] args) {
            // Logger Config
            log4net.Config.XmlConfigurator.Configure();
            Logger.logger.Info("Logger initialized.");

            ServiceHost sh = new ServiceHost(typeof(DepositRepository), new Uri[] { new Uri(myAddress) });

            try {
                // Starting service
                sh.AddServiceEndpoint(typeof(IDepositRepository), new NetTcpBinding(), myAddress);
                sh.Open();
                Logger.logger.Info("Service opened.");

                // Getting IServiceRepository address
                string serviceRepositoryAddress = System.Configuration.ConfigurationManager.AppSettings["serviceRepositoryAddress"];
                Logger.logger.Info("Got ServiceRepository address.");

                // Connecting to IServiceRepository
                ChannelFactory<IServiceRepository> cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), serviceRepositoryAddress);
                serviceRepository = cf.CreateChannel();
                Logger.logger.Info("Connected to IServiceRepository.");

                // Registering IDepositRepository
                serviceRepository.registerService("IDepositRepository", myAddress);
                Logger.logger.Info("Registered in IServiceRepository.");

                // Alive signal every 5 sec
                Timer amAlive = new Timer(1000 * 5);
                amAlive.Elapsed += new ElapsedEventHandler(Alive);
                amAlive.AutoReset = true;
                amAlive.Enabled = true;
                amAlive.Start();

                // Waiting for user to abort
                Console.ReadLine();

                // Unregistering IDepositRepository
                amAlive.Stop();
                serviceRepository.unregisterService("IDepositRepository");
                sh.Close();
                Logger.logger.Info("Connection to IServiceRepository shut down.");
            }

            catch (CommunicationException comEx) {
                Console.WriteLine("Communication error: " + comEx.Message);
                sh.Abort();
                Console.ReadLine();
            }
        }

        private static void Alive(object sender, EventArgs e) {
            serviceRepository.isAlive("IDepositRepository");
            Logger.logger.Info("Sent isAlive signal.");
        }


        public class Logger {
            internal static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        public class DepositRepository : IDepositRepository {
            IDatabase db = null;

            public Guid CreateDeposit(Guid ClientID, String AccountNumber, string DepositType, double InterestRate) {
                string accountRepositoryAddress = serviceRepository.getServiceAddress("IAccountRepository");
                ChannelFactory<IAccountRepository> cf = new ChannelFactory<IAccountRepository>(new NetTcpBinding(SecurityMode.None));
                IAccountRepository accountService = cf.CreateChannel();

                //check if ClientID exists
                List<AccountDetails> adlist = accountService.GetAccountsById(ClientID);
                if (adlist == null) {
                    Logger.logger.Error("Client does not exist.");
                    return Guid.Empty;
                }
                else {
                    //check if AccountID exists
                    Boolean aexists = false;
                    foreach (AccountDetails ad in adlist) {
                        if (ad.AccountNumber.Equals(AccountNumber)) {
                            aexists = true;
                        }
                    }
                    if (!aexists) {
                        Logger.logger.Error("Account does not exist or belongs to another client.");
                        return Guid.Empty;
                    }
                    Guid ret = db.SaveDeposit(ClientID, AccountNumber, DepositType, InterestRate);
                    return ret;
                }
            }

            public DepositDetails GetDepositDetails(Guid DepositID) {
                DepositDetails depo = db.GetDeposit(DepositID);
                return depo;
            }
        }

        public interface IDatabase {
            Guid SaveDeposit(Guid ClientID, string AccountNumber, String DepositType, double InterestRate);
            DepositDetails GetDeposit(Guid DepositId);
        }

        public class NHibernateDatabase : IDatabase {
            public Guid SaveDeposit(Guid ClientID, string AccountNumber, String DepositType, double InterestRate) {
                using (ISession session = NHibernateHelper.OpenSession()) {
                    using (ITransaction transaction = session.BeginTransaction()) {
                        DepositDetails deposit = new DepositDetails();
                        deposit.ClientID = ClientID;
                        deposit.DepositID = Guid.NewGuid();
                        deposit.AccountNumber = AccountNumber;
                        deposit.DepositType = DepositType;
                        deposit.InterestRate = InterestRate;
                        deposit.CreationDate = DateTime.Now;

                        session.Save(deposit);
                        transaction.Commit();
                        Logger.logger.Info("Saved details of deposit.");
                        return deposit.DepositID;
                    }
                }
            }
            public DepositDetails GetDeposit(Guid DepositID) {
                using (ISession session = NHibernateHelper.OpenSession()) {
                    DepositDetails found = session.QueryOver<DepositDetails>().Where(x => x.DepositID == DepositID).SingleOrDefault();

                    if (found != null) {
                        Logger.logger.Info("Got details of deposit.");
                        return found;
                    }
                    else {
                        Logger.logger.Error("Could not get details of deposit.");
                        return null;
                    }
                }
            }
        }

        //public class MockDatabase : IDatabase {
        //    public Guid SaveDeposit(Guid ClientID, string AccountNumber, String DepositType, double InterestRate) {
        //        DepositDetails deposit = new DepositDetails();
        //        deposit.ClientID = ClientID;
        //        deposit.DepositID = Guid.NewGuid();
        //        deposit.AccountNumber = AccountNumber;
        //        deposit.DepositType = DepositType;
        //        deposit.InterestRate = InterestRate;
        //        deposit.CreationDate = DateTime.Now;

        //        Logger.logger.Info("Saved details of deposit.");
        //        return deposit.DepositID;
        //    }
        //    public DepositDetails GetDeposit(Guid DepositID) {
        //        DepositDetails deposit = new DepositDetails();
        //        deposit.ClientID = Guid.NewGuid();
        //        deposit.DepositID = Guid.NewGuid();
        //        deposit.AccountNumber = "88 7777 6666 5555 4444 3333 2222";
        //        deposit.DepositType = "TimeDeposit";
        //        deposit.InterestRate = 4.4;
        //        deposit.CreationDate = DateTime.Now;

        //        return deposit;
        //    }
        //}
    }
}