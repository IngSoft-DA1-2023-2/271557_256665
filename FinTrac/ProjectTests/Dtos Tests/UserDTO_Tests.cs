using BusinessLogic.Dto_Components;


namespace TestProject1;
[TestClass]
public class UserDTO_Tests
{

    #region FirstName

    [TestMethod]
    public void GivenFirstName_ShouldBeSetted()
    {
        string firstName = "Ignacio";
        UserDTO userDTO = new UserDTO();
        userDTO.FirstName = firstName;
        
        Assert.AreEqual(firstName,userDTO.FirstName);
        
    }

    #endregion

    public void GivenLastName_ShouldBeSetted()
    {
        string lastName = "Quevedo";
        UserDTO userDto = new UserDTO();
        userDto.Lastname = lastName;
    }
}