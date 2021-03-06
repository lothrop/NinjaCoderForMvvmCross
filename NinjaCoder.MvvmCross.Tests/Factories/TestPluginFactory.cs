﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the TestPluginFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.Tests.Factories
{
    using Moq;
    using NinjaCoder.MvvmCross.Entities;
    using NinjaCoder.MvvmCross.Factories;
    using NinjaCoder.MvvmCross.Services.Interfaces;
    using NinjaCoder.MvvmCross.Tests.Mocks;
    using NUnit.Framework;
    using Scorchio.Infrastructure.Translators;
    using System.IO.Abstractions;

    /// <summary>
    ///  Defines the TestPluginFactory type.
    /// </summary>
    [TestFixture]
    public class TestPluginFactory
    {
        /// <summary>
        /// The plugin factory,
        /// </summary>
        private PluginFactory factory;

        /// <summary>
        /// The mock plugins service.
        /// </summary>
        private Mock<IPluginsService> mockPluginsService;

        /// <summary>
        /// The mock settings service.
        /// </summary>
        private Mock<ISettingsService> mockSettingsService;

        /// <summary>
        /// The mock plugins translator.
        /// </summary>
        private Mock<ITranslator<string, Plugins>> mockPluginsTranslator;

        /// <summary>
        /// The mock plugin translator.
        /// </summary>
        private Mock<ITranslator<FileInfoBase, Plugin>> mockPluginTranslator;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestFixtureSetUp]
        public void Initialize()
        {
            /*
            this.mockPluginsService = new Mock<IPluginsService>();
            this.mockSettingsService = new Mock<ISettingsService>();
            this.mockPluginsTranslator = new Mock<ITranslator<string, Plugins>>();
            this.mockPluginTranslator = new Mock<ITranslator<FileInfoBase, Plugin>>();

            MockDirectory mockDirectory = new MockDirectory { DirectoryExists = true };
            
            this.factory = new PluginFactory(
                this.mockPluginsService.Object,
                this.mockSettingsService.Object,
                this.mockPluginsTranslator.Object);*/
        }

        /// <summary>
        /// Tests the get plugins service.
        /// </summary>
        [Test]
        public void TestGetPluginsService()
        {
            IPluginsService pluginsService = this.factory.GetPluginsService();

            Assert.IsTrue(pluginsService == this.mockPluginsService.Object);
        }

        /// <summary>
        /// Tests the get plugins.
        /// </summary>
        [Test]
        public void TestGetPlugins()
        {
            Plugins plugins = this.factory.GetPlugins("");
        }
    }
}
