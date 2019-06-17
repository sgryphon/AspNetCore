// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
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
            var renderer = new TestRenderer();
            var builder = new RenderTreeBuilder(renderer);
            builder.OpenElement(0, "elem");
            builder.AddAttribute(1, "eventname", () => { });
            builder.AddAttribute(2, valuePropName, "initial value");
            builder.CloseElement();
            var frames = builder.GetFrames();
            frames.Array[1] = frames.Array[1].WithAttributeEventHandlerId(123); // An unrelated event

            // Act
            RenderTreeUpdater.UpdateToMatchClientState(builder, 456, valuePropName, "new value");

            // Assert
            Assert.Collection(frames.AsEnumerable(),
                frame => AssertFrame.Element(frame, "elem", 3, 0),
                frame => AssertFrame.Attribute(frame, "eventname", v => Assert.IsType<Action>(v), 1),
                frame => AssertFrame.Attribute(frame, valuePropName, "initial value", 2));
        }

        [Fact]
        public void UpdatesOnlyMatchingAttributeValue()
        {
            // Arrange
            var valuePropName = "testprop";
            var renderer = new TestRenderer();
            var builder = new RenderTreeBuilder(renderer);
            builder.OpenElement(0, "elem");
            builder.AddAttribute(1, "eventname", () => { });
            builder.AddAttribute(2, valuePropName, "unchanged 1");
            builder.CloseElement();
            builder.OpenElement(3, "elem");
            builder.AddAttribute(4, "eventname", () => { });
            builder.AddAttribute(5, "unrelated prop before", "unchanged 2");
            builder.AddAttribute(6, valuePropName, "initial value");
            builder.AddAttribute(7, "unrelated prop after", "unchanged 3");
            builder.CloseElement();
            var frames = builder.GetFrames();
            frames.Array[1] = frames.Array[1].WithAttributeEventHandlerId(123); // An unrelated event
            frames.Array[4] = frames.Array[4].WithAttributeEventHandlerId(456);

            // Act
            RenderTreeUpdater.UpdateToMatchClientState(builder, 456, valuePropName, "new value");

            // Assert
            Assert.Collection(frames.AsEnumerable(),
                frame => AssertFrame.Element(frame, "elem", 3, 0),
                frame => AssertFrame.Attribute(frame, "eventname", v => Assert.IsType<Action>(v), 1),
                frame => AssertFrame.Attribute(frame, valuePropName, "unchanged 1", 2),
                frame => AssertFrame.Element(frame, "elem", 5, 3),
                frame => AssertFrame.Attribute(frame, "eventname", v => Assert.IsType<Action>(v), 4),
                frame => AssertFrame.Attribute(frame, "unrelated prop before", "unchanged 2", 5),
                frame => AssertFrame.Attribute(frame, valuePropName, "new value", 6),
                frame => AssertFrame.Attribute(frame, "unrelated prop after", "unchanged 3", 7));
        }

        [Fact]
        public void AddsAttributeIfNotFound()
        {
            // Arrange
            var valuePropName = "testprop";
            var renderer = new TestRenderer();
            var builder = new RenderTreeBuilder(renderer);
            builder.OpenElement(0, "elem");
            builder.AddAttribute(1, "eventname", () => { });
            builder.CloseElement();
            var frames = builder.GetFrames();
            frames.Array[1] = frames.Array[1].WithAttributeEventHandlerId(123);

            // Act
            RenderTreeUpdater.UpdateToMatchClientState(builder, 123, valuePropName, "new value");
            frames = builder.GetFrames();

            // Assert
            Assert.Collection(frames.AsEnumerable(),
                frame => AssertFrame.Element(frame, "elem", 3, 0),
                frame => AssertFrame.Attribute(frame, valuePropName, "new value", 0),
                frame => AssertFrame.Attribute(frame, "eventname", v => Assert.IsType<Action>(v), 1));
        }

        [Fact]
        public void ExpandsAllAncestorsWhenAddingAttribute()
        {

        }

        private static ArrayRange<RenderTreeFrame> BuildFrames(params RenderTreeFrame[] frames)
            => new ArrayRange<RenderTreeFrame>(frames, frames.Length);
    }
}
