using System;
using SitesMonitoring.Models;
using Xunit;

namespace SitesMonitoring.Tests
{
    public class ModelTest
    {
        [Fact]
        public void Site_GetStatusString()
        {
            Site s = new Site();
            Assert.Equal("", s.GetStatusString);
            s.Status = 1;

            Assert.Equal("Доступен",s.GetStatusString);

            s.Status = 0;
            Assert.Equal("Недоступен", s.GetStatusString);

            s.Status = 9;
            Assert.Equal("", s.GetStatusString);        
           

        }
    }
}
