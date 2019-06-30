const openNewDisplayDeviceDialogType = 'OPEN_NEW_DISPLAY_DEVICE_DIALOG';
const closeNewDisplayDeviceDialogType = 'CLOSE_NEW_DISPLAY_DEVICE_DIALOG';

const initialState = { newDisplayDeviceDialogIsOpen: false };

export const actionCreators = {

    openNewDisplayDeviceDialog: () => async (dispatch) => {
        dispatch({ type: openNewDisplayDeviceDialogType });
    },

    addDisplayDevice: (device) => async (dispatch) => {

        const url = 'api/DisplayDevices/';
        await fetch(url, {
            method: 'post',
            body: JSON.stringify(device),
            headers: {
                "Content-Type": "application/json"
            }
        }).then(() => {
            dispatch({ type: closeNewDisplayDeviceDialogType });
        });
    },

    closeNewDisplayDeviceDialog: () => async (dispatch) => {
        dispatch({ type: closeNewDisplayDeviceDialogType });
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    if (action.type === openNewDisplayDeviceDialogType) {
        return {
            ...state,
            newDisplayDeviceDialogIsOpen: true
        };
    }

    if (action.type === closeNewDisplayDeviceDialogType) {
        return {
            ...state,
            newDisplayDeviceDialogIsOpen: false
        };
    }

    return state;
};
