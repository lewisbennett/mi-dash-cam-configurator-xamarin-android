﻿using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using DialogMessaging;
using MiCamConfig.App.Core.ViewModels.Base;
using MiCamConfig.App.Droid.Attributes;
using MvvmCross.Platforms.Android.Views;
using System.Reflection;

namespace MiCamConfig.App.Droid.Activities.Base
{
    public class BaseActivity<TViewModel> : MvxActivity<TViewModel>
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
        public IMessagingService MessagingService => DialogMessaging.MessagingService.Instance;

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