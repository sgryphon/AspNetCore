// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Components.Rendering
{
    /// <summary>
    /// Information supplied with an event notification that can be used to update an existing
    /// render tree to match the latest UI state when a form field has mutated.
    /// </summary>
    public class EventTreePatchInfo
    {
        /// <summary>
        /// Identifies the component whose render tree contains the affected form field.
        /// </summary>
        public int ComponentId { get; set; }

        /// <summary>
        /// Specifies the name of the attribute that corresponds to the form field's mutated value.
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// Specifies the form field's mutated value.
        /// </summary>
        public object AttributeValue { get; set; }
    }
}
