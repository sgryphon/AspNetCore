export class EventTreePatchInfo {
    constructor(public componentId: number, public attributeName: string, public attributeValue: any) {
    }

    public static fromEvent(componentId: number, event: Event): EventTreePatchInfo | null {
        const elem = event.target;
        if (elem instanceof Element) {
            const valueData = getValueDataForFormField(elem);
            if (valueData) {
                return new EventTreePatchInfo(componentId, valueData.attributeName, valueData.attributeValue);
            }
        }

        // This event isn't happening on a form field that we can reverse-map back to some incoming attribute
        return null;
    }
}

function getValueDataForFormField(elem: Element) {
    // The logic in here should be the inverse of the logic in BrowserRenderer's tryApplySpecialProperty.
    // That is, we're doing the reverse mapping, starting from an HTML property and reconstructing which
    // "special" attribute would have been mapped to that property.
    if (elem instanceof HTMLInputElement) {
        return (elem.type && elem.type.toLowerCase() === 'checkbox')
            ? { attributeName: 'checked', attributeValue: elem.checked }
            : { attributeName: 'value', attributeValue: elem.value };
    }

    if (elem instanceof HTMLSelectElement || elem instanceof HTMLTextAreaElement) {
        return { attributeName: 'value', attributeValue: elem.value };
    }

    return null;
}
