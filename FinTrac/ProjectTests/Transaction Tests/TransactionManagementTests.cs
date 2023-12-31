using BusinessLogic;
using NuGet.Frameworks;
using System.Runtime.ExceptionServices;
using BusinessLogic.Transaction_Components;
using BusinessLogic.Category_Components;
using BusinessLogic.Account_Components;
using BusinessLogic.User_Components;
using System.Security.Principal;
using System.Collections.Generic;
using BusinessLogic.Enums;

namespace TestProject1;
[TestClass]
public class TransactionManagementTests
{
    #region initializingAspects

    private User genericUser;
    private Category genericCategoryOutcome1;
    private Category genericCategoryOutcome2;
    private MonetaryAccount genericMonetaryAccount;
    private CreditCardAccount genericCreditAccount;
    private Transaction genericTransactionOutcome1;
    private Transaction genericTransactionOutcome2;

    [TestInitialize]
    public void TestInitialize()
    {
        genericUser = new User("Michael", "Santa", "michSanta@gmail.com", "AustinF2003", "NW 2nd Ave");

        genericCategoryOutcome1 = new Category("Clothes", StatusEnum.Enabled, TypeEnum.Outcome);

        genericCategoryOutcome2 = new Category("Food", StatusEnum.Enabled, TypeEnum.Outcome);

        genericUser.MyCategories.Add(genericCategoryOutcome1);
        genericUser.MyCategories.Add(genericCategoryOutcome2);

        genericMonetaryAccount = new MonetaryAccount("Saving Account", 1000, CurrencyEnum.UY, DateTime.Now);

        genericCreditAccount = new CreditCardAccount("Credit Account", CurrencyEnum.UY, DateTime.Now, "Brou", "1233", 1000, new DateTime(2024, 3, 19, 7, 0, 0));

        genericUser.AddMonetaryAccount(genericMonetaryAccount);
        genericUser.AddCreditAccount(genericCreditAccount);

        genericTransactionOutcome1 = new Transaction("Payment of Clothes", 200, DateTime.Now, CurrencyEnum.UY, TypeEnum.Outcome, genericCategoryOutcome1);

        genericTransactionOutcome2 = new Transaction("Payment of food", 400, DateTime.Now, CurrencyEnum.UY, TypeEnum.Outcome, genericCategoryOutcome2);
    }

    #endregion

    #region Add Transaction Tests

    [TestMethod]
    public void GivenTransactionToAddAccount_ShouldBeAdded()
    {
        genericUser.MyAccounts[0].AddTransaction(genericTransactionOutcome1);
        Transaction transactionAdded = genericUser.MyAccounts[0].MyTransactions[0];

        Assert.AreEqual(transactionAdded, genericTransactionOutcome1);
    }

    [TestMethod]
    public void GivenTransactionToAddToMonetaryAccount_ShouldModifyAccountAmountCorrectly()
    {
        decimal previousAccountAmount = genericMonetaryAccount.Amount;
        decimal costOfTransaction = genericTransactionOutcome1.Amount;
        genericUser.MyAccounts[0].UpdateAccountMoneyAfterAdd(genericTransactionOutcome1);
        genericUser.MyAccounts[0].AddTransaction(genericTransactionOutcome1);
        MonetaryAccount myMonetary = (MonetaryAccount)genericUser.MyAccounts[0];

        Assert.AreEqual(previousAccountAmount - costOfTransaction, myMonetary.Amount);
    }

    [TestMethod]
    public void GivenTransactionToAddToCreditAccount_ShouldModifyAccountAmountCorrectly()
    {
        decimal previousAccountCredit = genericCreditAccount.AvailableCredit;
        decimal costOfTransaction = genericTransactionOutcome2.Amount;
        genericUser.MyAccounts[1].UpdateAccountMoneyAfterAdd(genericTransactionOutcome2);
        genericUser.MyAccounts[1].AddTransaction(genericTransactionOutcome2);
        CreditCardAccount myCreditCard = (CreditCardAccount)genericUser.MyAccounts[1];

        Assert.AreEqual(previousAccountCredit - costOfTransaction, myCreditCard.AvailableCredit);
    }

    [TestMethod]
    public void GiventTransactionToAddToAccount_ShouldSetTransactionId()
    {
        int idShouldBe = 0;
        genericUser.MyAccounts[0].AddTransaction(genericTransactionOutcome1);
        int idSetted = genericUser.MyAccounts[0].MyTransactions[0].TransactionId;
        Assert.AreEqual(idShouldBe, idSetted);
    }

    #endregion

    #region Modify Transaction Tests

    [TestMethod]
    public void GivenTransactionToBeModified_ShouldBeModified()
    {
        genericUser.MyAccounts[1].UpdateAccountMoneyAfterAdd(genericTransactionOutcome2);
        genericUser.MyAccounts[1].AddTransaction(genericTransactionOutcome2);

        List<Category> modifiedListOfCategories = new List<Category>();

        modifiedListOfCategories.Add(genericCategoryOutcome1);

        Transaction modifiedTransaction = new Transaction("Payment of food", 100, DateTime.Now, CurrencyEnum.UY, TypeEnum.Outcome, genericCategoryOutcome1);

        modifiedTransaction.TransactionId = 0;

        genericUser.MyAccounts[1].ModifyTransaction(modifiedTransaction);

        Assert.AreEqual(modifiedTransaction.Amount, genericUser.MyAccounts[1].MyTransactions[0].Amount);
        Assert.AreEqual(modifiedTransaction.TransactionCategory, genericUser.MyAccounts[1].MyTransactions[0].TransactionCategory);
   
    }

    #endregion

    #region Get All lists test
    [TestMethod]
    public void GivenCallGet_ShouldReturnList()
    {
        genericUser.MyAccounts[0].UpdateAccountMoneyAfterAdd(genericTransactionOutcome2);
        genericUser.MyAccounts[0].AddTransaction(genericTransactionOutcome2);

        Assert.AreEqual(genericUser.MyAccounts[0].MyTransactions, genericMonetaryAccount.GetAllTransactions());
    }
    #endregion

    #region Delete Test

    [TestMethod]
    public void GivenTransactionToDelete_ShouldDeleteIt()
    {
        genericMonetaryAccount.AddTransaction(genericTransactionOutcome1);
        int amountOfTransactionsPreDelete = genericMonetaryAccount.MyTransactions.Count;

        genericMonetaryAccount.DeleteTransaction(genericTransactionOutcome1);
        int amountOfTransactionsPostDelete = genericMonetaryAccount.MyTransactions.Count;
        
        Assert.AreEqual(amountOfTransactionsPreDelete - 1,amountOfTransactionsPostDelete);
    }

    #endregion

}