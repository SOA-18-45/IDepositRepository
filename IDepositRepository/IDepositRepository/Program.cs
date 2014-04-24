using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;

namespace IDepositRepository {
    class Program {
        static void Main(string[] args) {
            DepositRepository deposit = new DepositRepository();
            ServiceHost sh = new ServiceHost(deposit, new Uri[] { new Uri("net.tcp://localhost:54321/IDepositRepository") });
            sh.AddServiceEndpoint(typeof(IDepositRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://localhost:54321/IDepositRepository");
            ServiceMetadataBehavior metadata = sh.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (metadata == null) {
                metadata = new ServiceMetadataBehavior();
                sh.Description.Behaviors.Add(metadata);
            }
            metadata.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            sh.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
            sh.Open();
            Console.ReadLine();
        }
    }

    [ServiceContract]
    public interface IDepositRepository {
        [OperationContract]
        long CreateDeposit(long ClientID, long AccountNumber, string DepositType, double InterestRate);
        [OperationContract]
        DepositDetails GetDepositDetails(long DepositID);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DepositRepository : IDepositRepository {
        public long CreateDeposit(long ClientID, long AccountNumber, string DepositType, double InterestRate) {
            // Wersja robocza
            DepositDetails deposit = new DepositDetails();
            deposit.ClientID = ClientID;
            deposit.DepositID = ClientID * 1000 + 321;
            deposit.AccountNumber = AccountNumber;
            deposit.DepositType = DepositType;
            deposit.InterestRate = InterestRate;
            deposit.CreationDate = DateTime.Now;
            return deposit.DepositID;
        }

        public DepositDetails GetDepositDetails(long DepositID) {
            // Wersja robocza
            DepositDetails deposit = new DepositDetails();
            deposit.ClientID = 123456789;
            deposit.AccountNumber = 1234567890123456789;
            deposit.DepositID = DepositID;
            deposit.DepositType = "Time Deposit";
            deposit.InterestRate = 7.5;
            deposit.CreationDate = new DateTime(2014, 4, 24, 14, 0, 0);
            return deposit;
        }
    }

    [DataContract(Namespace = "IDepositRepository")]
    public class DepositDetails {
        [DataMember]
        public long DepositID { get; set; }
        [DataMember]
        public long ClientID { get; set; }
        [DataMember]
        public long AccountNumber { get; set; }
        [DataMember]
        public string DepositType { get; set; } // Current Account, Time Deposit
        [DataMember]
        public double InterestRate { get; set; }
        [DataMember]
        public DateTime CreationDate { get; set; }
    }
}