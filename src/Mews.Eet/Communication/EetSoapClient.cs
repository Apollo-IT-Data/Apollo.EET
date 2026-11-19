using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Mews.Eet.Dto;
using Mews.Eet.Dto.Wsdl;
using Mews.Eet.Events;

namespace Mews.Eet.Communication
{
    public class EetSoapClient
    {
        static EetSoapClient()
        {
            CryptoConfig.AddAlgorithm(typeof(RsaPkCs1Sha256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
        }

        public EetSoapClient(Certificate certificate, EetEnvironment environment, TimeSpan httpTimeout, EetLogger logger = null)
        {
            Environment = environment;
            var subdomain = environment == EetEnvironment.Production ? "" : "pg.";
            var endpointUri = new Uri($"https://{subdomain}trzbyeet.gov.cz/eet/services/EETServiceSOAP/v4");
            SoapClient = new SoapClient(endpointUri, certificate, httpTimeout, SignAlgorithm.Sha256, logger);
            Logger = logger;
            SoapClient.HttpRequestFinished += (sender, args) => HttpRequestFinished?.Invoke(this, args);
            SoapClient.XmlMessageSerialized += (sender, args) => XmlMessageSerialized?.Invoke(this, args);
        }

        public event EventHandler<HttpRequestFinishedEventArgs> HttpRequestFinished;

        public event EventHandler<XmlMessageSerializedEventArgs> XmlMessageSerialized;

        public EetEnvironment Environment { get; }

        private SoapClient SoapClient { get; }

        private EetLogger Logger { get; }

        public async Task<SendRevenueXmlResponse> SendRevenueAsync(SendRevenueXmlMessage message, CancellationToken ct = default)
        {
            return await SoapClient.SendAsync<SendRevenueXmlMessage, SendRevenueXmlResponse>(message, ct).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
