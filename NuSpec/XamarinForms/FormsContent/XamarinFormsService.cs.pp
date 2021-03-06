﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the XamarinFormsService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace $rootnamespace$.Services
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Cirrious.CrossCore.IoC;
    using Cirrious.MvvmCross.ViewModels;

    using Xamarin.Forms;

    /// <summary>
    ///  Defines the XamarinFormsService type.
    /// </summary>
    public class XamarinFormsService : IXamarinFormsService
    {
        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// A Page.
        /// </returns>
        public Page GetPage(MvxViewModelRequest request)
        {
            string viewModelName = request.ViewModelType.Name;

            string pageName = viewModelName.Replace("ViewModel", "Page");

            Type type = request.ViewModelType
                                  .GetTypeInfo()
                                  .Assembly
                                  .CreatableTypes()
                                  .FirstOrDefault(x => x.Name == pageName);

            if (type != null)
            {
                return Activator.CreateInstance(type) as Page;
            }
            
            return null;
        }

    }
}
