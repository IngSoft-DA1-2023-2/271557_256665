using BusinessLogic;
using NuGet.Frameworks;
using System.Runtime.ExceptionServices;
using BusinessLogic.Transaction;
using BusinessLogic.Category_Components;
using BusinessLogic.Account_Components;
using System.Security.Principal;

namespace TestProject1;
[TestClass]
public class TransactionTests
{

    #region Initializing aspects

    private Transaction genericTransaction;
    private CreditCardAccount myCreditCardAccount;

    [TestInitialize]
    public void TestInitialize()
    {
        genericTransaction = new Transaction();
        genericTransaction.Title = "Title";
        genericTransaction.CreationDate = DateTime.Now.Date;
        genericTransaction.Amount = 100;
        genericTransaction.Currency = CurrencyEnum.UY;
        genericTransaction.Type = TypeEnum.Income;

        myCreditCardAccount = new CreditCardAccount();
        myCreditCardAccount.Name = "Scotia Credit Card";
        myCreditCardAccount.Currency = CurrencyEnum.UY;
        myCreditCardAccount.IssuingBank = "Santander";
        myCreditCardAccount.Last4Digits = "1233";
        myCreditCardAccount.AvailableCredit = 15000;
        myCreditCardAccount.ClosingDate = new DateTime(2023, 11, 1);
    }
    #endregion

    #region Validate Title Tests
    [TestMethod]
    public void GivenCorrectTitle_ShouldReturnTrue()
    {
        string titleToBeSet = "Title";
        Assert.AreEqual(titleToBeSet, genericTransaction.Title);
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateTransaction))]
    public void GivenEmptyTitle_ShouldReturnFalse()
    {
        genericTransaction.Title = "";
        genericTransaction.ValidateTitle();
    }

    #endregion

    #region Date Tests

    [TestMethod]

    public void GivenDateToSet_ShuldBeSetted()
    {
        DateTime dateToBeSetted = DateTime.Now.Date;
        Assert.AreEqual(genericTransaction.CreationDate, dateToBeSetted);
    }

    #endregion

    #region Amount Tests

    [TestMethod]
    public void GivenCorrectAmount_ShouldReturnTrue()
    {
        decimal amountToBeSet = 100;
        Assert.AreEqual(amountToBeSet, genericTransaction.Amount);
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateTransaction))]
    public void GivenWrongAmountToSet_ShouldThrowException()
    {
        genericTransaction.Amount = 0;
        genericTransaction.ValidateAmount();
    }

    #endregion

    #region Currency Tests

    [TestMethod]
    public void GivenCurrency_ShouldBelongToCurrencyEnum()
    {
        bool belongsToCurrencyEnum = Enum.IsDefined(typeof(CurrencyEnum), genericTransaction.Currency);
        Assert.IsTrue(belongsToCurrencyEnum);
    }

    #endregion

    #region Type Tests

    [TestMethod]
    public void GivenType_ShouldBelongToTypeEnum()
    {
        bool belongsToTypeEnum = Enum.IsDefined(typeof(TypeEnum), genericTransaction.Type);
        Assert.IsTrue(belongsToTypeEnum);
    }

    #endregion

    #region Account Tests

    [TestMethod]
    public void GivenMonetaryAccount_ShouldBeSetted()
    {
        MonetaryAccount myMonetaryAccount = new MonetaryAccount();
        myMonetaryAccount.Name = "Scotia Saving Bank";
        myMonetaryAccount.Currency = CurrencyEnum.UY;
        myMonetaryAccount.Ammount = 10;

        genericTransaction.Account = myMonetaryAccount;
        Assert.AreEqual(genericTransaction.Account, myMonetaryAccount);

    }

    [TestMethod]
    public void GivenCreditAccount_ShouldBeSetted()
    {
        genericTransaction.Account = myCreditCardAccount;

        Assert.AreEqual(genericTransaction.Account, myCreditCardAccount);
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateTransaction))]
    public void GivenCreditAccountWithTransactionTypeAsIncome_ShouldThrowException()
    {
        genericTransaction.Account = myCreditCardAccount;
        genericTransaction.ValidateAccount();
    }

    #endregion

    #region List of Categories Tests

    [TestMethod]
    public void GivenCorrectListToValidate_ShouldNotThrowException()
    {
        string name = "Business";
        StatusEnum status = StatusEnum.Enabled;
        TypeEnum type = TypeEnum.Income;
        Category categoryToBeAdded = new Category(name, status, type);

        List<Category> listToSet = new List<Category>();
        listToSet.Add(categoryToBeAdded);

        genericTransaction.MyCategories = listToSet;
        genericTransaction.ValidateListOfCategories();
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateTransaction))]
    public void GivenListWithDisabledCategory_ShouldThrowException()
    {
        string name = "Business One";
        StatusEnum status = StatusEnum.Enabled;
        TypeEnum type = TypeEnum.Income;
        Category categoryToBeAdded = new Category(name, status, type);

        string name2 = "Business Two";
        StatusEnum status2 = StatusEnum.Disabled;
        Category categoryToBeAdded2 = new Category(name2, status2, type);

        List<Category> listToSet = new List<Category>();
        listToSet.Add(categoryToBeAdded);
        listToSet.Add(categoryToBeAdded2);

        genericTransaction.MyCategories = listToSet;
        genericTransaction.ValidateListOfCategories();
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateTransaction))]
    public void GivenListWithTypeOutcomeInTransactionIncome_ShouldThrowException()
    {
        string name = "Business One";
        StatusEnum status = StatusEnum.Enabled;
        TypeEnum type = TypeEnum.Income;
        Category categoryToBeAdded = new Category(name, status, type);

        string name2 = "Food";
        StatusEnum status2 = StatusEnum.Enabled;
        TypeEnum type2 = TypeEnum.Outcome;
        Category categoryToBeAdded2 = new Category(name2, status2, type2);

        List<Category> listToSet = new List<Category>();
        listToSet.Add(categoryToBeAdded);
        listToSet.Add(categoryToBeAdded2);

        genericTransaction.MyCategories = listToSet;
        genericTransaction.ValidateListOfCategories();
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateTransaction))]
    public void GivenListWithTypeIncomeInTransactionOutcome_ShouldThrowException()
    {
        genericTransaction.Type = TypeEnum.Outcome;

        string name = "Business One";
        StatusEnum status = StatusEnum.Enabled;
        TypeEnum type = TypeEnum.Income;
        Category categoryToBeAdded = new Category(name, status, type);

        string name2 = "Food";
        StatusEnum status2 = StatusEnum.Enabled;
        TypeEnum type2 = TypeEnum.Outcome;
        Category categoryToBeAdded2 = new Category(name2, status2, type2);

        List<Category> listToSet = new List<Category>();
        listToSet.Add(categoryToBeAdded);
        listToSet.Add(categoryToBeAdded2);

        genericTransaction.MyCategories = listToSet;
        genericTransaction.ValidateListOfCategories();
    }
    #endregion


}