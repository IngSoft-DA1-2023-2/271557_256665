using BusinessLogic;
using NuGet.Frameworks;
using System.Runtime.ExceptionServices;

using BusinessLogic.Category;
namespace TestProject1;

[TestClass]
public class CategoryTests
{
    private Category genericCategory;

    [TestInitialize]
    public void TestInitialize()
    {
        genericCategory = new Category();
        genericCategory.Name = "Clothes";
        genericCategory.CreationDate = DateTime.Now;

    }


    [TestMethod]
    public void GivenCorrectName_ShouldReturnTrue()
    {
        Assert.AreEqual(true, genericCategory.ValidateCategory());
    }

    [TestMethod]
    [ExpectedException(typeof(ExceptionValidateCategory))]
    public void GivenEmptyName_ShouldThrowExceptione()
    {
        Category myCategory = new Category();
        myCategory.Name = "";
        Assert.AreEqual(true, myCategory.ValidateCategory());
    }

    [TestMethod]
    public void GivenCategory_ShouldReturnDate()
    {
        DateTime date;
        Assert.AreEqual(date, genericCategory.CreationDate);
    }



}
