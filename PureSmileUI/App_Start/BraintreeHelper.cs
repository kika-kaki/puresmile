using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Braintree;

namespace PureSmileUI.App_Start
{
    public class BraintreeHelper
    {
        public string Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        private IBraintreeGateway BraintreeGateway { get; set; }

        public IBraintreeGateway CreateGateway()
        {
            Environment = ConfigurationManager.Enviroment;
            MerchantId = ConfigurationManager.MerchantId;
            PublicKey = ConfigurationManager.PublicKey;
            PrivateKey = ConfigurationManager.PrivateKey;

            return new BraintreeGateway(Environment, MerchantId, PublicKey, PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (BraintreeGateway == null)
            {
                BraintreeGateway = CreateGateway();
            }

            return BraintreeGateway;
        }
    }
}