// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Microsoft.AspNetCore.Components.Rendering
{
    internal class RenderTreeUpdater
    {
        public static void UpdateToMatchClientState(ArrayRange<RenderTreeFrame> frames, int eventHandlerId, string attributeName, object attributeValue)
        {
            // Find the element that contains the event handler
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
                            UpdateFrameToMatchClientState(framesArray, closestElementFrameIndex, attributeName, attributeValue);
                            return;
                        }
                        break;
                }
            }
        }

        private static void UpdateFrameToMatchClientState(RenderTreeFrame[] framesArray, int elementFrameIndex, string attributeName, object attributeValue)
        {
            // Find the attribute frame
            ref var elementFrame = ref framesArray[elementFrameIndex];
            var elementSubtreeEndIndexExcl = elementFrameIndex + elementFrame.ElementSubtreeLength;
            for (var attributeFrameIndex = elementFrameIndex + 1; attributeFrameIndex < elementSubtreeEndIndexExcl; attributeFrameIndex++)
            {
                ref var attributeFrame = ref framesArray[attributeFrameIndex];
                if (attributeFrame.FrameType != RenderTreeFrameType.Attribute)
                {
                    // We ran out of attributes without finding the one we wanted. Give up.
                    break;
                }

                if (attributeFrame.AttributeName == attributeName)
                {
                    attributeFrame = attributeFrame.WithAttributeValue(attributeValue);
                    break;
                }
            }
        }
    }
}
