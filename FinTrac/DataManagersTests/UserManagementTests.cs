using BusinessLogic.User_Components;
using DataManagers;
using DataManagers.UserManager;

namespace DataManagersTests
{
    [TestClass]
    public class UserManagementTests
    {
        #region Initialize

        private User genericUser;
        private UserManagement userManager;
        private Repository memoryDatabase;

        [TestInitialize]
        public void TestInitialize()
        {
            string firstName = "Michael";
            string lastName = "Santa";
            string email = "michSanta@gmail.com";
            string password = "AustinF2003";
            string address = "NW 2nd Ave";

            memoryDatabase = new Repository();
            userManager = new UserManagement(memoryDatabase);
            genericUser = new User(firstName, lastName, email, password, address);
            userManager.AddUser(genericUser);
        }

        #endregion

        #region Add User

        [TestMethod]

        public void GivenUserToAddToUsers_ValidationShouldReturnTrue()
        {
            string firstName = "Austin";
            string lastName = "Ford";
            string email = "austinFord@gmail.com";
            string password = "AustinF2003";
            string address = "NW 2nd Ave";
            User myUser = new User(firstName, lastName, email, password, address);
            Assert.AreEqual(true, userManager.ValidateAddUser(myUser));
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionUserManagement))]
        public void GivenAlreadyRegisteredEmail_ShouldReturnException()
        {

            string firstName2 = "Kent";
            string lastName2 = "Beck";
            string emailUsed = "michSanta@gmail.com";
            string password2 = "JohnBeck1961";
            string address2 = "NW 3rd Ave";
            User incorrectUser = new User(firstName2, lastName2, emailUsed, password2, address2);

            userManager.ValidateAddUser(incorrectUser);

        }

        [TestMethod]

        public void GivenUserNotRegistered_ShouldRegisterIt()
        {

            string firstName = "Franklin";
            string lastName = "Oddisey";
            string email = "FranklinOddisey@gmail.com";
            string password = "Frank2003!!";
            string address = "NW 5nd Ave";

            User myUser = new User(firstName, lastName, email, password, address);
            int numberOfUsersAddedBefore = memoryDatabase.Users.Count;

            userManager.AddUser(myUser);

            Assert.AreEqual(numberOfUsersAddedBefore + 1, memoryDatabase.Users.Count);

        }
        #endregion

        #region Login User

        [TestMethod]

        public void GivenUserAlreadyAdded_ShouldBePossibleToLogin()
        {
            Assert.AreEqual(true, userManager.Login(genericUser));

        }
        [TestMethod]
        [ExpectedException(typeof(ExceptionUserManagement))]
        public void GivenUserNotAdded_ShouldThrowException()
        {
            User myUser = new User("Ronnie", "Belgman", "ronnieBelgam@gmail.com", "RonnieMan2003", "asd");
            userManager.Login(myUser);
        }

        #endregion

        #region User Modify

        [TestMethod]
        public void GivenAspectsOfUserToChange_ShouldBeChanged()
        {

            string firstName = "Michael";
            string lastName = "Santa";
            string passwordModified = "MichaelSanta1234";
            string address = "NW 2nd Ave";

            User userUpdated = new User(firstName, lastName, genericUser.Email, passwordModified, address);

            userManager.ModifyUser(genericUser, userUpdated);

            Assert.AreEqual(firstName, genericUser.FirstName);
            Assert.AreEqual(lastName, genericUser.LastName);
            Assert.AreEqual(passwordModified, genericUser.Password);
            Assert.AreEqual(address, genericUser.Address);
        }


        #endregion

        #region User Id

        [TestMethod]
        public void GivenUserToAdd_ShouldGenerateAnId()
        {
            Assert.AreEqual(genericUser.UserId, memoryDatabase.Users.Count -1);
        }

        [TestMethod]

        public void GivenUser_ShouldBePossibleToFoundId()
        {
            Assert.AreEqual(genericUser.UserId, userManager.FindUserId(genericUser));
        }

        #endregion
    }
}