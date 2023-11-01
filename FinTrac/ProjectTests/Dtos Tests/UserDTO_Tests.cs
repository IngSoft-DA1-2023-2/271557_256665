using BusinessLogic.Dto_Components;
using BusinessLogic.User_Components;


namespace TestProject1;

[TestClass]
public class UserDTO_Tests
{
    #region TestInit

    private UserDTO UserDTO;

    [TestInitialize]
    public void Initialize()
    {
        UserDTO = new UserDTO();
    }

    #endregion
    
    #region FirstName

    [TestMethod]
    public void GivenFirstName_ShouldBeSetted()
    {
        string firstName = "Ignacio";
        UserDTO.FirstName = firstName;

        Assert.AreEqual(firstName, UserDTO.FirstName);
    }

    #endregion

    #region LastName

    [TestMethod]
    public void GivenLastName_ShouldBeSetted()
    {
        string lastName = "Quevedo";
        UserDTO.LastName = lastName;
        Assert.AreEqual(lastName, UserDTO.LastName);
    }

    #endregion

    #region Email

    [TestMethod]
    public void GivenEmail_ShouldBeSetted()
    {
        string email = "nachitoquevedo@gmail.com";
        UserDTO.Email = email;

        Assert.AreEqual(email, UserDTO.Email);
    }

    #endregion

    #region Address

    [TestMethod]
    public void GivenAddress_ShouldBeSetted()
    {
        string address = "Av Brasil 1215";

        UserDTO.Address = address;

        Assert.AreEqual(address, UserDTO.Address);
    }

    #endregion

    public void GivenValues_ShouldBeToCreateAUserDTO()
    {

        string firstName = "Gonzalo";
        string lastName = "Camejo";
        string email = "gonchi@gmail.com";
        string address = "";

        UserDTO genericUserDTO = new UserDTO(firstName, lastName, email, address);
        
        Assert.AreEqual(firstName,genericUserDTO.FirstName);
        Assert.AreEqual(lastName,genericUserDTO.LastName);
        Assert.AreEqual(email,genericUserDTO.Email);
        Assert.AreEqual(address,genericUserDTO.Address);
        

    }
}