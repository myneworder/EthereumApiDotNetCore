// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.EthereumCore.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class CreateAssetModel
    {
        /// <summary>
        /// Initializes a new instance of the CreateAssetModel class.
        /// </summary>
        public CreateAssetModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateAssetModel class.
        /// </summary>
        public CreateAssetModel(string blockchain = default(string), bool? containsEth = default(bool?), string externalTokenAddress = default(string), int? multiplier = default(int?), string name = default(string))
        {
            Blockchain = blockchain;
            ContainsEth = containsEth;
            ExternalTokenAddress = externalTokenAddress;
            Multiplier = multiplier;
            Name = name;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "blockchain")]
        public string Blockchain { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "containsEth")]
        public bool? ContainsEth { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "externalTokenAddress")]
        public string ExternalTokenAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "multiplier")]
        public int? Multiplier { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
}
