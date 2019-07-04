const requestSourceDevicesType = 'REQUEST_SOURCE_DEVICES';
const receiveSourceDevicesType = 'RECEIVE_SOURCE_DEVICES';
const sourceDeviceAddedType = 'SOURCE_DEVICE_ADDED';
const sourceDeviceRemovedType = 'SOURCE_DEVICE_REMOVED';
const sourceDeviceUpdatedType = 'SOURCE_DEVICE_UPDATED';

const requestSourceDeviceProfilesType = 'REQUEST_SOURCE_DEVICE_PROFILES';
const receiveSourceDeviceProfilesType = 'RECEIVE_SOURCE_DEVICE_PROFILES';
const sourceDeviceProfileAddedType = 'SOURCE_DEVICE_PROFILE_ADDED';
const sourceDeviceProfileRemovedType = 'SOURCE_DEVICE_PROFILE_REMOVED';
const sourceDeviceProfileUpdatedType = 'SOURCE_DEVICE_PROFILE_UPDATED';

const requestSourceDeviceTypesType = 'REQUEST_SOURCE_DEVICE_TYPES';
const receiveSourceDeviceTypesType = 'RECEIVE_SOURCE_DEVICE_TYPES';
const sourceDeviceTypeAddedType = 'SOURCE_DEVICE_TYPE_ADDED';
const sourceDeviceTypeRemovedType = 'SOURCE_DEVICE_TYPE_REMOVED';
const sourceDeviceTypeUpdatedType = 'SOURCE_DEVICE_TYPE_UPDATED';

const initialState = {
    sourceDevices: [],
    isLoadingSourceDevices: false,
    sourceDeviceProfiles: [],
    isLoadingSourceDeviceProfiles: false,
    sourceDeviceTypes: [],
    isLoadingSourceDeviceTypes: false
};

export const actionCreators = {

    requestSourceDevices: () => async (dispatch) => {
        dispatch({ type: requestSourceDevicesType });

        const url = 'api/SourceDevices';
        const response = await fetch(url);
        const devices = await response.json();

        dispatch({ type: receiveSourceDevicesType, devices });
    },

    requestSourceDeviceProfiles: () => async (dispatch) => {
        dispatch({ type: requestSourceDeviceProfilesType });

        const url = 'api/SourceDeviceProfiles';
        const response = await fetch(url);
        const profiles = await response.json();

        dispatch({ type: receiveSourceDeviceProfilesType, profiles });
    },

    requestSourceDeviceTypes: () => async (dispatch) => {
        dispatch({ type: requestSourceDeviceTypesType });

        const url = 'api/SourceDeviceTypes';
        const response = await fetch(url);
        const types = await response.json();

        dispatch({ type: receiveSourceDeviceTypesType, types });
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    // Display Devices

    if (action.type === requestSourceDevicesType) {
        return {
            ...state,
            isLoadingSourceDevices: true
        };
    }

    if (action.type === receiveSourceDevicesType) {
        return {
            ...state,
            sourceDevices: action.devices,
            isLoadingSourceDevices: false
        };
    }

    if (action.type === sourceDeviceAddedType) {
        return {
            ...state,
            sourceDevices: [...state.sourceDevices, action.newSourceDevice]
        };
    }

    if (action.type === sourceDeviceRemovedType) {
        return {
            ...state,
            sourceDevices: [...state.sourceDevices.filter(d => d.id !== action.oldSourceDeviceId)]
        };
    }

    if (action.type === sourceDeviceUpdatedType) {
        return {
            ...state,
            sourceDevices: state.sourceDevices.map((device) => {
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

    if (action.type === requestSourceDeviceProfilesType) {
        return {
            ...state,
            isLoadingSourceDeviceProfiles: true
        };
    }

    if (action.type === receiveSourceDeviceProfilesType) {
        return {
            ...state,
            sourceDeviceProfiles: action.profiles,
            isLoadingSourceDeviceProfiles: false
        };
    }

    if (action.type === sourceDeviceProfileAddedType) {
        return {
            ...state,
            sourceDeviceProfiles: [...state.sourceDeviceProfiles, action.newSourceDeviceProfile]
        };
    }

    if (action.type === sourceDeviceProfileRemovedType) {
        return {
            ...state,
            sourceDeviceProfiles: [...state.sourceDeviceProfiles.filter(p => p.id !== action.oldSourceDeviceProfileId)]
        };
    }

    if (action.type === sourceDeviceProfileUpdatedType) {
        return {
            ...state,
            sourceDeviceProfiles: state.sourceDeviceProfiles.map((profile) => {
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

    if (action.type === requestSourceDeviceTypesType) {
        return {
            ...state,
            isLoadingSourceDeviceTypes: true
        };
    }

    if (action.type === receiveSourceDeviceTypesType) {
        return {
            ...state,
            sourceDeviceTypes: action.types,
            isLoadingSourceDeviceTypes: false
        };
    }

    if (action.type === sourceDeviceTypeAddedType) {
        return {
            ...state,
            sourceDeviceTypes: [...state.sourceDeviceTypes, action.newSourceDeviceType]
        };
    }

    if (action.type === sourceDeviceTypeRemovedType) {
        return {
            ...state,
            sourceDeviceTypes: [...state.sourceDeviceTypes.filter(p => p.id !== action.oldSourceDeviceTypeId)]
        };
    }

    if (action.type === sourceDeviceTypeUpdatedType) {
        return {
            ...state,
            sourceDeviceTypes: state.sourceDeviceTypes.map((type) => {
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
