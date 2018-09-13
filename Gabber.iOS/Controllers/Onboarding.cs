using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using Gabber.iOS.Helpers;
using GabberPCL.Models;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    public partial class Onboarding : UIViewController
    {
        public Onboarding(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var Pages = new List<OnboardingPageContent> {
                new OnboardingPageContent {
                    Title=StringResources.onboarding_ui_page_zero_title,
                    Content=StringResources.onboarding_ui_page_zero_content
                },
                new OnboardingPageContent {
                    Title=StringResources.onboarding_ui_page_first_title,
                    Content=StringResources.onboarding_ui_page_first_content
                },
                new OnboardingPageContent {
                    Title=StringResources.onboarding_ui_page_second_title,
                    Content=StringResources.onboarding_ui_page_second_content
                },
                new OnboardingPageContent {
                    Title=StringResources.onboarding_ui_page_third_title,
                    Content=StringResources.onboarding_ui_page_third_content
                },
                new OnboardingPageContent {
                    Title=StringResources.onboarding_ui_page_fourth_title,
                    Content=StringResources.onboarding_ui_page_fourth_content
                }
            };

            var controllers = new List<OnboardingContent>();
            for (int index = 0; index < Pages.Count; index++)
            {
                var view = Storyboard.InstantiateViewController("OnboardingContent") as OnboardingContent;
                view.Index = index;
                view.OBCTitle = Pages[index].Title;
                view.OBCContent = Pages[index].Content;
                controllers.Add(view);
            }

            var pageViewController = Storyboard.InstantiateViewController("OnboardingPager") as OnboardingPager;
            pageViewController.DataSource = new PageViewControllerDataSource(controllers);
            pageViewController.SetViewControllers(new UIViewController[] { controllers[0] }, UIPageViewControllerNavigationDirection.Forward, false, null);
            pageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Size.Height - 16);
            AddChildViewController(pageViewController);
            View.AddSubview(pageViewController.View);
            pageViewController.DidMoveToParentViewController(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            var pageControl = UIPageControl.Appearance;
            pageControl.PageIndicatorTintColor = UIColor.LightGray;
            pageControl.CurrentPageIndicatorTintColor = UIColor.FromCGColor(Application.MainColour);
            pageControl.BackgroundColor = UIColor.White;
        }

        class PageViewControllerDataSource : UIPageViewControllerDataSource
        {
            readonly List<OnboardingContent> Pages;

            public PageViewControllerDataSource(List<OnboardingContent> _pages) => Pages = _pages;
            public override nint GetPresentationCount(UIPageViewController pageViewController) => Pages.Count;
            public override nint GetPresentationIndex(UIPageViewController pageViewController) => 0;

            public override UIViewController GetNextViewController(UIPageViewController p, UIViewController rvc)
            {
                var vc = rvc as OnboardingContent;
                var index = vc.Index;
                Logger.LOG_EVENT_WITH_ACTION("ONBOARDING_SWIPE", index.ToString(), "POSITION");
                index++;
                if (index == Pages.Count) return null;
                return Pages[index];
            }

            public override UIViewController GetPreviousViewController(UIPageViewController p, UIViewController rvc)
            {
                var vc = rvc as OnboardingContent;
                var index = vc.Index;
                Logger.LOG_EVENT_WITH_ACTION("ONBOARDING_SWIPE", (index).ToString(), "POSITION");
                if (index == 0) return null;
                index--;
                return Pages[index];
            }
        }
    }
}