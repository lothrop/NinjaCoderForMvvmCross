﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the PluginsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.ViewModels
{
    using Entities;
    using Factories.Interfaces;

    using Scorchio.Infrastructure.Wpf;
    using Scorchio.Infrastructure.Wpf.ViewModels;
    using Scorchio.VisualStudio.Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    ///  Defines the PluginsViewModel type.
    /// </summary>
    public class PluginsViewModel : NinjaBaseViewModel
    {
        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// The visual studio service.
        /// </summary>
        private readonly IVisualStudioService visualStudioService;

        /// <summary>
        /// The core plugins.
        /// </summary>
        private readonly ObservableCollection<SelectableItemViewModel<Plugin>> corePlugins;

        /// <summary>
        /// The community plugins.
        /// </summary>
        private readonly ObservableCollection<SelectableItemViewModel<Plugin>> communityPlugins;

        /// <summary>
        /// The core plugins selected
        /// </summary>
        private bool corePluginsSelected;

        /// <summary>
        /// The community plugins selected
        /// </summary>
        public bool communityPluginsSelected;

        /// <summary>
        /// The view model names.
        /// </summary>
        private IEnumerable<string> viewModelNames;

        /// <summary>
        /// The implement in view model
        /// </summary>
        private string implementInViewModel;

        /// <summary>
        /// The include unit tests.
        /// </summary>
        private bool includeUnitTests = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsViewModel" /> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="visualStudioService">The visual studio service.</param>
        /// <param name="pluginFactory">The plugin factory.</param>
        public PluginsViewModel(
            ISettingsService settingsService,
            IVisualStudioService visualStudioService,
            IPluginFactory pluginFactory)
            : base(settingsService)
        {
            TraceService.WriteLine("PluginsViewModel::Constructor Start");

            this.settingsService = settingsService;
            this.visualStudioService = visualStudioService;

            Plugins allPlugins = pluginFactory.GetPlugins(this.settingsService.MvvmCrossPluginsUri);

            this.corePlugins = this.GetPlugins(allPlugins, false);
            this.communityPlugins = this.GetPlugins(allPlugins, true);

            if (this.corePlugins.Any()== false &&
                this.communityPlugins.Any())
            {
                this.CommunityPluginsSelected = true;
            }

            else
            {
                this.CorePluginsSelected = true;
            }

            TraceService.WriteLine("PluginsViewModel::Constructor End");
        }

        /// <summary>
        /// Gets the view model names.
        /// </summary>
        public IEnumerable<string> ViewModelNames
        {
            get { return this.viewModelNames ?? (this.viewModelNames = this.visualStudioService.GetPublicViewModelNames()); }
        }

        /// <summary>
        /// Gets or sets the implement in view model.
        /// </summary>
        public string ImplementInViewModel
        {
            get { return this.implementInViewModel; }
            set { this.SetProperty(ref this.implementInViewModel, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include unit tests].
        /// </summary>
        public bool IncludeUnitTests
        {
            get { return this.includeUnitTests; }
            set { this.SetProperty(ref this.includeUnitTests, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [core plugins selected].
        /// </summary>
        public bool CorePluginsSelected
        {
            get { return this.corePluginsSelected; }
            set { this.corePluginsSelected = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [community plugins selected].
        /// </summary>
        public bool CommunityPluginsSelected
        {
            get { return this.communityPluginsSelected; }
            set { this.communityPluginsSelected = value; }
        }

        /// <summary>
        /// Gets the core plugins.
        /// </summary>
        public IEnumerable<SelectableItemViewModel<Plugin>> CorePlugins
        {
            get { return this.corePlugins; }
        }

        /// <summary>
        /// Gets the community plugins.
        /// </summary>
        public IEnumerable<SelectableItemViewModel<Plugin>> CommunityPlugins
        {
            get { return this.communityPlugins; }
        }

        /// <summary>
        /// Gets the wiki page command.
        /// </summary>
        public ICommand WikiPageCommand
        {
            get { return new RelayCommand(this.DisplayWikiPage); }
        }

        /// <summary>
        /// Gets the required plugins.
        /// </summary>
        /// <returns>The required plugins.</returns>
        public IEnumerable<Plugin> GetRequiredPlugins()
        {
            TraceService.WriteLine("PluginsViewModel::GetRequiredPlugins");

            IEnumerable<SelectableItemViewModel<Plugin>> viewModels = this.CorePlugins
                                                                .Union(this.CommunityPlugins);

            return viewModels.ToList()
                .Where(x => x.IsSelected)
                .Select(x => x.Item).ToList();
        }

        /// <summary>
        /// Displays the wiki page.
        /// </summary>
        internal void DisplayWikiPage()
        {
            Process.Start(this.settingsService.MvvmCrossPluginsWikiPage);
        }

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        /// <param name="plugins">The plugins.</param>
        /// <param name="includeCommunityPlugins">if set to <c>true</c> [community plugins].</param>
        /// <returns>
        /// The view Models.
        /// </returns>
        internal ObservableCollection<SelectableItemViewModel<Plugin>> GetPlugins(
            Plugins plugins,
            bool includeCommunityPlugins)
        {
            ObservableCollection<SelectableItemViewModel<Plugin>> viewModels = new ObservableCollection<SelectableItemViewModel<Plugin>>();

            foreach (SelectableItemViewModel<Plugin> viewModel in from plugin in plugins.Items
                                                                  where plugin.IsCommunityPlugin == includeCommunityPlugins &&
                                                                        plugin.Frameworks.Contains(this.visualStudioService.GetFrameworkType())
                                                                  select new SelectableItemViewModel<Plugin>(plugin))
            {
                viewModels.Add(viewModel);
            }

            return viewModels;
        }
    }
}
