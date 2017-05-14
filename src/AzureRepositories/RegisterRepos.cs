﻿using System;
using AzureRepositories.Log;
using AzureRepositories.Repositories;
using Core;
using Core.Repositories;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using AzureStorage.Tables;
using AzureStorage.Queue;
using Common.Log;
using AzureStorage.Tables.Templates.Index;

namespace AzureRepositories
{
    public static class RegisterReposExt
    {
        public static void RegisterAzureLogs(this IServiceCollection services, IBaseSettings settings, string logPrefix)
        {
            var logToTable = new LogToTable(
                new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, Constants.StoragePrefix + logPrefix + "Error", null),
                new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, Constants.StoragePrefix + logPrefix + "Warning", null),
                new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, Constants.StoragePrefix + logPrefix + "Info", null));

            services.AddSingleton(logToTable);
            services.AddTransient<LogToConsole>();

            services.AddTransient<ILog, LogToTableAndConsole>();
        }

        public static void RegisterAzureStorages(this IServiceCollection services, IBaseSettings settings)
        {
            var blobStorage = new AzureStorage.Blob.AzureBlobStorage(settings.Db.DataConnString);
                services.AddSingleton<IEthereumContractRepository>(provider => new EthereumContractRepository(Constants.EthereumContractsBlob, blobStorage));

            services.AddSingleton<ITransferContractRepository>(provider => new TransferContractRepository(
                new AzureTableStorage<TransferContractEntity>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.TransferContractTable,
                provider.GetService<ILog>()),
                new AzureTableStorage<AzureIndex>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.TransferContractTable,
                provider.GetService<ILog>())));

            services.AddSingleton<IExternalTokenRepository>(provider => new ExternalTokenRepository(
                new AzureTableStorage<ExternalTokenEntity>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.ExternalTokenTable,
                    provider.GetService<ILog>())));

            services.AddSingleton<IUserPaymentHistoryRepository>(provider => new UserPaymentHistoryRepository(
                new AzureTableStorage<UserPaymentHistoryEntity>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.UserPaymentHistoryTable,
                    provider.GetService<ILog>())));

            services.AddSingleton<IUserPaymentRepository>(provider => new UserPaymentRepository());

            services.AddSingleton<IUserTransferWalletRepository>(provider => new UserTransferWalletRepository(
               new AzureTableStorage<UserTransferWalletEntity>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.UserTransferWalletTable,
                   provider.GetService<ILog>())
                   ));

            services.AddSingleton<IMonitoringRepository>(provider => new MonitoringRepository(
                new AzureTableStorage<MonitoringEntity>(settings.Db.SharedConnString, Constants.StoragePrefix + Constants.MonitoringTable,
                    provider.GetService<ILog>())
                    ));

            services.AddSingleton<IAppSettingsRepository>(provider => new AppSettingsRepository(
                new AzureTableStorage<AppSettingEntity>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.AppSettingsTable,
                    provider.GetService<ILog>())));

            services.AddSingleton<ICoinTransactionRepository>(provider => new CoinTransactionRepository(
                new AzureTableStorage<CoinTransactionEntity>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.TransactionsTable,
                    provider.GetService<ILog>())));

            services.AddSingleton<ICoinContractFilterRepository>(provider => new CoinContractFilterRepository(
                new AzureTableStorage<CoinContractFilterEntity>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.CoinFiltersTable,
                    provider.GetService<ILog>())));

            services.AddSingleton<ICoinRepository>((provider => new CoinRepository(
                new AzureTableStorage<CoinEntity>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.CoinTable
                    ,provider.GetService<ILog>())
                , new AzureTableStorage<AzureIndex>(settings.Db.DataConnString, Constants.StoragePrefix + Constants.CoinTableInedex
                   , provider.GetService<ILog>())) ));
        }

        public static void RegisterAzureQueues(this IServiceCollection services, IBaseSettings settings)
        {
            services.AddTransient<IQueueFactory, QueueFactory>();
            services.AddTransient<Func<string, IQueueExt>>(provider =>
            {
                return (x =>
                {
                    switch (x)
                    {
                        case Constants.TransferContractUserAssignmentQueueName:
                            return new AzureQueueExt(settings.Db.DataConnString, Constants.StoragePrefix + x);
                        case Constants.EthereumContractQueue:
                            return new AzureQueueExt(settings.Db.DataConnString, Constants.StoragePrefix + x);
                        case Constants.SlackNotifierQueue:
                            return new AzureQueueExt(settings.Db.SharedConnString, Constants.StoragePrefix + x);
                        case Constants.EthereumOutQueue:
                            return new AzureQueueExt(settings.Db.SharedTransactionConnString, Constants.StoragePrefix + x);
                        case Constants.EmailNotifierQueue:
                            return new AzureQueueExt(settings.Db.SharedConnString, Constants.StoragePrefix + x);
                        case Constants.ContractTransferQueue:
                            return new AzureQueueExt(settings.Db.DataConnString, Constants.StoragePrefix + x);
                        case Constants.TransactionMonitoringQueue:
                            return new AzureQueueExt(settings.Db.DataConnString, Constants.StoragePrefix + x);
                        case Constants.CoinTransactionQueue:
                            return new AzureQueueExt(settings.Db.EthereumHandlerConnString, Constants.StoragePrefix + x);
                        case Constants.CoinEventQueue:
                            return new AzureQueueExt(settings.Db.SharedTransactionConnString, Constants.StoragePrefix + x);
                        case Constants.UserContractManualQueue:
                            return new AzureQueueExt(settings.Db.DataConnString, Constants.StoragePrefix + x);
                        default:
                            throw new Exception("Queue is not registered");
                    }
                });
            });

        }
    }
}
