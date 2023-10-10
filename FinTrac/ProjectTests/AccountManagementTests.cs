﻿using BusinessLogic.Account_Components;
using BusinessLogic.User_Components;
using BusinessLogic.Transaction_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Category_Components;

namespace BusinessLogicTests
{
    [TestClass]
    public class AccountManagementTests
    {

        #region Initializing Aspects
        private MonetaryAccount genericMonetaryAccount;
        private User genericUser;
        private CreditCardAccount genericCreditCardAccount;
        private int numberOfAccountsAddedBefore;

        [TestInitialize]
        public void TestInitialize()
        {
            genericMonetaryAccount = new MonetaryAccount("Itau Saving Bank", 5000, CurrencyEnum.UY, DateTime.Now);

            string firstName = "Austin";
            string lastName = "Ford";
            string email = "austinFord@gmail.com";
            string password = "AustinF2003";
            string address = "NW 2nd Ave";
            genericUser = new User(firstName, lastName, email, password, address);


            string nameToBeSetted = "Prex";
            CurrencyEnum currencyToBeSetted = CurrencyEnum.UY;
            string issuingBankToBeSetted = "Santander";
            string last4DigitsToBeSetted = "1234";
            int availableCreditToBeSetted = 20000;
            DateTime closingDateToBeSetted = new DateTime(2024, 9, 30);
            genericCreditCardAccount = new CreditCardAccount(nameToBeSetted, currencyToBeSetted, DateTime.Now, issuingBankToBeSetted, last4DigitsToBeSetted, availableCreditToBeSetted, closingDateToBeSetted);

            numberOfAccountsAddedBefore = genericUser.MyAccounts.Count;
        }

        #endregion

        #region Add Monetary Account

        [TestMethod]
        public void GivenCorrectMonetaryAccountToAdd_ShouldAddIt()
        {
            genericUser.AddMonetaryAccount(genericMonetaryAccount);

            Assert.AreEqual(numberOfAccountsAddedBefore + 1, genericUser.MyAccounts.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionAccountManagement))]

        public void GivenNameOfMonetaryAccountAlreadyAdded_ShouldThrowException()
        {
            genericUser.AddMonetaryAccount(genericMonetaryAccount);

            MonetaryAccount repitedNameAccount = new MonetaryAccount("Itau Saving Bank", 100, CurrencyEnum.UY, DateTime.Now);
            genericUser.AddMonetaryAccount(repitedNameAccount);
        }

        #endregion

        #region Add Credit Card Account

