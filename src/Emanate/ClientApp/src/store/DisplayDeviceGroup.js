const requestDisplayDevicesType = 'REQUEST_DISPLAY_DEVICES';
const receiveDisplayDevicesType = 'RECEIVE_DISPLAY_DEVICES';
const displayDeviceAddedType = 'DISPLAY_DEVICE_ADDED';
const displayDeviceRemovedType = 'DISPLAY_DEVICE_REMOVED';
const displayDeviceUpdatedType = 'DISPLAY_DEVICE_UPDATED';

const requestDisplayDeviceProfilesType = 'REQUEST_DISPLAY_DEVICE_PROFILES';
const receiveDisplayDeviceProfilesType = 'RECEIVE_DISPLAY_DEVICE_PROFILES';
const displayDeviceProfileAddedType = 'DISPLAY_DEVICE_PROFILE_ADDED';
const displayDeviceProfileRemovedType = 'DISPLAY_DEVICE_PROFILE_REMOVED';
const displayDeviceProfileUpdatedType = 'DISPLAY_DEVICE_PROFILE_UPDATED';

const requestDisplayDeviceTypesType = 'REQUEST_DISPLAY_DEVICE_TYPES';
const receiveDisplayDeviceTypesType = 'RECEIVE_DISPLAY_DEVICE_TYPES';
const displayDeviceTypeAddedType = 'DISPLAY_DEVICE_TYPE_ADDED';
const displayDeviceTypeRemovedType = 'DISPLAY_DEVICE_TYPE_REMOVED';
const displayDeviceTypeUpdatedType = 'DISPLAY_DEVICE_TYPE_UPDATED';

const initialState = {
    displayDevices: [],
    isLoadingDisplayDevices: false,
    displayDeviceProfiles: [],
    isLoadingDisplayDeviceProfiles: false,
    displayDeviceTypes: [],
    isLoadingDisplayDeviceTypes: false
};

export const actionCreators = {

    requestDisplayDevices: () => async (dispatch) => {
        dispatch({ type: requestDisplayDevicesType });

        const url = 'api/DisplayDevices';
        const response = await fetch(url);
        const devices = await response.json();

        dispatch({ type: receiveDisplayDevicesType, devices });
    },

    requestDisplayDeviceProfiles: () => async (dispatch) => {
        dispatch({ type: requestDisplayDeviceProfilesType });

        const url = 'api/DisplayDeviceProfiles';
        const response = await fetch(url);
        const profiles = await response.json();

        dispatch({ type: receiveDisplayDeviceProfilesType, profiles });
    },

    requestDisplayDeviceTypes: () => async (dispatch) => {
        dispatch({ type: requestDisplayDeviceTypesType });

        const url = 'api/DisplayDeviceTypes';
        const response = await fetch(url);
        const types = await response.json();

        dispatch({ type: receiveDisplayDeviceTypesType, types });
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    // Display Devices

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
        return {
            ...state,
            displayDevices: [...state.displayDevices, action.newDisplayDevice]
        };
    }

    if (action.type === displayDeviceRemovedType) {
        return {
            ...state,
            displayDevices: [...state.displayDevices.filter(d => d.id !== action.oldDisplayDeviceId)]
        };
    }

    if (action.type === displayDeviceUpdatedType) {
        return {
            ...state,
            displayDevices: state.displayDevices.map((device) => {
                if (device.id === action.updatedDevice.id) {
                    return {
                        ...device,
                        ...action.updatedDevice
                    }
                }

                return device;
            })
        };
    }

    // Display Device Profiles

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

    if (action.type === displayDeviceProfileUpdatedType) {
        return {
            ...state,
            displayDeviceProfiles: state.displayDeviceProfiles.map((profile) => {
                if (profile.id === action.updatedProfile.id) {
                    return {
                        ...profile,
                        ...action.updatedProfile
                    }
                }

                return profile;
            })
        };
    }

    // Display Device Types

    if (action.type === requestDisplayDeviceTypesType) {
        return {
            ...state,
            isLoadingDisplayDeviceTypes: true
        };
    }

    if (action.type === receiveDisplayDeviceTypesType) {
        return {
            ...state,
            displayDeviceTypes: action.types,
            isLoadingDisplayDeviceTypes: false
        };
    }

    if (action.type === displayDeviceTypeAddedType) {
        return {
            ...state,
            displayDeviceTypes: [...state.displayDeviceTypes, action.newDisplayDeviceType]
        };
    }

    if (action.type === displayDeviceTypeRemovedType) {
        return {
            ...state,
            displayDeviceTypes: [...state.displayDeviceTypes.filter(p => p.id !== action.oldDisplayDeviceTypeId)]
        };
    }

    if (action.type === displayDeviceTypeUpdatedType) {
        return {
            ...state,
            displayDeviceTypes: state.displayDeviceTypes.map((type) => {
                if (type.id === action.updatedType.id) {
                    return {
                        ...type,
                        ...action.updatedType
                    }
                }

                return type;
            })
        };
    }

    return state;
};
