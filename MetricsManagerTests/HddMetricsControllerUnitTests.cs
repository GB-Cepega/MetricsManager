﻿using MetricsLibrary;
using MetricsManager.Controllers;
using MetricsManager.DAL;
using MetricsManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MetricsManagerTests
{
    
        public class HddMetricsControllerUnitTests
        {
            [Fact]
            public void GetMetricsFromAgentCheckRequestSelect()
            {
                //Arrange
                var mockLogger = new Mock<ILogger<HddMetricsController>>();
                var mock = new Mock<IHddMetricsRepository>();
                TimeSpan fromTime = TimeSpan.FromSeconds(5);
                TimeSpan toTime = TimeSpan.FromSeconds(10);
                int agentId = 1;
                mock.Setup(a => a.GetMetricsFromTimeToTimeFromAgent(fromTime, toTime, agentId)).Returns(new List<HddMetricModel>()).Verifiable();
                var controller = new HddMetricsController(mock.Object, mockLogger.Object);
                //Act
                var result = controller.GetMetricsFromAgent(agentId, fromTime, toTime);
                //Assert
                mock.Verify(repository => repository.GetMetricsFromTimeToTimeFromAgent(fromTime, toTime, agentId), Times.AtMostOnce());
                mockLogger.Verify();
            }

            [Fact]
            public void GetMetricsByPercentileFromAgentCheckRequestSelect()
            {
                //Arrange
                var mockLogger = new Mock<ILogger<HddMetricsController>>();
                var mock = new Mock<IHddMetricsRepository>();
                TimeSpan fromTime = TimeSpan.FromSeconds(5);
                TimeSpan toTime = TimeSpan.FromSeconds(10);
                int agentId = 1;
                Percentile percentile = Percentile.P90;
                string sort = "value";
                mock.Setup(a => a.GetMetricsFromTimeToTimeFromAgentOrderBy(fromTime, toTime, sort, agentId))
                    .Returns(new List<HddMetricModel>()).Verifiable();
                var controller = new HddMetricsController(mock.Object, mockLogger.Object);
                //Act
                var result = controller.GetMetricsByPercentileFromAgent(agentId, fromTime, toTime, percentile);
                //Assert
                mock.Verify(repository => repository.GetMetricsFromTimeToTimeFromAgentOrderBy(fromTime, toTime, sort, agentId), Times.AtMostOnce());
                mockLogger.Verify();
            }

            [Fact]
            public void GetHddMetricsFromClusterCheckRequestSelect()
            {
                //Arrange
                var mockLogger = new Mock<ILogger<HddMetricsController>>();
                var mock = new Mock<IHddMetricsRepository>();
                TimeSpan fromTime = TimeSpan.FromSeconds(5);
                TimeSpan toTime = TimeSpan.FromSeconds(10);
                mock.Setup(a => a.GetMetricsFromTimeToTime(fromTime, toTime)).Returns(new List<HddMetricModel>()).Verifiable();
                var controller = new HddMetricsController(mock.Object, mockLogger.Object);
                //Act
                var result = controller.GetMetricsFromAllCluster(fromTime, toTime);
                //Assert
                mock.Verify(repository => repository.GetMetricsFromTimeToTime(fromTime, toTime), Times.AtMostOnce());
                mockLogger.Verify();
            }

            [Fact]
            public void GetMetricsByPercentileFromClusterCheckRequestSelect()
            {
                //Arrange
                var mockLogger = new Mock<ILogger<HddMetricsController>>();
                var mock = new Mock<IHddMetricsRepository>();
                TimeSpan fromTime = TimeSpan.FromSeconds(5);
                TimeSpan toTime = TimeSpan.FromSeconds(10);
                Percentile percentile = Percentile.P90;
                string sort = "value";
                mock.Setup(a => a.GetMetricsFromTimeToTimeOrderBy(fromTime, toTime, sort))
                    .Returns(new List<HddMetricModel>()).Verifiable();
                var controller = new HddMetricsController(mock.Object, mockLogger.Object);
                //Act
                var result = controller.GetMetricsByPercentileFromCluster(fromTime, toTime, percentile);
                //Assert
                mock.Verify(repository => repository.GetMetricsFromTimeToTimeOrderBy(fromTime, toTime, sort), Times.AtMostOnce());
                mockLogger.Verify();
            }
        }
    
}
