/*
 * Tripous.Avalon JavaScript Runtime
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

/**
 * Provides helper functions for Tripous Web demo pages.
 * @type {object}
 */
var Demos = Demos || {};

// ● fields
/**
 * Gets or sets the Ace editor that displays HTML code.
 * @type {object|null}
 */
Demos.HtmlEditor = null;
/**
 * Gets or sets the Ace editor that displays JavaScript code.
 * @type {object|null}
 */
Demos.JavaScriptEditor = null;
/**
 * Gets or sets the iframe element that hosts the selected demo.
 * @type {HTMLIFrameElement|null}
 */
Demos.Frame = null;

// ● private
/**
 * Returns the text content of the first element matching a selector.
 * @param {string} Selector The selector to use.
 * @returns {string} Returns the element markup or text.
 */
Demos.GetSourceText = function (Selector) {
    var Element = tp(Selector);
    if (!Element)
        return "";
    if (Element.tagName && Element.tagName.toLowerCase() === "script")
        return Element.textContent.trim();
    return Element.innerHTML.trim();
};
/**
 * Creates an Ace editor.
 * @param {string} ElementId The editor host element id.
 * @param {string} Mode The Ace mode name.
 * @param {string} Text The editor text.
 * @returns {object|null} Returns the Ace editor or null when Ace is not available.
 */
Demos.CreateEditor = function (ElementId, Mode, Text) {
    var Element = tp("#" + ElementId);
    if (!Element)
        return null;
    if (typeof ace === "undefined") {
        Element.textContent = Text;
        return null;
    }

    var Editor = ace.edit(ElementId);
    Editor.setTheme("ace/theme/twilight");
    Editor.session.setMode("ace/mode/" + Mode);
    Editor.setFontSize(17);
    Editor.setReadOnly(true);
    Editor.setShowPrintMargin(false);
    Editor.setValue(Text, -1);
    return Editor;
};

// ● public
/**
 * Registers the source preview initialization callback.
 * @returns {void}
 */
Demos.RegisterSourcePreview = function () {
    tp.Ready(function () {
        Demos.InitializeSourcePreview();
    });
};
/**
 * Registers the demo shell initialization callback.
 * @returns {void}
 */
Demos.RegisterShell = function () {
    tp.Ready(function () {
        Demos.InitializeShell();
    });
};
/**
 * Initializes the demo shell.
 * @returns {void}
 */
Demos.InitializeShell = function () {
    Demos.Frame = tp("#DemoFrame");
    tp.SelectAll("[data-demo-group-button]").forEach(function (Button) {
        tp.On(Button, "click", function () {
            var Group = Button.closest(".demo-nav-group");
            if (Group)
                Group.classList.toggle("is-expanded");
        });
    });
    tp.SelectAll("[data-demo-link]").forEach(function (Link) {
        tp.On(Link, "click", function () {
            Demos.SelectDemo(Link);
        });
    });
    Demos.SelectDemo(tp("[data-demo-link]"));
};
/**
 * Initializes the source preview panel.
 * @returns {void}
 */
Demos.InitializeSourcePreview = function () {
    var HtmlText = Demos.GetSourceText(".html-code");
    var JavaScriptText = Demos.GetSourceText(".js-code");
    var Panel = tp("#DemoSourcePanel");

    if (!Panel || (tp.IsBlank(HtmlText) && tp.IsBlank(JavaScriptText)))
        return;

    Panel.hidden = false;
    Demos.HtmlEditor = Demos.CreateEditor("DemoHtmlEditor", "html", HtmlText);
    Demos.JavaScriptEditor = Demos.CreateEditor("DemoJavaScriptEditor", "javascript", JavaScriptText);
    tp.SelectAll("[data-demo-source-tab]").forEach(function (Button) {
        tp.On(Button, "click", function () {
            Demos.SelectSourceTab(Button.getAttribute("data-demo-source-tab"));
        });
    });
    Demos.SelectSourceTab(!tp.IsBlank(HtmlText) ? "html" : "javascript");
};
/**
 * Selects a source preview tab.
 * @param {string} Name The source tab name.
 * @returns {void}
 */
Demos.SelectSourceTab = function (Name) {
    tp.SelectAll("[data-demo-source-tab]").forEach(function (Button) {
        Button.classList.toggle("is-selected", Button.getAttribute("data-demo-source-tab") === Name);
    });
    tp.SelectAll("[data-demo-source-page]").forEach(function (Page) {
        Page.classList.toggle("is-selected", Page.getAttribute("data-demo-source-page") === Name);
    });
};
/**
 * Selects a demo link in the shell navigation.
 * @param {HTMLAnchorElement|null} Link The selected demo link.
 * @returns {void}
 */
Demos.SelectDemo = function (Link) {
    if (!Link)
        return;
    tp.SelectAll("[data-demo-link]").forEach(function (Item) {
        Item.classList.toggle("is-selected", Item === Link);
    });
};
