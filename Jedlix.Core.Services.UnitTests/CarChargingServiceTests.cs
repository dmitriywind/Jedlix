using System.Threading.Tasks;
using System;
using AutoFixture;
using Jedlix.Core.Contracts;
using Jedlix.Models;
using Jedlix.Models.Events;
using Jedlix.ServiceBusPublisher;
using Jedlix.ServiceBusPublisher.Messages;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Jedlix.Core.Services.UnitTests
{
    public class Tests
    {
        readonly Mock<IChargingScheduleGenerationService> chargingScheduleGenerationServiceMock = new Mock<IChargingScheduleGenerationService>();
        readonly Mock<IServiceBusMessageService<CarChargingProfileMessage>> _chargingProfileMessageSenderMock = new Mock<IServiceBusMessageService<CarChargingProfileMessage>>();

        private Fixture _fixture;
        private CarChargingService _service;

        [SetUp]
        public void Setup()
        {
            _service = new CarChargingService(
                chargingScheduleGenerationServiceMock.Object,
                _chargingProfileMessageSenderMock.Object);

            _fixture = new Fixture();
        }

        [Test]
        public async Task ProcessStartingChargingEvent_ShouldGenerateAndSendChargingProfile()
        {
            // Arrange
            var startingChargingEvent = new CarIsStartingChargingEvent
            {
                StartDateTimeOffset = DateTimeOffset.UtcNow
            };

            var profiles = _fixture.Create<List<CarChargingProfile>>();

            // Mock the GenerateChargingProfiles method to return some charging profiles
            chargingScheduleGenerationServiceMock
                .Setup(service => service.GenerateChargingProfiles(startingChargingEvent.StartDateTimeOffset))
                .ReturnsAsync(profiles);

            // Act
            await _service.ProcessStartingChargingEvent(startingChargingEvent);

            // Assert
            chargingScheduleGenerationServiceMock.Verify(service => service.GenerateChargingProfiles(startingChargingEvent.StartDateTimeOffset), Times.Once);
            _chargingProfileMessageSenderMock.Verify(sender => sender.SendAsync(It.Is<CarChargingProfileMessage>(message => message.ChargingProfiles != null), It.IsAny<string>()), Times.Once);
        }
    }
}