        [TestMethod]
        public void GivenCorrectCreditCardAccount_ShouldBeAdded()
        {
            genericUser.AddCreditAccount(genericCreditCardAccount);

            Assert.AreEqual(numberOfAccountsAddedBefore + 1, genericUser.MyAccounts.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionAccountManagement))]

        public void IssuingBankAndLast4DigitsOfCreditCardAccountAlreadyAdded_ShouldThrowException()
        {
            genericUser.AddCreditAccount(genericCreditCardAccount);

            int availableCreditNewCard = 66000;
            string equalName = "Prex";
            string issuingBankNewCard = "Santander";
            string last4DigitsNewCard = "1234";
            CurrencyEnum currencyNewCard = CurrencyEnum.UY;
            DateTime closingDateNewCard = new DateTime(2030, 6, 10);

            CreditCardAccount accountWithEqualValues = new CreditCardAccount(equalName, currencyNewCard, DateTime.Now, issuingBankNewCard, last4DigitsNewCard, availableCreditNewCard, closingDateNewCard);

            genericUser.AddCreditAccount(accountWithEqualValues);
        }
        #endregion

        #region Asignation of Id

        [TestMethod]
        public void GivenAccountToAdd_ShouldAssignId()
        {
            genericUser.AddMonetaryAccount(genericMonetaryAccount);
            Assert.AreEqual(genericUser.MyAccounts.Count - 1, genericMonetaryAccount.AccountId);

            genericUser.AddCreditAccount(genericCreditCardAccount);
            Assert.AreEqual(genericUser.MyAccounts.Count - 1, genericCreditCardAccount.AccountId);
        }

        #endregion

        #region Return Accounts

        [TestMethod]
        public void ShouldBePossibleToReturnList()
        {
            Assert.AreEqual(genericUser.MyAccounts, genericUser.GetAccounts());
        }
        #endregion

        #region Modify Aspects of Monetary Account

        [TestMethod]
        public void GivenMonetaryAccountToUpdate_ShouldBeModifiedCorrectly()
        {
            genericUser.AddMonetaryAccount(genericMonetaryAccount);

            MonetaryAccount accountToUpdate = new MonetaryAccount("Brou Saving Bank", 10000, CurrencyEnum.USA, DateTime.Now);

            genericUser.ModifyMonetaryAccount(accountToUpdate);

            Assert.AreEqual(accountToUpdate, genericUser.MyAccounts[genericMonetaryAccount.AccountId]);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionAccountManagement))]
        public void GivenMonetaryAccountToUpdateButNameIsAlreadyUsedByOtherAccount_ShouldThrowException()
        {
            genericUser.AddMonetaryAccount(genericMonetaryAccount);

            MonetaryAccount accountToUpdate = new MonetaryAccount("Brou Saving Bank", 10000, CurrencyEnum.USA, DateTime.Now);
            genericUser.AddMonetaryAccount(accountToUpdate);

            MonetaryAccount accountWithChanges = new MonetaryAccount("Itau Saving Bank", 10000, CurrencyEnum.USA, DateTime.Now);
            accountWithChanges.AccountId = accountToUpdate.AccountId;

            genericUser.ModifyMonetaryAccount(accountWithChanges);
        }

      [TestMethod]
      public void GivenMonetaryAccountToUpdate_InitialAmountShouldBeNotAffected()
        {
            genericUser.AddMonetaryAccount(genericMonetaryAccount);
            int idAccount = genericMonetaryAccount.AccountId;

            MonetaryAccount accountToUpdate = new MonetaryAccount("Brou Saving Bank", 2000, CurrencyEnum.USA, DateTime.Now);
            accountToUpdate.AccountId = idAccount;
            genericUser.ModifyMonetaryAccount(accountToUpdate);


            MonetaryAccount accountUpdated= (MonetaryAccount) genericUser.MyAccounts[idAccount];
            Assert.AreEqual(accountToUpdate.ReturnInitialAmount(), accountUpdated.ReturnInitialAmount());
        }

        #endregion

        #region Modify Aspects of Credit Card Account
        [TestMethod]
        public void GivenCorrectsValueToModifyOnCreditCardAccount_ShouldModifyIt()
        {
            genericUser.AddCreditAccount(genericCreditCardAccount);

            string nameToBeSetted = "Alpha Brou";
            CurrencyEnum currencyToBeSetted = CurrencyEnum.USA;
            string issuingBankToBeSetted = "Brou";
            string last4DigitsToBeSetted = "1234";
            int availableCreditToBeSetted = 10000;
            DateTime closingDateToBeSetted = new DateTime(2024, 9, 30);


            CreditCardAccount accountToUpdate = new CreditCardAccount(nameToBeSetted, currencyToBeSetted, DateTime.Now, issuingBankToBeSetted, last4DigitsToBeSetted, availableCreditToBeSetted, closingDateToBeSetted);

            accountToUpdate.AccountId = genericCreditCardAccount.AccountId;
            genericUser.ModifyCreditAccount(accountToUpdate);

            Assert.AreEqual(genericUser.GetAccounts()[genericCreditCardAccount.AccountId], accountToUpdate);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionAccountManagement))]

        public void ModifyingValueIssuingBankAndLast4DigitsToValuesThatAreAlreadyRegistered_ShouldThrowException()
        {
            genericUser.AddCreditAccount(genericCreditCardAccount);

            string nameToBeSetted = "Alpha Brou";
            CurrencyEnum currencyToBeSetted = CurrencyEnum.USA;
            string issuingBankToBeSetted = "Brou";
            string last4DigitsToBeSetted = "5432";
            int availableCreditToBeSetted = 10000;
            DateTime closingDateToBeSetted = new DateTime(2024, 9, 30);

            CreditCardAccount accountforUpdate = new CreditCardAccount(nameToBeSetted, currencyToBeSetted, DateTime.Now, issuingBankToBeSetted, last4DigitsToBeSetted, availableCreditToBeSetted, closingDateToBeSetted);
            genericUser.AddCreditAccount(accountforUpdate);

            issuingBankToBeSetted = "Santander";
            last4DigitsToBeSetted = "1234";
            CreditCardAccount accountUpdated = new CreditCardAccount(nameToBeSetted, currencyToBeSetted, DateTime.Now, issuingBankToBeSetted, last4DigitsToBeSetted, availableCreditToBeSetted, closingDateToBeSetted);
            accountUpdated.AccountId = accountforUpdate.AccountId;

            genericUser.ModifyCreditAccount(accountUpdated);

        }

        #endregion

        #region Delete Aspects of Account

        [TestMethod]
        public void GivenAccountToDelete_ShouldBeDeleted()
        {
            genericUser.AddMonetaryAccount(genericMonetaryAccount);
            int numberOfAccountsAdded = genericUser.GetAccounts().Count();

            genericUser.DeleteAccount(genericMonetaryAccount);

            Assert.AreEqual(numberOfAccountsAdded - 1, genericUser.GetAccounts().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionAccountManagement))]
        public void GivenAccountWithTransactionsToDelete_ShouldThrowException()
        {
            Category myCategory = new Category("Food", StatusEnum.Enabled, TypeEnum.Outcome);
            Transaction myTransaction = new Transaction("Payments", 200, DateTime.Now, CurrencyEnum.UY, TypeEnum.Outcome, myCategory);
            genericUser.AddMonetaryAccount(genericMonetaryAccount);

            genericMonetaryAccount.AddTransaction(myTransaction);

            genericUser.DeleteAccount(genericMonetaryAccount);
        }

        #endregion

    }
}
