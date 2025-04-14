﻿using FluentAssertions;
using Hospital.Managers;
using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Hospital.Models;

namespace PatientDashboardProfilePart.Tests
{
    public class PatientManagerModelTests
    {
        private readonly Mock<PatientsDatabaseService> _dbMock;
        private readonly PatientManagerModel _manager;

        public PatientManagerModelTests()
        {
            _dbMock = new Mock<PatientsDatabaseService>();
            _manager = new PatientManagerModel(_dbMock.Object);
        }

        [Fact]
        public async Task UpdateEmail_ValidEmail_ReturnsTrue()
        {
            _dbMock.Setup(x => x.UpdateEmail(1, "test@email.com")).ReturnsAsync(true);

            var result = await _manager.UpdateEmail(1, "test@email.com");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateEmail_InvalidEmail_Throws()
        {
            var action = async () => await _manager.UpdateEmail(1, "bademail");

            await action.Should().ThrowAsync<InputProfileException>();
        }

        [Fact]
        public async Task UpdateEmail_TooLong_Throws()
        {
            string longEmail = new string('a', 101) + "@mail.com";

            var action = async () => await _manager.UpdateEmail(1, longEmail);

            await action.Should().ThrowAsync<InputProfileException>().WithMessage("*cannot exceed 100*");
        }

        [Fact]
        public async Task UpdatePhoneNumber_Valid_ReturnsTrue()
        {
            _dbMock.Setup(x => x.UpdatePhoneNumber(1, "0712345678")).ReturnsAsync(true);

            var result = await _manager.UpdatePhoneNumber(1, "0712345678");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePhoneNumber_Invalid_Throws()
        {
            var action = async () => await _manager.UpdatePhoneNumber(1, "1234");

            await action.Should().ThrowAsync<InputProfileException>();
        }

        [Fact]
        public async Task UpdatePassword_ValidPassword_ReturnsTrue()
        {
            _dbMock.Setup(x => x.UpdatePassword(1, "securePass123")).ReturnsAsync(true);

            var result = await _manager.UpdatePassword(1, "securePass123");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePassword_Empty_Throws()
        {
            var action = async () => await _manager.UpdatePassword(1, "");

            await action.Should().ThrowAsync<InputProfileException>().WithMessage("*empty or contain spaces*");
        }

        [Fact]
        public async Task UpdatePassword_TooLong_Throws()
        {
            string longPassword = new string('a', 256);

            var action = async () => await _manager.UpdatePassword(1, longPassword);

            await action.Should().ThrowAsync<InputProfileException>().WithMessage("*cannot exceed 255*");
        }

        [Fact]
        public async Task UpdateUsername_WithSpaces_Throws()
        {
            var action = async () => await _manager.UpdateUsername(1, "bad username");

            await action.Should().ThrowAsync<InputProfileException>().WithMessage("*cannot be empty or contain spaces*");
        }

        [Fact]
        public async Task UpdateName_InvalidNameWithDigits_Throws()
        {
            var action = async () => await _manager.UpdateName(1, "John123");

            await action.Should().ThrowAsync<InputProfileException>().WithMessage("*contain digits*");
        }

        [Fact]
        public async Task UpdateBirthDate_Valid_CallsDb()
        {
            var date = DateOnly.FromDateTime(DateTime.Today);

            _dbMock.Setup(x => x.UpdateBirthDate(1, date)).ReturnsAsync(true);

            var result = await _manager.UpdateBirthDate(1, date);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAddress_TooLong_Throws()
        {
            string longAddress = new string('a', 256);

            var action = async () => await _manager.UpdateAddress(1, longAddress);

            await action.Should().ThrowAsync<InputProfileException>().WithMessage("*cannot exceed 255*");
        }

        [Fact]
        public async Task UpdateEmergencyContact_NonDigits_Throws()
        {
            var action = async () => await _manager.UpdateEmergencyContact(1, "123abc4567");

            await action.Should().ThrowAsync<InputProfileException>().WithMessage("*Only digits are allowed*");
        }

        [Fact]
        public async Task UpdateHeight_Invalid_Throws()
        {
            var action = async () => await _manager.UpdateHeight(1, 0);

            await action.Should().ThrowAsync<InputProfileException>().WithMessage("*must be greater than 0*");
        }

        [Fact]
        public async Task LoadPatientInfoByUserId_ReturnsTrue()
        {
            _dbMock.Setup(x => x.GetPatientByUserId(1)).ReturnsAsync(new PatientJointModel(1, 1, "Test", "", "", "", 60, 170, "user", "pass", "email", DateOnly.FromDateTime(DateTime.Now), "123", "addr", "0123456789", DateTime.Now));

            var result = await _manager.LoadPatientInfoByUserId(1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task LogUpdate_ValidAction_ReturnsTrue()
        {
            _dbMock.Setup(x => x.LogUpdate(1, ActionType.UPDATE_PROFILE)).ReturnsAsync(true);

            var result = await _manager.LogUpdate(1, ActionType.UPDATE_PROFILE);

            result.Should().BeTrue();
        }
    }
}
