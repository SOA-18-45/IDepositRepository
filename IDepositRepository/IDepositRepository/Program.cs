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
        private const string myAddress = "net.tcp://localhost:50007/IDepositRepository";
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
                ChannelFactory<IServiceRepository> cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(), serviceRepositoryAddress);
                serviceRepository = cf.CreateChannel();

                // Registering IDepositRepository
                serviceRepository.registerService("IDepositRepository", myAddress);
                Logger.logger.Info("Connected to IServiceRepository");

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
            Logger.logger.Info("Sent Alive() signal");
        }
    }

    public class Logger {
        internal static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DepositRepository : IDepositRepository {
        public Guid CreateDeposit(Guid ClientID, Guid AccountID, string DepositType, double InterestRate) {
            DepositDetails deposit = new DepositDetails();
            deposit.ClientID = ClientID;
            deposit.DepositID = Guid.NewGuid();
            deposit.AccountID = AccountID;
            deposit.DepositType = DepositType;
            deposit.InterestRate = InterestRate;
            deposit.CreationDate = DateTime.Now;
            return deposit.DepositID;
        }

        public DepositDetails GetDepositDetails(Guid DepositID) {
            using (ISession session = NHibernateHelper.OpenSession())
            {   DepositDetails found = session.QueryOver<DepositDetails>().Where(x => x.DepositID == DepositID).SingleOrDefault();

                if (found != null) {
                    DepositDetails depo = new DepositDetails();
                    depo.AccountID = found.AccountID;
                    depo.ClientID = found.ClientID;
                    depo.DepositID = found.DepositID;
                    depo.DepositType = found.DepositType;
                    depo.CreationDate = found.CreationDate;
                    depo.InterestRate = found.InterestRate;
                    Logger.logger.Error("Got details of deposit.");

                    return depo;

                } else {
                    Logger.logger.Error("Could not get details of deposit.");
                    return null;
                }
            }
        }
    }
}