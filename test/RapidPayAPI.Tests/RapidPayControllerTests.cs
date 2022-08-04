using System;
using System.Collections.Generic;
using Moq;
using AutoMapper;
using RapidPayAPI.Models;
using Xunit;
using RapidPayAPI.Controllers;
using RapidPayAPI.Data;
using RapidPayAPI.Profiles;
//using Microsoft.AspNetCore.Mvc;
using RapidPayAPI.Dtos;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Serialization;
using RapidPayAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace RapidPayAPI.Tests
{
    public class RapidPayControllerTests : IDisposable
    {
        Mock<ILogger<UFEService>> ufeServiceLogger;
        Mock<IRapidPayRepo> mockRepo;
        BankAccountProfile realProfile;
        MapperConfiguration configuration;
        IMapper mapper;
        Mock<UFEService> mockUfeService;

        Mock<ILogger<RapidPayController>> mockLogger;

        public RapidPayControllerTests()
        {
            mockRepo = new Mock<IRapidPayRepo>();
            mockLogger = new Mock<ILogger<RapidPayController>>();
            realProfile = new BankAccountProfile();
            configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            mapper = new Mapper(configuration);
            ufeServiceLogger = new Mock<ILogger<UFEService>>();
            mockUfeService = new Mock<UFEService>(ufeServiceLogger.Object);
        }

        public void Dispose()
        {
            mockRepo = null;
            mapper = null;
            configuration = null;
            realProfile = null;
        }

         //**************************************************
        //*
        //GET   /api/rapidpay/GetAllBankAccounts Unit Tests
        //*
        //**************************************************

        //TEST 1.1
        [Fact]
        public void GetAllBankAccounts_ReturnsZeroResources_WhenDBIsEmpty()
        {
            //Arrange 
            mockRepo.Setup(repo =>
              repo.GetAllBankAccounts()).Returns(GetBankAccounts(0));

            var controller = new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.GetAllBankAccounts();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result.Result);
        }

        //TEST 1.2
        [Fact]
        public void GetAllBankAccounts_ReturnsOneResource_WhenDBHasOneResource()
        {
            //Arrange 
            mockRepo.Setup(repo =>
              repo.GetAllBankAccounts()).Returns(GetBankAccounts(1));

            var controller = new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.GetAllBankAccounts();

            //Assert
            var okResult = result.Result.Result as OkObjectResult;

            var bankAccounts = okResult.Value as List<BankAccountReadDto>;

            Assert.Single(bankAccounts);
        }

        //TEST 1.3
        [Fact]
        public void GetAllBankAccounts_Returns200OK_WhenDBHasOneResource()
        {
            //Arrange 
            mockRepo.Setup(repo =>
              repo.GetAllBankAccounts()).Returns(GetBankAccounts(1));

            var controller = new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.GetAllBankAccounts();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result.Result);

        }

        

        // //**************************************************
        // //*
        // //GET   /api/RapidPay/GetBankAccountById/{bankaccountid}	 Unit Tests
        // //*
        // //**************************************************

        //TEST 2.1        
        [Fact]
        public void GetBankAccountByID_Returns404NotFound_WhenNonExistentIDProvided()
        {
            //Arrange 
            mockRepo.Setup(repo =>
              repo.GetBankAccountById(0)).Returns(() => null);

            var controller = new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.GetBankAccountById(1).Result;

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        //TEST 2.2
        [Fact]
        public void GetBankAccountByID_Returns200OK__WhenValidIDProvided()
        {
            //Arrange 
            mockRepo.Setup(repo =>
              repo.GetBankAccountById(1)).Returns(new BankAccount { 
                Id = 1, 
                  Bank = new Bank{Id=0, Name="Test Bank"},
                         FirstName = "Steve",
                         LastName = "Jobs",
                         Balance = 60,
                         CardNumber = "4111111111111113",
                         AccountType = AccountType.RecurringDeposit
                }
                );

            var controller =new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.GetBankAccountById(1).Result;

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        //TEST 2.3
        [Fact]
        public void GetBankAccountByID_ReturnsCorrectResouceType_WhenValidIDProvided()
        {
            //Arrange 
            mockRepo.Setup(repo =>
              repo.GetBankAccountById(1)).Returns(new BankAccount { 
                Id = 1, 
                  Bank = new Bank{Id=0, Name="Test Bank"},
                         FirstName = "Steve",
                         LastName = "Jobs",
                         Balance = 60,
                         CardNumber = "4111111111111113",
                         AccountType = AccountType.RecurringDeposit
                });

            var controller = new  RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.GetBankAccountById(1);

            //Assert
            Assert.IsNotType<ActionResult<BankAccount>>(result);
        }

        //**************************************************
        //*
        //POST   /api/RapidPay/CreateBankAccount Unit Tests
        //*
        //**************************************************

        //TEST 3.1
        [Fact]
        public void CreateBankAccount_ReturnsCorrectResourceType_WhenValidObjectSubmitted()
        {
            //Arrange 
             mockRepo.Setup(repo =>
              repo.GetBankAccountById(1)).Returns(new BankAccount { 
                Id = 1, 
                  Bank = new Bank{Id=0, Name="Test Bank"},
                         FirstName = "Steve",
                         LastName = "Jobs",
                         Balance = 60,
                         CardNumber = "4111111111111113",
                         AccountType = AccountType.RecurringDeposit
                });

            var controller = new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.CreateBankAccount(new BankAccountCreateDto { }).Result;

            //Assert
            Assert.IsType<ActionResult<BankAccountReadDto>>(result);
        }

        //TEST 3.2
        [Fact]
        public void CreateBankAccount_Returns201Created_WhenValidObjectSubmitted()
        {
            //Arrange 
               mockRepo.Setup(repo =>
              repo.GetBankAccountById(1)).Returns(new BankAccount { 
                Id = 1, 
                  Bank = new Bank{Id=0, Name="Test Bank"},
                         FirstName = "Steve",
                         LastName = "Jobs",
                         Balance = 60,
                         CardNumber = "4111111111111113",
                         AccountType = AccountType.RecurringDeposit
                });

            var controller = new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.CreateBankAccount(new BankAccountCreateDto { }).Result;

            //Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }


        //**************************************************
        //*
        //PUT   /api/RapidPay/Pay/{bankaccountid} Unit Tests
        //*
        //**************************************************

        //TEST 4.1
        [Fact]
        public void Pay_Returns204NoContent_WhenValidObjectSubmitted()
        {
            //Arrange 
            mockRepo.Setup(repo =>
              repo.GetBankAccountById(1)).Returns(new BankAccount { 
                Id = 1, 
                  Bank = new Bank{Id=0, Name="Test Bank"},
                         FirstName = "Steve",
                         LastName = "Jobs",
                         Balance = 60,
                         CardNumber = "4111111111111113",
                         AccountType = AccountType.RecurringDeposit
                });

            var controller = new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.Pay(1, new BankAccountPayDto { PayType = PayType.Deposit}).Result;

            //Assert
            Assert.IsType<NoContentResult>(result);
        }


        //TEST 4.2
        [Fact]
        public void Pay_Returns404NotFound_WhenNonExistentResourceIDSubmitted()
        {
            //Arrange 
          mockRepo.Setup(repo =>
              repo.GetBankAccountById(1)).Returns(new BankAccount { 
                Id = 1, 
                  Bank = new Bank{Id=0, Name="Test Bank"},
                         FirstName = "Steve",
                         LastName = "Jobs",
                         Balance = 60,
                         CardNumber = "4111111111111113",
                         AccountType = AccountType.RecurringDeposit
                });

            var controller = new RapidPayController(mockRepo.Object, mapper,mockLogger.Object, mockUfeService.Object);

            //Act
            var result = controller.Pay(0, new BankAccountPayDto { PayType = PayType.Deposit}).Result;

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


       

     

        //**************************************************
        //*
        //Private Support Methods
        //*
        //**************************************************




        private List<BankAccount> GetBankAccounts(int num)
        {
            var bankAccounts = new List<BankAccount>();
            if (num > 0)
            {
                
                bankAccounts.Add(
                    new BankAccount()
                     {
                         Id = 0,
                         Bank = new Bank{Id=0, Name="Test Bank"},
                         FirstName = "Steve",
                         LastName = "Jobs",
                         Balance = 60,
                         CardNumber = "4111111111111113",
                         AccountType = AccountType.RecurringDeposit
                     }
                );
            }
            return bankAccounts;
        }
        private List<Bank> GetBanks(int num)
        {
            var banks = new List<Bank>();
            if (num > 0)
            {
                
                banks.Add(
                    new Bank{Id=0, Name="Test Bank"}
                );
            }
            return banks;
        }
        
    }

}