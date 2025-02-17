﻿using System.Drawing;
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;
using Microsoft.Maui.AppiumTests;
using OpenQA.Selenium.Appium.MultiTouch;

namespace Microsoft.Maui.AppiumTests.Issues;
public class Issue19956: _IssuesUITest
{
    public Issue19956(TestDevice device) : base(device) { }

    public override string Issue => "Sticky headers and bottom content insets";

    [Test]
    public void ContentAccountsForStickyHeaders()
    {
        this.IgnoreIfPlatforms(new TestDevice[] { TestDevice.Android, TestDevice.Mac, TestDevice.Windows },
            "This is an iOS Keyboard Scrolling issue.");

        var app = App as AppiumApp;
        if (app is null)
            return;

        var stickyHeader = App.WaitForElement("StickyHeader");
        var stickyHeaderRect = stickyHeader.GetRect();

        // Scroll to the bottom of the page
        var actions = new TouchAction(app.Driver);
        actions.LongPress(null, 5, 650).MoveTo(null, 5, 100).Release().Perform();

        app.Click("Entry12");
        ValidateEntryPosition("Entry12", app, stickyHeaderRect);
        ValidateEntryPosition("Entry1", app, stickyHeaderRect);
        ValidateEntryPosition("Entry2", app, stickyHeaderRect);
    }

    void ValidateEntryPosition (string entryName, AppiumApp app, Rectangle stickyHeaderRect)
    {
        var entryRect = App.WaitForElement(entryName).GetRect();
        var keyboardPos = KeyboardScrolling.FindiOSKeyboardLocation(app.Driver);

        Assert.AreEqual(App.WaitForElement("StickyHeader").GetRect(), stickyHeaderRect);
        Assert.Less(stickyHeaderRect.Bottom, entryRect.Top);
        Assert.NotNull(keyboardPos);
        Assert.Less(entryRect.Bottom, keyboardPos!.Value.Y);

        KeyboardScrolling.NextiOSKeyboardPress(app.Driver);
    }

    [Test]
    public void BottomInsetsSetCorrectly()
    {
        this.IgnoreIfPlatforms(new TestDevice[] { TestDevice.Android, TestDevice.Mac, TestDevice.Windows },
            "This is an iOS Keyboard Scrolling issue.");

        var app = App as AppiumApp;
        if (app is null)
            return;

        app.Click("Entry5");
        ScrollToBottom(app);
        CheckForBottomEntry(app);
        KeyboardScrolling.NextiOSKeyboardPress(app.Driver);

        app.Click("Entry10");
        ScrollToBottom(app);
        CheckForBottomEntry(app);
        KeyboardScrolling.NextiOSKeyboardPress(app.Driver);

        ScrollToBottom(app);
        CheckForBottomEntry(app);
    }

    static void ScrollToBottom(AppiumApp app)
    {
        var actions = new TouchAction(app.Driver);
        // scroll up once to trigger resetting content insets
        actions.LongPress(null, 5, 300).MoveTo(null, 5, 450).Release().Perform();
        actions.LongPress(null, 5, 400).MoveTo(null, 5, 100).Release().Perform();

        actions.LongPress(null, 5, 400).MoveTo(null, 5, 100).Release().Perform();
        actions.LongPress(null, 5, 400).MoveTo(null, 5, 100).Release().Perform();
    }

    void CheckForBottomEntry (AppiumApp app)
    {
        var bottomEntryRect = App.WaitForElement("Entry12").GetRect();
        var keyboardPosition = KeyboardScrolling.FindiOSKeyboardLocation(app.Driver);
        Assert.NotNull(keyboardPosition);
        Assert.Less(bottomEntryRect.Bottom, keyboardPosition!.Value.Y);
    }
}
