// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.EthereumCore.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class TransferWithChangeModel
    {
        /// <summary>
        /// Initializes a new instance of the TransferWithChangeModel class.
        /// </summary>
        public TransferWithChangeModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the TransferWithChangeModel class.
        /// </summary>
        public TransferWithChangeModel(string change, string signFrom, System.Guid id, string coinAdapterAddress, string fromAddress, string toAddress, string amount, string signTo = default(string))
        {
            Change = change;
            SignFrom = signFrom;
            SignTo = signTo;
            Id = id;
            CoinAdapterAddress = coinAdapterAddress;
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Amount = amount;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "change")]
        public string Change { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "signFrom")]
        public string SignFrom { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "signTo")]
        public string SignTo { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "coinAdapterAddress")]
        public string CoinAdapterAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fromAddress")]
        public string FromAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "toAddress")]
        public string ToAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Change == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Change");
            }
            if (SignFrom == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SignFrom");
            }
            if (CoinAdapterAddress == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "CoinAdapterAddress");
            }
            if (FromAddress == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "FromAddress");
            }
            if (ToAddress == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ToAddress");
            }
            if (Amount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Amount");
            }
            if (Change != null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(Change, "^[1-9][0-9]*$"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "Change", "^[1-9][0-9]*$");
                }
            }
            if (Amount != null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(Amount, "^[1-9][0-9]*$"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "Amount", "^[1-9][0-9]*$");
                }
            }
        }
    }
}