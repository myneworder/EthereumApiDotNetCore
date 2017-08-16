// Code generated by Microsoft (R) AutoRest Code Generator 1.2.2.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace EthereumSamuraiApiCaller.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class Erc20BalanceResponse
    {
        /// <summary>
        /// Initializes a new instance of the Erc20BalanceResponse class.
        /// </summary>
        public Erc20BalanceResponse()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Erc20BalanceResponse class.
        /// </summary>
        public Erc20BalanceResponse(string address = default(string), string amount = default(string), string contract = default(string))
        {
            Address = address;
            Amount = amount;
            Contract = contract;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "contract")]
        public string Contract { get; set; }

    }
}
