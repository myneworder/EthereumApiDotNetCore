﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Core;
using Core.ContractEvents;
using Core.Repositories;
using Core.Settings;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Services
{
	public interface IContractService
	{
		Task<string> CreateContract(string abi, string bytecode, params object[] constructorParams);
		Task<HexBigInteger> GetFilterEventForUserContractPayment();
		Task<HexBigInteger> CreateFilterEventForUserContractPayment();
		Task<UserPaymentEvent[]> GetNewPaymentEvents(HexBigInteger filter);
		Task<string[]> GenerateUserContracts(int count = 10);
		Task<BigInteger> GetCurrentBlock();
		Task<List<T>> GetEvents<T>(string address, string abi, string eventName, HexBigInteger filter) where T : new();
		Task<HexBigInteger> CreateFilter(string address, string abi, string eventName);
	}

	public class ContractService : IContractService
	{
		private readonly IBaseSettings _settings;
		private readonly IAppSettingsRepository _appSettings;

		public ContractService(IBaseSettings settings, IAppSettingsRepository appSettings)
		{
			_settings = settings;
			_appSettings = appSettings;
		}

		public async Task<string> CreateContract(string abi, string bytecode, params object[] constructorParams)
		{
			var web3 = new Web3(_settings.EthereumUrl);

			// unlock account for 120 seconds
			await web3.Personal.UnlockAccount.SendRequestAsync(_settings.EthereumMainAccount, _settings.EthereumMainAccountPassword, new HexBigInteger(120));

			// deploy contract
			var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, bytecode, _settings.EthereumMainAccount, new HexBigInteger(1000000), constructorParams);

			// get contract transaction
			TransactionReceipt receipt;
			while ((receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash)) == null)
			{
				await Task.Delay(100);
			}

			// check if contract byte code is deployed
			var code = await web3.Eth.GetCode.SendRequestAsync(receipt.ContractAddress);

			if (string.IsNullOrWhiteSpace(code) || code == "0x")
			{
				throw new Exception("Code was not deployed correctly, verify bytecode or enough gas was to deploy the contract");
			}

			return receipt.ContractAddress;

		}

		public async Task<HexBigInteger> GetFilterEventForUserContractPayment()
		{
			var setting = await _appSettings.GetSettingAsync(Constants.EthereumFilterSettingKey);
			if (!string.IsNullOrWhiteSpace(setting))
				return new HexBigInteger(setting);

			return await CreateFilterEventForUserContractPayment();
		}

		public async Task<HexBigInteger> CreateFilterEventForUserContractPayment()
		{
			var filter = await CreateFilter(_settings.EthereumMainContractAddress, _settings.MainContract.Abi, Constants.UserPaymentEvent);
			//save filter for next launch
			await _appSettings.SetSettingAsync(Constants.EthereumFilterSettingKey, filter.HexValue);
			return filter;
		}

		public async Task<HexBigInteger> CreateFilter(string address, string abi, string eventName)
		{
			var contract = new Web3(_settings.EthereumUrl).Eth.GetContract(abi, address);
			var filter = await contract.GetEvent(eventName).CreateFilterAsync();
			return filter;
		}

		public async Task<List<T>> GetEvents<T>(string address, string abi, string eventName, HexBigInteger filter) where T : new()
		{
			var web3 = new Web3(_settings.EthereumUrl);
			var contract = web3.Eth.GetContract(abi, address);
			var ev = contract.GetEvent(eventName);
			var events = await ev.GetFilterChanges<T>(filter);
			if (events == null) return new List<T>();
			// group by because of block chain reconstructions
			return events.GroupBy(o => new { o.Log.Address, o.Log.Data }).Select(o => o.First()).Select(o => o.Event).ToList();
		}


		public async Task<UserPaymentEvent[]> GetNewPaymentEvents(HexBigInteger filter)
		{
			return (await GetEvents<UserPaymentEvent>(_settings.EthereumMainContractAddress, _settings.MainContract.Abi, Constants.UserPaymentEvent, filter)).ToArray();
		}

		public async Task<string[]> GenerateUserContracts(int count = 10)
		{
			var web3 = new Web3(_settings.EthereumUrl);

			// unlock account for 120 seconds
			await web3.Personal.UnlockAccount.SendRequestAsync(_settings.EthereumMainAccount, _settings.EthereumMainAccountPassword, new HexBigInteger(120));

			var transactionHashList = new List<string>();

			// sends <count> contracts
			for (var i = 0; i < count; i++)
			{
				// deploy contract (pass mainContractAddress to contract contructor)
				var transactionHash =
					await
						web3.Eth.DeployContract.SendRequestAsync(_settings.UserContract.Abi, _settings.UserContract.ByteCode,
							_settings.EthereumMainAccount, new HexBigInteger(500000), _settings.EthereumMainContractAddress);

				transactionHashList.Add(transactionHash);
			}

			// wait for all <count> contracts transactions
			var contractList = new List<string>();
			for (var i = 0; i < count; i++)
			{
				// get contract transaction
				TransactionReceipt receipt;
				while ((receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHashList[i])) == null)
				{
					await Task.Delay(100);
				}

				// check if contract byte code is deployed
				var code = await web3.Eth.GetCode.SendRequestAsync(receipt.ContractAddress);

				if (string.IsNullOrWhiteSpace(code) || code == "0x")
				{
					throw new Exception("Code was not deployed correctly, verify bytecode or enough gas was to deploy the contract");
				}

				contractList.Add(receipt.ContractAddress);
			}

			return contractList.ToArray();
		}

		public async Task<BigInteger> GetCurrentBlock()
		{
			var web3 = new Web3(_settings.EthereumUrl);
			var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
			return blockNumber.Value;
		}

	}
}
