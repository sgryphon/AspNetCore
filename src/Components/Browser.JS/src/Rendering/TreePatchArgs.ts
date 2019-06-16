export class TreePatchArgs {
    constructor(public componentId: number, public attributeName: string) {
    }

    public static fromEvent(componentId: number, event: Event): TreePatchArgs | null {
        const elem = event.target;
        if (elem instanceof Element && isPossibleFormFieldMutationEvent(event)) {
            const attributeName = getValueAttributeNameForFormField(elem);
            if (attributeName) {
                return new TreePatchArgs(componentId, attributeName);
            }
        }

        // Either it's not a form field mutation event, or we don't recognize what kind
        // of form field element it's happening on
        return null;
    }
}

function getValueAttributeNameForFormField(elem: Element) {
    // The logic in here should be the inverse of the logic in BrowserRenderer's tryApplySpecialProperty.
    // That is, we're doing the reverse mapping, starting from an HTML property and reconstructing which
    // "special" attribute would have been mapped to that property.
    switch (elem.tagName) {
        case 'INPUT': {
            const inputType = elem.getAttribute('type');
            return (inputType && inputType.toLowerCase() === 'checkbox') ? 'checked' : 'value';
        }
        case 'SELECT':
        case 'TEXTAREA':
            return 'value';
        default:
            return null;
    }
}

function isPossibleFormFieldMutationEvent(event: Event) {
    // Events for which we will send the latest state of the corresponding form field element
    // in case it represents a mutation to the field value. The purpose of this filter is to
    // avoid sending potentially large amounts of extra data pointlessly (e.g., if there's a
    // mousemove event handler on a textarea).
    switch (event.type) {
        case 'input':
        case 'change':
        case 'keydown':
        case 'keyup':
        case 'keypress':
        case 'click':
        case 'mousedown':
        case 'mouseup':
        case 'dblclick':
        case 'touchend':
        case 'touchmove':
        case 'touchenter':
        case 'touchleave':
        case 'touchstart':
        case 'pointerdown':
        case 'pointerup':
            return true;
        default:
            return false;
    }
}
