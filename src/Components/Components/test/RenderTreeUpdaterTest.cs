// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Test.Helpers;
using Xunit;

namespace Microsoft.AspNetCore.Components.Test
{
    public class RenderTreeUpdaterTest
    {
        [Fact]
        public void IgnoresUnknownEventHandlerId()
        {
            // Arrange
            var valuePropName = "testprop";
            var frames = BuildFrames(
                RenderTreeFrame.Element(0, "elem").WithElementSubtreeLength(3),
                RenderTreeFrame.Attribute(1, "eventname", null).WithAttributeEventHandlerId(123),
                RenderTreeFrame.Attribute(2, valuePropName, "initial value"));

            // Act
            RenderTreeUpdater.UpdateToMatchClientState(frames, 456, valuePropName, "new value");

            // Assert
            Assert.Equal("initial value", frames.Array[2].AttributeValue);
        }

        [Fact]
        public void UpdatesOnlyMatchingAttributeValue()
        {
            // Arrange
            var valuePropName = "testprop";
            var frames = BuildFrames(
                // Element with different event handler ID
                RenderTreeFrame.Element(0, "elem").WithElementSubtreeLength(3),
                RenderTreeFrame.Attribute(1, "eventname", null).WithAttributeEventHandlerId(123),
                RenderTreeFrame.Attribute(2, valuePropName, "unchanged 1"),

                // Element with matching event handler ID
                RenderTreeFrame.Element(0, "elem").WithElementSubtreeLength(5),
                RenderTreeFrame.Attribute(1, "eventname", null).WithAttributeEventHandlerId(456),
                RenderTreeFrame.Attribute(2, "unrelated property before", "unchanged 2"),
                RenderTreeFrame.Attribute(3, valuePropName, "initial value"),
                RenderTreeFrame.Attribute(4, "unrelated property after", "unchanged 3"));

            // Act
            RenderTreeUpdater.UpdateToMatchClientState(frames, 456, valuePropName, "new value");

            // Assert
            Assert.Equal("unchanged 1", frames.Array[2].AttributeValue);
            Assert.Equal("unchanged 2", frames.Array[5].AttributeValue);
            Assert.Equal("new value", frames.Array[6].AttributeValue);
            Assert.Equal("unchanged 3", frames.Array[7].AttributeValue);
        }

        private static ArrayRange<RenderTreeFrame> BuildFrames(params RenderTreeFrame[] frames)
            => new ArrayRange<RenderTreeFrame>(frames, frames.Length);
    }
}
