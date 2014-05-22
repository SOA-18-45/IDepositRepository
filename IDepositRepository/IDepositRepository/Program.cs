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
<<<<<<< HEAD
using System.Timers;
using log4net;
using Contracts;
=======
using System.Guid;
>>>>>>> 1f645dba5e5925b2e59e28bd92936cbe9eb9580b

namespace IDepositService {
    class Program {
        private const string myAddress = "net.tcp://localhost:54321/IDepositRepository";
        private static IServiceRepository serviceRepository;

        static void Main(string[] args) {
            // string serviceRepositoryAddress = <adres z App.Config>
            // DepositRepository deposit = new DepositRepository(serviceRepositoryAddress);
            // stworzenie instancji loggera - z log4net
            // logowanie o stanie aplikacji
            // rejestrowanie usługi w ServiceRepository

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

                // Alive signal every 10 sec
                Timer amAlive = new Timer(1000 * 10);
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

<<<<<<< HEAD
    public class Logger {
        internal static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
=======
    [ServiceContract]
    public interface IDepositRepository {
        [OperationContract]
        Guid CreateDeposit(Guid ClientID, Guid AccountID, string DepositType, double InterestRate);
        [OperationContract]
        DepositDetails GetDepositDetails(Guid DepositID);
>>>>>>> 1f645dba5e5925b2e59e28bd92936cbe9eb9580b
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
<<<<<<< HEAD
            return null;
        }
    }
=======
            DepositDetails deposit = new DepositDetails();
            deposit.ClientID = DepositID;
            deposit.AccountID = DepositID;
            deposit.DepositID = DepositID;
            deposit.DepositType = "Time Deposit";
            deposit.InterestRate = 7.5;
            deposit.CreationDate = new DateTime(2014, 4, 24, 14, 0, 0);
            return deposit;
        }
    }

    [DataContract]
    public class DepositDetails {
        [DataMember]
        public Guid DepositID { get; set; }
        [DataMember]
        public Guid ClientID { get; set; }
        [DataMember]
        public Guid AccountID { get; set; }
        [DataMember]
        public string DepositType { get; set; } // Current Account, Time Deposit
        [DataMember]
        public double InterestRate { get; set; }
        [DataMember]
        public DateTime CreationDate { get; set; }
    }
>>>>>>> 1f645dba5e5925b2e59e28bd92936cbe9eb9580b
}