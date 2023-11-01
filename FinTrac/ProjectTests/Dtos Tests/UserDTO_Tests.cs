using BusinessLogic.Dto_Components;
using BusinessLogic.User_Components;


namespace TestProject1;
[TestClass]
public class UserDTO_Tests
{

    private UserDTO UserDTO;

    [TestInitialize]
    public void Initialize()
    {
        UserDTO = new UserDTO();
    }
    
    #region FirstName

    [TestMethod]
    public void GivenFirstName_ShouldBeSetted()
    {
        string firstName = "Ignacio";
        UserDTO.FirstName = firstName;
        
        Assert.AreEqual(firstName,UserDTO.FirstName);
        
    }

    #endregion
    
    #region LastName

    [TestMethod]
    public void GivenLastName_ShouldBeSetted()
    {
        string lastName = "Quevedo";
        UserDTO.LastName = lastName;
        Assert.AreEqual(lastName,UserDTO.LastName);
    }

    #endregion
  
}