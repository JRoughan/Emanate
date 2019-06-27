const requestDisplayDevicesType = 'REQUEST_DISPLAY_DEVICES';
const receiveDisplayDevicesType = 'RECEIVE_DISPLAY_DEVICES';
const addDisplayDeviceType = 'ADD_DISPLAY_DEVICE';
const displayDeviceAddedType = 'DISPLAY_DEVICE_ADDED';
const removeDisplayDeviceType = 'REMOVE_DISPLAY_DEVICE';
const displayDeviceRemovedType = 'DISPLAY_DEVICE_REMOVED';
const initialState = { displayDevices: [], isLoadingDisplayDevices: false };

export const actionCreators = {
    requestDisplayDevices: () => async (dispatch) => {
        dispatch({ type: requestDisplayDevicesType });

        const url = 'api/DisplayDevices';
        const response = await fetch(url);
        const devices = await response.json();

        dispatch({ type: receiveDisplayDevicesType, devices });
    },

    addDisplayDevice: (device) => async () => {

        const url = 'api/DisplayDevices/';
        await fetch(url, {
            method: 'post',
            body: JSON.stringify(device),
            headers: {
                "Content-Type": "application/json"
            }
        });
    },

    removeDisplayDevice: (id) => async () => {

        const url = 'api/DisplayDevices/' + id;
        await fetch(url, {
            method: 'delete'
        });
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    if (action.type === requestDisplayDevicesType) {
        return {
            ...state,
            isLoadingDisplayDevices: true
        };
    }

    if (action.type === receiveDisplayDevicesType) {
        return {
            ...state,
            displayDevices: action.devices,
            isLoadingDisplayDevices: false
        };
    }

    if (action.type === displayDeviceAddedType) {
        return { ...state, displayDevices: [...state.displayDevices, action.newDisplayDevice] };
    }

    if (action.type === displayDeviceRemovedType) {
        return { ...state, displayDevices: [...state.displayDevices.filter(d => d.id !== action.oldDisplayDeviceId)] };
    }

    return state;
};
