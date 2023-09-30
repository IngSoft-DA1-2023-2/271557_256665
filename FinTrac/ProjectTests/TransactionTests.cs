using BusinessLogic;
using NuGet.Frameworks;
using System.Runtime.ExceptionServices;
using BusinessLogic.Transaction;
using BusinessLogic.Category;

namespace TestProject1;
[TestClass]
public class TransactionTests
{
    private Transaction genericTransaction;

    [TestInitialize]
    public void TestInitialize()
    {
        genericTransaction = new Transaction();
        genericTransaction.Title = "Title";
    }

    [TestMethod]
    public void GivenCorrectTitle_ShouldReturnTrue()
    {
        bool callToValidationTitleMethod = genericTransaction.ValidateTitle();
        Assert.AreEqual(true, callToValidationTitleMethod);
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateTransaction))]
    public void GivenIncorrectTitle_ShouldReturnFalse()
    {
        genericTransaction.Title = "";
    }




}