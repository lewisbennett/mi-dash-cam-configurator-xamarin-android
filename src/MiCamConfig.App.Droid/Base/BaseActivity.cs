﻿using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using MiCamConfig.App.Core.Base;
using MiCamConfig.App.Core.Services;
using MiCamConfig.App.Droid.Attributes;
using MiCamConfig.App.Droid.Services;
using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using System.Reflection;

namespace MiCamConfig.App.Droid.Base
{
    public class BaseActivity<TViewModel> : MvxAppCompatActivity<TViewModel>
        where TViewModel : BaseViewModel
    {
        #region Properties
        /// <summary>
        /// Gets the layout attributes for this activity.
        /// </summary>
        public ActivityLayoutAttribute LayoutAttributes { get; private set; }

        /// <summary>
        /// Gets the messaging service.
        /// </summary>
        public IMessagingService MessagingService { get; } = Mvx.IoCProvider.Resolve<IMessagingService>();

        /// <summary>
        /// Gets the permissions service.
        /// </summary>
        public IPermissionsService PermissionsService { get; } = Mvx.IoCProvider.Resolve<IPermissionsService>();

        /// <summary>
        /// Gets the toolbar for this activity.
        /// </summary>
        public Toolbar Toolbar { get; private set; }
        #endregion

        #region Public Methods
        public override void SetContentView(View view)
        {
            base.SetContentView(view);

            SetSupportActionBar();
        }

        public override void SetContentView(int layoutResId)
        {
            base.SetContentView(layoutResId);

            SetSupportActionBar();
        }

        public override void SetContentView(View view, ViewGroup.LayoutParams @params)
        {
            base.SetContentView(view, @params);

            SetSupportActionBar();
        }

        /// <summary>
        /// Sets the action bar automatically if available.
        /// </summary>
        public void SetSupportActionBar()
        {
            Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            if (Toolbar != null)
                SetSupportActionBar(Toolbar);
        }
        #endregion

        #region Event Handlers
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    ViewModel.BackButtonClickCommand.Execute();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        #endregion

        #region Lifecycle
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Initialize();
            SetupView();
        }

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

            Initialize();
            SetupView();
        }

        protected override void OnResume()
        {
            base.OnResume();

            Services.MessagingService.Context = this;
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            LayoutAttributes = GetType().GetCustomAttribute<ActivityLayoutAttribute>(true);
        }

        private void SetupView()
        {
            if (LayoutAttributes == null)
                return;

            if (LayoutAttributes.LayoutResourceId != 0)
                SetContentView(LayoutAttributes.LayoutResourceId);

            SupportActionBar?.SetDisplayHomeAsUpEnabled(LayoutAttributes.EnableBackButton);
        }
        #endregion
    }
}