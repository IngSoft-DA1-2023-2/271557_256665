using System.Net.Mime;
using BusinessLogic.Account_Components;
using BusinessLogic.Category_Components;
using BusinessLogic.Dtos_Components;
using BusinessLogic.Enums;
using BusinessLogic.Transaction_Components;
using BusinessLogic.User_Components;
using DataManagers;


namespace DataManagersTests
{
    [TestClass]
    public class HelperTests
    {
        #region Initialize

        private UserRepositorySql _userRepo;
        private SqlContext _testDb;
        private readonly IAppContextFactory _contextFactory = new InMemoryAppContextFactory();
        private User _genericUser;
        private UserDTO _genericUserDTO;
        private UserLoginDTO _genericUserLoginDTO;


        [TestInitialize]
        public void TestInitialize()
        {
            _testDb = _contextFactory.CreateDbContext();
            _userRepo = new UserRepositorySql(_testDb);
            _genericUser = new User("Jhon", "Sans", "jhonny@gmail.com", "Jhooony12345", "");
            _genericUserDTO = new UserDTO("Jhon", "Sans", "jhonny@gmail.com", "Jhooony12345", "");
            _genericUserLoginDTO = new UserLoginDTO(1, "jhonny@gmail.com", "Jhooony12345");
        }

        #endregion

        #region Cleanup

        [TestCleanup]
        public void CleanUp()
        {
            _testDb.Database.EnsureDeleted();
        }

        #endregion

        [TestMethod]
        public void GivenTwoSimplePropertiesThatAreEqual_AreTheSameObject_ShouldReturnTrue()
        {
            CategoryDTO categoryDto = new CategoryDTO("Food", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);
            CategoryDTO categoryDto2 = new CategoryDTO("Food", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);


            Assert.IsTrue(Helper.AreTheSameObject(categoryDto.Name, categoryDto2.Name));
        }

        [TestMethod]
        public void GivenTwoSimplePropertiesThatAreNotEqual_AreTheSameObject_ShouldReturnFalse()
        {
            CategoryDTO categoryDto = new CategoryDTO("Food", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);
            CategoryDTO categoryDto2 = new CategoryDTO("Fsood", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);

            Assert.IsFalse(Helper.AreTheSameObject(categoryDto.Name, categoryDto2.Name));
        }

        [TestMethod]
        public void GivenTwoObjectsThatAreEqual_AreTheSameObject_ShouldReturnTrue()
        {
            CategoryDTO categoryDto = new CategoryDTO("Food", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);
            CategoryDTO categoryDto2 = new CategoryDTO("Food", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);

            List<CategoryDTO> categories = new List<CategoryDTO>();
            List<CategoryDTO> categories2 = new List<CategoryDTO>();

            categories.Add(categoryDto);
            categories.Add(categoryDto2);

            categories2.Add(categoryDto);
            categories2.Add(categoryDto2);

            GoalDTO goalDto = new GoalDTO("Goal", 200, CurrencyEnumDTO.UY, categories, 1);
            GoalDTO goalDto2 = new GoalDTO("Goal", 200, CurrencyEnumDTO.UY, categories2, 1);


            Assert.IsTrue(Helper.AreTheSameObject(goalDto.CategoriesOfGoalDTO[0], goalDto2.CategoriesOfGoalDTO[0]));
            Assert.IsTrue(Helper.AreTheSameObject(goalDto.CategoriesOfGoalDTO[1], goalDto2.CategoriesOfGoalDTO[1]));
        }

        [TestMethod]
        public void GivenTwoObjectsThatAreNotEqual_AreTheSameObject_ShouldReturnTrue()
        {
            CategoryDTO categoryDto = new CategoryDTO("Food", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);
            CategoryDTO categoryDto2 = new CategoryDTO("Foo2d", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);

            List<CategoryDTO> categories = new List<CategoryDTO>();
            List<CategoryDTO> categories2 = new List<CategoryDTO>();

            categories.Add(categoryDto);
            categories.Add(categoryDto2);

            categories2.Add(categoryDto);
            categories2.Add(categoryDto2);

            GoalDTO goalDto = new GoalDTO("Goal", 200, CurrencyEnumDTO.UY, categories, 1);
            GoalDTO goalDto2 = new GoalDTO("Goal", 200, CurrencyEnumDTO.UY, categories2, 1);


            Assert.IsFalse(Helper.AreTheSameObject(goalDto.CategoriesOfGoalDTO[0], goalDto2.CategoriesOfGoalDTO[1]));
            Assert.IsFalse(Helper.AreTheSameObject(goalDto.CategoriesOfGoalDTO[1], goalDto2.CategoriesOfGoalDTO[0]));
        }

        [TestMethod]
        public void GivenTwoObjectsThatAreTotallyDiferent_AreTheSameObject_ShouldReturnFalse()
        {
            CategoryDTO categoryDto = new CategoryDTO("Food", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);
            CategoryDTO categoryDto2 = new CategoryDTO("Foo2d", StatusEnumDTO.Enabled, TypeEnumDTO.Income, 1);

            List<CategoryDTO> categories = new List<CategoryDTO>();
            List<CategoryDTO> categories2 = new List<CategoryDTO>();

            categories.Add(categoryDto);
            categories.Add(categoryDto2);

            categories2.Add(categoryDto);

            GoalDTO goalDto = new GoalDTO("Goal", 200, CurrencyEnumDTO.UY, categories, 1);
            GoalDTO goalDto2 = new GoalDTO("Goal", 200, CurrencyEnumDTO.UY, categories2, 1);


            Assert.IsFalse(Helper.AreTheSameObject(goalDto, goalDto2));
        }
    }
}