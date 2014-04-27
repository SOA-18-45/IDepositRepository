using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using System.Guid;

namespace IDepositRepository {
    class Program {
        static void Main(string[] args) {
            // string serviceRepositoryAddress = <adres z App.Config>
            // DepositRepository deposit = new DepositRepository(serviceRepositoryAddress);
            // stworzenie instancji loggera - z log4net
            // logowanie o stanie aplikacji
            // rejestrowanie usługi w ServiceRepository
            Console.ReadLine();
        }
    }

    [ServiceContract]
    public interface IDepositRepository {
        [OperationContract]
        Guid CreateDeposit(Guid ClientID, Guid AccountID, string DepositType, double InterestRate);
        [OperationContract]
        DepositDetails GetDepositDetails(Guid DepositID);
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

    [DataContract(Namespace = "IDepositRepository")]
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
}