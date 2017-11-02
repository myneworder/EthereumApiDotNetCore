﻿using Core;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.New
{
    public interface IOwnerService
    {
        Task<IEnumerable<IOwner>> GetAll();
        Task AddOwners(IEnumerable<IOwner> owners);
        Task RemoveOwners(IEnumerable<IOwner> owners);
    }

    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IOwnerBlockchainService _ownerBlockchainService;
        private readonly IEthereumTransactionService _ethereumTransactionService;

        public OwnerService(IOwnerRepository ownerRepository,
            IOwnerBlockchainService ownerBlockchainService,
            IEthereumTransactionService ethereumTransactionService)
        {
            _ownerRepository = ownerRepository;
            _ownerBlockchainService = ownerBlockchainService;
            _ethereumTransactionService = ethereumTransactionService;
        }

        public async Task<IEnumerable<IOwner>> GetAll()
        {
            IEnumerable<IOwner> owners = await _ownerRepository.GetAllAsync();

            return owners;
        }

        public async Task AddOwners(IEnumerable<IOwner> owners)
        {
            string transactionHash = await _ownerBlockchainService.AddOwnersToMainExchangeAsync(owners);
            while (!await _ethereumTransactionService.IsTransactionExecuted(transactionHash, Constants.GasForCoinTransaction))
            {
                await Task.Delay(300);
            }

            List<Task> wait = new List<Task>(owners.Count());
            foreach (var owner in owners)
            {
                var task = _ownerRepository.SaveAsync(owner);
                wait.Add(task);
            }

            await Task.WhenAll(wait);
        }

        public async Task RemoveOwners(IEnumerable<IOwner> owners)
        {
            string transactionHash = await _ownerBlockchainService.RemoveOwnersFromMainExchangeAsync(owners);
            while (!await _ethereumTransactionService.IsTransactionExecuted(transactionHash, Constants.GasForCoinTransaction))
            {
                await Task.Delay(300);
            }

            List<Task> wait = new List<Task>(owners.Count());
            foreach (var owner in owners)
            {
                var task = _ownerRepository.RemoveAsync(owner.Address);
                wait.Add(task);
            }

            await Task.WhenAll(wait);
        }
    }
}