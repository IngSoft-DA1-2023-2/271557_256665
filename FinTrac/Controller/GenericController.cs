using BusinessLogic.Account_Components;
using BusinessLogic.Transaction_Components;
using BusinessLogic.Category_Components;
using BusinessLogic.Dtos_Components;
using BusinessLogic.Enums;
using BusinessLogic.Exceptions;
using BusinessLogic.ExchangeHistory_Components;
using BusinessLogic.Goal_Components;
using BusinessLogic.Transaction_Components;
using BusinessLogic.Report_Components;
using BusinessLogic.User_Components;
using Controller.IControllers;
using Controller.Mappers;
using DataManagers;
using Mappers;
using BusinessLogic.Enums;
using System.Collections.Generic;

namespace Controller
{
    public class GenericController : IUserController, ICategoryController, IGoalController, IExchangeHistoryController,
        IMonetaryAccount, ICreditAccount, ITransactionController, IReportController
    {
        #region Atributes

        private UserRepositorySql _userRepo;
        private User _userConnected { get; set; }

        #endregion

        #region Constructor

        public GenericController(UserRepositorySql userRepo)
        {
            _userRepo = userRepo;
        }

        #endregion

        #region User Repo

        public void SetUserConnected(int? userIdToConnect)
        {
            if (_userConnected == null || _userConnected.UserId != userIdToConnect)
            {
                _userConnected = _userRepo.FindUserInDb(userIdToConnect);
                _userRepo.InstanceLists(_userConnected);
            }
        }

        #region FindUser

        public UserDTO FindUser(int userId)
        {
            User userFound = _userRepo.FindUserInDb(userId);

            if (userFound != null)
            {
                return MapperUser.ToUserDTO(userFound);
            }
            else
            {
                throw new Exception("User not found.");
            }
        }

        #endregion

        #region Register

        public void RegisterUser(UserDTO userDtoToCreate)
        {
            try
            {
                _userRepo.EmailUsed(userDtoToCreate.Email);
                User userToAdd = MapperUser.ToUser(userDtoToCreate);

                _userRepo.Create(userToAdd);
            }
            catch (Exception ExceptionType) when (
                ExceptionType is ExceptionUserRepository ||
                ExceptionType is ExceptionMapper
            )
            {
                throw new Exception(ExceptionType.Message);
            }
        }

        public void PasswordMatch(string password, string passwordRepeated)
        {
            bool passwordMatch = Helper.AreTheSameObject(password, passwordRepeated);

            if (!passwordMatch)
            {
                throw new Exception("Passwords are not the same, try again.");
            }
        }

        #endregion

        #region UpdateUser

        public void UpdateUser(UserDTO userDtoUpdated)
        {
            int userConnectedId = _userRepo.GetUserViaEmail(userDtoUpdated.Email).UserId;
            userDtoUpdated.UserId = userConnectedId;

            SetUserConnected(userConnectedId);
            try
            {
                User userPreUpdates = new User(_userConnected.FirstName, _userConnected.LastName, _userConnected.Email,
                    _userConnected.Password, _userConnected.Address);

                User userWithUpdates = MapperUser.ToUser(userDtoUpdated);

                userPreUpdates.UserId = userWithUpdates.UserId;

                if (Helper.AreTheSameObject(userPreUpdates, userWithUpdates))
                {
                    throw new Exception("You need to change at least one value.");
                }

                _userRepo.Update(userWithUpdates);
            }
            catch (ExceptionMapper Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        #endregion

        #region LoginUser

        public bool LoginUser(UserLoginDTO userToLogin)
        {
            userToLogin.Email = userToLogin.Email.ToLower();
            bool logged = _userRepo.Login(userToLogin);

            if (!logged)
            {
                throw new Exception("User not exists, maybe you have an error on the email or password?");
            }

            return true;
        }

        #endregion

        #endregion

        #region Category Section

        public void CreateCategory(CategoryDTO dtoToAdd)
        {
            try
            {
                SetUserConnected(dtoToAdd.UserId);
                Category categoryToAdd = MapperCategory.ToCategory(dtoToAdd);
                categoryToAdd.CategoryId = 0;

                _userConnected.AddCategory(categoryToAdd);

                _userRepo.Update(_userConnected);
            }
            catch (ExceptionMapper Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        //Only controller 
        public Category FindCategoryInDb(CategoryDTO categoryToFind)
        {
            if (categoryToFind == null)
            {
                throw new Exception("Must select a category, otherwise there would not be changes");
            }

            SetUserConnected(categoryToFind.UserId);

            return SearchCategoryInDb(categoryToFind.CategoryId);
        }

        private Category SearchCategoryInDb(int idCategoryToFind)
        {
            foreach (var category in _userConnected.MyCategories)
            {
                if (category.CategoryId == idCategoryToFind)
                {
                    return category;
                }
            }

            throw new Exception("Category was not found, an error on index must be somewhere.");
        }

        //For UI
        public CategoryDTO FindCategory(int idCategoryToFind, int userId)
        {
            SetUserConnected(userId);
            Category categoryFound = SearchCategoryInDb(idCategoryToFind);
            CategoryDTO categoryFoundDTO = MapperCategory.ToCategoryDTO(categoryFound);

            return categoryFoundDTO;
        }


        public void UpdateCategory(CategoryDTO categoryDtoWithUpdates)
        {
            SetUserConnected(categoryDtoWithUpdates.UserId);
            Category categoryToUpd = MapperCategory.ToCategory(categoryDtoWithUpdates);
            Category categoryWithoutUpd = FindCategoryInDb(categoryDtoWithUpdates);

            categoryToUpd.CategoryUser = _userConnected;
            if (Helper.AreTheSameObject(categoryToUpd, categoryWithoutUpd))
            {
                throw new Exception("There are non existential changes, change at least one please.");
            }
            else
            {
                _userConnected.ModifyCategory(categoryToUpd);
                _userRepo.Update(_userConnected);
            }
        }

        public void DeleteCategory(CategoryDTO categoryDtoToDelete)
        {
            try
            {
                SetUserConnected(categoryDtoToDelete.UserId);
                _userConnected.DeleteCategory(FindCategoryInDb(categoryDtoToDelete));
                _userRepo.Update(_userConnected);
            }
            catch (ExceptionCategoryManagement Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        public List<CategoryDTO> GetAllCategories(int userConnectedId)
        {
            SetUserConnected(userConnectedId);
            List<CategoryDTO> listCategoryDTO = new List<CategoryDTO>();

            listCategoryDTO = MapperCategory.ToListOfCategoryDTO(_userConnected.MyCategories);

            return listCategoryDTO;
        }

        public List<Category> ReceiveCategoryListFromUser(int userConnectedId)
        {
            SetUserConnected(userConnectedId);
            return _userConnected.MyCategories;
        }

        #endregion

        #region Goal Section

        public void CreateGoal(GoalDTO goalDtoToCreate)
        {
            SetUserConnected(goalDtoToCreate.UserId);

            try
            {
                List<Category> categoriesOfGoal = SetListOfCategories(goalDtoToCreate);
                Goal goalToAdd = MapperGoal.ToGoal(goalDtoToCreate, categoriesOfGoal);
                goalToAdd.GoalId = 0;

                _userConnected.AddGoal(goalToAdd);
                _userRepo.Update(_userConnected);
            }
            catch (Exception ExceptionType) when (
                ExceptionType is ExceptionUserRepository ||
                ExceptionType is ExceptionMapper
            )
            {
                throw new Exception(ExceptionType.Message);
            }
        }

        public List<GoalDTO> GetAllGoalsDTO(int userConnectedId)
        {
            SetUserConnected(userConnectedId);
            List<GoalDTO> listGoalDTO = new List<GoalDTO>();

            listGoalDTO = MapperGoal.ToListOfGoalDTO(_userConnected.MyGoals);

            return listGoalDTO;
        }

        public List<Goal> ReceiveGoalListFromUser(int userConnectedId)
        {
            SetUserConnected(userConnectedId);
            return _userConnected.MyGoals;
        }

        public Goal FindGoalInDb(GoalDTO goalToFind)
        {
            SetUserConnected(goalToFind.UserId);

            foreach (Goal goal in _userConnected.MyGoals)
            {
                if (goal.GoalId == goalToFind.GoalId)
                {
                    return goal;
                }
            }

            throw new Exception("Category was not found, an error on index must be somewhere.");
        }

        private List<Category> SetListOfCategories(GoalDTO goalDtoToCreate)
        {
            List<Category> result = new List<Category>();
            foreach (CategoryDTO categoryDTO in goalDtoToCreate.CategoriesOfGoalDTO)
            {
                result.Add(FindCategoryInDb(categoryDTO));
            }

            return result;
        }

        #endregion

        #region Exchange History Section

        public void CreateExchangeHistory(ExchangeHistoryDTO exchangeDTO)
        {
            try
            {
                SetUserConnected(exchangeDTO.UserId);

                ExchangeHistory exchangeHistoryToCreate = MapperExchangeHistory.ToExchangeHistory(exchangeDTO);
                exchangeHistoryToCreate.ExchangeHistoryId = 0;
                _userConnected.AddExchangeHistory(exchangeHistoryToCreate);
                _userRepo.Update(_userConnected);
            }
            catch (ExceptionMapper Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        public ExchangeHistoryDTO FindExchangeHistory(int IdOfExchangeToFound, int idUserConnected)
        {
            SetUserConnected(idUserConnected);

            ExchangeHistoryDTO exchangeHistoryDTOFound =
                MapperExchangeHistory.ToExchangeHistoryDTO(searchInDbForAnExchange(IdOfExchangeToFound));

            return exchangeHistoryDTOFound;
        }


        #region ExchangeHistory Find method specifically for controller section.

        //This method will only be used in the controller section. Is necessary for some methods like update,delete,etc
        public ExchangeHistory FindExchangeHistoryInDB(ExchangeHistoryDTO exchangeToFound)
        {
            SetUserConnected(exchangeToFound.UserId);
            return searchInDbForAnExchange(exchangeToFound.ExchangeHistoryId);
        }

        private ExchangeHistory searchInDbForAnExchange(int idOfExchangeToSearch)
        {
            foreach (var exchangeHistory in _userConnected.MyExchangesHistory)
            {
                if (exchangeHistory.ExchangeHistoryId == idOfExchangeToSearch)
                {
                    {
                        return exchangeHistory;
                    }
                }
            }

            throw new Exception("Exchange History was not found, an error on index must be somewhere.");
        }

        #endregion


        public void UpdateExchangeHistory(ExchangeHistoryDTO dtoWithUpdates)
        {
            try
            {
                SetUserConnected(dtoWithUpdates.UserId);

                ExchangeHistory exchangeHistoryToUpdate = FindExchangeHistoryInDB(dtoWithUpdates);
                exchangeHistoryToUpdate.ValidateApplianceExchangeOnTransaction();

                ExchangeHistory exchangeHistoryWithUpdates = MapperExchangeHistory.ToExchangeHistory(dtoWithUpdates);
                _userConnected.ModifyExchangeHistory(exchangeHistoryWithUpdates);
                _userRepo.Update(_userConnected);
            }

            catch (Exception ExceptionType)
                when (
                    ExceptionType is ExceptionExchangeHistoryManagement or ExceptionMapper
                )
            {
                throw new Exception(ExceptionType.Message);
            }
        }

        public void DeleteExchangeHistory(ExchangeHistoryDTO dtoToDelete)
        {
            try
            {
                SetUserConnected(dtoToDelete.UserId);
                ExchangeHistory exchangeHistoryToDelete = FindExchangeHistoryInDB(dtoToDelete);
                exchangeHistoryToDelete.ValidateApplianceExchangeOnTransaction();
                _userConnected.DeleteExchangeHistory(exchangeHistoryToDelete);

                _userRepo.UpdateDbWhenDeleting(_userConnected, exchangeHistoryToDelete);
            }
            catch (Exception ExceptionType)
                when (
                    ExceptionType is ExceptionExchangeHistoryManagement or ExceptionMapper
                )
            {
                throw new Exception(ExceptionType.Message);
            }
        }

        public List<ExchangeHistoryDTO> GetAllExchangeHistories(int userConnectedId)
        {
            SetUserConnected(userConnectedId);
            return MapperExchangeHistory.ToListOfExchangeHistoryDTO(_userConnected.MyExchangesHistory);
        }

        #endregion

        #region Monetary Account section

        public void CreateMonetaryAccount(MonetaryAccountDTO monetAccountDTOToAdd)
        {
            try
            {
                SetUserConnected(monetAccountDTOToAdd.UserId);
                MonetaryAccount monetAccountToAdd = MapperMonetaryAccount.ToMonetaryAccount(monetAccountDTOToAdd);
                monetAccountToAdd.AccountId = 0;

                _userConnected.AddMonetaryAccount(monetAccountToAdd);

                _userRepo.Update(_userConnected);
            }
            catch (ExceptionMapper Exception)
            {
                throw new Exception(Exception.Message);
            }
        }


        public MonetaryAccountDTO FindMonetaryAccount(int idMonetToFind, int userId)
        {
            SetUserConnected(userId);
            MonetaryAccount monetAccountFound = (MonetaryAccount)FindAccountByIdInDb(idMonetToFind, userId);
            MonetaryAccountDTO monetAccountFoundDTO = MapperMonetaryAccount.ToMonetaryAccountDTO(monetAccountFound);

            return monetAccountFoundDTO;
        }

        public MonetaryAccount FindMonetaryAccountInDb(MonetaryAccountDTO monetDTO)
        {
            SetUserConnected(monetDTO.UserId);

            return (MonetaryAccount)FindAccountByIdInDb(monetDTO.AccountId, monetDTO.UserId);
        }


        public Account FindAccountByIdInDb(int? idAccountToFind, int? userId)
        {
            if (idAccountToFind == -1)
            {
                throw new Exception("Must select a credit card account, otherwise there would not be changes");
            }
            
            SetUserConnected(userId);
            bool isFound = false;
            Account accountFound = new MonetaryAccount();

            foreach (var account in _userConnected.MyAccounts)
            {
                if (account.AccountId == idAccountToFind)
                {
                    accountFound = account;
                    isFound = true;
                }
            }

            if (!isFound)
            {
                throw new Exception("Account was not found, an error on index must be somewhere.");
            }
            return accountFound;
        }

        public AccountDTO FindAccountById(int? idAccountToFind, int? userId)
        {
            SetUserConnected(userId);

            Account accountFound = FindAccountByIdInDb(idAccountToFind, userId);
            MonetaryAccount possibleMonetaryAccount = new MonetaryAccount();
            CreditCardAccount possibleCreditCardAccount = new CreditCardAccount();

            if (accountFound is MonetaryAccount)
            {
                possibleMonetaryAccount = accountFound as MonetaryAccount;
                return MapperMonetaryAccount.ToMonetaryAccountDTO(possibleMonetaryAccount);
            }
            else
            {
                possibleCreditCardAccount = accountFound as CreditCardAccount;
                return MapperCreditAccount.ToCreditAccountDTO(possibleCreditCardAccount);
            }
        }

        public void UpdateMonetaryAccount(MonetaryAccountDTO monetaryDtoWithUpdates)
        {
            SetUserConnected(monetaryDtoWithUpdates.UserId);
            MonetaryAccount monetaryToUpd = MapperMonetaryAccount.ToMonetaryAccount(monetaryDtoWithUpdates);
            MonetaryAccount monetaryWithoutUpd = FindMonetaryAccountInDb(monetaryDtoWithUpdates);

            monetaryToUpd.AccountUser = _userConnected;
            if (Helper.AreTheSameObject(monetaryToUpd, monetaryWithoutUpd))
            {
                throw new Exception("There are non existential changes, change at least one please.");
            }
            else
            {
                _userConnected.ModifyMonetaryAccount(monetaryToUpd);
                _userRepo.Update(_userConnected);
            }
        }

        public void DeleteMonetaryAccount(MonetaryAccountDTO monetaryDtoToDelete)
        {
            try
            {
                SetUserConnected((int)monetaryDtoToDelete.UserId);
                MonetaryAccount monetaryAccountToDelete = FindMonetaryAccountInDb(monetaryDtoToDelete);
                _userConnected.DeleteAccount(monetaryAccountToDelete);
                _userRepo.UpdateDbWhenDeleting(_userConnected, monetaryAccountToDelete);
            }
            catch (Exception ExceptionType) when (
                ExceptionType is Exception ||
                ExceptionType is ExceptionAccountManagement
            )
            {
                throw new Exception(ExceptionType.Message);
            }
        }

        public List<MonetaryAccountDTO> GetAllMonetaryAccounts(int userConnectedId)
        {
            SetUserConnected(userConnectedId);
            List<MonetaryAccountDTO> monetaryAccountList = new List<MonetaryAccountDTO>();

            FindMonetariesAccountsAndMap(monetaryAccountList);

            return monetaryAccountList;
        }

        private void FindMonetariesAccountsAndMap(List<MonetaryAccountDTO> monetaryAccountList)
        {
            foreach (Account account in _userConnected.MyAccounts)
            {
                if (account is MonetaryAccount)
                {
                    monetaryAccountList.Add(MapperMonetaryAccount.ToMonetaryAccountDTO((MonetaryAccount)account));
                }
            }
        }

        #endregion

        #region Credit Card Account Section

        public void CreateCreditAccount(CreditCardAccountDTO creditAccountDTOToAdd)
        {
            try
            {
                SetUserConnected(creditAccountDTOToAdd.UserId);
                CreditCardAccount creditAccountToAdd = MapperCreditAccount.ToCreditAccount(creditAccountDTOToAdd);
                creditAccountToAdd.AccountId = 0;

                _userConnected.AddCreditAccount(creditAccountToAdd);

                _userRepo.Update(_userConnected);
            }
            catch (ExceptionMapper Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        public CreditCardAccountDTO FindCreditAccount(int idCreditAccountToFind, int userId)
        {
            CreditCardAccount creditAccountFound =
                (CreditCardAccount)FindAccountByIdInDb(idCreditAccountToFind, userId);

            CreditCardAccountDTO creditAccountFoundDTO = MapperCreditAccount.ToCreditAccountDTO(creditAccountFound);

            return creditAccountFoundDTO;
        }

        public CreditCardAccount FindCreditAccountInDb(CreditCardAccountDTO creditAccount)
        {
            SetUserConnected(creditAccount.UserId);
            return (CreditCardAccount)FindAccountByIdInDb(creditAccount.AccountId, creditAccount.UserId);
        }

        public void UpdateCreditAccount(CreditCardAccountDTO creditDtoWithUpdates)
        {
            SetUserConnected(creditDtoWithUpdates.UserId);
            CreditCardAccount creditToUpd = MapperCreditAccount.ToCreditAccount(creditDtoWithUpdates);
            CreditCardAccount creditWithoutUpd = FindCreditAccountInDb(creditDtoWithUpdates);

            creditToUpd.AccountUser = _userConnected;
            if (Helper.AreTheSameObject(creditToUpd, creditWithoutUpd))
            {
                throw new Exception("There are non existential changes, change at least one please.");
            }
            else
            {
                _userConnected.ModifyCreditAccount(creditToUpd);
                _userRepo.Update(_userConnected);
            }
        }

        public void DeleteCreditAccount(CreditCardAccountDTO accountToDelete)
        {
            try
            {
                SetUserConnected((int)accountToDelete.UserId);
                CreditCardAccount creditAccountToDelete = FindCreditAccountInDb(accountToDelete);
                _userConnected.DeleteAccount(creditAccountToDelete);
                _userRepo.UpdateDbWhenDeleting(_userConnected, creditAccountToDelete);
            }
            catch (Exception ExceptionType) when (
                ExceptionType is Exception ||
                ExceptionType is ExceptionAccountManagement
            )
            {
                throw new Exception(ExceptionType.Message);
            }
        }

        public List<CreditCardAccountDTO> GetAllCreditAccounts(int userId)
        {
            SetUserConnected(userId);
            List<CreditCardAccountDTO> creditAccountList = new List<CreditCardAccountDTO>();

            foreach (Account account in _userConnected.MyAccounts)
            {
                if (account is CreditCardAccount)
                {
                    creditAccountList.Add(MapperCreditAccount.ToCreditAccountDTO((CreditCardAccount)account));
                }
            }

            return creditAccountList;
        }

        #endregion

        #region Transaction Section

        public void CreateTransaction(TransactionDTO dtoToAdd, int? userId)
        {
            try
            {
                Account transactionAccount = FindAccountByIdInDb(dtoToAdd.AccountId, userId);
                Category categoryOfTransaction = FindCategoryInDb(dtoToAdd.TransactionCategory);
                SetUserConnected(transactionAccount.UserId);

                Transaction transactionToCreate = MapperTransaction.ToTransaction(dtoToAdd);
                Transaction.CheckExistenceOfExchange(transactionToCreate, _userConnected.MyExchangesHistory);
                transactionToCreate.TransactionId = 0;
                transactionToCreate.TransactionCategory = categoryOfTransaction;

                transactionAccount.AddTransaction(transactionToCreate);
                transactionAccount.UpdateAccountMoneyAfterAdd(transactionToCreate);
                _userRepo.Update(_userConnected);
            }
            catch (ExceptionMapper Exception)
            {
                throw new Exception(Exception.Message);
            }
        }


        public Transaction FindTransactionInDb(int transactionId, int? accountId, int? userId)
        {
            SetUserConnected(userId);
            Account accountAssigned = FindAccountByIdInDb(accountId, userId);

            foreach (var transaction in accountAssigned.MyTransactions)
            {
                if (transaction.TransactionId == transactionId)
                {
                    return transaction;
                }
            }

            throw new Exception("Transaction was not found, seems to be an error on index");
        }

        public TransactionDTO FindTransaction(int idToFound, int? accountId, int? userId)
        {
            SetUserConnected(userId);
            TransactionDTO transactionFound =
                MapperTransaction.ToTransactionDTO(FindTransactionInDb(idToFound, accountId, userId));
            return transactionFound;
        }

        public void UpdateTransaction(TransactionDTO dtoWithUpdates, int? userId)
        {
            try
            {
                SetUserConnected(userId);

                Transaction transactionPreUpdate =
                    FindTransactionInDb(dtoWithUpdates.TransactionId, dtoWithUpdates.AccountId, userId);


                Account accountOfTransaction = transactionPreUpdate.TransactionAccount;

                Transaction transactionWithUpdates = MapperTransaction.ToTransaction(dtoWithUpdates);

                transactionWithUpdates.TransactionAccount = transactionPreUpdate.TransactionAccount;
                transactionWithUpdates.TransactionCategory = FindCategoryInDb(dtoWithUpdates.TransactionCategory);

                accountOfTransaction.ModifyTransaction(transactionWithUpdates);

                accountOfTransaction.UpdateAccountAfterModify(transactionWithUpdates, transactionPreUpdate.Amount);
                _userRepo.Update(_userConnected);
            }
            catch (ExceptionMapper Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        public void DeleteTransaction(TransactionDTO transactionDtoToDelete, int? userId)
        {
            try
            {
                Account accountWhereIsTransaction = FindAccountByIdInDb(transactionDtoToDelete.AccountId, userId);
                SetUserConnected(accountWhereIsTransaction.UserId);
                Transaction transactionToDelete = FindTransactionInDb(transactionDtoToDelete.TransactionId,
                    transactionDtoToDelete.AccountId, accountWhereIsTransaction.UserId);

                accountWhereIsTransaction.DeleteTransaction(transactionToDelete);
                accountWhereIsTransaction.UpdateAccountAfterDelete(transactionToDelete);
                _userRepo.UpdateDbWhenDeleting(_userConnected, transactionToDelete);
            }
            catch (Exception Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        public List<TransactionDTO> GetAllTransactions(AccountDTO accountWithTransactions)
        {
            try
            {
                SetUserConnected(accountWithTransactions.UserId);

                Account accountToGetTransactions =
                    FindAccountByIdInDb(accountWithTransactions.AccountId, accountWithTransactions.UserId);
                List<TransactionDTO> transactionsDTO = new List<TransactionDTO>();

                transactionsDTO =
                    MapperTransaction.ToListOfTransactionsDTO(accountToGetTransactions.GetAllTransactions());
                return transactionsDTO;
            }
            catch (Exception Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        #endregion

        #region Report Section

        #region Monthly Report Per Goal

        public List<ResumeOfGoalReportDTO> GiveMonthlyReportPerGoal(UserDTO userLoggedDTO)
        {
            try
            {
                SetUserConnected(userLoggedDTO.UserId);
                List<ResumeOfGoalReportDTO> myListDTO = new List<ResumeOfGoalReportDTO>();
                
                myListDTO = MapperResumeOfGoalReport.ToListResumeOfGoalReportDTO(
                    Report.MonthlyReportPerGoal(_userConnected));


                return myListDTO;
            }
            catch (Exception Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        #endregion

        #region Give All Outcome Transactions

        public List<TransactionDTO> GiveAllOutcomeTransactions(UserDTO userConnectedDTO)
        {
            SetUserConnected(userConnectedDTO.UserId);
            User userInDb = _userRepo.FindUserInDb(userConnectedDTO.UserId);

            List<Transaction> spendingsPerCategory = Report.GiveAllOutcomeTransactions(userInDb);

            List<TransactionDTO> spendingsPerCategoryDTO =
                MapperTransaction.ToListOfTransactionsDTO(spendingsPerCategory);

            return spendingsPerCategoryDTO;
        }

        #endregion

        #region Spendings Report Per Category Detailed

        public List<ResumeOfCategoryReportDTO> GiveAllSpendingsPerCategoryDetailed(UserDTO userLoggedDTO,
            MonthsEnumDTO monthGiven)
        {
            try
            {
                SetUserConnected(userLoggedDTO.UserId);

                User userInDb = _userRepo.FindUserInDb(userLoggedDTO.UserId);

                List<ResumeOfCategoryReport> resumeDTOList =
                    Report.GiveAllSpendingsPerCategoryDetailed(userInDb, (MonthsEnum)monthGiven);

                List<ResumeOfCategoryReportDTO> resumeList =
                    MapperResumeOfCategoryReport.ToListResumeOfCategoryReportDTO(resumeDTOList);

                return resumeList;
            }
            catch (Exception Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        #endregion

        #region Report Of Spendings Per Card

        public List<TransactionDTO> ReportOfSpendingsPerCard(CreditCardAccountDTO creditCard)
        {
            try
            {
                CreditCardAccount accountGiven = FindCreditAccountInDb(creditCard);

                List<Transaction> spendingsPerCard = Report.ReportOfSpendingsPerCard(accountGiven);

                List<TransactionDTO> spendingsPerCardDTO = MapperTransaction.ToListOfTransactionsDTO(spendingsPerCard);

                return spendingsPerCardDTO;
            }
            catch (Exception Exception)
            {
                throw new Exception(Exception.Message);
            }
        }

        #endregion

        #region Filtering Lists

        public List<TransactionDTO> FilterListByRangeOfDate(List<TransactionDTO> listOfSpendingsDTO,
            RangeOfDatesDTO rangeOfDates)
        {
            try
            {
                List<Transaction> listOfTransactions = MapperTransaction.ToListOfTransactions(listOfSpendingsDTO);

                RangeOfDates myRangeOfDates = new RangeOfDates(rangeOfDates.InitialDate, rangeOfDates.FinalDate);

                listOfTransactions = Report.FilterListByRangeOfDate(listOfTransactions, myRangeOfDates);

                listOfSpendingsDTO = MapperTransaction.ToListOfTransactionsDTO(listOfTransactions);

                return listOfSpendingsDTO;
            }
            catch (ExceptionReport e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<TransactionDTO> FilterListByNameOfCategory(List<TransactionDTO> listOfSpendingsDTO,
            string nameOfCategory)
        {
            List<Transaction> listOfTransactions = MapperTransaction.ToListOfTransactions(listOfSpendingsDTO);

            listOfTransactions = Report.FilterListByNameOfCategory(listOfTransactions, nameOfCategory);

            listOfSpendingsDTO = MapperTransaction.ToListOfTransactionsDTO(listOfTransactions);

            return listOfSpendingsDTO;
        }

        public List<TransactionDTO> FilterByAccountAndTypeOutcome(AccountDTO accountSelected)
        {
            try
            {
                SetUserConnected(accountSelected.UserId);

                Account myAccount = FindAccountByIdInDb(accountSelected.AccountId, accountSelected.UserId);

                List<Transaction> myTransactions = Report.FilterListByAccountAndOutcome(myAccount);

                return MapperTransaction.ToListOfTransactionsDTO(myTransactions);
            }
            catch (Exception Exception)
            {
                throw new Exception();
            }
        }

        #endregion

        #region Balance of Monetary account

        public decimal GiveAccountBalance(MonetaryAccountDTO accountSelected)
        {
            MonetaryAccount monetGiven =
                ((MonetaryAccount)(FindAccountByIdInDb(accountSelected.AccountId, accountSelected.UserId)));
            decimal initialMoney = monetGiven.ReturnInitialAmount();

            decimal accountBalance = Report.GiveAccountBalance(monetGiven, initialMoney);

            return accountBalance;
        }

        #endregion

        #endregion

        public MovementInXDaysDTO GetMovementsOfTransactionsInXDays(int userId, RangeOfDatesDTO rangeOfDatesDTO,
            MonthsEnumDTO monthSelected)
        {
            try
            {
                if ((int)monthSelected != rangeOfDatesDTO.InitialDate.Month)
                {
                    throw new Exception("Month selected must be equal to the month of the dates");
                }

                SetUserConnected(userId);
                MovementInXDays movementsOfTransactionsPerDay = new MovementInXDays();
                RangeOfDates rangeOfDates = new RangeOfDates(rangeOfDatesDTO.InitialDate, rangeOfDatesDTO.FinalDate);

                movementsOfTransactionsPerDay = Report.GetMovementInXDays(_userConnected.MyAccounts, rangeOfDates);
                return MapperMovementInXDays.ToMovementDTO(movementsOfTransactionsPerDay);
            }
            catch (ExceptionReport Exception)
            {
                throw new Exception(Exception.Message);
            }
        }
    }
}