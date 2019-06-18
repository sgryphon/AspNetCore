// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components.RenderTree;

namespace Microsoft.AspNetCore.Components.Rendering
{
    internal class RenderTreeUpdater
    {
        public static void UpdateToMatchClientState(RenderTreeBuilder renderTreeBuilder, int eventHandlerId, string attributeName, object attributeValue)
        {
            // We only allow the client to supply string or bool currently, since those are the only kinds of
            // values we output on attributes that go to the client
            if (!(attributeValue is string || attributeValue is bool))
            {
                return;
            }

            // Find the element that contains the event handler
            var frames = renderTreeBuilder.GetFrames();
            var framesArray = frames.Array;
            var framesLength = frames.Count;
            var closestElementFrameIndex = -1;
            for (var frameIndex = 0; frameIndex < framesLength; frameIndex++)
            {
                ref var frame = ref framesArray[frameIndex];
                switch (frame.FrameType)
                {
                    case RenderTreeFrameType.Element:
                        closestElementFrameIndex = frameIndex;
                        break;
                    case RenderTreeFrameType.Attribute:
                        if (frame.AttributeEventHandlerId == eventHandlerId)
                        {
                            UpdateFrameToMatchClientState(renderTreeBuilder, framesArray, closestElementFrameIndex, attributeName, attributeValue);
                            return;
                        }
                        break;
                }
            }
        }

        private static void UpdateFrameToMatchClientState(RenderTreeBuilder renderTreeBuilder, RenderTreeFrame[] framesArray, int elementFrameIndex, string attributeName, object attributeValue)
        {
            // Find the attribute frame
            ref var elementFrame = ref framesArray[elementFrameIndex];
            var elementSubtreeEndIndexExcl = elementFrameIndex + elementFrame.ElementSubtreeLength;
            for (var attributeFrameIndex = elementFrameIndex + 1; attributeFrameIndex < elementSubtreeEndIndexExcl; attributeFrameIndex++)
            {
                ref var attributeFrame = ref framesArray[attributeFrameIndex];
                if (attributeFrame.FrameType != RenderTreeFrameType.Attribute)
                {
                    // We're now looking at the descendants not attributes, so the search is over
                    break;
                }

                if (attributeFrame.AttributeName == attributeName)
                {
                    // Found an existing attribute we can update, as long as it really is just a simple
                    // attribute and not a special one (event handler)
                    if (attributeFrame.AttributeEventHandlerId == 0)
                    {
                        attributeFrame = attributeFrame.WithAttributeValue(attributeValue);
                    }
                    return;
                }
            }

            // If we get here, we didn't find the desired attribute
            // We do *not* insert the attribute to force the render tree into sync with the UI, because that
            // would make one-way binding impossible. For example, a textbox with only "onchange" (and not "value")
            // would keep clearing itself after each edit to keep things in sync.
            // To preserve the ability to do one-way bindings (in the UI->Component direction), we interpret
            // the absence of a corresponding value attribute as meaning the tree should not be updated.
            // This means we have to stop omitting attribute rendering for attributes that represent a "value"
            // in a possible two-way binding. This will be a future enhancement.
        }
    }
}
