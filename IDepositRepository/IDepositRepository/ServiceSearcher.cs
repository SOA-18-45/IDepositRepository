using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using Contracts;

namespace IDepositService {
    class ServiceSearcher {
      /*  private IServiceRepository serviceRepository = null;
        private IClientRepository clientService = null;
        private IAccountRepository accountRepository = null;

/*        private Stopwatch clientRepositoryTimer;
        private Stopwatch accountRepositoryTimer;*//*

        public ServiceSearcher(IServiceRepository ServiceRepository)
        {
            serviceRepository = ServiceRepository;
        }

        public IClientRepository GetClientRepository()
        {
            if (clientService != null)
                return clientService;


                var cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(), serviceRepositoryAddr);
                serviceRepository = cf.CreateChannel();
                NetTcpBinding binding = new NetTcpBinding();
                ChannelFactory<IServiceRepository> cf = new ChannelFactory<IServiceRepository>(binding, new EndpointAddress(serviceRepositoryAddress));
                return cf.CreateChannel();
        }

        public IAccountRepository GetAccountRepository()
        {
            if (accountRepository != null)
                return accountRepository;

            string address = GetServiceRepository().GetServiceAddress("IAccountRepository");
            NetTcpBinding binding = new NetTcpBinding();
            ChannelFactory<IAccountRepository> cf = new ChannelFactory<IAccountRepository>(binding, new EndpointAddress(address));
            return cf.CreateChannel();
        }*/
    }
}
