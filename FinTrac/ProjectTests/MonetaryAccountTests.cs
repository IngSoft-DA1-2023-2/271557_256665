using BusinessLogic;
using BusinessLogic.User_Components;
using NuGet.Frameworks;
using System.Runtime.ExceptionServices;
using BusinessLogic.Account_Components;
using BusinessLogic.Transaction_Components;
using System.Security.Principal;
using BusinessLogic.Category_Components;

namespace TestProject1;
[TestClass]


public class MonetaryAccountTests
{
    #region Init
    private Account myAccount;
    private MonetaryAccount myMonetaryAccount;
    private Transaction genericTransaction;
    private Category genericCategory;


    [TestInitialize]
    public void TestInitialize()
    {
        myAccount = new MonetaryAccount();
        myMonetaryAccount = new MonetaryAccount();

        //myAccout is a polimorfed object
        myAccount.Name = "Scotia Saving Bank";
        myAccount.Currency = CurrencyEnum.UY;

        //monetaryAccount is a 100% object that references only to monetaryAccount
        myMonetaryAccount.Name = myAccount.Name;
        myMonetaryAccount.Currency = CurrencyEnum.UY;
        myMonetaryAccount.Amount = 10;

        genericCategory = new Category("Food", StatusEnum.Enabled, TypeEnum.Outcome);

        genericTransaction = new Transaction("Payment of food", 200, DateTime.Now, CurrencyEnum.UY, TypeEnum.Outcome, genericCategory);
    }

    #endregion

    #region Name
    [TestMethod]

    public void GivenCorrectMonetaryAccountName_ShouldItBeSet()
    {
        string name = "Santander Saving Bank";
        myAccount.Name = name;

        Assert.AreEqual(name, myAccount.Name);
    }


    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateAccount))]
    public void GivenEmptyMonetaryAccountName_ShouldReturnException()
    {
        string name = "";
        myMonetaryAccount.Name = name;

        myMonetaryAccount.ValidateAccount();
    }

    #endregion

    #region Ammount

    [TestMethod]
    public void GivenInitialAmmount_ShouldBeSetted()
    {
        int initialAmmount = 100;
        myMonetaryAccount.Amount = initialAmmount;

        Assert.AreEqual(myMonetaryAccount.Amount, initialAmmount);

    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateAccount))]

    public void GivenInitialNegativeAmmount_ShouldThrowException()
    {
        myMonetaryAccount.Amount = -1;
        myMonetaryAccount.ValidateMonetaryAccount();

    }

    #endregion

    #region Creation Date
    [TestMethod]
    public void MadeAnAccount_DateShouldBeActualDate()
    {
        DateTime actualDate = DateTime.Now.Date;

        Assert.AreEqual(myMonetaryAccount.CreationDate, actualDate);
    }
    #endregion

    [TestMethod]
    public void GivenTransaction_ShouldReturnAmountfterModifyCorrect()
    {
        myMonetaryAccount.UpdateAccountAfterModify(genericTransaction, 1000);
    }

    #region Currency

    [TestMethod]
    public void GivenCurrency_ShouldBelongToCurrencyEnum()
    {
        bool belongToEnum = Enum.IsDefined(typeof(CurrencyEnum), myMonetaryAccount.Currency);
        Assert.IsTrue(belongToEnum);

    }
    #endregion

    #region Constructor
    [TestMethod]
    public void CreationOfMonetaryAccount_ShouldBeValidated()
    {

        string nameToBeSetted = "Itau Saving Bank";
        int ammountToBeSetted = 100;
        CurrencyEnum currencyToBeSetted = CurrencyEnum.UY;
        DateTime creationDate = DateTime.Now.Date;

        MonetaryAccount monetaryAccountExample = new MonetaryAccount(nameToBeSetted, ammountToBeSetted, currencyToBeSetted, DateTime.Now);

        Assert.AreEqual(nameToBeSetted, monetaryAccountExample.Name);
        Assert.AreEqual(ammountToBeSetted, monetaryAccountExample.Amount);
        Assert.AreEqual(currencyToBeSetted, monetaryAccountExample.Currency);
        Assert.AreEqual(creationDate.Date, monetaryAccountExample.CreationDate.Date);
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateAccount))]

    public void GivenIncorrectValuesInMonetaryCreation_ShouldThrowException()
    {
        string nameToBeSetted = "";
        int ammountToBeSetted = 100;
        CurrencyEnum currencyToBeSetted = CurrencyEnum.UY;

        MonetaryAccount monetaryAccountExample = new MonetaryAccount(nameToBeSetted, ammountToBeSetted, currencyToBeSetted, DateTime.Now);

    }
    #endregion
}