using Hospital.Managers;
using Hospital.Models;
using Hospital.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Tests.ViewModels
{
    [TestClass]
    public class LoggerViewModelTests
    {
        private Mock<ILoggerManagerModel> _mockLoggerManager;
        private LoggerViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _mockLoggerManager = new Mock<ILoggerManagerModel>();
            _viewModel = new LoggerViewModel(_mockLoggerManager.Object);
        }

        [TestMethod]
        public async Task LoadLogsCommand_LoadAllLogsCommand_UpdatesLogsCollection()
        {
            // Arrange
            var expectedLogs = new List<LogEntryModel>
            {
                new LogEntryModel(1, 1, ActionType.LOGIN, DateTime.Now),
                new LogEntryModel(2, 2, ActionType.LOGOUT, DateTime.Now)
            };
            _mockLoggerManager.Setup(manage => manage.GetAllLogs())
                .ReturnsAsync(expectedLogs);

            // Act
            await Task.Run(() => _viewModel.LoadAllLogsCommand.Execute(null));

            // Assert
            Assert.AreEqual(expectedLogs.Count, _viewModel.Logs.Count);
            _mockLoggerManager.Verify(manage => manage.GetAllLogs(), Times.Once);
        }

        [TestMethod]
        public async Task FilterLogsById_CheckIfUserIdIsValid_UpdatesLogsCollection()
        {
            // Arrange
            _viewModel.UserIdInput = "1";
            var expectedLogs = new List<LogEntryModel>
            {
                new LogEntryModel(1, 1, ActionType.LOGIN, DateTime.Now)
            };
            _mockLoggerManager.Setup(manage => manage.GetLogsByUserId(1))
                .ReturnsAsync(expectedLogs);

            // Act
            await Task.Run(() => _viewModel.FilterLogsByUserIdCommand.Execute(null));

            // Assert
            Assert.AreEqual(expectedLogs.Count, _viewModel.Logs.Count);
            _mockLoggerManager.Verify(manage => manage.GetLogsByUserId(1), Times.Once);
        }

        [TestMethod]
        public async Task FilterLogsById_CheckIfUserIdIsInvalid_DoesNotUpdateLogsCollection()
        {
            // Arrange
            _viewModel.UserIdInput = "invalid";
            
            // Act
            await Task.Run(() => _viewModel.FilterLogsByUserIdCommand.Execute(null));

            // Assert
            _mockLoggerManager.Verify(manage => manage.GetLogsByUserId(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task FilterLogsByActionTypeCommand_ValidateActionType_UpdatesLogsCollection()
        {
            // Arrange
            _viewModel.SelectedActionType = ActionType.LOGIN;
            var expectedLogs = new List<LogEntryModel>
            {
                new LogEntryModel(1, 1, ActionType.LOGIN, DateTime.Now),
                new LogEntryModel(3, 2, ActionType.LOGIN, DateTime.Now)
            };
            _mockLoggerManager.Setup(manage => manage.GetLogsByActionType(ActionType.LOGIN))
                .ReturnsAsync(expectedLogs);

            // Act
            await Task.Run(() => _viewModel.FilterLogsByActionTypeCommand.Execute(null));

            // Assert
            Assert.AreEqual(expectedLogs.Count, _viewModel.Logs.Count);
            _mockLoggerManager.Verify(manage => manage.GetLogsByActionType(ActionType.LOGIN), Times.Once);
        }

        [TestMethod]
        public async Task FilterLogsByTimestampCommand_ValidateTimestamp_UpdatesLogsCollection()
        {
            // Arrange
            var timestamp = DateTime.Now;
            _viewModel.SelectedTimestamp = timestamp;
            var expectedLogs = new List<LogEntryModel>
            {
                new LogEntryModel(1, 1, ActionType.LOGIN, timestamp.AddDays(-1))
            };
            _mockLoggerManager.Setup(manage => manage.GetLogsBeforeTimestamp(It.IsAny<DateTime>()))
                .ReturnsAsync(expectedLogs);

            // Act
            await Task.Run(() => _viewModel.FilterLogsByTimestampCommand.Execute(null));

            // Assert
            Assert.AreEqual(expectedLogs.Count, _viewModel.Logs.Count);
            _mockLoggerManager.Verify(manage => manage.GetLogsBeforeTimestamp(It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public async Task ApplyAllFiltersCommand_AllFiltersApplied_UpdatesLogsCollection()
        {
            // Arrange
            _viewModel.UserIdInput = "1";
            _viewModel.SelectedActionType = ActionType.LOGIN;
            _viewModel.SelectedTimestamp = DateTime.Now;
            
            var expectedLogs = new List<LogEntryModel>
            {
                new LogEntryModel(1, 1, ActionType.LOGIN, DateTime.Now.AddHours(-1))
            };
            
            _mockLoggerManager.Setup(manage => manage.GetLogsWithParameters(
                It.IsAny<int?>(), It.IsAny<ActionType>(), It.IsAny<DateTime>()))
                .ReturnsAsync(expectedLogs);

            // Act
            await Task.Run(() => _viewModel.ApplyAllFiltersCommand.Execute(null));

            // Assert
            Assert.AreEqual(expectedLogs.Count, _viewModel.Logs.Count);
            _mockLoggerManager.Verify(manage => manage.GetLogsWithParameters(
                It.IsAny<int>(), It.IsAny<ActionType>(), It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public async Task ApplyAllFiltersCommand_CheckForInvalidUserId_UsesDefaultFilters()
        {
            // Arrange
            _viewModel.UserIdInput = "invalid";
            _viewModel.SelectedActionType = ActionType.LOGIN;
            _viewModel.SelectedTimestamp = DateTime.Now;
            
            var expectedLogs = new List<LogEntryModel>
            {
                new LogEntryModel(1, 1, ActionType.LOGIN, DateTime.Now.AddHours(-1))
            };

            _mockLoggerManager.Setup(manage => manage.GetLogsWithParameters(
                    It.Is<int?>(uid => !uid.HasValue), It.IsAny<ActionType>(), It.IsAny<DateTime>()))
                .ReturnsAsync(expectedLogs);

            // Act
            await Task.Run(() => _viewModel.ApplyAllFiltersCommand.Execute(null));

            // Assert
            _mockLoggerManager.Verify(manage => manage.GetLogsWithParameters(
                null, It.IsAny<ActionType>(), It.IsAny<DateTime>()), Times.Once);
        }
        
        [TestMethod]
        public void Constructor_InitializesProperties_SetsDefaultValues()
        {
            // Arrange & Act
            var viewModel = new LoggerViewModel(_mockLoggerManager.Object);
            
            // Assert
            Assert.IsNotNull(viewModel.Logs);
            Assert.AreEqual(string.Empty, viewModel.UserIdInput);
            Assert.IsNotNull(viewModel.ActionTypes);
            Assert.IsTrue(viewModel.ActionTypes.Count > 0);
            Assert.IsNotNull(viewModel.LoadAllLogsCommand);
            Assert.IsNotNull(viewModel.FilterLogsByUserIdCommand);
            Assert.IsNotNull(viewModel.FilterLogsByActionTypeCommand);
            Assert.IsNotNull(viewModel.FilterLogsByTimestampCommand);
            Assert.IsNotNull(viewModel.ApplyAllFiltersCommand);
        }
    }
}
