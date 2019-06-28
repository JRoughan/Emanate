const requestDisplayDeviceProfilesType = 'REQUEST_DISPLAY_DEVICE_PROFILES';
const receiveDisplayDeviceProfilesType = 'RECEIVE_DISPLAY_DEVICE_PROFILES';
const displayDeviceProfileAddedType = 'DISPLAY_DEVICE_PROFILE_ADDED';
const displayDeviceProfileRemovedType = 'DISPLAY_DEVICE_PROFILE_REMOVED';
const initialState = { displayDeviceProfiles: [], isLoadingDisplayDeviceProfiles: false };

export const actionCreators = {
    requestDisplayDeviceProfiles: () => async (dispatch) => {
        dispatch({ type: requestDisplayDeviceProfilesType });

        const url = 'api/DisplayDeviceProfiles';
        const response = await fetch(url);
        const profiles = await response.json();

        dispatch({ type: receiveDisplayDeviceProfilesType, profiles });
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    if (action.type === requestDisplayDeviceProfilesType) {
        return {
            ...state,
            isLoadingDisplayDeviceProfiles: true
        };
    }

    if (action.type === receiveDisplayDeviceProfilesType) {
        return {
            ...state,
            displayDeviceProfiles: action.profiles,
            isLoadingDisplayDeviceProfiles: false
        };
    }

    if (action.type === displayDeviceProfileAddedType) {
        return {
            ...state,
            displayDeviceProfiles: [...state.displayDeviceProfiles, action.newDisplayDeviceProfile]
        };
    }

    if (action.type === displayDeviceProfileRemovedType) {
        return {
            ...state,
            displayDeviceProfiles: [...state.displayDeviceProfiles.filter(p => p.id !== action.oldDisplayDeviceProfileId)]
        };
    }

    return state;
};
