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
            Assert.Equal(3, frames.Count);
            Assert.Equal("initial value", frames.Array[2].AttributeValue);
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
            Assert.Equal("unchanged 1", frames.Array[2].AttributeValue);
            Assert.Equal("unchanged 2", frames.Array[5].AttributeValue);
            Assert.Equal("new value", frames.Array[6].AttributeValue);
            Assert.Equal("unchanged 3", frames.Array[7].AttributeValue);
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
            Assert.Equal(3, frames.Count);
            Assert.Equal(3, frames.Array[0].ElementSubtreeLength);
            Assert.Equal(valuePropName, frames.Array[1].AttributeName);
            Assert.Equal("new value", frames.Array[1].AttributeValue);
            Assert.Equal("eventname", frames.Array[2].AttributeName);
            Assert.IsType<Action>(frames.Array[2].AttributeValue);
        }

        [Fact]
        public void ExpandsAllAncestorsWhenAddingAttribute()
        {

        }

        private static ArrayRange<RenderTreeFrame> BuildFrames(params RenderTreeFrame[] frames)
            => new ArrayRange<RenderTreeFrame>(frames, frames.Length);
    }
}
