﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the ProjectsService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.Services
{
    using Interfaces;
    using NinjaCoder.MvvmCross.Entities;
    using Scorchio.Infrastructure.Extensions;
    using Scorchio.VisualStudio.Entities;
    using Scorchio.VisualStudio.Services;
    using Scorchio.VisualStudio.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    ///  Defines the ProjectsService type.
    /// </summary>
    internal class ProjectsService : BaseService, IProjectsService
    {
        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// The file system.
        /// </summary>
        protected readonly IFileSystem FileSystem;

        /// <summary>
        /// The visual studio service.
        /// </summary>
        private IVisualStudioService visualStudioService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsService" /> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="fileSystem">The file system.</param>
        public ProjectsService(
            ISettingsService settingsService,
            IFileSystem fileSystem)
        {
            TraceService.WriteLine("ProjectsService::constructor");

            this.settingsService = settingsService;
            this.FileSystem = fileSystem;
        }

        /// <summary>
        /// Adds the projects.
        /// </summary>
        /// <param name="visualStudioServiceInstance">The visual studio service.</param>
        /// <param name="path">The path.</param>
        /// <param name="projectsInfos">The projects infos.</param>
        /// <returns>
        /// The messages.
        /// </returns>
        public IEnumerable<string> AddProjects(
            IVisualStudioService visualStudioServiceInstance,
            string path, 
            IEnumerable<ProjectTemplateInfo> projectsInfos)
        {
            TraceService.WriteLine("ProjectsService::AddProjects");

            IEnumerable<ProjectTemplateInfo> projectTemplateInfos = projectsInfos as ProjectTemplateInfo[] ?? projectsInfos.ToArray();

            string message = string.Format("ProjectsService::AddProjects project count={0} path={1}", projectTemplateInfos.Count(), path);
            
            TraceService.WriteLine(message);

            //// reset the messages.
            this.Messages = new List<string>
            {
                string.Empty,
                this.settingsService.FrameworkType.GetDescription() + " framework selected.", 
                string.Empty
            };

            if (this.settingsService.UsePreReleaseMvvmCrossNugetPackages &&
               (this.settingsService.FrameworkType == FrameworkType.MvvmCross ||
                 this.settingsService.FrameworkType == FrameworkType.MvvmCrossAndXamarinForms))
            {
                this.Messages.Add("Pre Release MvvmCross Nuget Packages requested.");
                this.Messages.Add(string.Empty);
            }

            if (this.settingsService.UsePreReleaseMvvmCrossNugetPackages &&
               (this.settingsService.FrameworkType == FrameworkType.XamarinForms ||
                this.settingsService.FrameworkType == FrameworkType.MvvmCrossAndXamarinForms))
            {
                this.Messages.Add("Pre Release Xamarin Forms Nuget Packages requested.");
                this.Messages.Add(string.Empty);
            }

            this.visualStudioService = visualStudioServiceInstance;
            
            foreach (ProjectTemplateInfo projectInfo in projectTemplateInfos)
            {
                this.AddProject(path, projectInfo);
            }

            return this.Messages;
        }

        /// <summary>
        /// Adds the project.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="projectInfo">The project information.</param>
        internal void AddProject(
            string path,
            ProjectTemplateInfo projectInfo)
        {
            string message = string.Format("ProjectsService::AddProjectIf project {0}", projectInfo.Name);

            TraceService.WriteLine(message);
            
            this.settingsService.ActiveProject = projectInfo.FriendlyName;

            this.TryToAddProject(path, projectInfo);

            //// add reference to core project

            IProjectService coreProjectService = this.visualStudioService.CoreProjectService;

            IProjectService projectService = this.visualStudioService.GetProjectServiceBySuffix(projectInfo.ProjectSuffix);

            if (coreProjectService != null && 
                projectService != null && 
                projectInfo.ReferenceCoreProject)
            {
                projectService.AddProjectReference(coreProjectService);
            }

            //// now add references to xamarin forms if required.

            if (projectInfo.ReferenceXamarinFormsProject)
            {
                IProjectService formsProjectService = this.visualStudioService.XamarinFormsProjectService;

                if (formsProjectService != null && 
                    projectService != null )
                {
                    projectService.AddProjectReference(formsProjectService);
                }
            }

            //// now add reference to the plaform project from the test platform project

            if (projectInfo.ReferencePlatformProject)
            {
                //// TODO : tidy this up a little bit!
                IProjectService platformProjectService = this.visualStudioService.GetProjectServiceBySuffix(
                    projectInfo.ProjectSuffix.Replace(".Tests", string.Empty));

                if (platformProjectService != null && 
                    projectService != null)
                {
                    //// for some reason cant add project reference to windows store project !!
                    //// as no one is developing mvvmcross windows store apps dont worry about it :-)
                    try
                    {
                        projectService.AddProjectReference(platformProjectService);

                    }
                    catch (Exception exception)
                    {
                        TraceService.WriteError("Unable to add project reference to " + platformProjectService.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the project.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="projectInfo">The project info.</param>
        internal void TryToAddProject(
            string path,
            ProjectTemplateInfo projectInfo)
        {
            TraceService.WriteLine("ProjectsService::TryToAddProject  project=" + projectInfo.Name);

            //// Project may actually already exist - if so just skip it!

            string projectPath = string.Format(@"{0}\{1}\", path, projectInfo.Name);

            if (this.FileSystem.Directory.Exists(projectPath) == false)
            {
                this.AddProject(projectInfo, projectPath);
            }

            else
            {
                TraceService.WriteError("Directory " + projectPath + " not empty");
            }
        }

        /// <summary>
        /// Adds the project.
        /// </summary>
        /// <param name="projectInfo">The project info.</param>
        /// <param name="projectPath">The project path.</param>
        internal void AddProject(
            ProjectTemplateInfo projectInfo,
            string projectPath)
        {
            TraceService.WriteLine("ProjectsService::AddProjectIf project=" + projectInfo.Name + " templateName=" + projectInfo.TemplateName);

            try
            {
                string template = this.visualStudioService.SolutionService.GetProjectTemplate(projectInfo.TemplateName);

                TraceService.WriteLine("Template=" + template);

                this.visualStudioService.SolutionService.AddProjectToSolution(projectPath, template, projectInfo.Name);

                this.Messages.Add(projectInfo.Name + " project successfully added. (template " + projectInfo.TemplateName + ")");
            }
            catch (Exception exception)
            {
                TraceService.WriteError("error adding project " + projectPath + " exception=" + exception.Message + " templateName=" + projectInfo.TemplateName);
               
                this.Messages.Add("ERROR " + projectInfo.Name + " not added. exception " + exception.Message + " (template " + projectInfo.TemplateName + ")");
            }
        }
    }
}
