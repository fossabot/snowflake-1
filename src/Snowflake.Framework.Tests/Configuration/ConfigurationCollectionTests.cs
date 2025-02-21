﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Snowflake.Configuration.Tests
{
    public class ConfigurationCollectionTests
    {
        [Fact]
        public void ValueInitialization_Tests()
        {
            var values = new List<(string, string, IConfigurationValue)>()
            {
                ("ExampleConfiguration", "ISOPath0", new ConfigurationValue("Test", Guid.Empty))
            };
            var configuration =
                new ConfigurationCollection<ExampleConfigurationCollection>(new ConfigurationValueCollection(values));
            Assert.Equal("Test", configuration.Configuration.ExampleConfiguration.ISOPath0);
            Assert.Equal(Guid.Empty, configuration.Configuration.ExampleConfiguration.Values["ISOPath0"].Guid);
        }

        [Fact]
        public void StringInitialization_Tests()
        {
            var values = new List<(string, string, (string, Guid))>()
            {
                ("ExampleConfiguration", "ISOPath0", ("Test", Guid.Empty)),
                ("ExampleConfiguration", "FullscreenResolution",
                    (FullscreenResolution.Resolution1024X600.ToString(), Guid.Empty))
            };

            var collection =
                ConfigurationValueCollection.MakeExistingValueCollection<ExampleConfigurationCollection>(values,
                    Guid.Empty);
            var configuration = new ConfigurationCollection<ExampleConfigurationCollection>(collection);
            Assert.Equal("Test", configuration.Configuration.ExampleConfiguration.ISOPath0);
            Assert.Equal(FullscreenResolution.Resolution1024X600,
                configuration.Configuration.ExampleConfiguration.FullscreenResolution);
            Assert.Equal(Guid.Empty, configuration.Configuration.ExampleConfiguration.Values["ISOPath0"].Guid);
            Assert.Equal(Guid.Empty,
                configuration.Configuration.ExampleConfiguration.Values["FullscreenResolution"].Guid);
        }

        [Fact]
        public void Defaults_Tests()
        {
            var configuration = new ConfigurationCollection<ExampleConfigurationCollection>();
            Assert.Equal(configuration.Configuration.ExampleConfiguration.Descriptor["FullscreenResolution"].Default,
                configuration.Configuration.ExampleConfiguration.FullscreenResolution);
        }

        [Fact]
        public void Descriptor_Tests()
        {
            var configuration = new ConfigurationCollection<ExampleConfigurationCollection>();
            Assert.Equal("Display", configuration.Configuration.ExampleConfiguration.Descriptor.SectionName);
        }

        [Fact]
        public void Nested_Test()
        {
            var configuration = new ConfigurationCollection<ExampleConfigurationCollection>();
            Assert.Equal(configuration.Configuration.ExampleConfiguration.Descriptor["FullscreenResolution"].Default,
                configuration.Configuration.ExampleConfiguration.Configuration.Configuration.FullscreenResolution);
        }

        [Fact]
        public void Order_Test()
        {
            var configuration = new ConfigurationCollection<OrderSensitiveConfigurationCollection>();
            var enumerator = configuration.GetEnumerator();
            enumerator.MoveNext();
            Assert.Equal("Display", enumerator.Current.Value.Descriptor.SectionName);
            enumerator.MoveNext();
            Assert.Equal("video", enumerator.Current.Value.Descriptor.SectionName);
        }

        [Fact]
        public void SetStringToNull_Test()
        {
            var configuration = new ConfigurationCollection<ExampleConfigurationCollection>();
            configuration.Configuration.ExampleConfiguration.ISOPath0 = null;
            Assert.Equal(String.Empty, configuration.Configuration.ExampleConfiguration.ISOPath0);
        }

        [Fact]
        public void SetStringToNullDefault_Test()
        {
            var configuration = new ConfigurationCollection<NullDefaultConfigurationCollection>();
            Assert.Equal("UNSET", configuration.Configuration.NullConfiguration.NullDefault);
            configuration.Configuration.NullConfiguration.NullDefault = "NotUNSET";
            Assert.Equal("NotUNSET", configuration.Configuration.NullConfiguration.NullDefault);
            configuration.Configuration.NullConfiguration.NullDefault = null;
            Assert.Equal("UNSET", configuration.Configuration.NullConfiguration.NullDefault);
        }
    }
}
