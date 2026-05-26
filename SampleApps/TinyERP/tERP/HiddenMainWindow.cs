/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

public class HiddenMainWindow: Window
{
    // ● construction
    public HiddenMainWindow()
    {
        this.Width = 5;
        this.Height = 5;
        this.Position = new PixelPoint(10000, 10000);
        this.ShowInTaskbar = false;
    }
}